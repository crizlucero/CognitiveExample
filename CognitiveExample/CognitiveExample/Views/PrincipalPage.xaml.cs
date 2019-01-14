using CognitiveExample.Views.Cognitive;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CognitiveExample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrincipalPage : MasterDetailPage
    {
        public PrincipalPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as PrincipalPageMenuItem;
            if (item == null)
                return;

            Page page;
            switch (item.Id)
            {
                case 0: page = (Page)Activator.CreateInstance(typeof(SpeechRecognition)); break;
                case 1: page = (Page)Activator.CreateInstance(typeof(SpellRecognition)); break;
                case 2: page = (Page)Activator.CreateInstance(typeof(Translate)); break;
                case 3: page = (Page)Activator.CreateInstance(typeof(FaceRecognition)); break;
                default: page = (Page)Activator.CreateInstance(item.TargetType); break;
            }
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}