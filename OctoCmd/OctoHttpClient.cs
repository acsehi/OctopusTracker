using Octo;

namespace OctoCmd
{
    internal class OctoHttpClient : IHttpClient
    {

        private string apiKey;
        public OctoHttpClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public Task<string> GetStringAsync(string uri, string apiKey)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Base64Encode(apiKey));
            return httpClient.GetStringAsync(uri);
        }

        public Task<string> GetStringAsync(string uri)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Base64Encode(this.apiKey));
            return httpClient.GetStringAsync(uri);
        }

        public static string Base64Encode(string text)
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }
    }
}
