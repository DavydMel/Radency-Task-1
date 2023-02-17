using Newtonsoft.Json;
using RadencyTask1.classes.payment;
using Quartz;
using Quartz.Impl;
using RadencyTask1.classes.schedule;

namespace RadencyTask1.classes
{
    public class FileProcessing
    {
        private FileSystemWatcher _watcher;
        private List<PaymentProcessed> paymentList;
        private string inputPath;
        private string outputPath;
        private int fileNumber;
        private IScheduler scheduler;

        public FileProcessing(string inputPath, string outputPath)
        {
            _watcher = new FileSystemWatcher(inputPath);
            paymentList = new List<PaymentProcessed>();
            fileNumber = 1;
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }

        public async void ConfigureWatcher()
        {
            _watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            _watcher.Created += OnCreated;
            _watcher.Error += OnError;

            //_watcher.Filter = "*.txt";
            _watcher.EnableRaisingEvents = true;

            async void OnCreated(object sender, FileSystemEventArgs e)
            {
                if (Path.GetExtension(e.FullPath) == ".txt" || Path.GetExtension(e.FullPath) == ".csv")
                {
                    try
                    {
                        using (var reader = new StreamReader(e.FullPath))
                        {
                            lock(paymentList)
                            {
                                while (!reader.EndOfStream)
                                {
                                    try
                                    {
                                        PaymentRaw paymentRaw = new PaymentRaw(reader.ReadLine());
                                        PaymentProcessed.Add(paymentList, paymentRaw);
                                        LoggingJob.parsedLines++;
                                    }
                                    catch (Exception ex)
                                    {
                                        LoggingJob.errorLines++;
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch { }

                    if (paymentList.Count > 0)
                    {
                        await SaveDataToJson();
                        Console.WriteLine($"Added: {e.FullPath}");
                    }
                }
                else
                {
                    LoggingJob.errorFilesPath.Add(e.FullPath);
                }
                LoggingJob.fileAmount++;
            }

            void OnError(object sender, System.IO.ErrorEventArgs e) =>
                PrintException(e.GetException());

            void PrintException(Exception? ex)
            {
                if (ex != null)
                {
                    Console.WriteLine($"Message: {ex.Message}");
                    Console.WriteLine("Stacktrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine();
                    PrintException(ex.InnerException);
                }
            }
        }

        public async void ConfigureScheduleLogging()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();
            await scheduler.Start();
            IJobDetail job = JobBuilder.Create<LoggingJob>()
                .WithIdentity("logging")
                .Build();
            LoggingJob.outputPath = outputPath;

            ITrigger trigger = TriggerBuilder.Create()
                          .WithIdentity("triggerLogging")
                          .WithSchedule(CronScheduleBuilder
                          .DailyAtHourAndMinute(0, 0))
                          .Build();
            await scheduler.ScheduleJob(job, trigger);
        }

        public async Task SaveDataToJson()
        {
            string rootDirectory = outputPath;
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
            rootDirectory = Path.Combine(rootDirectory, DateTime.Now.ToString("MM-dd-yyyy"));
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
            rootDirectory = Path.Combine(rootDirectory, $"output{fileNumber}.json");
            fileNumber++;

            lock(paymentList)
            {
                string json = JsonConvert.SerializeObject(paymentList);
                File.WriteAllText(rootDirectory, json);

                paymentList.Clear();
            }
        }

        public void CreateLogManually()
        {
            scheduler.TriggerJob(new JobKey("logging"));
        }

        public void Restart()
        {
            _watcher.Dispose();
            _watcher = new FileSystemWatcher(inputPath);
            ConfigureWatcher();
        }
    }
}