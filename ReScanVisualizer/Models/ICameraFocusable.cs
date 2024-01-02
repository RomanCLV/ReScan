using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    public interface ICameraFocusable
    {
        /// <summary>
        /// Get the theorical position to set the camera to focus the model.<br />
        /// The direction from the camera to the model's center is (-1, -1, -1).
        /// </summary>
        /// <returns>Get the camera configuration that can be used to a set a viewport camera to get the focus of the current model according the given direction.</returns>
        CameraConfiguration GetCameraConfigurationToFocus();

        /// <summary>
        /// Get the theorical position to set the camera to focus the object.
        /// </summary>
        /// <param name="direction">The direction from the camera to the model's center.</param>
        /// <returns>Get the camera configuration that can be used to a set a viewport camera to get the focus of the current model according the given direction.</returns>
        CameraConfiguration GetCameraConfigurationToFocus(Vector3D direction);

        /// <summary>
        /// Get the theorical position to set the camera to focus the object.
        /// </summary>
        /// <param name="direction">The direction from the camera to the model's center.</param>
        /// <param name="minDistance">The minimal distance of the camera over the given direction.</param>
        /// <returns>Get the camera configuration that can be used to a set a viewport camera to get the focus of the current model according the given direction.</returns>
        CameraConfiguration GetCameraConfigurationToFocus(Vector3D direction, double minDistance);
    }
}
