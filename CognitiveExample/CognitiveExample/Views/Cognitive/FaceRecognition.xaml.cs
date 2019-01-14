using CognitiveExample.Services;
using CognitiveUtils.Exceptions;
using CognitiveUtils.Models.Face;
using CognitiveUtils.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CognitiveExample.Views.Cognitive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FaceRecognition : ContentPage
    {
        IFaceRecognitionService faceRecognitionService;
        MediaFile photo;
        public FaceRecognition()
        {
            InitializeComponent();
            faceRecognitionService = new FaceRecognitionService();
        }

        async void OnTakePhotoButtonClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (CrossMedia.Current.IsCameraAvailable || CrossMedia.Current.IsTakePhotoSupported)
            {
                photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Name = "emotion.jpg",
                    PhotoSize = PhotoSize.Small
                });
                if (photo != null)
                    image.Source = ImageSource.FromStream(photo.GetStream);
            }
            else
                await DisplayAlert("No Camera", "Camera unavailable", "Ok");

            ((Button)sender).IsEnabled = false;
            activityIndicator.IsRunning = true;

            try
            {
                if (photo != null)
                {
                    var faceAttributes = new FaceAttributeType[] { FaceAttributeType.Emotion };
                    using (var photoStream = photo.GetStream())
                    {
                        Face[] faces = await faceRecognitionService.DetectAsync(photoStream, true, false, faceAttributes);
                        if (faces.Any())
                            emotionResultLabel.Text = faces.FirstOrDefault().FaceAttributes.Emotion.ToRankedList().FirstOrDefault().Key;
                        photo.Dispose();
                    }
                }
            }
            catch (FaceAPIException fx)
            {
                Debug.WriteLine(fx.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            activityIndicator.IsRunning = false;
            ((Button)sender).IsEnabled = true;
        }

    }
}