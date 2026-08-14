using SmartLedger.Models;
using SmartLedger.Services;
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
        private MitgliedRepository _repository;

        public ObservableCollection<MitgliedViewModel> Mitglieder { get; set; }

        public MainViewModel()
        {
            _repository = new MitgliedRepository();
            Mitglieder = new ObservableCollection<MitgliedViewModel>();

            var alleMitglieder = _repository.GetAlle();
            foreach (var mitglied in alleMitglieder)
            {
                Mitglieder.Add(new MitgliedViewModel(mitglied));
            }
        }
    }
}