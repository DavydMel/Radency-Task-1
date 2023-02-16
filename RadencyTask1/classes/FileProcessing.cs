using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RadencyTask1.classes.payment;
using System.IO;
using Microsoft.VisualBasic;

namespace RadencyTask1.classes
{
    public class FileProcessing
    {
        private readonly FileSystemWatcher _watcher;
        private List<PaymentProcessed> paymentList;
        private string outputPath;
        private int fileNumber;

        public FileProcessing(string inputPath, string outputPath)
        {
            _watcher = new FileSystemWatcher(inputPath);
            paymentList = new List<PaymentProcessed>();
            fileNumber = 1;
            this.outputPath = outputPath;
        }

        public async void Configure()
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
                                    }
                                    catch (Exception ex)
                                    {
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
    }
}