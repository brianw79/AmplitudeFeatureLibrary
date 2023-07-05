using AmplitudeFeatureLibrary;
using System;

namespace AmplitudeFeatureConsoleTester
{
    internal class Program
    {
        private const string API_KEY = "YOUR_API_KEY";

        static void Main()
        {
            var featureApi = new AmplitudeFeature(API_KEY);
 
            string readLine;
            do
            {
                WriteLine("Enter a flag name (ENTER to exit): ");
                readLine = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(readLine))
                {
                    var enabled = featureApi.FeatureIsEnabled(readLine);
                    WriteLine($"Feature Enabled: {enabled}", enabled ? ConsoleColor.Green : ConsoleColor.Red);
                }

                WriteLine();
            } while (!string.IsNullOrWhiteSpace(readLine));
        }

        private static void WriteLine(string line = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
        }
    }
}
