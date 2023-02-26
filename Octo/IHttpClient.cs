using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octo
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri, string auth);
        Task<string> GetStringAsync(string uri);
    }
}
