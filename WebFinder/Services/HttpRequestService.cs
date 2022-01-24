using System.Net;
using WebFinder.Controllers;

namespace WebFinder.Services
{
    public class HttpRequestService : BackgroundService
    {
        private readonly ILogger<HttpRequestService> _logger;
        //private readonly LdlcFinderController _ldlcFinderController;

        public HttpRequestService(/*LdlcFinderController ldlcFinderController, */ILogger<HttpRequestService> logger)
        {
            //this._ldlcFinderController = ldlcFinderController;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //_ldlcFinderController.Get("rtx 3090");
                try
                {
                    _logger.LogInformation(GetSourceCode("https://localhost:7092/LdlcFinder?name=3090"));
                }
                catch (Exception ex)
                {
                    _logger.LogError("Impossible d'automatiser la requête");
                }
                await Task.Delay(3000, cancellationToken);
            }
        }

        private string GetSourceCode(string url)
        {
            string htmlCode;
            using (WebClient client = new WebClient())
            {
                htmlCode = client.DownloadString(url);
            }

            return htmlCode;
        }
    }
}
