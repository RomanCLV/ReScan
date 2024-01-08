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
        /// Obtain the theoretical configuration enabling the camera to focus the model.<br />
        /// Camera direction to model is (-1, -1, -1).
        /// </summary>
        /// <param name="fov">The field of view.<br />
        /// Must be in ]0 ; 180[</param>
        /// <param name="distanceScaling">A scaling factor to apply to the found distance before to compute the final camera position.<br />
        /// Must be greater than 0.</param>
        /// <param name="minDistance">The minimal distance to apply between the camera's position and the target.<br />
        /// Must be greater (or equal) than 0.</param>
        /// <returns>Get the camera configuration that can be used to a set (for example, in a viewport) a perspective camera to get the focus of the current model according the given direction and the minimal distance in between.</returns>
        /// <exception cref="ArgumentException"></exception>
        CameraConfiguration GetCameraConfigurationToFocus(double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0);

        /// <summary>
        /// Obtain the theoretical configuration enabling the camera to focus the model.
        /// </summary>
        /// <param name="direction">The camera direction towards the model.</param>
        /// <param name="fov">The field of view.<br />
        /// Must be in ]0 ; 180[</param>
        /// <param name="distanceScaling">A scaling factor to apply to the found distance before to compute the final camera position.<br />
        /// Must be greater than 0.</param>
        /// <param name="minDistance">The minimal distance to apply between the camera's position and the target.<br />
        /// Must be greater (or equal) than 0.</param>
        /// <returns>Get the camera configuration that can be used to a set (for example, in a viewport) a perspective camera to get the focus of the current model according the given direction and the minimal distance in between.</returns>
        /// <exception cref="ArgumentException"></exception>
        CameraConfiguration GetCameraConfigurationToFocus(Vector3D direction, double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0);
    }
}
