using System;
using System.Text.Json.Serialization;

namespace ugh.Api
{
    public class GitHubApiResponse
    {
        [Serializable()]
        public class User
        {
            [JsonPropertyName("login")] public string Login { get; set; }
            [JsonPropertyName("name")] public string Name { get; set; }
            [JsonPropertyName("followers")] public int Followers { get; set; }
            [JsonPropertyName("following")] public int Following { get; set; }
            [JsonPropertyName("public_repos")] public int Repos { get; set; }
            [JsonPropertyName("bio")] public string Bio { get; set; }
        }

        public class Repository
        {
            [JsonPropertyName("full_name")] public Uri FullName { get; set; }
            [JsonPropertyName("description")] public string Description { get; set; }
            [JsonPropertyName("watchers")] public int Watchers { get; set; }
            [JsonPropertyName("stargazers_count")] public int Stars { get; set; }
            [JsonPropertyName("open_issues_count")] public int Issues { get; set; }
            [JsonPropertyName("pushed_at")] public DateTime PushTime { get; set; }
            [JsonPropertyName("html_url")] public Uri HtmlUrl { get; set; }
            [JsonPropertyName("ssh_url")] public Uri SshUrl { get; set; }
            [JsonPropertyName("clone_url")] public Uri HttpUrl { get; set; }
        }

        public class Key
        {
            [JsonPropertyName("key")] public string Value { get; set; }
        }

        public class Issue
        {
            [JsonPropertyName("number")] public int Number { get; set; }
            [JsonPropertyName("title")] public int Title { get; set; }
            [JsonPropertyName("user")] public User User { get; set; }
            [JsonPropertyName("state")] public string State { get; set; }
        }

        [Serializable()]
        public class Token
        {
            [JsonPropertyName("access_token")] public string Value { get; set; }
        }
    }
}