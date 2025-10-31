using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieCatalog.Web.Utils;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;

namespace MovieCatalog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMovieCatalogDataService _dataService;

        public Dictionary<Genre, int> GenresWithCounts { get; set; } = new Dictionary<Genre, int>();
        public PagedResult<Title> Titles { get; set; } = PagedResult<Title>.Empty;

        public IndexModel(IMovieCatalogDataService dataService)
        {
            _dataService = dataService;
        }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public TitleSort TitleSort { get; set; } = TitleSort.ReleaseYear;

        [BindProperty(SupportsGet = true)]
        public bool SortDescending { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        public TitleFilter Filter { get; set; } = TitleFilter.Empty;

        public IReadOnlyList<int> PageNumberOptions =>
            new[]{
                1, 2, 3, PageNumber - 1, PageNumber,    PageNumber + 1, Titles.LastPageNumber -    1,
                Titles.LastPageNumber, Titles.  LastPageNumber + 1
            }
            .Where(i => i > 0 && i <= Titles.LastPageNumber + 1)
            .Distinct()
            .OrderBy(i => i)
            .ToArray();

        public async Task<IActionResult> OnGetAsync()
        {
            GenresWithCounts = await _dataService.GetGenresWithTitleCountsAsync();

            if (!Request.Query.Any())
            {
                return RedirectToPage("/Index", new
                {
                    PageSize,
                    PageNumber,
                    TitleSort,
                    SortDescending
                });
            }

            Titles = await _dataService.GetTitlesAsync(
                pageSize: PageSize,
                page: PageNumber,
                filter: Filter,
                titleSort: TitleSort,
                sortDescending: SortDescending
            );

            return Page();
        }
    }
}
