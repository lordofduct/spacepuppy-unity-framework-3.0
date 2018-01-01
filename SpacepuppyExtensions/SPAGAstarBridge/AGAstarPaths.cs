using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Utils;

using Pathfinding;
using System;

namespace com.spacepuppy.Pathfinding
{
    public class AGAstarABPath : ABPath, IPath
    {
        
        public AGAstarABPath()
            : base()
        {
            this.Reset();
        }

        public AGAstarABPath(Vector3 start, Vector3 end)
            : base()
        {
            this.Reset();
            this.UpdateStartEnd(start, end);
        }

        #region IPath Interface

        IList<Vector3> IPath.Waypoints
        {
            get
            {
                return this.vectorPath;
            }
        }

        PathCalculateStatus IPath.Status
        {
            get
            {
                switch (this.CompleteState)
                {
                    case PathCompleteState.NotCalculated:
                        return PathCalculateStatus.Uncalculated;
                    case PathCompleteState.Error:
                        return PathCalculateStatus.Invalid;
                    case PathCompleteState.Partial:
                        return PathCalculateStatus.Partial;
                    case PathCompleteState.Complete:
                        return PathCalculateStatus.Success;
                    default:
                        return PathCalculateStatus.Invalid;
                }
            }
        }

        public void UpdateTarget(Vector3 start, Vector3 target)
        {
            this.UpdateStartEnd(start, target);
        }

        #endregion
        
    }

    public sealed class AGAstarPath : IPath
    {
        #region Fields

        private Path _path;

        #endregion

        #region CONSTRUCTOR

        private AGAstarPath()
        {
            //block constructor
        }

        public AGAstarPath(Path path)
        {
            if (path == null) throw new System.ArgumentNullException("path");
            _path = path;
        }

        #endregion

        #region Properties

        public Path InnerPath
        {
            get { return _path; }
        }

        #endregion

        #region IPath Interface

        public PathCalculateStatus Status
        {
            get
            {
                switch (_path.CompleteState)
                {
                    case PathCompleteState.Error:
                    case PathCompleteState.NotCalculated:
                        return PathCalculateStatus.Invalid;
                    case PathCompleteState.Partial:
                        return PathCalculateStatus.Partial;
                    case PathCompleteState.Complete:
                        return PathCalculateStatus.Success;
                    default:
                        return PathCalculateStatus.Invalid;
                }
            }
        }
        
        public IList<Vector3> Waypoints
        {
            get
            {
                return _path.vectorPath;
            }
        }

        #endregion

        #region Static Interface

        private static OnPathDelegate _onPathCallback;
        public static OnPathDelegate OnPathCallback
        {
            get
            {
                if (_onPathCallback == null) _onPathCallback = new OnPathDelegate((Path p) =>
                {
                    if (p.CompleteState == PathCompleteState.Complete && 
                        p is ABPath && 
                        (p.vectorPath.Count == 0 || !VectorUtil.FuzzyEquals(p.vectorPath[p.vectorPath.Count - 1], (p as ABPath).endPoint)))
                    {
                        p.vectorPath.Add((p as ABPath).endPoint);
                    }
                });
                return _onPathCallback;
            }
        }

        public static Path GetInnerPath(IPath path)
        {
            if (path is AGAstarABPath)
            {
                return path as Path;
            }
            else if (path is AGAstarPath)
            {
                return (path as AGAstarPath).InnerPath;
            }
            else
            {
                return null;
            }
        }

        public static IPath Create(Vector3 start, Vector3 end)
        {
            return new AGAstarABPath(start, end);
        }

        public static AGAstarPath Create(Path path)
        {
            return new AGAstarPath(path);
        }

        public static AGAstarPath CreateRandom(Vector3 start, int length)
        {
            return new AGAstarPath(RandomPath.Construct(start, length, null));
        }

        public static AGAstarPath CreateRandom(Vector3 start, int length, System.Action<RandomPath> config)
        {
            var path = RandomPath.Construct(start, length, null);
            if (config != null) config(path);
            return new AGAstarPath(path);
        }

        public static AGAstarPath CreateFlee(Vector3 start, Vector3 avoid, int searchLength)
        {
            return new AGAstarPath(FleePath.Construct(start, avoid, searchLength));
        }

        #endregion

    }

}
