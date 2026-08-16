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

            if (result.Tables.Count == 0) return buchungen;

            int jahr = ErmittleJahrAusDokument(result); // <== NEU: automatisch erkanntes Jahr

            var tabelle = result.Tables[0];
            var zeilenGruppen = tabelle.Cells
                .GroupBy(c => c.RowIndex)
                .OrderBy(g => g.Key);

            foreach (var zeile in zeilenGruppen)
            {
                if (zeile.Key == 0) continue;

                var zellen = zeile.OrderBy(c => c.ColumnIndex).ToList();
                if (zellen.Count < 4) continue;

                string datumText = zellen[0].Content?.Trim();
                string verwendungszweck = zellen[2].Content?.Trim();
                string betragText = zellen[3].Content?.Trim();

                if (string.IsNullOrWhiteSpace(datumText) || !datumText.Contains(".")) continue;
                if (string.IsNullOrWhiteSpace(verwendungszweck)) continue;
                if (string.IsNullOrWhiteSpace(betragText)) continue;

                DateTime? datum = ParseDatum(datumText, jahr); // <== GEÄNDERT: nutzt jetzt automatisch erkanntes Jahr
                decimal? betrag = ParseBetrag(betragText);

                if (datum == null || betrag == null) continue;

                buchungen.Add(new Buchung
                {
                    BuchungsDatum = datum.Value,
                    Verwendungszweck = verwendungszweck,
                    Betrag = betrag.Value
                });
            }

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
    }
}