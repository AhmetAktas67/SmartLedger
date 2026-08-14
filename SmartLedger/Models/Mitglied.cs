using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Models
{
    public class Mitglied
    {
       public string Vorname {  get; set; }
        public string Nachname { get; set; }
        public decimal Monatsbeitrag { get; set; }
        public string? Haushaltsgruppe { get; set; }

    }
}
