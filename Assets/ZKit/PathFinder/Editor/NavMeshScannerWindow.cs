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
    static int _exceptMask;

    static float _cellSize = 0.4f;
    static float _cellHeight = 0.4f;

    static float _agentHeight = 1.6f;
    static float _agentRadius = 0.5f;
    static float _maxClimb = 0.5f;
    static float _maxSlope = 30f;

    static bool _debug_DrawVoxel = false;
    static bool _debug_DrawReverseVoxel = false;
    static bool _debug_DrawDistance = false;
    static bool _debug_DrawWalkable = false;
    static bool _debug_DrawFirstLedge = false;
    static bool _debug_DrawLedge = false;

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

        var va = RecastNaviMesh.Instance.GetArea();
        GUIStyle gs = new GUIStyle();
        gs.normal.textColor = Color.magenta;
        if (va != null && _debug_DrawLedge == true)
        {
            foreach (var ele in va.Voxels.Values)
            {
                if (!ele.IsTop) continue;
                if (ele.FaceReverse) continue;

                Vector3 pos;
                va.GetCellPosition(ele.Index, out pos);
                // 퍼포먼스를 위해 가까운것만 그리자.
                if (Vector3.Distance(pos, sceneView.camera.transform.position) < 15f)
                {
                    Handles.Label(pos, ele.Index.ToString(), gs);
                }
            }
        }

        //if (va != null )// && _debug_DrawLedge == true)
        //{
        //    Handles.color = Color.white;
        //    //var x = va.Voxels.Values;
        //    foreach (var ele in RecastNaviMesh.Instance.GetWalkable())
        //    {
        //        if (ele.Distance == null) continue;

        //        Vector3 pos;
        //        va.GetCellPosition(ele.Index, out pos);
        //        //퍼포먼스를 위해 가까운것만 그리자.
        //        if (Vector3.Distance(pos, sceneView.camera.transform.position) < 15f)
        //        {
        //            //Handles.Label(pos, ele.Index.ToString());
        //            //Handles.Label(pos, ele.Index.ToString() + "\n" +  ele._distance.Value.ToString());
        //            Handles.Label(pos, ele.Distance.ToString());
        //        }
        //    }
        //}
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    static void DrawGizmo(GizmoDummy dummy, GizmoType gizmoType)
    {
        var va = RecastNaviMesh.Instance.GetArea();
        #region Draw Voxel
        if (va != null && _debug_DrawVoxel == true)
        {
            Color c = Color.red; c.a = 0.7f;
            Gizmos.color = c;

            foreach (var ele in va.Voxels.Values)
            {
                if (ele._ex) continue;
                if (!ele.IsTop) continue;
                if (ele.FaceReverse) continue;

                int count = 1;
                if (ele.IsSpan)
                {
                    for (Voxel vc = ele; ; ++count)
                    {
                        if (vc.IsBottom == true) break;
                        vc = va.GetCell(vc.SpanNext);
                        if (vc == null) break;
                    }
                }

                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;

                tmp.y = tmp.y - (_cellHeight * count * 0.5f) + _cellHeight * 0.5f;
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight * count, _cellSize));
            }
        }
        #endregion

        #region Draw Reverse Voxel
        if (va != null && _debug_DrawReverseVoxel == true)
        {
            Color c = Color.white; c.a = 0.7f;
            Gizmos.color = c;

            foreach (var ele in va.Voxels.Values)
            {
                if (ele._ex) continue;
                if (!ele.IsTop) continue;
                if (!ele.FaceReverse) continue;

                int count = 1;
                if (ele.IsSpan)
                {
                    for (Voxel vc = ele; ; ++count)
                    {
                        if (vc.IsBottom == true) break;
                        vc = va.GetCell(vc.SpanNext);
                        if (vc == null) break;
                    }
                }

                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;

                tmp.y = tmp.y - (_cellHeight * count * 0.5f) + _cellHeight * 0.5f;
                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight * count, _cellSize));
            }
        }
        #endregion

        #region Draw Distance
        if (va != null && _debug_DrawDistance == true)
        {
            foreach (var ele in RecastNaviMesh.Instance.GetWalkable())
            {
                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                float colorFactor = 1f; // white
                if (ele.Distance != null)
                    colorFactor = (float)ele.Distance.Value * (float)ele.Distance.Value / (float)RecastNaviMesh.Instance._maxDistance * 0.3f;

                Color c = Color.white;
                c.r = c.g = c.b = colorFactor;
                Gizmos.color = c;

                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight, _cellSize));
            }
        }
        #endregion

        #region Draw Walkable Cell
        if (va != null && _debug_DrawWalkable == true)
        {
            foreach (var ele in RecastNaviMesh.Instance.GetWalkable())
            {
                if (ele.Legde) continue;

                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;
                Gizmos.color = Color.green;

                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight, _cellSize));
            }
        }
        #endregion

        #region Draw First Ledge
        if (va != null && _debug_DrawFirstLedge == true)
        {
            Gizmos.color = Color.gray;
            foreach (var ele in RecastNaviMesh.Instance._firstLedge)
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
            Color c = Color.cyan; c.a = 0.7f;
            Gizmos.color = c;

            foreach (var ele in RecastNaviMesh.Instance.GetWalkable())
            {
                if (!ele.Legde) continue;
                Vector3 tmp;
                if (!va.GetCellPosition(ele.Index, out tmp)) continue;

                Gizmos.DrawCube(tmp, new Vector3(_cellSize, _cellHeight, _cellSize));
            }
        }
        #endregion

        if (RecastNaviMesh.Instance._contours.Count != 0)
        {
            Gizmos.color = Color.magenta;
            Vector3 prev = Vector3.zero;
            foreach (var ele in RecastNaviMesh.Instance._contours)
            {
                bool bFirst = true;
                foreach (var v in ele)
                {
                    Gizmos.DrawCube(v, Vector3.one * 0.01f);
                    if (bFirst) { prev = v; bFirst = false; continue; }
                    Gizmos.DrawLine(prev, v);
                    prev = v;
                }
            }
        }
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
        _exceptMask = EditorGUILayout.MaskField("Excepts", _exceptMask, layerNames.ToArray());

        if (GUILayout.Button("Scan the map"))
        {
            List<string> pathLayers = new List<string>();
            List<string> exceptMask = new List<string>();

            for (int i = 1; i <= layerNames.Count; ++i)
            {
                if ((_pathMask & (1 << i)) > 0) pathLayers.Add(layerNames[i]);
                if ((_exceptMask & (1 << i)) > 0) exceptMask.Add(layerNames[i]);
            }

            var ai = new AgentInfo();
            ai.AgentHeight = _agentHeight;
            ai.AgentRadius = _agentRadius;
            ai.MaxClimb = _maxClimb;
            ai.MaxSlope = _maxSlope;
            RecastNaviMesh.Instance.SetAgentInfo(ai);
            RecastNaviMesh.Instance.Build(_cellSize, _cellHeight, LayerMask.GetMask(pathLayers.ToArray()), LayerMask.GetMask(exceptMask.ToArray()));
            _mapSize = RecastNaviMesh.Instance.GetMapSize();
        }

        _debug_DrawVoxel = EditorGUILayout.Toggle("Show Voxel", _debug_DrawVoxel);
        _debug_DrawReverseVoxel = EditorGUILayout.Toggle("Show Reverse Voxel", _debug_DrawReverseVoxel);
        _debug_DrawWalkable = EditorGUILayout.Toggle("Show Walkable", _debug_DrawWalkable);
        _debug_DrawDistance = EditorGUILayout.Toggle("Show Distance Field", _debug_DrawDistance);
        _debug_DrawFirstLedge = EditorGUILayout.Toggle("Show First Ledge", _debug_DrawFirstLedge);
        _debug_DrawLedge = EditorGUILayout.Toggle("Show Ledge", _debug_DrawLedge);
        EditorGUILayout.EndVertical();

        SceneView.RepaintAll();
    }
}