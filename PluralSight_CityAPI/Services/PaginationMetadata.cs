namespace PluralSight_CityAPI.Services
{
    public class PaginationMetadata
    {
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; set; }
        public int PageSize { get; set; }
        public int CurPage { get; set; }

        public PaginationMetadata(int totalItem, int pageSize, int curPage) {
            TotalItemCount = totalItem;
            PageSize = pageSize;
            TotalPageCount = (int) Math.Ceiling(1.0 * totalItem/ pageSize);
            CurPage = curPage;
        }
    }
}
