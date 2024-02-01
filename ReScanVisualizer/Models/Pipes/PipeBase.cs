using ReScanVisualizer.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.Pipes
{
    public abstract class PipeBase
    {
        protected bool _isStarted;
        public bool IsStarted => _isStarted;

        public PipeBase()
        {
            _isStarted = false;
        }

        public virtual void Start()
        {
            _isStarted = true;
        }

        public virtual void Stop()
        {
            _isStarted = false;
        }

        protected abstract void Run();
    }
}
