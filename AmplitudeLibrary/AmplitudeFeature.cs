using AmplitudeFeatureLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AmplitudeFeatureLibrary
{
    public class AmplitudeFeature : IAmplitudeFeature
    {
        private const string API_URI = "https://api.lab.amplitude.com/v1/vardata";
        private const string DEFAULT_USER_ID = "DEFAULT-USER";
        private static HttpClient client;
        private readonly string apiKey;

        public AmplitudeFeature(string apiKey) : this(apiKey, new WebRequestHandler())
        {
        }

        public AmplitudeFeature(string apiKey, HttpMessageHandler httpMessageHandler)
        {
            this.apiKey = apiKey;
            SetupClient(httpMessageHandler);
        }

        public bool FeatureIsEnabled(string flagName, string userId = DEFAULT_USER_ID)
        {
            var response = GetApiResponseAsync(flagName, userId).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return ParseFlagResponse(content, flagName);
        }

        public async Task<bool> FeatureIsEnabledAsync(string flagName, string userId = DEFAULT_USER_ID)
        {
            var response = await GetApiResponseAsync(flagName, userId);
            var content = response.Content.ReadAsStringAsync();
            return ParseFlagResponse(await content, flagName);
        }

        private void SetupClient(HttpMessageHandler httpMessageHandler)
        {
            if (client == null)
            {
                client = new HttpClient(httpMessageHandler);
                client.DefaultRequestHeaders.Add("Authorization", $"Api-Key {apiKey}");
            }
        }

        private string GetApiUri(string flagName, string userId)
        {
            var uriBuilder = new UriBuilder(API_URI)
            {
                Query = $"user_id={userId}&flag_key={flagName}"
            };

            return uriBuilder.ToString();
        }

        private Task<HttpResponseMessage> GetApiResponseAsync(string flagName, string userId)
        {
            var path = GetApiUri(flagName, userId);
            return client.GetAsync(path);
        }

        private bool ParseFlagResponse(string response, string flagName)
        {
            var flagDictionary = JsonConvert.DeserializeObject<Dictionary<string, FeatureFlagValue>>(response);

            FeatureFlagValue featureFlag = null;
            if (flagDictionary.ContainsKey(flagName))
            {
                featureFlag = flagDictionary[flagName];
            }

            if (featureFlag == null || string.IsNullOrWhiteSpace(featureFlag.Value))
            {
                return false;
            }

            return this.StringIsTruthy(featureFlag.Value);
        }

        private bool StringIsTruthy(string value)
        {
            var truthyValues = new List<string> { "on", "true", "yes", "1" };
            if (truthyValues.Contains(value.Trim().ToLower()))
            {
                return true;
            }

            return false;
        }
    }
}
