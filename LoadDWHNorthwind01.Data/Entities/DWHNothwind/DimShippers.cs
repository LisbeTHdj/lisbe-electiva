using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind01.Data.Entities.DWHNothwind
{
    [Table("DimShippers")]
    public class DimShippers
    {
        [Key]
        public int ShipViaKey { get; set; }
        public string? Name { get; set; }

    }
}
