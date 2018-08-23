using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrlShortener.Data;
using UrlShortener.General;

namespace UrlShortener.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUrlRepo _urlRepo;
        private ILogger _log = null;
        private IHttpContextAccessor _accessor;

        public IndexModel(IUrlRepo urlRepo, ILogger<UrlRepo> logger, IOptions<Settings> settings, IHttpContextAccessor accessor)
        {
            _urlRepo = urlRepo;
            _log = logger;
            _accessor = accessor;
        }


        [BindProperty]
        [MaxLength(1000)]
        [Display(Name = "Input your valid URL here.")]
        public string InputUrl { get; set; }

        [BindProperty]
        public TheUrl newUrl { get; set; }

        [BindProperty]
        public IEnumerable<TheUrl> savedUrls { get; set; }

        [TempData]
        public string Message { get; set; }

        public async Task OnGetAsync()
        {
            savedUrls = await _urlRepo.GetAllUrls();
            savedUrls = savedUrls.OrderBy(x => x.CreatedDate).Take(15).ToList();

            if (savedUrls == null)
            {
                savedUrls = new List<TheUrl>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            InputUrl = InputUrl.Trim().ToLower();

            string hash = Functions.GenerateRandomHash(5);
            var existingUrl = await _urlRepo.GetUrlByOriginalUrl(hash);

            do
            {
                hash = Functions.GenerateRandomHash(5);
                var exits = await _urlRepo.GetUrlByShortUrl(hash);
                if(exits == null)
                {
                    break;
                }
            } while (existingUrl != null && hash == existingUrl.ShortUrl);

            if (!ModelState.IsValid)
            {
                Message = "Something went wrong, please try again";

                return Page();
            }
            else if (!Functions.CheckValidUrl(InputUrl))
            {
                Message = "Something went wrong, your url is not valid and could not be validated before being posted to the server. Please try again.";
                _log.LogWarning("Javascript validation failed.");

                return Page();
            }

            newUrl.ShortUrl = hash;
            newUrl.OriginalUrl = InputUrl;
            newUrl.CreatedDate = DateTime.Now;
            newUrl.Counter = 0;
            newUrl.IP = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            await _urlRepo.AddUrl(newUrl);

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var theUrl = await _urlRepo.RemoveUrl(id);

            return RedirectToPage("/Index");
        }

    }
}
