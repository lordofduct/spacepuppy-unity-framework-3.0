using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Pathfinding
{
    public class AGAstarPathFactory : IPathFactory
    {

        #region Static Interface

        private static AGAstarPathFactory _default;

        public static AGAstarPathFactory Default
        {
            get
            {
                if (_default == null) _default = new AGAstarPathFactory();
                return _default;
            }
        }

        #endregion

        #region IPathFactory Interface

        public IPath Create(IPathSeeker seeker, Vector3 target)
        {
            Transform t;
            var root = SPEntity.Pool.GetFromSource(seeker);
            if (root != null)
            {
                t = root.transform;
            }
            else
            {
                t = GameObjectUtil.GetTransformFromSource(seeker);
                if (t == null) throw new PathArgumentException("IPathSeeker has no known Transform to get a position from.");
            }

            return new AGAstarABPath(t.position, target);
        }

        public IPath Create(Vector3 start, Vector3 end)
        {
            return new AGAstarABPath(start, end);
        }

        #endregion

    }
}
