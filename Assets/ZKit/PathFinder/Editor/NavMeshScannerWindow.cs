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
    static bool _debug_DrawGrid = false;
    static bool _debug_DrawLabel = false;

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

        var va = Voxel.Instance.VoxelArea;

        #region Draw Label
        if (va != null && _debug_DrawLabel == true)
        {
            Handles.color = Color.white;

            uint xCount = va.WidthCount;
            uint yCount = va.HeightCount;
            uint zCount = va.DepthCount;
            for (int i = 0; i < yCount; ++i)
            {
                float y = _mapSize.min.y + (i * _cellHeight);
                for (int j = 0; j < zCount; ++j)
                {
                    for (int k = 0; k < xCount; ++k)
                    {
                        uint index;
                        Vector3 position;
                        if (!va.GetCellIndex(out index, (uint)k, (uint)i, (uint)j)) continue;
                        if (!va.GetCellPosition(index, out position)) continue;
                        Handles.Label(position, string.Format("{0}", index));
                    }
                }
            }
        }
        #endregion

        #region Draw Cell Grid
        if (va != null && _debug_DrawGrid == true)
        {
            Handles.color = Color.white;

            uint xCount = va.WidthCount;
            uint yCount = va.HeightCount;
            uint zCount = va.DepthCount;
            //for (int i = 1; i <= yCount; ++i)
            {
                //float y = _mapSize.min.y + (i * _cellHeight);
                float y = _mapSize.min.y;
                for (int j = 1; j <= zCount; ++j)
                {
                    for (int k = 1; k < xCount; ++k)
                    {
                        Handles.DrawLine(new Vector3(_mapSize.min.x + (k * _cellSize), y, _mapSize.min.z), new Vector3(_mapSize.min.x + (k * _cellSize), y, _mapSize.max.z));
                    }
                    Handles.DrawLine(new Vector3(_mapSize.min.x, y, _mapSize.min.z + (j * _cellSize)), new Vector3(_mapSize.max.x, y, _mapSize.min.z + (j * _cellSize)));
                }
            }
        }
        #endregion
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    static void DrawGizmo(GizmoDummy dummy, GizmoType gizmoType)
    {
        var va = Voxel.Instance.VoxelArea;
        #region Draw Voxel
        if (va != null && _debug_DrawVoxel == true)
        {
            Gizmos.color = Color.blue;
            foreach (var ele in va.WalkableCells)
            {
                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                Gizmos.DrawWireCube(tmp, new Vector3(_cellSize, _cellHeight, _cellSize));
            }
        }
        #endregion
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

            Voxel.Instance.InitVoxelArea(_cellSize, _cellHeight, LayerMask.GetMask(pathLayers.ToArray()), LayerMask.GetMask(obstacleMask.ToArray()));
            _mapSize = Voxel.Instance.VoxelArea.AreaBound;
            Voxel.Instance.ScanVoxelSpace();
        }

        if(GUILayout.Button("ddkfk"))
        {
            CreateDebugVoxel();
        }

        _debug_DrawGrid = EditorGUILayout.Toggle("Show Grid", _debug_DrawGrid);
        _debug_DrawVoxel = EditorGUILayout.Toggle("Show Voxel", _debug_DrawVoxel);
        _debug_DrawLabel = EditorGUILayout.Toggle("Show Label", _debug_DrawLabel);
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