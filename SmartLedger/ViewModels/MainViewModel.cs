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

        private Mitglied _bearbeitetesMitglied;

        public ObservableCollection<MitgliedViewModel> Mitglieder { get; set; }
        public ObservableCollection<MitgliedMitZahlungenViewModel> Beitragsmatrix { get; set; }

        public ObservableCollection<int> VerfuegbareJahre { get; set; }


        private int _aktuelleSeite = 1; 
        public int AktuelleSeite
        {
            get => _aktuelleSeite;
            set
            {
                _aktuelleSeite = value;
                OnPropertyChanged(nameof(AktuelleSeite));
            }
        }




        private string _fehlerText;
        public string FehlerText
        {
            get => _fehlerText;
            set
            {
                _fehlerText = value;
                OnPropertyChanged(nameof(FehlerText));
            }
        }

        private int _ausgewaehltesJahr; 
        public int AusgewaehltesJahr
        {
            get => _ausgewaehltesJahr;
            set
            {
                _ausgewaehltesJahr = value;
                OnPropertyChanged(nameof(AusgewaehltesJahr));
                LadeAlles();
            }
        }


        public string NeuerVorname { get; set; }
        public string NeuerNachname { get; set; }
        public decimal NeuerBeitrag { get; set; }

        private string _buttonText = "Hinzufügen";
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public ICommand AddMitgliedCommand { get; set; }
        public ICommand DeleteMitgliedCommand { get; set; }

        public ICommand NavigateCommand { get; set; }

        public ICommand EditMitgliedCommand { get; set; }

        public MainViewModel()
        {
            _repository = new MitgliedRepository();
            _beitragsRepository = new BeitragszahlungRepository();  

            Mitglieder = new ObservableCollection<MitgliedViewModel>();
            Beitragsmatrix = new ObservableCollection<MitgliedMitZahlungenViewModel>();

            VerfuegbareJahre = new ObservableCollection<int>();
            int aktuellesJahr = DateTime.Now.Year;
            for (int jahr = aktuellesJahr - 2; jahr <= aktuellesJahr + 1; jahr++)
            {
                VerfuegbareJahre.Add(jahr);
            }

            _ausgewaehltesJahr = aktuellesJahr;


            LadeAlles();  

            AddMitgliedCommand = new RelayCommand(AddOderUpdateMitglied);
            DeleteMitgliedCommand = new RelayCommand<MitgliedViewModel>(DeleteMitglied);
            EditMitgliedCommand = new RelayCommand<MitgliedViewModel>(StarteBearbeitung);
            NavigateCommand = new RelayCommand<string>(seite => AktuelleSeite = int.Parse(seite));
        }

        private void LadeAlles()
        {
            Mitglieder.Clear();
            Beitragsmatrix.Clear();

            var alleMitglieder = _repository.GetAlle();
            var alleZahlungen = _beitragsRepository.GetFuerJahr(AusgewaehltesJahr);

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

        private void StarteBearbeitung(MitgliedViewModel mitgliedVm)
        {
            if (mitgliedVm == null) return;

            var alleMitglieder = _repository.GetAlle();
            _bearbeitetesMitglied = alleMitglieder.First(m => m.Id == mitgliedVm.Id);

            NeuerVorname = _bearbeitetesMitglied.Vorname;
            OnPropertyChanged(nameof(NeuerVorname));

            NeuerNachname = _bearbeitetesMitglied.Nachname;
            OnPropertyChanged(nameof(NeuerNachname));

            NeuerBeitrag = _bearbeitetesMitglied.Monatsbeitrag;
            OnPropertyChanged(nameof(NeuerBeitrag));

            ButtonText = "Speichern";
        }

        private void AddOderUpdateMitglied()
        {
            if (string.IsNullOrWhiteSpace(NeuerVorname) || string.IsNullOrWhiteSpace(NeuerNachname))
            {
                FehlerText = "Vorname und Nachname dürfen nicht leer sein.";
                return;
            }

            if (NeuerBeitrag <= 0)
            {
                FehlerText = "Der Monatsbeitrag muss größer als 0 sein.";
                return;
            }

            FehlerText = "";


            if (_bearbeitetesMitglied != null)
            {
               
                _bearbeitetesMitglied.Vorname = NeuerVorname;
                _bearbeitetesMitglied.Nachname = NeuerNachname;
                _bearbeitetesMitglied.Monatsbeitrag = NeuerBeitrag;

                _repository.Aktualisieren(_bearbeitetesMitglied);

                _bearbeitetesMitglied = null;
                ButtonText = "Hinzufügen";
            }
            else
            {
                
                var neuesMitglied = new Mitglied
                {
                    Vorname = NeuerVorname,
                    Nachname = NeuerNachname,
                    Monatsbeitrag = NeuerBeitrag
                };

                _repository.Speichern(neuesMitglied);
                _beitragsRepository.ErstelleJahrFuerMitglied(neuesMitglied.Id, AusgewaehltesJahr);
            }

            
            NeuerVorname = "";
            OnPropertyChanged(nameof(NeuerVorname));
            NeuerNachname = "";
            OnPropertyChanged(nameof(NeuerNachname));
            NeuerBeitrag = 0;
            OnPropertyChanged(nameof(NeuerBeitrag));

            LadeAlles();
        }

        private void DeleteMitglied(MitgliedViewModel mitgliedVm)
        {
            if (mitgliedVm == null) return;

            _repository.Loeschen(mitgliedVm.Id);
            LadeAlles();
        }
    }
}