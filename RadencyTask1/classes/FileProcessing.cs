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

        public FileProcessing(string rootDirectory)
        {
            _watcher = new FileSystemWatcher(rootDirectory);
            paymentList = new List<PaymentProcessed>();
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

            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;

            //_watcher.Filter = "*.txt";
            _watcher.EnableRaisingEvents = true;

            void OnChanged(object sender, FileSystemEventArgs e)
            {
                if (e.ChangeType != WatcherChangeTypes.Changed)
                {
                    return;
                }

                if (Path.GetExtension(e.FullPath) == ".txt" || Path.GetExtension(e.FullPath) == ".csv")
                {
                    try
                    {
                        using (var reader = new StreamReader(e.FullPath))
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
                    catch { }
                }

                Console.WriteLine($"Changed: {e.FullPath}");
            }

            void OnCreated(object sender, FileSystemEventArgs e)
            {
                if (Path.GetExtension(e.FullPath) != ".txt" && Path.GetExtension(e.FullPath) != ".csv")
                {
                    return;
                }

                string value = $"Created: {e.FullPath}";
                Console.WriteLine(value);
            }

            void OnDeleted(object sender, FileSystemEventArgs e) =>
                Console.WriteLine($"Deleted: {e.FullPath}");

            void OnRenamed(object sender, RenamedEventArgs e)
            {
                Console.WriteLine($"Renamed:");
                Console.WriteLine($"    Old: {e.OldFullPath}");
                Console.WriteLine($"    New: {e.FullPath}");
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

        public async void SaveDataToJson(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
            rootDirectory = Path.Combine(rootDirectory, "data.json");
            var json = JsonConvert.SerializeObject(paymentList);
            Console.WriteLine(json);
            File.WriteAllText(rootDirectory, json);
        }
    }
}