using SmartLedger.Models;
using SmartLedger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartLedger.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private MitgliedRepository _repository;
        private BeitragszahlungRepository _beitragsRepository;

        public ObservableCollection<MitgliedViewModel> Mitglieder { get; set; }
        public ObservableCollection<MitgliedMitZahlungenViewModel> Beitragsmatrix { get; set; }

        public string NeuerVorname { get; set; }
        public string NeuerNachname { get; set; }
        public decimal NeuerBeitrag { get; set; }

        public ICommand AddMitgliedCommand { get; set; }

        public MainViewModel()
        {
            _repository = new MitgliedRepository();
            _beitragsRepository = new BeitragszahlungRepository();  // <== GEFEHLT

            Mitglieder = new ObservableCollection<MitgliedViewModel>();
            Beitragsmatrix = new ObservableCollection<MitgliedMitZahlungenViewModel>();  // <== GEFEHLT

            LadeAlles();  // <== ersetzt die alte foreach-Schleife, die hier vorher stand

            AddMitgliedCommand = new RelayCommand(AddMitglied);
        }

        private void LadeAlles()
        {
            Mitglieder.Clear();
            Beitragsmatrix.Clear();

            var alleMitglieder = _repository.GetAlle();
            var alleZahlungen = _beitragsRepository.GetFuerJahr(DateTime.Now.Year);

            foreach (var mitglied in alleMitglieder)
            {
                Mitglieder.Add(new MitgliedViewModel(mitglied));

                var zahlungenDesMitglieds = alleZahlungen
                    .Where(z => z.MitgliedId == mitglied.Id)
                    .ToList();

                if (zahlungenDesMitglieds.Count == 12)
                {
                    Beitragsmatrix.Add(new MitgliedMitZahlungenViewModel(mitglied, zahlungenDesMitglieds));
                }
            }
        }

        private void AddMitglied()
        {
            var neuesMitglied = new Mitglied
            {
                Vorname = NeuerVorname,
                Nachname = NeuerNachname,
                Monatsbeitrag = NeuerBeitrag
            };

            _repository.Speichern(neuesMitglied);
            _beitragsRepository.ErstelleJahrFuerMitglied(neuesMitglied.Id, DateTime.Now.Year);

            LadeAlles();  // <== GEÄNDERT (vorher: Mitglieder.Add(new MitgliedViewModel(neuesMitglied));)
        }
    }
}