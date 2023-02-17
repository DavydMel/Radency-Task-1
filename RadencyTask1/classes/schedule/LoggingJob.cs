using Quartz;

namespace RadencyTask1.classes.schedule
{
    public class LoggingJob : IJob
    {
        public static int fileAmount = 0;
        public static int parsedLines = 0;
        public static int errorLines = 0;
        public static string outputPath = "";
        public static List<string> errorFilesPath = new List<string>();

        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("Time to create log file");

            string? rootDirectory = outputPath;

            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
            rootDirectory = Path.Combine(rootDirectory, DateTime.Now.ToString("MM-dd-yyyy"));
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
            rootDirectory = Path.Combine(rootDirectory, "meta.log");

            string logs = $"parsed_files: {fileAmount}\n" +
                $"parsed_lines: {parsedLines}\n" +
                $"found_errors: {errorLines}\n" +
                "invalid_files: [";
            logs += String.Join(", ", errorFilesPath);
            logs += "]";

            File.WriteAllText(rootDirectory, logs);

            fileAmount = 0;
            parsedLines = 0;
            errorLines = 0;
            errorFilesPath.Clear();
        }
    }
}