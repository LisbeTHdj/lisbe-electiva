using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHNorthwind01.Data.Entities.DWHNothwind
{
    [Table("DimProducts")]
    public class DimProducts
    {
        [Key]
        public int ProductKey { get; set; }
        public string? ProductID { get; set; }
        public string? Name { get; set; }
        public int? CategoryKey { get; set; }
    }
}
