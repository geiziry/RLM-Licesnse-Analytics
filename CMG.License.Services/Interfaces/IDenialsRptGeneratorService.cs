using CMG.License.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMG.License.Services.Interfaces
{
    public interface IDenialsRptGeneratorService
    {
       void Aggregate(LogFile logFile);
    }
}
