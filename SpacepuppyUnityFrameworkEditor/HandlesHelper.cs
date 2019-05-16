using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    public static class HandlesHelper
    {

        #region Spheroid Methods

        public static void DrawWireSphere(Vector3 pos, Quaternion rot, float radius)
        {
            DrawWireSphere(pos, rot, radius, 360f);
        }

        /// <summary>
        /// Draws a partial sphere with a circular opening opposite its forward.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="hemiAngle">The measure of the angle around the entire sphere, minus the opening. 360 is a full sphere.</param>
        public static void DrawWireSphere(Vector3 pos, Quaternion rot, float radius, float hemiAngle)
        {
            Vector3 forw = rot * Vector3.forward;
            Vector3 up = rot * Vector3.up;
            Vector3 right = rot * Vector3.right;

            Handles.DrawWireArc(pos, up, Quaternion.Euler(0f, -hemiAngle / 2f, 0f) * forw, hemiAngle, radius);
            Handles.DrawWireArc(Vector3.zero, right, Quaternion.Euler(-hemiAngle / 2f, 0f, 0f) * forw, hemiAngle, radius);

            float far = Mathf.Cos(hemiAngle * Mathf.Deg2Rad / 2f);
            float mid = Mathf.Cos(hemiAngle * Mathf.Deg2Rad / 4f);
            Handles.DrawWireArc(new Vector3(0f, 0f, mid * radius), forw, right, 360f, Mathf.Sqrt(1f - mid * mid) * radius);
            if (far > -1f)
            {
                Handles.DrawWireArc(new Vector3(0f, 0f, far * radius), forw, right, 360f, Mathf.Sqrt(1f - far * far) * radius);
            }
        }


        /// <summary>
        /// Draws a spherical frustum at Handles.matrix.
        /// </summary>
        /// <param name="nearRadius"></param>
        /// <param name="farRadius"></param>
        /// <param name="horFov"></param>
        /// <param name="verFov"></param>
        public static void DrawWireSphereFrustum(float nearRadius, float farRadius, float horFov, float verFov)
        {
            float dx = Mathf.Cos(verFov * Mathf.Deg2Rad / 2f);
            float dy = Mathf.Sqrt(1f - dx * dx);

            //draw facing arcs
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Quaternion.Euler(0f, -horFov / 2f, 0f) * Vector3.forward, horFov, farRadius);
            Handles.DrawWireArc(Vector3.zero, Vector3.right, Quaternion.Euler(-verFov / 2f, 0f, 0f) * Vector3.forward, verFov, farRadius);

            Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(horFov / 2f, Vector3.up) * Vector3.left,
                                Quaternion.AngleAxis(horFov / 2f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                verFov, farRadius);
            Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(horFov / 4f, Vector3.up) * Vector3.left,
                                Quaternion.AngleAxis(horFov / 4f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                verFov, farRadius);

            if(horFov < 360f)
            {
                Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(-horFov / 2f, Vector3.up) * Vector3.left,
                                    Quaternion.AngleAxis(-horFov / 2f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                    verFov, farRadius);
            }
            Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(-horFov / 4f, Vector3.up) * Vector3.left,
                                Quaternion.AngleAxis(-horFov / 4f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                verFov, farRadius);

            //draw edge arcs
            if (verFov < 180f)
            {
                dx = Mathf.Cos(verFov * Mathf.Deg2Rad / 2f);
                dy = Mathf.Sqrt(1f - dx * dx);
                Handles.DrawWireArc(new Vector3(0f, dy * farRadius, 0f), Vector3.up, Quaternion.Euler(0f, -horFov / 2f, 0f) * Vector3.forward, horFov, dx * farRadius);
                Handles.DrawWireArc(new Vector3(0f, -dy * farRadius, 0f), Vector3.up, Quaternion.Euler(0f, -horFov / 2f, 0f) * Vector3.forward, horFov, dx * farRadius);
            }


            //near plane
            if(nearRadius > 0f)
            {
                //draw facing arcs
                Handles.DrawWireArc(Vector3.zero, Vector3.up, Quaternion.Euler(0f, -horFov / 2f, 0f) * Vector3.forward, horFov, nearRadius);
                Handles.DrawWireArc(Vector3.zero, Vector3.right, Quaternion.Euler(-verFov / 2f, 0f, 0f) * Vector3.forward, verFov, nearRadius);

                Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(horFov / 2f, Vector3.up) * Vector3.left,
                                    Quaternion.AngleAxis(horFov / 2f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                    verFov, nearRadius);
                Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(horFov / 4f, Vector3.up) * Vector3.left,
                                    Quaternion.AngleAxis(horFov / 4f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                    verFov, nearRadius);

                if (horFov < 360f)
                {
                    Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(-horFov / 2f, Vector3.up) * Vector3.left,
                                        Quaternion.AngleAxis(-horFov / 2f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                        verFov, nearRadius);
                }
                Handles.DrawWireArc(Vector3.zero, Quaternion.AngleAxis(-horFov / 4f, Vector3.up) * Vector3.left,
                                    Quaternion.AngleAxis(-horFov / 4f, Vector3.up) * Quaternion.AngleAxis(verFov / 2f, Vector3.right) * Vector3.forward,
                                    verFov, nearRadius);

                //draw edge arcs
                if (verFov < 180f)
                {
                    dx = Mathf.Cos(verFov * Mathf.Deg2Rad / 2f);
                    dy = Mathf.Sqrt(1f - dx * dx);
                    Handles.DrawWireArc(new Vector3(0f, dy * nearRadius, 0f), Vector3.up, Quaternion.Euler(0f, -horFov / 2f, 0f) * Vector3.forward, horFov, dx * nearRadius);
                    Handles.DrawWireArc(new Vector3(0f, -dy * nearRadius, 0f), Vector3.up, Quaternion.Euler(0f, -horFov / 2f, 0f) * Vector3.forward, horFov, dx * nearRadius);
                }
            }



            //draw lines
            Vector3 v1, v2;
            v1 = new Vector3(0f, dy, dx);
            v2 = new Vector3(0f, -dy, dx);
            Handles.DrawLine(v1 * nearRadius, v1 * farRadius);
            Handles.DrawLine(v2 * nearRadius, v2 * farRadius);
            v1 = Quaternion.Euler(0f, horFov / 4f, 0f) * new Vector3(0f, dy, dx);
            v2 = Quaternion.Euler(0f, horFov / 4f, 0f) * new Vector3(0f, -dy, dx);
            Handles.DrawLine(v1 * nearRadius, v1 * farRadius);
            Handles.DrawLine(v2 * nearRadius, v2 * farRadius);
            v1 = Quaternion.Euler(0f, -horFov / 4f, 0f) * new Vector3(0f, dy, dx);
            v2 = Quaternion.Euler(0f, -horFov / 4f, 0f) * new Vector3(0f, -dy, dx);
            Handles.DrawLine(v1 * nearRadius, v1 * farRadius);
            Handles.DrawLine(v2 * nearRadius, v2 * farRadius);
            v1 = Quaternion.Euler(0f, horFov / 2f, 0f) * new Vector3(0f, dy, dx);
            v2 = Quaternion.Euler(0f, horFov / 2f, 0f) * new Vector3(0f, -dy, dx);
            Handles.DrawLine(v1 * nearRadius, v1 * farRadius);
            Handles.DrawLine(v2 * nearRadius, v2 * farRadius);
            if(horFov < 360f)
            {
                v1 = Quaternion.Euler(0f, -horFov / 2f, 0f) * new Vector3(0f, dy, dx);
                v2 = Quaternion.Euler(0f, -horFov / 2f, 0f) * new Vector3(0f, -dy, dx);
                Handles.DrawLine(v1 * nearRadius, v1 * farRadius);
                Handles.DrawLine(v2 * nearRadius, v2 * farRadius);
            }
        }

        #endregion

    }

}
