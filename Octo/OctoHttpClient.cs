namespace Octo
{
    public class OctoHttpClient : IHttpClient
    {

        private string apiKey;
        public OctoHttpClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public Task<string> GetStringAsync(string uri, string apiKey)
        {
            HttpClient httpClient = GetClient(apiKey);
            return httpClient.GetStringAsync(uri);
        }

        public Task<string> GetStringAsync(string uri)
        {
            HttpClient httpClient = GetClient(apiKey);
            return httpClient.GetStringAsync(uri);
        }

        private static HttpClient GetClient(string apiKey)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Base64Encode(apiKey));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("OctoWeb", "1.0"));
            return httpClient;
        }

        public static string Base64Encode(string text)
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }
    }
}
