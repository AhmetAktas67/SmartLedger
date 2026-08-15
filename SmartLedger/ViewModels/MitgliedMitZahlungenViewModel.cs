using SmartLedger.Models;
using SmartLedger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartLedger.ViewModels
{
    public class MitgliedMitZahlungenViewModel : BaseViewModel
    {
        private BeitragszahlungRepository _beitragsRepository; 

        public int Id { get; set; } 
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public decimal Monatsbeitrag { get; set; } 
        public string? Haushaltsgruppe { get; set; } 

        private Dictionary<int, Beitragszahlung> _zahlungenProMonat;

        public ICommand ToggleMonatCommand { get; set; } 

        public MitgliedMitZahlungenViewModel(Mitglied mitglied, List<Beitragszahlung> zahlungenDesMitglieds, BeitragszahlungRepository beitragsRepository)
        {
            Id = mitglied.Id;
            Vorname = mitglied.Vorname;
            Nachname = mitglied.Nachname;
            Monatsbeitrag = mitglied.Monatsbeitrag;
            Haushaltsgruppe = mitglied.Haushaltsgruppe;

            _beitragsRepository = beitragsRepository;
            _zahlungenProMonat = zahlungenDesMitglieds.ToDictionary(z => z.Monat, z => z);

            ToggleMonatCommand = new RelayCommand<string>(ToggleMonat); 
        }

        
        public ZahlungsStatus Januar => _zahlungenProMonat[1].Status;
        public ZahlungsStatus Februar => _zahlungenProMonat[2].Status;
        public ZahlungsStatus Maerz => _zahlungenProMonat[3].Status;
        public ZahlungsStatus April => _zahlungenProMonat[4].Status;
        public ZahlungsStatus Mai => _zahlungenProMonat[5].Status;
        public ZahlungsStatus Juni => _zahlungenProMonat[6].Status;
        public ZahlungsStatus Juli => _zahlungenProMonat[7].Status;
        public ZahlungsStatus August => _zahlungenProMonat[8].Status;
        public ZahlungsStatus September => _zahlungenProMonat[9].Status;
        public ZahlungsStatus Oktober => _zahlungenProMonat[10].Status;
        public ZahlungsStatus November => _zahlungenProMonat[11].Status;
        public ZahlungsStatus Dezember => _zahlungenProMonat[12].Status;

        
        private void ToggleMonat(string monatStr)
        {
            int monat = int.Parse(monatStr);
            var zahlung = _zahlungenProMonat[monat];

            zahlung.Status = zahlung.Status == ZahlungsStatus.BestaetigtManuell
                ? ZahlungsStatus.Offen
                : ZahlungsStatus.BestaetigtManuell;

            _beitragsRepository.Speichern(zahlung);

            OnPropertyChanged(MonatsName(monat));
        }

        
        private string MonatsName(int monat)
        {
            string[] namen = { "", "Januar", "Februar", "Maerz", "April", "Mai", "Juni",
                                "Juli", "August", "September", "Oktober", "November", "Dezember" };
            return namen[monat];
        }
    }
}