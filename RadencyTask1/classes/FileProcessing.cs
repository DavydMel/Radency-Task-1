using System;
using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;

namespace RadencyTask1.classes
{
    public class FileProcessing
    {
        private readonly FileSystemWatcher _watcher;
        private List<string> _data;

        public FileProcessing(string rootDirectory)
        {
            _watcher = new FileSystemWatcher(rootDirectory);
            _data = new List<string>();
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
                                var line = reader.ReadLine();
                                _data.Add(line);
                                Console.WriteLine(line);
                            }
                        }
                    }
                    catch { }
                    Console.WriteLine(1);

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

            void OnError(object sender, ErrorEventArgs e) =>
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
    }
}