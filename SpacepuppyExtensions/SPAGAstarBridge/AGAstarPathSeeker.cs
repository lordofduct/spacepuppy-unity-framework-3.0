using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Utils;

using Pathfinding;

namespace com.spacepuppy.Pathfinding
{
    
    public class AGAstarPathSeeker : Seeker, IComponent, IPathSeeker
    {

        #region Fields
        
        private GameObject _entityRoot;

        #endregion
        
        #region Properties

        public GameObject entityRoot
        {
            get
            {
                if (object.ReferenceEquals(_entityRoot, null))
                    _entityRoot = this.FindRoot();
                return _entityRoot;
            }
        }

        #endregion

        #region IPathSeeker Interface

        public IPathFactory PathFactory
        {
            get { return AGAstarPathFactory.Default; }
        }

        public IPath CreatePath(Vector3 target)
        {
            var path = new AGAstarABPath();
            path.UpdateTarget(this.entityRoot.transform.position, target);
            return path;
        }

        /// <summary>
        /// Only returns true for paths that can be used when calling CalculatePath with targets. 
        /// Paths that are suitable for CalculatePath(IPath path) vary more, and may return false for this method.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ValidPath(IPath path)
        {
            return path is AGAstarABPath;
        }

        public void CalculatePath(IPath path)
        {
            if (path == null) throw new System.ArgumentNullException("path");

            var p = AGAstarPath.GetInnerPath(path);
            if (p == null) throw new PathArgumentException();
            this.StartPath(p, AGAstarPath.OnPathCallback);
        }

        #endregion

        #region IComponent Interface

        Component IComponent.component
        {
            get
            {
                return this;
            }
        }

        #endregion

    }

}