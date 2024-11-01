
namespace api.src.Helpers
{
    public class QueryObjectProductClient
    {
        public string? productType { get; set; } = string.Empty;
        public string? sortByPrice { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}