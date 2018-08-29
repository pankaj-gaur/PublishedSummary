using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishedSummary.Models
{
   public class ItemSummary
    {
        public string title { get; set; }
        public int? page { get; set; }
        public int? componentTemplate { get; set; }

        public int? component { get; set; }

        public int? category { get; set; }
    }
}
