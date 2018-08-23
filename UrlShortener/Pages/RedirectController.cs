using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrlShortener.Data;

namespace UrlShortener.Pages
{
    public class RedirectController : Controller
    {
        private readonly IUrlRepo _urlRepo;
        private ILogger _log = null;
        public TheUrl theUrl { get; set; }

        public RedirectController(IUrlRepo urlRepo, ILogger<UrlRepo> logger)
        {
            _urlRepo = urlRepo;
            _log = logger;
        }

        public async Task<IActionResult> s(string hash)
        {
            theUrl = await _urlRepo.GetUrlByShortUrl(hash);

            if (theUrl == null)
            {
                _log.LogWarning($"Redirect failed. The url could not be found.");
                return Content($"Redirect failed. The url could not be found.");
            }

            theUrl.Counter = theUrl.Counter + 1;
            theUrl.LastAccessed = DateTime.Now;

            await _urlRepo.UpdateUrl(theUrl.Id, theUrl);

            if (!theUrl.OriginalUrl.Contains("http://") || !theUrl.OriginalUrl.Contains("https://"))
            {
                return Redirect("http://" + theUrl.OriginalUrl);
            }

            return Redirect(theUrl.OriginalUrl);
        }
    }
}