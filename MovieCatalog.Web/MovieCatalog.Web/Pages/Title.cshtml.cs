using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;
using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Web.Pages
{
    [BindProperties]
    public class TitleModel : PageModel
    {
        private readonly IMovieCatalogDataService _dataService;

        public TitleModel(IMovieCatalogDataService dataService)
        {
            _dataService = dataService;
        }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        [Required, StringLength(500)]
        public string PrimaryTitle { get; set; } = string.Empty;

        [StringLength(500)]
        public string? OriginalTitle { get; set; }

        [Required]
        public TitleType TitleType { get; set; }

        [Range(1900, 2100)]
        public int? StartYear { get; set; }

        [Range(1900, 2100)]
        public int? EndYear { get; set; }

        [Range(1, 9999)]
        public int? RuntimeMinutes { get; set; }

        [MaxLength(3, ErrorMessage = "Maximum 3 mûfaj választható.")]
        public List<int> Genres { get; set; } = new List<int>();

        public List<SelectListItem> GenreOptions { get; set; } = new List<SelectListItem>();

        [TempData]
        public string? SuccessMessage { get; set; }
        public async Task OnGetAsync()
        {
            var allGenres = await _dataService.GetGenresAsync();
            GenreOptions = allGenres
                .Select(g => new SelectListItem(g.Name, g.Id.ToString(), Genres.Contains(g.Id)))
                .ToList();

            if (Id != null)
            {
                var title = await _dataService.GetTitleByIdAsync(Id.Value);
                PrimaryTitle = title.PrimaryTitle;
                OriginalTitle = title.OriginalTitle;
                TitleType = title.TitleType;
                StartYear = title.StartYear;
                EndYear = title.EndYear;
                RuntimeMinutes = title.RuntimeMinutes;
                Genres = title.TitleGenres.Select(tg => tg.GenreId).ToList();

                foreach (var item in GenreOptions)
                {
                    item.Selected = Genres.Contains(int.Parse(item.Value));
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var allGenres = await _dataService.GetGenresAsync();
            GenreOptions = allGenres
                .Select(g => new SelectListItem(g.Name, g.Id.ToString(), Genres.Contains(g.Id)))
                .ToList();

            if (!ModelState.IsValid)
                return Page();

            var updatedTitle = await _dataService.InsertOrUpdateTitleAsync(
                Id,
                PrimaryTitle,
                OriginalTitle,
                TitleType,
                StartYear,
                EndYear,
                RuntimeMinutes,
                Genres.ToArray()
            );

            SuccessMessage = Id == null ? "Film létrehozva!" : "Film módosítva!";
            return RedirectToPage("/Title", new { Id = updatedTitle.Id });
        }
    }
}
