using CognitiveExample.Services;
using CognitiveUtils.Services;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CognitiveExample.Views.Cognitive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Translate : ContentPage
    {
        ITextTranslationService textTranslationService;

        public static readonly BindableProperty IsProcessingProperty =
            BindableProperty.Create("IsProcessing", typeof(bool), typeof(Translate), false);

        public bool IsProcessing
        {
            get { return (bool)GetValue(IsProcessingProperty); }
            set { SetValue(IsProcessingProperty, value); }
        }

        public Translate()
        {
            InitializeComponent();
            textTranslationService = new TextTranslationService(new AuthenticationService(Constants.TextTranslatorApiKey));
        }

        async void OnTranlsateButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    IsProcessing = true;
                    txtOutput.Text = await textTranslationService.TranslateTextAsync(txtInput.Text);
                    IsProcessing = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}