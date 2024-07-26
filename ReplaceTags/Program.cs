using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        string inputFilePath = GetFilePath("Zadejte cestu k vstupnímu souboru: ");
        string outputFilePath = GetFilePath("Zadejte cestu k výslednému souboru: ");

        try
        {
            Encoding detectedEncoding = DetectEncoding(inputFilePath);

            if (detectedEncoding == null)
            {
                Console.WriteLine("Nepodařilo se detekovat správné kódování. Zkontrolujte vstupní soubor.");
                return;
            }

            Console.WriteLine($"Detekované kódování: {detectedEncoding.EncodingName}");

            string content = File.ReadAllText(inputFilePath, detectedEncoding);

            // Úpravy obsahu
            content = Regex.Replace(content, "<italic>", "\\it");
            content = Regex.Replace(content, "</italic>", "\\it*");
            content = Regex.Replace(content, "<p/>", "\\p");
            content = Regex.Replace(content, "<kap/>", "\\cl_");
            content = Regex.Replace(content, @"<vers n=""(\d+)""/>", @"\v_$1_");
            content = Regex.Replace(content, @"<kap n=""(\d+)""/>", @"\cl_$1");

            // Zápis souboru v UTF-8 bez BOM
            File.WriteAllText(outputFilePath, content, new UTF8Encoding(false));
            Console.WriteLine($"Úpravy byly úspěšně provedeny. Výstup byl uložen do: {outputFilePath} v kódování UTF-8");

            // Pro kontrolu vypíšeme část obsahu
            Console.WriteLine("Prvních 100 znaků výstupního souboru:");
            Console.WriteLine(content.Substring(0, Math.Min(100, content.Length)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Došlo k chybě: {ex.Message}");
        }
    }

    private static string GetFilePath(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine().Trim();
    }

    private static Encoding DetectEncoding(string filePath)
    {
        Encoding[] encodingsToTry = new Encoding[]
        {
            Encoding.UTF8,
            Encoding.GetEncoding(1250),  // Windows-1250
            Encoding.GetEncoding(28592), // ISO-8859-2
            Encoding.GetEncoding(852),   // CP852
            Encoding.GetEncoding(1252)   // Windows-1252
        };

        string czechChars = "ěščřžýáíéůúňťďóĚŠČŘŽÝÁÍÉŮÚŇŤĎÓ";

        foreach (var encoding in encodingsToTry)
        {
            try
            {
                string content = File.ReadAllText(filePath, encoding);

                // Kontrola platnosti českých znaků
                int czechCharCount = content.Count(c => czechChars.Contains(c));
                double czechCharRatio = (double)czechCharCount / content.Length;

                // Pokud je dostatek českých znaků, považujeme kódování za správné
                if (czechCharRatio > 0.01) // 1% českých znaků
                {
                    return encoding;
                }
            }
            catch (Exception)
            {
                // Pokud se nepodaří přečíst soubor s daným kódováním, pokračujeme na další
                continue;
            }
        }

        return null;
    }
}