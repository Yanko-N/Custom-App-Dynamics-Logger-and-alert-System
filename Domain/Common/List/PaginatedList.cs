using Domain.Interfaces;

namespace Domain.Common.List
{
    public class PaginatedList<T> : List<T>, IPagination
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public PaginatedList(IEnumerable<T> items, int totalCount, int currentPage, int take)
        {
            TotalCount = totalCount;
            Take = take;
            CurrentPage = currentPage;
            Skip = (currentPage - 1) * take;
            TotalPages = (int)Math.Ceiling(totalCount / (double)take);

            AddRange(items);
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public static PaginatedList<T> Create(IEnumerable<T> source, int currentPage, int take)
        {
            var list = source.ToList();
            var count = list.Count;
            var items = list.Skip((currentPage - 1) * take).Take(take);
            return new PaginatedList<T>(items, count, currentPage, take);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int currentPage, int take)
        {
            var count = source.Count();
            var items = source.Skip((currentPage - 1) * take).Take(take).ToList();
            return new PaginatedList<T>(items, count, currentPage, take);
        }
    }
}