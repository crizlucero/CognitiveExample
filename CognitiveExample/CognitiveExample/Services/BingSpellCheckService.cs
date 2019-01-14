using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CognitiveUtils.Models;
using CognitiveUtils.Services;
using Newtonsoft.Json;

namespace CognitiveExample.Services
{
    public class BingSpellCheckService : IBingSpellCheckService
    {
        readonly HttpClient httpClient;

        public BingSpellCheckService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constants.BingSpellCheckApiKey);
        }
        public async Task<SpellCheckResult> SpellCheckTextAsync(string text)
        {
            string requestUri = GenerateRequestUri(Constants.BingSpellCheckEndpoint, text, SpellCheckMode.Spell);
            var response = await SendRequestAsync(requestUri);
            return JsonConvert.DeserializeObject<SpellCheckResult>(response);
        }

        async Task<string> SendRequestAsync(string requestUri)
        {
            var response = await httpClient.GetAsync(requestUri);
            return await response.Content.ReadAsStringAsync();
        }

        private string GenerateRequestUri(string bingSpellCheckEndpoint, string text, SpellCheckMode spell) =>
            $"{bingSpellCheckEndpoint}?text={WebUtility.UrlEncode(text)}&mode={spell.ToString().ToLower()}&cc=MX&mkt=es-MX";
    }
}
