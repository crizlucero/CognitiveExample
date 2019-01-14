using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CognitiveExample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrincipalPageMaster : ContentPage
    {
        public ListView ListView;

        public PrincipalPageMaster()
        {
            InitializeComponent();

            BindingContext = new PrincipalPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class PrincipalPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<PrincipalPageMenuItem> MenuItems { get; set; }

            public PrincipalPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<PrincipalPageMenuItem>(new[]
                {
                    new PrincipalPageMenuItem { Id = 0, Title = "Speech Recognition" },
                    new PrincipalPageMenuItem { Id = 1, Title = "Spell Recognition" },
                    new PrincipalPageMenuItem { Id = 2, Title = "Translate" },
                    new PrincipalPageMenuItem { Id = 3, Title = "Face Recognition" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}