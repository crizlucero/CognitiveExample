using CognitiveExample.Services;
using CognitiveUtils.Services;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CognitiveExample.Views.Cognitive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpeechRecognition : ContentPage
    {
        IBingSpeechService bingSpeechService;
        bool isRecording = false;

        public static readonly BindableProperty IsProcessingProperty =
            BindableProperty.Create("IsProcessing", typeof(bool), typeof(SpeechRecognition), false);

        public bool IsProcessing
        {
            get { return (bool)GetValue(IsProcessingProperty); }
            set { SetValue(IsProcessingProperty, value); }
        }

        public SpeechRecognition()
        {
            InitializeComponent();
            bingSpeechService = new BingSpeechService(new AuthenticationService(Constants.BingSpeechApiKey));
        }

        private async void OnRecognizeSpeechButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var audioRecordingService = DependencyService.Get<IAudioRecorderService>();
                if (!isRecording)
                {
                    audioRecordingService.StartRecording();
                    ((Button)sender).Image = "recording.png";
                    IsProcessing = true;
                }
                else
                {
                    audioRecordingService.StopRecording();
                }

                isRecording = !isRecording;
                if (!isRecording)
                {
                    var speechResult = await bingSpeechService.RecognizeSpeechAsync(Constants.AudioFilename);
                    Debug.WriteLine($"Name: {speechResult.DisplayText}");
                    Debug.WriteLine($"Recognition status: {speechResult.RecognitionStatus}");

                    if (!string.IsNullOrWhiteSpace(speechResult.DisplayText))
                        txtOutput.Text = speechResult.DisplayText;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (!isRecording)
                {
                    ((Button)sender).Image = "record.png";
                    IsProcessing = false;
                }
            }
        }
    }
}