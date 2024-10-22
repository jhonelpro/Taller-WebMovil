
namespace api.src.Helpers
{
    public class QueryObjectUsers
    {
        public string? Name { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}