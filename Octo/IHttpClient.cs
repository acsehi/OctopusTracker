namespace Octo
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri, string auth);
        Task<string> GetStringAsync(string uri);
    }
}
