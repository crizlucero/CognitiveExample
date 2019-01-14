using CognitiveExample.Services;
using CognitiveUtils.Services;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CognitiveExample.Views.Cognitive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpellRecognition : ContentPage
    {
        IBingSpellCheckService bingSpellCheckService;

        public static readonly BindableProperty IsProcessingProperty =
            BindableProperty.Create("IsProcessing", typeof(bool), typeof(SpellRecognition), false);

        public bool IsProcessing
        {
            get { return (bool)GetValue(IsProcessingProperty); }
            set { SetValue(IsProcessingProperty, value); }
        }

        public SpellRecognition()
        {
            InitializeComponent();
            bingSpellCheckService = new BingSpellCheckService();
        }

        protected async void OnSpellCheckButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    IsProcessing = true;
                    var spellCheckResult = await bingSpellCheckService.SpellCheckTextAsync(txtInput.Text);
                    foreach (var flaggedToken in spellCheckResult.FlaggedTokens)
                        txtOutput.Text += txtInput.Text.Replace(flaggedToken.Token, flaggedToken.Suggestions.FirstOrDefault().Suggestion);

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