using System.Configuration;
using RadencyTask1.classes;

if (ConfigurationManager.AppSettings["PathInput"] == null || 
    ConfigurationManager.AppSettings["PathOuput"] == null)
{
    Console.WriteLine("Application config file (App.config) is unavailable or does not have required values");
    return;
}

var fileProc = new FileProcessing(
        ConfigurationManager.AppSettings.Get("PathInput"),
        ConfigurationManager.AppSettings.Get("PathOuput")
        );
fileProc.ConfigureWatcher();
fileProc.ConfigureScheduleLogging();
Console.WriteLine("Program is running...\n");

int action = -1;
while (action != 0)
{
    Console.WriteLine("0 - enter to exit");
    Console.WriteLine("1 - enter to restart");
    Console.WriteLine("2 - enter to save mata.log file manually");

    bool check = false;
    while (!check)
    {
        check = int.TryParse(Console.ReadLine(), out action);
        if (!check)
        {
            Console.WriteLine("Unknown action");
        }
    }

    switch (action)
    {
        case 0:
            {
                Console.WriteLine("Exiting...");
                Environment.Exit(1);
                break;
            }
        case 1:
            {
                fileProc.Restart();
                Console.Clear();
                Console.WriteLine("The program has been restarted\n");
                break;
            }
        case 2:
            {
                Console.WriteLine("Saving meta.log file manually");
                fileProc.CreateLogManually();
                break;
            }
        default:
            {
                Console.WriteLine("Unknown action");
                break;
            }
    }
}