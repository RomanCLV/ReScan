using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ActionCommand : CommandBase
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public override void Execute(object? parameter)
        {
            _action();
        }

        public static ActionCommand DoNothing => new ActionCommand(() => { });
    }
}
