using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CognitiveUtils.Services;
using CognitiveUtils.Models;
using Newtonsoft.Json;

namespace CognitiveExample.Services
{
    public class TextTranslationService : ITextTranslationService
    {
        IAuthenticationService authenticationService;
        HttpClient httpClient;

        public TextTranslationService(IAuthenticationService authenticationService) =>
            this.authenticationService = authenticationService;
        public async Task<string> TranslateTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(authenticationService.GetAccessToken()))
                await authenticationService.InitializeAsync();

            string requestUri = GenerateRequestUri(Constants.TextTranslatorEndpoint, "en", "es");
            string accessToken = authenticationService.GetAccessToken();
            var response = await SendRequestAsync(requestUri, accessToken, text);
            var result = JsonConvert.DeserializeObject<List<TranslateModel>>(response);

            return result[0].translations[0].text;
        }

        async Task<string> SendRequestAsync(string requestUri, string accessToken, string text)
        {
            if (httpClient == null)
                httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent("[{'Text':'" + text + "'}]", Encoding.UTF8, Constants.JsonContentType);

            var response = await httpClient.PostAsync(requestUri, content);
            return await response.Content.ReadAsStringAsync();
        }

        private string GenerateRequestUri(string textTranslatorEndpoint, string from, string to) =>
            $"{textTranslatorEndpoint}&from={from}&to={to}";
    }
}
