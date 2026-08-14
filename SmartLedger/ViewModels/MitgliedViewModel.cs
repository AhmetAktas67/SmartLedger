using SmartLedger.Models;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.ViewModels
{
    public class MitgliedViewModel : BaseViewModel
    {
        private Mitglied _mitglied;
        public int Id => _mitglied.Id;

        public MitgliedViewModel(Mitglied mitglied)
        {
            _mitglied = mitglied;
        }

        public string Vorname
        {
            get => _mitglied.Vorname;
            set
            {
                _mitglied.Vorname = value;
                OnPropertyChanged(nameof(Vorname));
            }
        }

        public string Nachname
        {
            get => _mitglied.Nachname;
            set
            {
                _mitglied.Nachname = value;
                OnPropertyChanged(nameof(Nachname));
            }
        }

        public decimal Monatsbeitrag
        {
            get => _mitglied.Monatsbeitrag;
            set
            {
                _mitglied.Monatsbeitrag = value;
                OnPropertyChanged(nameof(Monatsbeitrag));
            }
        }
    }
}