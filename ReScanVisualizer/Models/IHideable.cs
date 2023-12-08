using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace ReScanVisualizer.Models
{
    public interface IHideable
    {
        public event EventHandler<bool>? IsHiddenChanged;

        public bool IsHidden { get; }

        public void Hide();

        public void Show();

        public void InverseIsHidden();
    }
}
