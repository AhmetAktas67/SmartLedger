using Azure;
using Azure.AI.DocumentIntelligence;
using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartLedger.Services
{
    public class KontoauszugService
    {
        public string TesteImport(string pdfPfad)
        {
            var client = new DocumentIntelligenceClient(
                new Uri(AzureConfig.DocIntelEndpoint),
                new AzureKeyCredential(AzureConfig.DocIntelKey));

            using var stream = File.OpenRead(pdfPfad);

            var operation = client.AnalyzeDocument(
                WaitUntil.Completed,
                "prebuilt-layout",
                BinaryData.FromStream(stream));

            var result = operation.Value;

            var sb = new StringBuilder();
            sb.AppendLine($"Seiten erkannt: {result.Pages.Count}");
            sb.AppendLine($"Tabellen erkannt: {result.Tables.Count}");

            if (result.Tables.Count > 0)
            {
                var ersteTabelle = result.Tables[0];
                sb.AppendLine($"\nErste Tabelle: {ersteTabelle.RowCount} Zeilen, {ersteTabelle.ColumnCount} Spalten\n");

                foreach (var cell in ersteTabelle.Cells)
                {
                    sb.AppendLine($"Zeile {cell.RowIndex}, Spalte {cell.ColumnIndex}: {cell.Content}");
                }
            }

            return sb.ToString();
        }


        public List<Buchung> LeseBuchungenAus(string pdfPfad)
        {
            var client = new DocumentIntelligenceClient(
                new Uri(AzureConfig.DocIntelEndpoint),
                new AzureKeyCredential(AzureConfig.DocIntelKey));

            using var stream = File.OpenRead(pdfPfad);

            var operation = client.AnalyzeDocument(
                WaitUntil.Completed,
                "prebuilt-layout",
                BinaryData.FromStream(stream));

            var result = operation.Value;
            var buchungen = new List<Buchung>();

            int jahr = ErmittleJahrAusDokument(result);

            var zeilen = result.Content
                .Split('\n')
                .Select(z => z.Trim())
                .Where(z => !string.IsNullOrWhiteSpace(z))
                .ToList();

            // <== GEÄNDERT: Muster erkennt nur die Datums-Startzeile, OHNE Betrag am Ende
            var buchungsStartMuster = new Regex(@"^(\d{2}\.\d{2}\.)\s+(\d{2}\.\d{2}\.)\s+(.+)$");

            // <== NEU: eigenes Muster für die Betragszeile
            var betragMuster = new Regex(@"^([\d\.]+,\d{2})\s*([HS])$");

            Buchung aktuelleBuchung = null;
            var verwendungszweckZeilen = new List<string>();
            bool wartetAufBetrag = false;

            void SchliesseAktuelleBuchungAb()
            {
                if (aktuelleBuchung != null && aktuelleBuchung.Betrag > 0)
                {
                    aktuelleBuchung.Verwendungszweck = string.Join(" ", verwendungszweckZeilen).Trim();
                    if (!string.IsNullOrWhiteSpace(aktuelleBuchung.Verwendungszweck))
                    {
                        buchungen.Add(aktuelleBuchung);
                    }
                }
            }

            foreach (var zeile in zeilen)
            {
                var startMatch = buchungsStartMuster.Match(zeile);
                var betragMatch = betragMuster.Match(zeile);

                if (startMatch.Success)
                {
                    // Neue Buchung beginnt -> alte zuerst abschließen
                    SchliesseAktuelleBuchungAb();

                    string datumText = startMatch.Groups[1].Value;
                    DateTime? datum = ParseDatum(datumText, jahr);

                    if (datum != null)
                    {
                        aktuelleBuchung = new Buchung { BuchungsDatum = datum.Value };
                        verwendungszweckZeilen = new List<string> { startMatch.Groups[3].Value };
                        wartetAufBetrag = true;
                    }
                    else
                    {
                        aktuelleBuchung = null;
                        wartetAufBetrag = false;
                    }
                }
                else if (wartetAufBetrag && betragMatch.Success)
                {
                    // Diese Zeile enthält den Betrag zur aktuellen Buchung
                    decimal? betrag = ParseBetrag(betragMatch.Groups[1].Value);
                    if (aktuelleBuchung != null && betrag != null)
                    {
                        aktuelleBuchung.Betrag = betrag.Value;
                    }
                    wartetAufBetrag = false;
                }
                else if (aktuelleBuchung != null)
                {
                    if (zeile.StartsWith("Übertrag") || zeile.Contains("Kontonummer") || zeile.Contains("erstellt am") || zeile.Contains("Bank 1 Saar"))
                    {
                        continue;
                    }
                    verwendungszweckZeilen.Add(zeile);
                }
            }

            SchliesseAktuelleBuchungAb();

            return buchungen;
        }


        private DateTime? ParseDatum(string datumText, int jahr)
        {
            
            var teile = datumText.TrimEnd('.').Split('.');
            if (teile.Length != 2) return null;

            if (int.TryParse(teile[0], out int tag) && int.TryParse(teile[1], out int monat))
            {
                try
                {
                    return new DateTime(jahr, monat, tag);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        
        private decimal? ParseBetrag(string betragText)
        {
            
            string bereinigt = Regex.Replace(betragText, @"[^\d,\.]", "");
            bereinigt = bereinigt.Replace(",", ".");

            if (decimal.TryParse(bereinigt, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal betrag))
            {
                return betrag;
            }
            return null;
        }

        public string TesteSpaltenzahlen(string pdfPfad)
        {
            var client = new DocumentIntelligenceClient(
                new Uri(AzureConfig.DocIntelEndpoint),
                new AzureKeyCredential(AzureConfig.DocIntelKey));

            using var stream = File.OpenRead(pdfPfad);
            var operation = client.AnalyzeDocument(WaitUntil.Completed, "prebuilt-layout", BinaryData.FromStream(stream));
            var result = operation.Value;

            var sb = new StringBuilder();
            int i = 0;
            foreach (var tabelle in result.Tables)
            {
                sb.AppendLine($"Tabelle {i}: {tabelle.RowCount} Zeilen, {tabelle.ColumnCount} Spalten");
                i++;
            }
            return sb.ToString();
        }

        private int ErmittleJahrAusDokument(AnalyzeResult result)
        {
            // Durchsucht den gesamten erkannten Text nach einem Datum im Format TT.MM.JJJJ
            var match = Regex.Match(result.Content, @"\d{2}\.\d{2}\.(\d{4})");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            // Fallback: aktuelles Jahr, falls nichts gefunden wird
            return DateTime.Now.Year;
        }


        public string TesteRohtext(string pdfPfad)
        {
            var client = new DocumentIntelligenceClient(
                new Uri(AzureConfig.DocIntelEndpoint),
                new AzureKeyCredential(AzureConfig.DocIntelKey));

            using var stream = File.OpenRead(pdfPfad);
            var operation = client.AnalyzeDocument(WaitUntil.Completed, "prebuilt-layout", BinaryData.FromStream(stream));
            var result = operation.Value;

            return result.Content.Substring(0, Math.Min(2000, result.Content.Length));
        }
    }
}