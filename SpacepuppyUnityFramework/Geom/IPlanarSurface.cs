using UnityEngine;

namespace com.spacepuppy.Geom
{

    /// <summary>
    /// Interface for describing a 2d surface on which 3d information can be projected.
    /// 
    /// IPlanarSurfaces are useful for things like a 2.5d games where your motion is along a 2d cylindrical surface traveling through 3-space.
    /// </summary>
    public interface IPlanarSurface
    {

        Vector3 GetSurfaceNormal(Vector2 location);

        Vector3 GetSurfaceNormal(Vector3 location);

        /// <summary>
        /// Converts a 3d vector to closest 2d vector on 2D gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector2 ProjectVectorTo2D(Vector3 location, Vector3 v);

        /// <summary>
        /// Converts a 2d vector from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector3 ProjectVectorTo3D(Vector3 location, Vector2 v);

        /// <summary>
        /// Converts a 3d position to closest 2d position on the 2d gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector2 ProjectPosition2D(Vector3 v);

        /// <summary>
        /// Converts a 2d position from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector3 ProjectPosition3D(Vector2 v);

        /// <summary>
        /// Returns a position on the closest point on the planar surface.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector3 ClampToSurface(Vector3 v);

    }

}
