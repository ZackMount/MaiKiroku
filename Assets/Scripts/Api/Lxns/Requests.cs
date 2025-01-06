using Assets.Scripts.Api.Lxns.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Api.Lxns
{
    public class Requests
    {
        private readonly Request request;

        /// <summary>
        /// 初始化 Requests 实例
        /// </summary>
        /// <param name="token">开发者 API 密钥（必须）</param>
        public Requests(string token)
        {
            request = new Request(token);
        }
        public async Task<Best50> GetBest50Async(string friend_code)
        {
            var resultContent = await request.GetAsync($"api/v0/maimai/player/{friend_code}/bests");
            var apiMessageResult = JsonUtility.FromJson<ApiMessageResult<Best50>>(resultContent);
            return apiMessageResult.data;
        }

        public async Task<(Best50, Player)> GetBest50ByQQAsync(string qq)
        {
            var player = await GetPlayerByQQAsync(qq);
            var resultContent = await request.GetAsync($"api/v0/maimai/player/{player.friend_code}/bests");
            var apiMessageResult = JsonUtility.FromJson<ApiMessageResult<Best50>>(resultContent);
            return (apiMessageResult.data, player);
        }
        public async Task<Player> GetPlayer(string friend_code)
        {
            var resultContent = await request.GetAsync($"api/v0/maimai/player/{friend_code}");
            var apiMessageResult = JsonUtility.FromJson<ApiMessageResult<Player>>(resultContent);
            return apiMessageResult.data;
        }

        public async Task<Player> GetPlayerByQQAsync(string qq)
        {
            var resultContent = await request.GetAsync($"api/v0/maimai/player/qq/{qq}");
            var apiMessageResult = JsonUtility.FromJson<ApiMessageResult<Player>>(resultContent);
            return apiMessageResult.data;
        }
        public async Task<Song> GetSongAsync(int song_id)
        {
            var resultContent = await request.GetAsync($"api/v0/maimai/song/{song_id}");
            var apiMessageResult = JsonUtility.FromJson<Song>(resultContent);
            return apiMessageResult;
        }

        private class Request
        {
            private readonly HttpClient client = new();
            private const string baseUrl = "https://maimai.lxns.net";
            private readonly string token;

            public Request(string token)
            {
                this.token = token;
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            }

            public async Task<string> GetAsync(string path)
                => await SendRequestAsync(path, HttpMethod.Get);

            private async Task<string> SendRequestAsync(string path, HttpMethod method, HttpContent content = null)
            {
                var url = $"{baseUrl}/{path}";

                var httpResponse = method switch
                {
                    _ when method == HttpMethod.Get => client.GetAsync(url).Result,
                    _ when method == HttpMethod.Post => client.PostAsync(url, content).Result,
                    _ => throw new NotSupportedException($"Method {method} not supported"),
                };

                var resultContent = await httpResponse.Content.ReadAsStringAsync();

                return resultContent;
            }
        }
    }
}
