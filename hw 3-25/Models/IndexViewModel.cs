using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hw_3_25.Models
{
    public class IndexViewModel
    {
        public List<BlogPost> Posts { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
