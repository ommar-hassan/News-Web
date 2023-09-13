namespace NewsApp.Models
{
    public class NewsIndexViewModel
    {
        public List<NewsViewModel> News { get; set; }
        public string Search { get; set; }
        public string SortType { get; set; }
        public string SortOrder { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
