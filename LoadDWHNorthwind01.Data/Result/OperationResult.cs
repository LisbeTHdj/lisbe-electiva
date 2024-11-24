using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHNorthwind01.Data.Result
{
    public class OperationResult
    {
        public OperationResult() 
        {
            this.Success = true;
        }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
