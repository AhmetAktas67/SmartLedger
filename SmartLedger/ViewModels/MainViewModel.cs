using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<MitgliedViewModel> Mitglieder { get; set; }

        public MainViewModel()
        {
            Mitglieder = new ObservableCollection<MitgliedViewModel>();

            Mitglieder.Add(new MitgliedViewModel(new Mitglied
            {
                Vorname = "Hans",
                Nachname = "Müller",
                Monatsbeitrag = 10
            }));

            Mitglieder.Add(new MitgliedViewModel(new Mitglied
            {
                Vorname = "Erika",
                Nachname = "Müller",
                Monatsbeitrag = 50
            }));

            Mitglieder.Add(new MitgliedViewModel(new Mitglied
            {
                Vorname = "Peter",
                Nachname = "Schmidt",
                Monatsbeitrag = 10
            }));
        }
    }
}