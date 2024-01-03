using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    /*
     * Raisonnement générique pour positionner une camera correctement pour avoir le focus de l'ensemble d'un objet 3D
     * 
     * On se place dans le cas de la méthode la plus complete :
     * GetCameraConfigurationToFocus(Vector3D direction, double minDistance, double fov = 45);
     * 
     * Nous souhaitons positionner la camera pour voir la pièce entière.
     * La camera est sur l'axe dont la direction est donnée en entrée et qui passe par un point ciblé.
     * Il suffit donc de déterminer la distance qui part du point ciblé et qui suit l'axe afin que la camera voit l'ensemble de la piece.
     * 
     * Pour une forme quelconque, on pose le schema suivant :
     * 
     *     ______
     *    /  |  |
     *   / M |  |  
     *  |    |   \            \
     *  |    o____|___________θ\ C
     *   \_    __/    L        /
     *     \__/               /
     *    
     * o est la cible visée (par exemple le centre 3D de la piece)
     * M est la distance maximale d'un point projeté sur le plan de normal "direction" par rapport à o.
     * L est une distance inconnue sur l'axe "direction" (reliant la cible à la caméra).
     * θ est l'angle de vue de la camera.
     * 
     * On a la relation suivante : tan(θ/2) = M / L   <=> L = M / tan(θ/2)
     * 
     * Il suffit donc de trouver la distance maximale M et nous pouvons déterminer L.
     * 
     * Une fois la distance L obtenue, on sait que C(x,y,z) appartient à la droite de vecteur directeur "direction" (appelé u) donc :
     * x = ox + t * ux
     * y = oy + t * uy
     * z = oz + t * uz
     * 
     * Le cas le plus simple est d'avoir fait un changement de base au préalable dont le point visé est l'origine comme ça ox = oy = oz = 0
     * x = t * ux
     * y = t * uy
     * z = t * uz
     * 
     * Comme on veut que ||OC|| = L, on en déduit : sqrt(x²+y²+z²) = L <=> t = L / sqrt(ux²+uy²+uz²) = L / ||u||
     * On détermine ainsi t.
     * 
     * Sinon, dans le cas où le point ciblé n'est pas l'origine, on résout également ||OC|| = L ce qui donne :
     * t²*||u||² + 2*t*(ox*ux + oy*uy + oz*uz) + ox² + oy² + oz² - L² = 0
     * <=> a*t² + b*t + c = 0
     * On résout l'équation du second degré et on a t.
     * D = b²-4ac = 4*(ox*ux + oy*uy + oz*uz)² - 4*||u||²*(ox² + oy² + oz² - L²)
     *            = 4*[(ox*ux + oy*uy + oz*uz)² - ||u||²*(ox² + oy² + oz² - L²)]
     * Si D < 0 => impossible à résoudre..
     * Si D >= 0 :
     * t = (-b + sqrt(D)) / 2*a ou (-b - sqrt(D)) / 2*a
     * 
     * Enfin, avec t on en déduit C(x,y,z)
     */

    public interface ICameraFocusable
    {
        /// <summary>
        /// Get the theorical position to set the camera to focus the model.<br />
        /// The direction from the camera to the model's center is (-1, -1, -1).
        /// </summary>
        /// <param name="fov">Field of view</param>
        /// <returns>Get the camera configuration that can be used to a set a viewport camera to get the focus of the current model according the given direction.</returns>
        CameraConfiguration GetCameraConfigurationToFocus(double fov = 45);

        /// <summary>
        /// Get the theorical position to set the camera to focus the object.
        /// </summary>
        /// <param name="direction">The direction from the camera to the model's center.</param>
        /// <param name="fov">Field of view</param>
        /// <returns>Get the camera configuration that can be used to a set a viewport camera to get the focus of the current model according the given direction.</returns>
        CameraConfiguration GetCameraConfigurationToFocus(Vector3D direction, double fov = 45);

        /// <summary>
        /// Get the theorical position to set the camera to focus the object.
        /// </summary>
        /// <param name="direction">The direction from the camera to the model's center.</param>
        /// <param name="minDistance">The minimal distance of the camera over the given direction.</param>
        /// <param name="fov">Field of view</param>
        /// <returns>Get the camera configuration that can be used to a set a viewport camera to get the focus of the current model according the given direction.</returns>
        CameraConfiguration GetCameraConfigurationToFocus(Vector3D direction, double minDistance, double fov = 45);
    }
}
