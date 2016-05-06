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
    static int _exceptMask;

    static float _cellSize = 0.4f;
    static float _cellHeight = 0.4f;

    static float _agentHeight = 1.6f;
    static float _agentRadius = 0.5f;
    static float _maxClimb = 0.5f;
    static float _maxSlope = 30f;

    static bool _debug_DrawVoxel = false;
    static bool _debug_DrawLedge = false;
    static bool _debug_DrawWalkableVoxel = false;

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

//        var va = Voxelizer.Instance.VoxelArea;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    static void DrawGizmo(GizmoDummy dummy, GizmoType gizmoType)
    {
        var va = Voxelizer.Instance.VoxelArea;
        #region Draw Voxel
        if (va != null && _debug_DrawVoxel == true)
        {
            Color c = Color.white;
            c.a = 0.7f;
            Gizmos.color = c;
            foreach (var ele in va.FirstLedges)
            {
                Voxel vc;
                int count = 1;
                for (vc = va.GetCell(VoxelArea.DIRECTION.Lower, ele.Index); vc != null; vc = va.GetCell(VoxelArea.DIRECTION.Lower, vc.Index), ++count)
                    if (vc.CellType != VOXEL_NAVI_TYPE.Walkable) break;

                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                tmp.y = tmp.y - (_cellHeight * count * 0.5f) + _cellHeight * 0.5f;
                //Gizmos.DrawWireCube(tmp, new Vector3(_cellSize, _cellHeight*count, _cellSize));
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight * count, _cellSize));
            }
            foreach (var ele in va.Ledges)
            {
                Voxel vc;
                int count = 1;
                for (vc = va.GetCell(VoxelArea.DIRECTION.Lower, ele.Index); vc != null; vc = va.GetCell(VoxelArea.DIRECTION.Lower, vc.Index), ++count)
                    if (vc.CellType != VOXEL_NAVI_TYPE.Walkable) break;

                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                tmp.y = tmp.y - (_cellHeight * count * 0.5f) + _cellHeight * 0.5f;
                //Gizmos.DrawWireCube(tmp, new Vector3(_cellSize, _cellHeight*count, _cellSize));
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight * count, _cellSize));
            }
            foreach (var ele in va.WalkableCells)
            {
                Voxel vc;
                int count = 1;
                for (vc = va.GetCell(VoxelArea.DIRECTION.Lower, ele.Index); vc != null; vc = va.GetCell(VoxelArea.DIRECTION.Lower, vc.Index), ++count)
                    if (vc.CellType != VOXEL_NAVI_TYPE.Walkable) break;

                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                tmp.y = tmp.y - (_cellHeight * count * 0.5f) + _cellHeight * 0.5f;
                //Gizmos.DrawWireCube(tmp, new Vector3(_cellSize, _cellHeight*count, _cellSize));
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight * count, _cellSize));
            }
        }
        #endregion

        #region Draw First Ledge
        if (va != null && _debug_DrawLedge == true)
        {
            Gizmos.color = Color.gray;
            foreach (var ele in va.FirstLedges)
            {
                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight, _cellSize));
            }
        }
        #endregion

        #region Draw Ledge
        if (va != null && _debug_DrawLedge == true)
        {
            Gizmos.color = Color.cyan;
            foreach (var ele in va.Ledges)
            {
                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight, _cellSize));
            }
        }
        #endregion

        #region Draw Walkable Voxel
        if (va != null && _debug_DrawWalkableVoxel == true)
        {
            Color c = Color.red;
            //c.a = 0.7f;
            Gizmos.color = c;
            foreach (var ele in va.WalkableCells)
            {
                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight, _cellSize));
                //Gizmos.DrawCube(tmp, new Vector3(_cellSize, 0f, _cellSize));
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
        _agentRadius = EditorGUILayout.Slider("Agent Radius", _agentRadius, 0.1f, 4f);
        _agentHeight = EditorGUILayout.Slider("Agent Height", _agentHeight, 0.5f, 4f);
        _maxClimb = EditorGUILayout.Slider("Max Climb", _maxClimb, 0.0f, 4f);
        _maxSlope = EditorGUILayout.Slider("Max Slope", _maxSlope, 0.1f, 90f);
        
        List<string> layerNames = new List<string>();
        for(int i = 0; i < 32; ++i)
        {
            if (LayerMask.LayerToName(i).Length != 0)
                layerNames.Add(LayerMask.LayerToName(i));
        }

        _pathMask = EditorGUILayout.MaskField("Path",_pathMask, layerNames.ToArray());
        _obstacleMask = EditorGUILayout.MaskField("Obstacles", _obstacleMask, layerNames.ToArray());
        _exceptMask = EditorGUILayout.MaskField("Excepts", _exceptMask, layerNames.ToArray());

        if (GUILayout.Button("Scan the map"))
        {
            List<string> pathLayers = new List<string>();
            List<string> obstacleMask = new List<string>();
            List<string> exceptMask = new List<string>();

            for (int i = 1; i <= layerNames.Count; ++i)
            {
                if ((_pathMask & (1 << i)) > 0) pathLayers.Add(layerNames[i]);
                if ((_obstacleMask & (1 << i)) > 0) obstacleMask.Add(layerNames[i]);
                if ((_exceptMask & (1 << i)) > 0) exceptMask.Add(layerNames[i]);
            }

            Voxelizer.Instance.InitVoxelArea(_cellSize, _cellHeight
                , LayerMask.GetMask(pathLayers.ToArray()), LayerMask.GetMask(obstacleMask.ToArray()), LayerMask.GetMask(exceptMask.ToArray()));
            _mapSize = Voxelizer.Instance.VoxelArea.AreaBound;
            Voxelizer.Instance.VoxelArea.SetAgentInfo(_agentHeight, _agentRadius, _maxClimb, _maxSlope);
            Voxelizer.Instance.ScanVoxelSpace();
        }

        _debug_DrawVoxel = EditorGUILayout.Toggle("Show Voxel", _debug_DrawVoxel);
        _debug_DrawLedge = EditorGUILayout.Toggle("Show Ledge", _debug_DrawLedge);
        _debug_DrawWalkableVoxel = EditorGUILayout.Toggle("Show Walkable Voxel", _debug_DrawWalkableVoxel);
        EditorGUILayout.EndVertical();

        SceneView.RepaintAll();
    }
}