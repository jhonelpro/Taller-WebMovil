namespace api.src.Helpers
{
    public class QueryObjectPurchase
    {
        public string? clientName { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string? orderByDate { get; set; }
        public bool isDescending { get; set; } = false;
    }
}