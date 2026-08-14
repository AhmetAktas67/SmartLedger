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

        public ObservableCollection<MitgliedViewModel> Mitglieder { get; set; }

        public string NeuerVorname { get; set; }
        public string NeuerNachname { get; set; }
        public decimal NeuerBeitrag { get; set; }

        public ICommand AddMitgliedCommand { get; set; }

        public MainViewModel()
        {
            _repository = new MitgliedRepository();
            Mitglieder = new ObservableCollection<MitgliedViewModel>();

            var alleMitglieder = _repository.GetAlle();
            foreach (var mitglied in alleMitglieder)
            {
                Mitglieder.Add(new MitgliedViewModel(mitglied));
            }

            AddMitgliedCommand = new RelayCommand(AddMitglied);
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
            Mitglieder.Add(new MitgliedViewModel(neuesMitglied));
        }
    }
}