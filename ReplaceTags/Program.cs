using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        string inputFilePath = @"D:\Downloads\GenModified.txt";
        string outputFilePath = @"D:\Downloads\GenModifiedFormatted.txt";

        try
        {
            string content = File.ReadAllText(inputFilePath);

           
            content = Regex.Replace(content, "<italic>", "\\it");
            content = Regex.Replace(content, "</italic>", "\\it*");
            content = Regex.Replace(content, "<p/>", "\\p");
            content = Regex.Replace(content, "<kap/>", "\\cl_");
            content = Regex.Replace(content, @"<vers n=""(\d+)""/>", @"\v_$1_");

            File.WriteAllText(outputFilePath, content);

            Console.WriteLine($"Úpravy byly úspěšně provedeny. Výstup byl uložen do: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Došlo k chybě: {ex.Message}");
        }
    }
}