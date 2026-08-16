using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Models
{
    public class Buchung
    {
        public int Id { get; set; }
        public DateTime BuchungsDatum { get; set; }
        public string Verwendungszweck { get; set; }
        public decimal Betrag { get; set; }


        public string ZugeordneteMitgliederNamen { get; set; }
    }
}
