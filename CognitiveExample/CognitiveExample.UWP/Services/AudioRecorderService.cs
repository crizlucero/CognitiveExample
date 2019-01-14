using CognitiveExample.UWP.Services;
using CognitiveUtils.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioRecorderService))]
namespace CognitiveExample.UWP.Services
{
    public class AudioRecorderService : IAudioRecorderService
    {
        AudioGraph audioGraph;
        AudioFileOutputNode audioFileOutputNode;
        StorageFile recordingFile = null;

        public void StartRecording() =>
            StartRecordingAsync();

        public void StopRecording()
        {
            var task = Task.Run(async () => await StopRecordingAsync());
            task.Wait();
        }

        async Task StartRecordingAsync()
        {
            try
            {
                recordingFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(Constants.AudioFilename, CreationCollisionOption.ReplaceExisting);
                Debug.WriteLine(recordingFile.Path);

                var result = await AudioGraph.CreateAsync(new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media));
                if (result.Status == AudioGraphCreationStatus.Success)
                {
                    audioGraph = result.Graph;

                    var microphone = await DeviceInformation.CreateFromIdAsync(MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default));
                    var outputProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);
                    outputProfile.Audio = AudioEncodingProperties.CreatePcm(16000, 1, 16);

                    var inputProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.High);
                    var outputResult = await audioGraph.CreateFileOutputNodeAsync(recordingFile, outputProfile);

                    if (outputResult.Status == AudioFileNodeCreationStatus.Success)
                    {
                        audioFileOutputNode = outputResult.FileOutputNode;

                        var inputResult = await audioGraph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Media, inputProfile.Audio, microphone);
                        if (inputResult.Status == AudioDeviceNodeCreationStatus.Success)
                        {
                            inputResult.DeviceInputNode.AddOutgoingConnection(audioFileOutputNode);
                            audioGraph.Start();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        async Task StopRecordingAsync()
        {
            if (audioGraph != null)
            {
                audioGraph.Stop();
                await audioFileOutputNode.FinalizeAsync();
                audioGraph.Dispose();
                audioGraph = null;
                Debug.WriteLine("Stopped recording.");
            }
        }
    }
}
