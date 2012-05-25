using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SignalR.Web;

namespace AsyncMvc.Controllers
{
    public class AsyncController : TaskAsyncController
    {
        public async Task<ActionResult> Index()
        {
            var result = await FetchResult();
            return Content(result);
        }

        private async Task<string> FetchResult()
        {
            try
            {
                var webRequest = WebRequest.CreateHttp(Urls.RequestUrl);
                using (var response = await webRequest.GetResponseAsync())
                {
                    var result = response.GetResponseString();
                    return result;
                }
            }
            catch
            {
                return "Error";
            }
        }
    }
}