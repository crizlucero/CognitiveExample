using CognitiveUtils.Services;
using CognitiveUtils.Utils;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace CognitiveExample.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        string token;
        Timer accessTokenRenewer;
        const int RefreshTokenDureation = 9;
        readonly HttpClient httpClient;

        public AuthenticationService(string apiKey)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        }

        public string GetAccessToken() => token;

        public async Task InitializeAsync()
        {
            token = await FetchTokenAsync(Constants.AuthenticationTokenEndpoint);
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallBack), this, TimeSpan.FromMinutes(RefreshTokenDureation), TimeSpan.FromMilliseconds(-1));

        }

        async Task OnTokenExpiredCallBack(object state)
        {
            try
            {
                await RenewerAccessToken();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to renew access token. Details {e.Message}");
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDureation), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to reschedule the timer to renew the access token. Details: {e.Message}");
                }
            }
        }

        async Task RenewerAccessToken() =>
            token = await FetchTokenAsync(Constants.AuthenticationTokenEndpoint);

        async Task<string> FetchTokenAsync(string fetchUri)
        {
            UriBuilder uriBuilder = new UriBuilder(fetchUri);
            uriBuilder.Path += "/issueToken";
            var result = await httpClient.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
            return await result.Content.ReadAsStringAsync();
        }

        public async Task InitializeSpeechAsync()
        {
            token = await FetchTokenAsync(Constants.SpeechAuthenticationTokenEndpoint);
            accessTokenRenewer = new Timer(new TimerCallback(OnSpeechTokenExpiredCallBack), this, TimeSpan.FromMinutes(RefreshTokenDureation), TimeSpan.FromMilliseconds(-1));
        }

        async Task OnSpeechTokenExpiredCallBack(object state)
        {
            try
            {
                await RenewerSpeechAccessToken();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to renew access token. Details {e.Message}");
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDureation), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to reschedule the timer to renew the access token. Details: {e.Message}");
                }
            }
        }

        async Task RenewerSpeechAccessToken() =>
            token = await FetchTokenAsync(Constants.SpeechAuthenticationTokenEndpoint);
    }
}
