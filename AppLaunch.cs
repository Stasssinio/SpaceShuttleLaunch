using System.Text.RegularExpressions;

namespace SpaceShuttleLaunch
{
    public class AppLaunch
    {
        public static void Main()
        {
            try
            {

                string ascii = @" 
     _____                         _____ _           _   _   _        _                            _     
    / ____|                       / ____| |         | | | | | |      | |                          | |    
   | (___  _ __   __ _  ___ ___  | (___ | |__  _   _| |_| |_| | ___  | |     __ _ _   _ _ __   ___| |__  
    \___ \| '_ \ / _` |/ __/ _ \  \___ \| '_ \| | | | __| __| |/ _ \ | |    / _` | | | | '_ \ / __| '_ \ 
    ____) | |_) | (_| | (_|  __/  ____) | | | | |_| | |_| |_| |  __/ | |___| (_| | |_| | | | | (__| | | |
   |_____/| .__/ \__,_|\___\___| |_____/|_| |_|\__,_|\__|\__|_|\___| |______\__,_|\__,_|_| |_|\___|_| |_|
          | |                                                                                            
          |_|                                                                                            
";
                Console.WriteLine(ascii);
                string senderEmail = string.Empty;
                string password = string.Empty;
                string receiverEmail = string.Empty;
                string pattern = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

                Console.Write("Enter folder path: ");
                string folderPath = Console.ReadLine()!;
                if (Directory.Exists(folderPath))
                {
                    Console.Write("Enter sender email: ");
                    senderEmail = Console.ReadLine()!;
                    if (senderEmail == null || !Regex.IsMatch(senderEmail, pattern))
                    {
                        Console.Write("Enter valid email!");
                        return;
                    }
                    Console.Write("Enter email password: ");
                    password = Console.ReadLine()!;
                    Console.Write("Enter receiver email: ");
                    receiverEmail = Console.ReadLine()!;
                    if (receiverEmail == null || !Regex.IsMatch(receiverEmail, pattern))
                    {
                        Console.Write("Enter valid email!");
                        return;
                    }
                    else
                    {
                        Console.Write("\n Wait for the program to generate the results... ");
                    }
                }
                else
                {
                    Console.WriteLine("The folder does not exist.");
                    return;
                }

                LaunchAnalysis analyzer = new LaunchAnalysis(folderPath);
                analyzer.AnalyzeAndReport();
                string reportPath = Path.Combine(folderPath, "LaunchAnalysisReport.csv");

                if (analyzer.IsValid)
                {
                    EmailService email = new(senderEmail, password);
                    email.SendMail(receiverEmail, "Launch Space Shuttle Report", "File is attached to the email!", reportPath);

                    Console.WriteLine("\nReport generated and email sent successfully.");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}\n{ex.Data}");
            }
        }
    }
}