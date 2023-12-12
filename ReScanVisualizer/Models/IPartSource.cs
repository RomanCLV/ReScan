using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.Models
{
    public interface IPartSource
    {
        IEnumerable<PartViewModelBase> Parts { get; }
    }
}
