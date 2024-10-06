using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Helpers
{
    public class QueryObject
    {
        public string? textFilter { get; set; }= string.Empty;
        public string? productType { get; set; } = string.Empty;
        public string? sortByPrice { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}