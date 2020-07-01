using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;

namespace ugh.Api
{
    public partial class GitHubApi
    {
        private static readonly HttpClient Http = new HttpClient();
        public readonly string BaseUrl = "https://api.github.com";

        public readonly string ClientId;
        public readonly string ClientSecret;

        public GitHubApi(string clientId, string clientSecret)
        {
            Http.DefaultRequestHeaders.Accept.Clear();
            Http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            Http.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Http.DefaultRequestHeaders.Add("User-Agent", "ugh-client");
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
        }

        public void RemoveToken()
        {
            Http.DefaultRequestHeaders.Remove("Authorization");
        }

        public void SetToken(string token)
        {
            if (Http.DefaultRequestHeaders.Contains("Authorization")) RemoveToken();
            Http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", token);
        }

        private string BuildUri(params string[] parameters)
        {
            return parameters.Aggregate(
                $"{BaseUrl}",
                (url, parameter) => $"{url}/{parameter}");
        }

        private async Task<T> GetResponse<T>(params string[] parameters)
        {
            var streamTask = Http.GetStreamAsync(BuildUri(parameters));
            return await JsonSerializer.DeserializeAsync<T>(await streamTask);
        }

        private async Task<T> GetRestrictedResponse<T>(params string[] parameters)
        {
            if (Http.DefaultRequestHeaders.Contains("Authorization"))
                return await GetResponse<T>(parameters);
            throw new AuthenticationException("Не судьба. Надо авторизоваться!");
        }

        private async Task<T> PostResponse<T>(Dictionary<string, string> form, params string[] parameters)
        {
            var url = "";
            if (parameters?[0] != null)
            {
                if (parameters[0].StartsWith("http"))
                {
                    url = parameters[0];
                }
                else
                {
                    url = BuildUri(parameters);
                }
            }

            var postTask = await Http.PostAsync(url, new FormUrlEncodedContent(form));
            var streamTask = postTask.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(await streamTask);
        }
    }
}