using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHNorthwind01.Data.Entities.DWHNothwind
{
    [Table("DimCustomers")]
    public class DimCustomers
    {
        [Key]
        public int CustKey { get; set; }
        public string CustName { get; set; }
        public string CustID { get; set; }


    }
}
