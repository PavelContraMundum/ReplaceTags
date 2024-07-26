using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        string inputFilePath = GetFilePath("Zadejte cestu k vstupnímu souboru: ");
        string outputFilePath = GetFilePath("Zadejte cestu k výslednému souboru: ");

        try
        {
            // Zkusíme různá kódování
            Encoding[] encodingsToTry = new Encoding[]
            {
                Encoding.GetEncoding(1250),  // Windows-1250
                Encoding.GetEncoding(28592), // ISO-8859-2
                Encoding.GetEncoding(852),   // CP852
                Encoding.UTF8,
                Encoding.GetEncoding(1252)   // Windows-1252
            };

            string content = null;
            Encoding detectedEncoding = null;

            foreach (var encoding in encodingsToTry)
            {
                content = File.ReadAllText(inputFilePath, encoding);
                if (content.Contains("Výčet Jákobových synů"))
                {
                    detectedEncoding = encoding;
                    break;
                }
            }

            if (detectedEncoding == null)
            {
                Console.WriteLine("Nepodařilo se detekovat správné kódování. Zkontrolujte vstupní soubor.");
                return;
            }

            Console.WriteLine($"Detekované kódování: {detectedEncoding.EncodingName}");

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

            // Kontrolní vypís části obsahu
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
}