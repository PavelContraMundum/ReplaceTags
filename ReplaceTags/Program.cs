using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        // Registrace poskytovatelů kódování
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string inputFilePath = @"D:\Downloads\GenModified.txt";
        string outputFilePath = @"D:\Downloads\GenModifiedFormatted.txt";

        try
        {
            // Detekce kódování souboru
            Encoding encoding = DetectFileEncoding(inputFilePath);

            // Čtení souboru s detekovaným kódováním
            string content = File.ReadAllText(inputFilePath, encoding);

            // Úpravy obsahu
            content = Regex.Replace(content, "<italic>", "\\it");
            content = Regex.Replace(content, "</italic>", "\\it*");
            content = Regex.Replace(content, "<p/>", "\\p");
            content = Regex.Replace(content, "<kap/>", "\\cl_");
            content = Regex.Replace(content, @"<vers n=""(\d+)""/>", @"\v_$1_");

            // Zápis souboru se stejným kódováním
            File.WriteAllText(outputFilePath, content, encoding);

            Console.WriteLine($"Úpravy byly úspěšně provedeny. Výstup byl uložen do: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Došlo k chybě: {ex.Message}");
        }
    }

    // Metoda pro detekci kódování souboru
    private static Encoding DetectFileEncoding(string filePath)
    {
        // Čtení prvních několika bytů souboru pro detekci BOM
        using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            var bom = new byte[4];
            reader.Read(bom, 0, 4);

            // UTF-8 BOM
            if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                return Encoding.UTF8;

            // UTF-16 LE BOM
            if (bom[0] == 0xFF && bom[1] == 0xFE)
                return Encoding.Unicode;

            // UTF-16 BE BOM
            if (bom[0] == 0xFE && bom[1] == 0xFF)
                return Encoding.BigEndianUnicode;

            // Pokud není BOM, pokusíme se o heuristickou detekci
            return DetectEncodingHeuristic(filePath);
        }
    }

    // Heuristická detekce kódování
    private static Encoding DetectEncodingHeuristic(string filePath)
    {
        // Pokud není BOM, můžeme se pokusit o heuristickou detekci kódování
        // Zde přidáme logiku, která zkontroluje obsah souboru
        // Můžete přidat další detekce dle potřeby
        using (var reader = new StreamReader(filePath, Encoding.Default, detectEncodingFromByteOrderMarks: false))
        {
            // Zjednodušená heuristika, kontrola obsahuje-li text v CP1250 nebo UTF-8
            string firstLine = reader.ReadLine();
            if (firstLine != null && firstLine.Contains("some UTF-8 specific pattern"))
                return Encoding.UTF8;

            return Encoding.GetEncoding(1250); // Defaultně vrátí CP1250
        }
    }
}
