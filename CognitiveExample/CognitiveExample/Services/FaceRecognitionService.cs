using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CognitiveUtils.Exceptions;
using CognitiveUtils.Models.Face;
using CognitiveUtils.Services;
using CognitiveUtils.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CognitiveExample.Services
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        HttpClient httpClient;

        static JsonSerializerSettings s_settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public FaceRecognitionService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("ocp-apim-subscription-key", Constants.FaceApiKey);
        }

        ~FaceRecognitionService() => Dispose(false);

        string GetAttributeString(IEnumerable<FaceAttributeType> types) =>
            string.Join(",", types.Select(a => a.ToString()).ToArray());

        public async Task<Face[]> DetectAsync(Stream imageStream, bool returnFaceId, bool returnFaceLandmarks, IEnumerable<FaceAttributeType> returnFaceAttributes)
        {
            var requestUrl = $"{Constants.FaceEndpoint}/detect?returnFaceId={returnFaceId}&returnFaceLandmarks={returnFaceLandmarks}&returnFaceAttributes={GetAttributeString(returnFaceAttributes)}";
            return await SendRequestAsync<Stream, Face[]>(HttpMethod.Post, requestUrl, imageStream);
        }

        async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod post, string requestUrl, TRequest imageStream)
        {
            var request = new HttpRequestMessage(post, Constants.FaceEndpoint)
            {
                RequestUri = new Uri(requestUrl)
            };
            if (imageStream != null)
            {
                if (imageStream is Stream)
                {
                    request.Content = new StreamContent(imageStream as Stream);
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Constants.OctetStreamContentType);
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(imageStream, s_settings), Encoding.UTF8, Constants.JsonContentType);
                }
            }
            HttpResponseMessage responseMessage = await httpClient.SendAsync(request);
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (responseMessage.Content != null)
                    responseContent = await responseMessage.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseContent))
                    return JsonConvert.DeserializeObject<TResponse>(responseContent, s_settings);
                return default(TResponse);
            }
            else
            {
                if (responseMessage.Content != null && responseMessage.Content.Headers.ContentType.MediaType.Contains(Constants.JsonContentType))
                {
                    string error = await responseMessage.Content.ReadAsStringAsync();
                    ClientError ex = JsonConvert.DeserializeObject<ClientError>(error);
                    if (ex.Error != null)
                        throw new FaceAPIException(ex.Error.ErrorCode, ex.Error.Message, responseMessage.StatusCode);
                    else
                    {
                        ServiceError serviceEx = JsonConvert.DeserializeObject<ServiceError>(error);
                        if (ex != null)
                            throw new FaceAPIException(serviceEx.ErrorCode, serviceEx.Message, responseMessage.StatusCode);
                        else
                            throw new FaceAPIException("Unknown", "Unknown Error", responseMessage.StatusCode);
                    }
                }
                responseMessage.EnsureSuccessStatusCode();
            }
            return default(TResponse);

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                httpClient?.Dispose();
                httpClient = null;
            }
        }
    }
}
