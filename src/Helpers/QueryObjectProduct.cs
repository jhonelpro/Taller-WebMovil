
namespace api.src.Helpers
{
    public class QueryObjectProduct
    {
        public string? textFilter { get; set; }= string.Empty;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}