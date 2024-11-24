using LoadDWHNorthwind01.Data.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHNorthwind01.Data.Interfaces
{
    public interface IDataServicesDWHNorthwind
    {
        Task<OperationResult> LoadDWH();
    }
}
