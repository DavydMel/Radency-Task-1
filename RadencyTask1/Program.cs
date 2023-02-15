using System.Configuration;
using RadencyTask1.classes;

if (ConfigurationManager.AppSettings["Path"] != null)
{
    var fileProc = new FileProcessing(ConfigurationManager.AppSettings.Get("Path"));
    fileProc.Configure();

    Console.WriteLine("Press enter to exit.");
    Console.ReadLine();
}
else
{
    throw new Exception("Unccorect file path");
}