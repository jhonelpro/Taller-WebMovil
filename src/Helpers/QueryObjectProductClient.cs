
namespace api.src.Helpers
{
    public class QueryObjectProductClient
    {
        public string? productType { get; set; } = string.Empty;
        public string? sortByPrice { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
    }
}