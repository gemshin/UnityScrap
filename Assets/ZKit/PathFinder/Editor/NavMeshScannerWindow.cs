using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using ZKit;
using ZKit.PathFinder;

public class NavMeshScannerWindow : EditorWindow
{
    static Bounds _mapSize = new Bounds();

    static int _pathMask;
    static int _obstacleMask;

    static float _cellSize = 1f;
    static float _cellHeight = 1f;


    static bool _debug_DrawVoxel = false;

    [MenuItem("TEST/NavMeshScanner")]
    static void Init()
    {
        NavMeshScannerWindow window = (NavMeshScannerWindow)EditorWindow.GetWindow(typeof(NavMeshScannerWindow), false, "NavScanner");
        window.Show();
    }

    private void OnEnable()
    {
        GizmoDummy.Init();
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        #region Draw Map Bound
        Handles.color = Color.red;
        Handles.DrawLine(new Vector3(_mapSize.max.x, _mapSize.max.y, _mapSize.max.z), new Vector3(_mapSize.min.x, _mapSize.max.y, _mapSize.max.z));
        Handles.DrawLine(new Vector3(_mapSize.min.x, _mapSize.max.y, _mapSize.max.z), new Vector3(_mapSize.min.x, _mapSize.max.y, _mapSize.min.z));
        Handles.DrawLine(new Vector3(_mapSize.min.x, _mapSize.max.y, _mapSize.min.z), new Vector3(_mapSize.max.x, _mapSize.max.y, _mapSize.min.z));
        Handles.DrawLine(new Vector3(_mapSize.max.x, _mapSize.max.y, _mapSize.min.z), new Vector3(_mapSize.max.x, _mapSize.max.y, _mapSize.max.z));

        Handles.DrawLine(new Vector3(_mapSize.max.x, _mapSize.min.y, _mapSize.max.z), new Vector3(_mapSize.min.x, _mapSize.min.y, _mapSize.max.z));
        Handles.DrawLine(new Vector3(_mapSize.min.x, _mapSize.min.y, _mapSize.max.z), new Vector3(_mapSize.min.x, _mapSize.min.y, _mapSize.min.z));
        Handles.DrawLine(new Vector3(_mapSize.min.x, _mapSize.min.y, _mapSize.min.z), new Vector3(_mapSize.max.x, _mapSize.min.y, _mapSize.min.z));
        Handles.DrawLine(new Vector3(_mapSize.max.x, _mapSize.min.y, _mapSize.min.z), new Vector3(_mapSize.max.x, _mapSize.min.y, _mapSize.max.z));

        Handles.DrawLine(new Vector3(_mapSize.max.x, _mapSize.max.y, _mapSize.max.z), new Vector3(_mapSize.max.x, _mapSize.min.y, _mapSize.max.z));
        Handles.DrawLine(new Vector3(_mapSize.min.x, _mapSize.max.y, _mapSize.max.z), new Vector3(_mapSize.min.x, _mapSize.min.y, _mapSize.max.z));
        Handles.DrawLine(new Vector3(_mapSize.min.x, _mapSize.max.y, _mapSize.min.z), new Vector3(_mapSize.min.x, _mapSize.min.y, _mapSize.min.z));
        Handles.DrawLine(new Vector3(_mapSize.max.x, _mapSize.max.y, _mapSize.min.z), new Vector3(_mapSize.max.x, _mapSize.min.y, _mapSize.min.z));
        #endregion

        #region Draw Cell Size
        Handles.color = Color.white;
        uint xCount = (uint)System.Math.Truncate((_mapSize.max.x - _mapSize.min.x) / _cellSize);
        uint yCount = (uint)System.Math.Truncate((_mapSize.max.y - _mapSize.min.y) / _cellHeight);
        uint zCount = (uint)System.Math.Truncate((_mapSize.max.z - _mapSize.min.z) / _cellSize);
        ////for (int i = 1; i <= yCount; ++i)
        //{
        //    //float y = _mapSize.min.y + (i * _cellHeight);
        //    float y = _mapSize.min.y;
        //    for (int j = 1; j <= zCount; ++j)
        //    {
        //        for (int k = 1; k < xCount; ++k)
        //        {
        //            Handles.DrawLine(new Vector3(_mapSize.min.x + (k * _cellSize), y, _mapSize.min.z), new Vector3(_mapSize.min.x + (k * _cellSize), y, _mapSize.max.z));
        //        }
        //        Handles.DrawLine(new Vector3(_mapSize.min.x, y, _mapSize.min.z + (j * _cellSize)), new Vector3(_mapSize.max.x, y, _mapSize.min.z + (j * _cellSize)));
        //    }
        //}
        #endregion

        //List<string> layerNames = new List<string>();
        //for (int i = 0; i < 32; ++i)
        //{
        //    if (LayerMask.LayerToName(i).Length != 0)
        //        layerNames.Add(LayerMask.LayerToName(i));
        //}
        //List<string> pathLayers = new List<string>();
        //for (int i = 0; i < layerNames.Count; ++i)
        //{
        //    if ((_pathLayerMask & (1 << i)) != 0)
        //        pathLayers.Add(layerNames[i]);
        //}
        //int pathLayerMask = LayerMask.GetMask(pathLayers.ToArray());

        //GameObject[] gameObjects = (GameObject[])FindObjectsOfType(typeof(GameObject));
        //foreach (GameObject go in gameObjects)
        //{
        //    if ((pathLayerMask & (1 << go.layer)) == 0) continue;

        //    MeshFilter mf = go.GetComponent<MeshFilter>();
        //    if (!mf) continue;
        //    if (!mf.sharedMesh) continue;

        //    foreach (var vertex in mf.sharedMesh.vertices)
        //    {
        //        Vector3 v = vertex;
        //        v.Scale(go.transform.localScale);
        //        v = go.transform.rotation * v;
        //        Handles.DotCap(0, (go.transform.position + (v)), Quaternion.identity, 0.05f);
        //    }
        //}
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    static void DrawGizmo(GizmoDummy dummy, GizmoType gizmoType)
    {
        //uint xCount = (uint)System.Math.Truncate((_mapSize.max.x - _mapSize.min.x) / _cellSize);
        //uint yCount = (uint)System.Math.Truncate((_mapSize.max.y - _mapSize.min.y) / _cellHeight);
        //uint zCount = (uint)System.Math.Truncate((_mapSize.max.z - _mapSize.min.z) / _cellSize);
        //for (int i = 1; i <= zCount; ++i)
        //{
        //    for (int j = 1; j <= xCount; ++j)
        //    {
        //        Gizmos.DrawCube(new Vector3(_mapSize.min.x + (j * _cellSize) - (_cellSize * 0.5f), 0f, _mapSize.min.z + (i * _cellSize) - (_cellSize * 0.5f)), new Vector3(_cellSize, _cellHeight, _cellSize));
        //    }
        //}
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("1st Step - Scan");
        EditorGUILayout.EndVertical();
        _cellSize = EditorGUILayout.Slider("Cell Size", _cellSize, 0.1f, 4f);
        _cellHeight = EditorGUILayout.Slider("Cell Height", _cellHeight, 0.1f, 4f);
        
        List<string> layerNames = new List<string>();
        for(int i = 0; i < 32; ++i)
        {
            if (LayerMask.LayerToName(i).Length != 0)
                layerNames.Add(LayerMask.LayerToName(i));
        }

        _pathMask = EditorGUILayout.MaskField("Path",_pathMask, layerNames.ToArray());
        _obstacleMask = EditorGUILayout.MaskField("Obstacles", _obstacleMask, layerNames.ToArray());

        if (GUILayout.Button("Scan the map"))
        {
            List<string> pathLayers = new List<string>();
            List<string> obstacleMask = new List<string>();
            for (int i = 1; i <= layerNames.Count; ++i)
            {
                if ((_pathMask & (1 << i)) > 0) pathLayers.Add(layerNames[i]);
                if ((_obstacleMask & (1 << i)) > 0) obstacleMask.Add(layerNames[i]);
            }

            VoxelSpace.Instance.InitVoxelSpace(_cellSize, _cellHeight, LayerMask.GetMask(pathLayers.ToArray()), LayerMask.GetMask(obstacleMask.ToArray()));
            _mapSize = VoxelSpace.Instance.SpaceSize;
            VoxelSpace.Instance.ScanVoxelSpace();
        }

        if(GUILayout.Button("ddkfk"))
        {
            CreateDebugVoxel();
        }

        _debug_DrawVoxel = EditorGUILayout.Toggle(_debug_DrawVoxel);
        EditorGUILayout.EndVertical();

        SceneView.RepaintAll();
    }

    private void CreateDebugVoxel()
    {
        GameObject root = GameObject.Find("Debug Voxels");
        if (root != null) GameObject.DestroyImmediate(root);
        root = new GameObject("Debug Voxels");
        //root.hideFlags = HideFlags.HideAndDontSave;

        uint xCount = (uint)System.Math.Truncate((_mapSize.max.x - _mapSize.min.x) / _cellSize);
        uint yCount = (uint)System.Math.Truncate((_mapSize.max.y - _mapSize.min.y) / _cellHeight);
        uint zCount = (uint)System.Math.Truncate((_mapSize.max.z - _mapSize.min.z) / _cellSize);
        for (int i = 1; i <= zCount; ++i)
        {
            for (int j = 1; j <= xCount; ++j)
            {
                GameObject newMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(newMesh.GetComponent<BoxCollider>());
                MeshRenderer mr = newMesh.GetComponent<MeshRenderer>();
                //mr.material.color = Color.red;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.useLightProbes = false;
                mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                newMesh.transform.parent = root.transform;
                newMesh.transform.position = new Vector3(_mapSize.min.x + (j * _cellSize) - (_cellSize * 0.5f), 0f, _mapSize.min.z + (i * _cellSize) - (_cellSize * 0.5f));
                newMesh.transform.localScale = new Vector3(_cellSize, _cellHeight, _cellSize);
            }
        }
    }
}