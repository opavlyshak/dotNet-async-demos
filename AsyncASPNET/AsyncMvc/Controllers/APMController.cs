using System;
using System.Net;
using System.Web.Mvc;

namespace AsyncMvc.Controllers
{
    public class APMController : AsyncController
    {
        public void IndexAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            var webRequest = WebRequest.CreateHttp(Urls.RequestUrl);
            webRequest.BeginGetResponse(asyncResult =>
                {
                    AsyncManager.Parameters["result"] = ReadResult(webRequest, asyncResult);
                    AsyncManager.OutstandingOperations.Decrement();
                }, null);
        }

        public ActionResult IndexCompleted(string result)
        {
            return Content(result);
        }

        private string ReadResult(WebRequest webRequest, IAsyncResult asyncResult)
        {
            try
            {
                using (var response = webRequest.EndGetResponse(asyncResult))
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