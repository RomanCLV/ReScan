using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer
{
    public class PositionEventArgs : EventArgs
    {
        private Point3D _oldPosition; 
        private Point3D _newPosition;

        public Point3D OldPosition { get => _oldPosition; }
        public Point3D NewPosition { get => _newPosition; }

        public PositionEventArgs(Point3D oldPosition, Point3D newPosition)
        {
            _oldPosition = oldPosition;
            _newPosition = newPosition;
        }
    }
}
