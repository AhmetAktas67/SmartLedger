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
        private BuchungRepository _buchungRepository;

        private BeitragszahlungRepository _beitragsRepository;

        private KontoauszugService _kontoauszugService;
        private MatchingService _matchingService;

        private string _kassenauszugJahrFilter = "Alle";
        public string KassenauszugJahrFilter
        {
            get => _kassenauszugJahrFilter;
            set
            {
                _kassenauszugJahrFilter = value;
                OnPropertyChanged(nameof(KassenauszugJahrFilter));
                LadeKassenauszug();
            }
        }

        private string _kassenauszugMonatFilter = "Alle";
        public string KassenauszugMonatFilter
        {
            get => _kassenauszugMonatFilter;
            set
            {
                _kassenauszugMonatFilter = value;
                OnPropertyChanged(nameof(KassenauszugMonatFilter));
                LadeKassenauszug();
            }
        }

        private Mitglied _bearbeitetesMitglied;

        public ObservableCollection<MitgliedViewModel> Mitglieder { get; set; }
        public ObservableCollection<MitgliedMitZahlungenViewModel> Beitragsmatrix { get; set; }
        public ObservableCollection<Buchung> KassenauszugBuchungen { get; set; }

        public ObservableCollection<string> KassenauszugJahre { get; set; }
        public ObservableCollection<string> KassenauszugMonate { get; set; }

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

        public int GesamtMitglieder => Mitglieder.Count;

        public int BeitraegeBestaetigt => Beitragsmatrix
        .SelectMany(m => new[] { m.Januar, m.Februar, m.Maerz, m.April, m.Mai, m.Juni,
                                  m.Juli, m.August, m.September, m.Oktober, m.November, m.Dezember })
        .Count(status => status == ZahlungsStatus.BestaetigtManuell);

            public int OffeneBeitraege => Beitragsmatrix
                .SelectMany(m => new[] { m.Januar, m.Februar, m.Maerz, m.April, m.Mai, m.Juni,
                                  m.Juli, m.August, m.September, m.Oktober, m.November, m.Dezember })
                .Count(status => status == ZahlungsStatus.Offen);

            public int VorschlaegeKI => Beitragsmatrix
                .SelectMany(m => new[] { m.Januar, m.Februar, m.Maerz, m.April, m.Mai, m.Juni,
                                  m.Juli, m.August, m.September, m.Oktober, m.November, m.Dezember })
                .Count(status => status == ZahlungsStatus.VorschlagKI);



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

        public ICommand ImportKontoauszugCommand { get; set; }

        private string _importStatus;
        public string ImportStatus
        {
            get => _importStatus;
            set
            {
                _importStatus = value;
                OnPropertyChanged(nameof(ImportStatus));
            }
        }

        public ICommand EditMitgliedCommand { get; set; }

        public MainViewModel()
        {
            _repository = new MitgliedRepository();
            _buchungRepository = new BuchungRepository();
            _beitragsRepository = new BeitragszahlungRepository();
            _kontoauszugService = new KontoauszugService(); 
            _matchingService = new MatchingService();

            Mitglieder = new ObservableCollection<MitgliedViewModel>();
            Beitragsmatrix = new ObservableCollection<MitgliedMitZahlungenViewModel>();
            KassenauszugBuchungen = new ObservableCollection<Buchung>();

            VerfuegbareJahre = new ObservableCollection<int>();
            int aktuellesJahr = DateTime.Now.Year;
            for (int jahr = aktuellesJahr - 2; jahr <= aktuellesJahr + 1; jahr++)
            {
                VerfuegbareJahre.Add(jahr);
            }

            KassenauszugJahre = new ObservableCollection<string> { "Alle" };
            for (int jahr = aktuellesJahr - 2; jahr <= aktuellesJahr + 1; jahr++)
            {
                KassenauszugJahre.Add(jahr.ToString());
            }

            KassenauszugMonate = new ObservableCollection<string>
                {
                    "Alle", "Januar", "Februar", "März", "April", "Mai", "Juni",
                    "Juli", "August", "September", "Oktober", "November", "Dezember"
                };

            _ausgewaehltesJahr = aktuellesJahr;


            LadeAlles();  

            AddMitgliedCommand = new RelayCommand(AddOderUpdateMitglied);
            DeleteMitgliedCommand = new RelayCommand<int>(DeleteMitglied);
            EditMitgliedCommand = new RelayCommand<int>(StarteBearbeitung);
            ImportKontoauszugCommand = new RelayCommand(ImportKontoauszug);

            NavigateCommand = new RelayCommand<string>(seite =>
            {
                AktuelleSeite = int.Parse(seite);
                if (AktuelleSeite == 0) 
                {
                    OnPropertyChanged(nameof(GesamtMitglieder));
                    OnPropertyChanged(nameof(BeitraegeBestaetigt));
                    OnPropertyChanged(nameof(OffeneBeitraege));
                    OnPropertyChanged(nameof(VorschlaegeKI));
                }
            });

          
        }

       
        private void LadeAlles()
        {
            Mitglieder.Clear();
            Beitragsmatrix.Clear();

            var alleMitglieder = _repository.GetAlle();

            foreach (var mitglied in alleMitglieder)
            {
                _beitragsRepository.ErstelleJahrFuerMitglied(mitglied.Id, AusgewaehltesJahr);
            }

            var alleZahlungen = _beitragsRepository.GetFuerJahr(AusgewaehltesJahr);

            foreach (var mitglied in alleMitglieder)
            {
                Mitglieder.Add(new MitgliedViewModel(mitglied));

                var zahlungenDesMitglieds = alleZahlungen
                    .Where(z => z.MitgliedId == mitglied.Id)
                    .ToList();

                if (zahlungenDesMitglieds.Count == 12)
                {
                    Beitragsmatrix.Add(new MitgliedMitZahlungenViewModel(mitglied, zahlungenDesMitglieds, _beitragsRepository));
                }
            }

            OnPropertyChanged(nameof(GesamtMitglieder));
            OnPropertyChanged(nameof(BeitraegeBestaetigt));
            OnPropertyChanged(nameof(OffeneBeitraege));
            OnPropertyChanged(nameof(VorschlaegeKI));

            LadeKassenauszug();
        }

        private void LadeKassenauszug()
        {
            KassenauszugBuchungen.Clear();
            var alle = _buchungRepository.GetAlle();

            var gefiltert = alle.AsEnumerable();

            if (KassenauszugJahrFilter != "Alle")
            {
                int jahr = int.Parse(KassenauszugJahrFilter);
                gefiltert = gefiltert.Where(b => b.BuchungsDatum.Year == jahr);
            }

            if (KassenauszugMonatFilter != "Alle")
            {
                string[] monatsNamen = { "", "Januar", "Februar", "März", "April", "Mai", "Juni",
                                  "Juli", "August", "September", "Oktober", "November", "Dezember" };
                int monatIndex = Array.IndexOf(monatsNamen, KassenauszugMonatFilter);
                gefiltert = gefiltert.Where(b => b.BuchungsDatum.Month == monatIndex);
            }

            foreach (var b in gefiltert)
            {
                KassenauszugBuchungen.Add(b);
            }
        }


        private void ImportKontoauszug()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF-Dateien (*.pdf)|*.pdf",
                Title = "Kontoauszug auswählen"
            };




            if (dialog.ShowDialog() != true) return;

            ImportStatus = "Importiere und analysiere...";

            try
            {
                var buchungen = _kontoauszugService.LeseBuchungenAus(dialog.FileName);
                var alleMitglieder = _repository.GetAlle();
                var matches = _matchingService.MatcheBuchungen(buchungen, alleMitglieder);
                int anzahl = _matchingService.WendeMatchesAn(matches, _beitragsRepository);

                foreach (var match in matches)
                {
                    match.Buchung.ZugeordneteMitgliederNamen = match.GematchteMitglieder.Count > 0
                        ? string.Join(", ", match.GematchteMitglieder.Select(m => $"{m.Vorname} {m.Nachname}"))
                        : "Kein Treffer";

                    _buchungRepository.Speichern(match.Buchung);
                }

                ImportStatus = $"{buchungen.Count} Buchungen erkannt, {anzahl} neue KI-Vorschläge gesetzt.";

                LadeAlles();
            }
            catch (Exception ex)
            {
                ImportStatus = $"Fehler beim Import: {ex.Message}";
            }
        }



        private void StarteBearbeitung(int mitgliedId)
        {
            var alleMitglieder = _repository.GetAlle();
            _bearbeitetesMitglied = alleMitglieder.First(m => m.Id == mitgliedId);

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

        private void DeleteMitglied(int mitgliedId)
        {
            _repository.Loeschen(mitgliedId);
            LadeAlles();
        }
    }
}