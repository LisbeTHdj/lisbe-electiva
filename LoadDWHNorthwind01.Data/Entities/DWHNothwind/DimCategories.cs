using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHNorthwind01.Data.Entities.DWHNothwind
{
    [Table("DimCategories")]
    public class DimCategories
    {
        [Key]
        public int CategoryKey { get; set; }
        public string? CategoryName { get; set; }

    }
}
