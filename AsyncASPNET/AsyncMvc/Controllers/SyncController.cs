using System.Net;
using System.Web.Mvc;

namespace AsyncMvc.Controllers
{
    public class SyncController : Controller
    {
        public ActionResult Index()
        {
            var result = FetchResult();
            return Content(result);
        }

        private string FetchResult()
        {
            try
            {
                var webRequest = WebRequest.CreateHttp(Urls.RequestUrl);
                using (var response = webRequest.GetResponse())
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
