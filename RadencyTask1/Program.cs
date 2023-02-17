using System.Configuration;
using RadencyTask1.classes;

if (ConfigurationManager.AppSettings["PathInput"] != null)
{
    var fileProc = new FileProcessing(
        ConfigurationManager.AppSettings.Get("PathInput"),
        ConfigurationManager.AppSettings.Get("PathOuput")
        );
    fileProc.Configure();

    Console.WriteLine("Program is running...\n");
    Console.WriteLine("0 - type to exit");
    Console.WriteLine("1 - type to save mata.log file");
    int action = -1;
    while (action != 0)
    {
        bool check = false;
        while(!check)
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
                    Console.WriteLine("Saving meta.log file...");
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

    Console.WriteLine("Press enter to exit.");
    Console.ReadLine();
}
else
{
    throw new Exception("Unccorect file path");
}