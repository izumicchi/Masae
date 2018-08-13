using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Masae.Services
{
    class SearchService
    {
        public static async Task<Stream> GetResponseStream(string v)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(v);
            try
            {
                return (await (webRequest).GetResponseAsync()).GetResponseStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetResponse Stream " + ex);
                return null;
            }
        }

        public static async Task<string> GetResponseAsync(string v) =>
            await new StreamReader((await ((HttpWebRequest)WebRequest.Create(v)).GetResponseAsync()).GetResponseStream()).ReadToEndAsync();

        public static async Task<string> GetResponseAsync(string v, WebHeaderCollection headers)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(v);
            webRequest.Headers = headers;
            return await new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()).ReadToEndAsync();
        }
    }
}