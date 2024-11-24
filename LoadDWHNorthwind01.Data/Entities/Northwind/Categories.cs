using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  // Agrega este espacio de nombres
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHNorthwind01.Data.Entities.Northwind
{
    [Table("Categories")]
    public class Categories
    {
        [Key]  // Este atributo proviene de System.ComponentModel.DataAnnotations
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public string? Picture { get; set; }
    }
}
