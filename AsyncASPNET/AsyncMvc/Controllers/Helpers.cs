using System;
using System.IO;
using System.Net;

namespace AsyncMvc.Controllers
{
    public static class Helpers
    {
        public static string GetResponseString(this WebResponse response)
        {
            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                return String.Empty;
            }
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}