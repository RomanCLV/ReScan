using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models
{
    public interface ISelectable
    {
        public bool IsSelected { get; }

        public void Select();

        public void Unselect();
    }
}
