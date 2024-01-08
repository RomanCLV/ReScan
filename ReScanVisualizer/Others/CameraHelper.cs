using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using ReScanVisualizer.Models;

namespace ReScanVisualizer
{
    public static class CameraHelper
    {
        /// <summary>
        /// Obtain the theoretical configuration enabling the camera to focus the model.<br />
        /// The direction is (-1, -1, -1).<br />
        /// The target point is the center of the given rectangle.
        /// </summary>
        /// <param name="rect3D">The rectangle to focus.</param>
        /// <param name="fov">The field of view.<br />
        /// Must be in ]0 ; 180[</param>
        /// <param name="distanceScaling">A scaling factor to apply to the found distance before to compute the final camera position.<br />
        /// Must be greater than 0.</param>
        /// <param name="minDistance">The minimal distance to apply between the camera's position and the target.<br />
        /// Must be greater (or equal) than 0.</param>
        /// <returns>Get the camera configuration that can be used to a set (for example, in a viewport) a perspective camera to get the focus of the current model according the direction (-1, -1, -1) and the minimal distance in between.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static CameraConfiguration GetCameraConfigurationToFocus(Rect3D rect3D, double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0)
        {
            return GetCameraConfigurationToFocus(
                rect3D,
                new Point3D(rect3D.Location.X + rect3D.SizeX / 2.0, rect3D.Location.Y + rect3D.SizeY / 2.0, rect3D.Location.Z + rect3D.SizeZ / 2.0),
                new Vector3D(-1.0, -1.0, -1.0),
                fov, distanceScaling, minDistance);
        }

        /// <summary>
        /// Obtain the theoretical configuration enabling the camera to focus the model.<br />
        /// The target point is the center of the given rectangle.
        /// </summary>
        /// <param name="rect3D">The rectangle to focus.</param>
        /// <param name="direction">The camera direction towards the center of the rectangle.</param>
        /// <param name="fov">The field of view.<br />
        /// Must be in ]0 ; 180[</param>
        /// <param name="distanceScaling">A scaling factor to apply to the found distance before to compute the final camera position.<br />
        /// Must be greater than 0.</param>
        /// <param name="minDistance">The minimal distance to apply between the camera's position and the target.<br />
        /// Must be greater (or equal) than 0.</param>
        /// <returns>Get the camera configuration that can be used to a set (for example, in a viewport) a perspective camera to get the focus of the current model according the given direction and the minimal distance in between.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static CameraConfiguration GetCameraConfigurationToFocus(Rect3D rect3D, Vector3D direction, double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0)
        {
            return GetCameraConfigurationToFocus(
                rect3D,
                new Point3D(rect3D.Location.X + rect3D.SizeX / 2.0, rect3D.Location.Y + rect3D.SizeY / 2.0, rect3D.Location.Z + rect3D.SizeZ / 2.0),
                direction, 
                fov, distanceScaling, minDistance);
        }

        /// <summary>
        /// Obtain the theoretical configuration enabling the camera to focus the model.
        /// </summary>
        /// <param name="rect3D">The rectangle to focus.</param>
        /// <param name="target">The target point in the center of the camera. This point can be everywhere (not necessary in the rectangle).</param>
        /// <param name="direction">The camera direction towards the center of the target.</param>
        /// <param name="fov">The field of view.<br />
        /// Must be in ]0 ; 180[</param>
        /// <param name="distanceScaling">A scaling factor to apply to the found distance before to compute the final camera position.<br />
        /// Must be greater than 0.</param>
        /// <param name="minDistance">The minimal distance to apply between the camera's position and the target.<br />
        /// Must be greater (or equal) than 0.</param>
        /// <returns>Get the camera configuration that can be used to a set (for example, in a viewport) a perspective camera to get the focus of the current model according the given direction and the minimal distance in between.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static CameraConfiguration GetCameraConfigurationToFocus(Rect3D rect3D, Point3D target, Vector3D direction, double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0)
        {
            /*
             * Raisonnement générique pour positionner une camera correctement pour avoir le focus de l'ensemble d'un objet 3D
             * 
             * Nous souhaitons positionner la camera pour voir une pièce entièrement (qui est présenté sous forme d'un rectangle
             * donc tous les points se situent à la surface ou à l'intérieur de ce rectangle).
             * 
             * La camera se positionnera sur l'axe dont la direction est donnée en entrée et qui passe par un point ciblé, également donné en entrée.
             * Il faut déterminer la distance entre le point ciblé et le point C(x,y,z) (position de la caméra) afin de positionner celle ci correctement
             * de façon à voir tout le rectangle en fonction du point de vue.
             * 
             * Pour une forme quelconque, on pose le schema suivant :
             * 
             *     ______
             *    /  |  |
             *   / M |  |  
             *  |    |   \            \
             *  |    O____|___________θ\ C
             *   \_    __/    L        /
             *     \__/               /
             *    
             * O est la cible visée (par exemple le centre 3D de la piece)
             * M est la distance maximale d'un point projeté conique sur le plan normal à la direction et passant par O.
             * L est une distance inconnue sur l'axe "direction" (reliant la cible à la caméra).
             * θ est l'angle de vue de la camera (fov).
             * 
             * On a la relation suivante : tan(θ/2) = M / L   <=> L = M / tan(θ/2)
             * 
             * Il faut donc d'abord trouver la distance maximale M et nous pourrons déterminer L.
             * 
             * Pour déterminer le projeté conique, on utilise le schéma suivant :
             * 
             *           |
             *        H''|\
             *           | \
             *           |  \
             *           |   \
             *           |    \
             *    _______|H'_(α\
             *    |      |  H|  \
             *    |      |   |   \
             *    |      |   |    \
             *    |      |   |     \
             *    |      O___|____(α\ C
             *    |      |   |      /
             *    |      |   |     /
             *    |     P|   |    /
             *    |      |   |   /
             *    |__________|  /
             *           |     /
             *           
             * α = θ/2 (la moitié du fov)
             * 
             * Avec le vecteur directeur U, on va facilement trouver le plan normal P passant par le point ciblé O.
             * 
             * Ce qu'on cherche est la distance OH''
             * Le point H'' est le projeté conique de H sur P. Il dépend de la position de la camera (C) et comme on cherche C,
             * c'est compliqué de déterminer H'' directement.
             * 
             * Néanmoins ce qui ne change jamais c'est le point H', le projeté orthogonal de H sur P.
             * On peut donc facilement trouver OH' est avoir une première partie de la distance cherchée.
             * Reste à trouver la distance H'H''.
             * Il faut remarquer que la droite HH' est parallèle à la droite OC donc on retrouve l'angle α entre les droites HH' et HH''.
             * Et donc nous pouvons appliquer :
             *     tan(α) = H'H'' / HH' 
             * <=> H'H'' = HH' * tan(α) = HH' * tan(θ/2)
             * 
             * Finalement, on a la distance M = OH'' = OH' + HH' * tan(θ/2)
             * 
             * Ainsi on a  L = M / tan(θ/2).
             *    
             * Une fois la distance L obtenue, on sait que C(x,y,z) appartient à la droite de vecteur directeur "direction" (appelé u) donc :
             * x = ox + t * ux
             * y = oy + t * uy
             * z = oz + t * uz
             * 
             * Le cas le plus simple est d'avoir fait un changement de base au préalable dont le point visé est l'origine comme ça ox = oy = oz = 0 et :
             * x = t * ux
             * y = t * uy
             * z = t * uz
             * 
             * Comme on veut que ||OC|| = L, on en déduit (dans le cas le plus simple) :
             *     sqrt(x²+y²+z²) = L 
             * <=> t = L / sqrt(ux²+uy²+uz²) 
             * <=> t = L / ||u||
             * On détermine ainsi t.
             * 
             * Sinon, dans le cas où le point ciblé n'est pas l'origine, on résout également ||OC|| = L ce qui donne :
             *     sqrt(x²+y²+z²) = L
             * <=> t²*||u||² + 2*t*(ox*ux + oy*uy + oz*uz) + ox² + oy² + oz² - L² = 0
             * <=> a*t² + b*t + c = 0
             * On résout l'équation du second degré et on a t.
             * D = b²-4ac = 4*(ox*ux + oy*uy + oz*uz)² - 4*||u||²*(ox² + oy² + oz² - L²)
             *            = 4*[(ox*ux + oy*uy + oz*uz)² - ||u||²*(ox² + oy² + oz² - L²)]
             * Si D < 0 => impossible à résoudre...
             * Si D >= 0 :
             * t = (-b + sqrt(D)) / 2*a ou (-b - sqrt(D)) / 2*a (il faut sélectionner le bon t).
             * 
             * Enfin, avec t on en déduit C(x,y,z)
             */

            if (rect3D.SizeX == 0.0 && rect3D.SizeY == 0.0 && rect3D.SizeZ == 0.0)
            {
                throw new ArgumentException("invalid rectangle. Rectangle must have at least one dimension greater than 0.", nameof(rect3D));
            }
            if (direction.Length == 0.0)
            {
                throw new ArgumentException("invalid given direction.", nameof(direction));
            }
            if (double.IsNaN(fov) || double.IsInfinity(fov) || fov <= 0.0 || fov >= 180.0)
            {
                throw new ArgumentException("fov must be in : 0 < fov < 180. Given: " + fov, nameof(fov));
            }
            if (double.IsNaN(minDistance) || double.IsInfinity(minDistance) || minDistance < 0.0)
            {
                throw new ArgumentException("minDistance must be greater (or equal) than 0. Given: " + minDistance, nameof(minDistance));
            }
            if (double.IsNaN(distanceScaling) || double.IsInfinity(distanceScaling) || distanceScaling <= 0.0)
            {
                throw new ArgumentException("distanceScaling must be greater than 0. Given: " + distanceScaling, nameof(distanceScaling));
            }

            direction.Normalize();
            double subFov = Tools.DegreeToRadian(fov / 2.0);

            Vector3D targetTranslation = new Vector3D(target.X, target.Y, target.Z);

            Vector3D xDirection = new Vector3D(rect3D.SizeX, 0.0, 0.0);
            Vector3D yDirection = new Vector3D(0.0, rect3D.SizeY, 0.0);
            Vector3D zDirection = new Vector3D(0.0, 0.0, rect3D.SizeZ);
            Point3D[] keyPointsInWorld0 = new Point3D[8]
            {
                rect3D.Location,
                rect3D.Location + xDirection,
                rect3D.Location + yDirection,
                rect3D.Location + xDirection + yDirection,
                rect3D.Location + zDirection,
                rect3D.Location + zDirection + xDirection,
                rect3D.Location + zDirection + yDirection,
                rect3D.Location + zDirection + xDirection + yDirection
            };

            // projection des points sur le plan donné par la direction de la camera
            // M : et determination de la distance max
            Plan plan = new Plan(direction);
            double maxDistance = 0.0;
            double distance;
            double distance1;
            double distance2;
            for (int i = 0; i < keyPointsInWorld0.Length; i++)
            {
                // takes the current point and applies the opposite translation to the target, in order to pass into a base whose center is the target.
                Point3D keyPointInBase = keyPointsInWorld0[i] - targetTranslation; 

                // find the orthogonal projection of this point on the plan
                Point3D projectedPoint = plan.GetOrthogonalProjection(keyPointInBase);

                // compute the distance between the center (= the target) and this point (=> we get the distance OH')
                distance1 = Math.Sqrt(projectedPoint.X * projectedPoint.X + projectedPoint.Y * projectedPoint.Y + projectedPoint.Z * projectedPoint.Z);

                // compute the distance between the current point and its projection (=> we get the distance HH')
                distance2 = Math.Sqrt(
                    (keyPointInBase.X - projectedPoint.X) * (keyPointInBase.X - projectedPoint.X) +
                    (keyPointInBase.Y - projectedPoint.Y) * (keyPointInBase.Y - projectedPoint.Y) +
                    (keyPointInBase.Z - projectedPoint.Z) * (keyPointInBase.Z - projectedPoint.Z));

                // compute the total distance between the center (= the target) and the conic projection of the current point
                distance = distance1 + distance2 * Math.Tan(subFov);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }

            distance = (maxDistance / Math.Tan(subFov) * distanceScaling);
            if (distance < minDistance)
            {
                distance = minDistance;
            }

            // t = L / ||u|| (car point visé est l'origine)
            double t = distance / direction.Length;
            double x = t * direction.X;
            double y = t * direction.Y;
            double z = t * direction.Z;
            Vector3D c = new Vector3D(x, y, z); // position cam dans le base courrante

            // determination de la position de la camera dans le World0
            Point3D camPosition = target - c;

            return new CameraConfiguration(camPosition, target);
        }
    }
}
