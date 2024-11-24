using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind01.Data.Entities.DWHNorthwind
{
    [Table("DimEmployees")] 
    public class DimEmployees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int EmployeeKey { get; set; }

        [StringLength(50)] 
        public string? LastName { get; set; }

        [StringLength(50)] 
        public string? FirstName { get; set; }
    }
}
