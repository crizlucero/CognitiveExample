using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CognitiveUtils.Models;
using CognitiveUtils.Services;
using Newtonsoft.Json;

namespace CognitiveExample.Services
{
    public class BingSpeechService : IBingSpeechService
    {
        IAuthenticationService authenticationService;
        HttpClient httpClient;

        public BingSpeechService(IAuthenticationService authenticationService) =>
            this.authenticationService = authenticationService;

        public async Task<SpeechResult> RecognizeSpeechAsync(string filename)
        {
            if (string.IsNullOrWhiteSpace(authenticationService.GetAccessToken()))
                await this.authenticationService.InitializeSpeechAsync();

            var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(filename);
            var fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read);

            string requestUri = GenerateRequestUri(Constants.SpeechRecognitionEndpoint);
            string accessToken = authenticationService.GetAccessToken();
            var response = await SendRequestAsync(fileStream, requestUri, accessToken, Constants.AudioContentType);
            var speechResult = JsonConvert.DeserializeObject<SpeechResult>(response);

            fileStream.Dispose();
            return speechResult;
        }

        async Task<string> SendRequestAsync(Stream fileStream, string requestUri, string accessToken, string audioContentType)
        {
            if (httpClient == null)
                httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StreamContent(fileStream);
            content.Headers.TryAddWithoutValidation("Content-Type", audioContentType);
            var response = await httpClient.PostAsync(requestUri, content);
            return await response.Content.ReadAsStringAsync();
        }

        string GenerateRequestUri(string speechRecognitionEndpoint) =>
           $"{speechRecognitionEndpoint}?language=es-MX&format-simple";
    }
}
