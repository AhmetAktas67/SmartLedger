using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.ViewModels
{
    public class MitgliedMitZahlungenViewModel : BaseViewModel
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }

        private Dictionary<int, Beitragszahlung> _zahlungenProMonat;

        public MitgliedMitZahlungenViewModel(Mitglied mitglied, List<Beitragszahlung> zahlungenDesMitglieds)
        {
            Vorname = mitglied.Vorname;
            Nachname = mitglied.Nachname;
            _zahlungenProMonat = zahlungenDesMitglieds.ToDictionary(z => z.Monat, z => z);
        }

        public bool Januar
        {
            get => _zahlungenProMonat[1].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[1].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Januar));
            }
        }

        public bool Februar
        {
            get => _zahlungenProMonat[2].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[2].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Februar));
            }
        }

        public bool Maerz
        {
            get => _zahlungenProMonat[3].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[3].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Maerz));
            }
        }

        public bool April
        {
            get => _zahlungenProMonat[4].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[4].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(April));
            }
        }

        public bool Mai
        {
            get => _zahlungenProMonat[5].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[5].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Mai));
            }
        }

        public bool Juni
        {
            get => _zahlungenProMonat[6].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[6].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Juni));
            }
        }

        public bool Juli
        {
            get => _zahlungenProMonat[7].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[7].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Juli));
            }
        }

        public bool August
        {
            get => _zahlungenProMonat[8].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[8].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(August));
            }
        }

        public bool September
        {
            get => _zahlungenProMonat[9].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[9].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(September));
            }
        }

        public bool Oktober
        {
            get => _zahlungenProMonat[10].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[10].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Oktober));
            }
        }

        public bool November
        {
            get => _zahlungenProMonat[11].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[11].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(November));
            }
        }

        public bool Dezember
        {
            get => _zahlungenProMonat[12].Status == ZahlungsStatus.BestaetigtManuell;
            set
            {
                _zahlungenProMonat[12].Status = value ? ZahlungsStatus.BestaetigtManuell : ZahlungsStatus.Offen;
                OnPropertyChanged(nameof(Dezember));
            }
        }
    }
}