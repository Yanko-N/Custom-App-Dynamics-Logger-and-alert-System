
namespace Domain.Interfaces
{
    public interface IPagination
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
