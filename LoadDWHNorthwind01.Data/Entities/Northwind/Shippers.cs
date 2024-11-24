using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind01.Data.Entities.Northwind
{
    [Table("Shippers")]
    public class Shippers
    {
        [Key] 
        public int ShipperID { get; set; }

        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
    }
}
