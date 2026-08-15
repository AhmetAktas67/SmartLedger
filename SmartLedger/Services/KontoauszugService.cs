using Azure;
using Azure.AI.DocumentIntelligence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            sb.AppendLine($"Erkannter Text (erste 2000 Zeichen):");
            sb.AppendLine(result.Content.Length > 2000 ? result.Content.Substring(0, 2000) : result.Content);

            return sb.ToString();
        }
    }
}