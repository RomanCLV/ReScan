using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models
{
    internal interface IModelisableBuilder
    {
        bool ModelHasToUpdate { set; }
    }
}
