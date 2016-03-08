using UnityEngine;
using System.Collections;

namespace ZKit
{
    public class GizmoDummy : MonoBehaviour
    {
        #region Gizmo Root
        public const string GizmoRootName = "Gizmo Dummy";
        private static GameObject _gizmoRoot = null;
        public static GameObject GizmoRoot
        {
            get
            {
                Init();
                return _gizmoRoot;
            }
        }
        public static void Init()
        {
            if (_gizmoRoot == null)
            {
                GameObject root = GameObject.Find(GizmoRootName);
                if (root != null) GameObject.DestroyImmediate(root);
                _gizmoRoot = new GameObject(GizmoRootName);
                _gizmoRoot.AddComponent<GizmoDummy>();
                //_gizmoRoot.hideFlags = HideFlags.HideAndDontSave;
                _gizmoRoot.hideFlags = HideFlags.DontSave;
            }
        }
        #endregion
    }
}