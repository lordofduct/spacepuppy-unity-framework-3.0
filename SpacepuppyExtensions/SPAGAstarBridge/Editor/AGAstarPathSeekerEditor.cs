using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy.Pathfinding;
using Pathfinding;

namespace com.spacepuppyeditor.Pathfinding
{

    [CustomEditor(typeof(AGAstarPathSeeker))]
    public class AGAstarPathSeekerEditor : SeekerEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

    }
    
}