using System;
using System.Threading;
using System.Web.Mvc;
using System.Threading.Tasks;
using SignalR.Web;

namespace FlightInfoWebService.Controllers
{
    public class ServerController : TaskAsyncController
    {
        public async Task<ActionResult> Index()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return Content("Response from server application.");
        }
        
        public ActionResult IndexSync()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return Content("Response from server application.");
        }
    }
}
