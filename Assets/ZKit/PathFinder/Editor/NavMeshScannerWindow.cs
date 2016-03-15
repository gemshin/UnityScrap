using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using ZKit;

public class NavMeshScannerWindow : EditorWindow
{
    static Rect _mapSize = new Rect();

    static int _pathLayerMask;
    static int _obstacleLayerMask;

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
        for (int y = 0; y < 3; ++y)
        {
            Handles.DrawLine(new Vector3(_mapSize.xMax, y, _mapSize.yMax), new Vector3(_mapSize.xMin, y, _mapSize.yMax));
            Handles.DrawLine(new Vector3(_mapSize.xMin, y, _mapSize.yMax), new Vector3(_mapSize.xMin, y, _mapSize.yMin));
            Handles.DrawLine(new Vector3(_mapSize.xMin, y, _mapSize.yMin), new Vector3(_mapSize.xMax, y, _mapSize.yMin));
            Handles.DrawLine(new Vector3(_mapSize.xMax, y, _mapSize.yMin), new Vector3(_mapSize.xMax, y, _mapSize.yMax));
        }
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
        
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("1st Step - Scan");
        EditorGUILayout.EndVertical();
        //cells.CellSize = EditorGUILayout.FloatField("CellSize", cells.CellSize);
        //cells.HeightLimit = EditorGUILayout.FloatField("height Limit", cells.HeightLimit);

        List<string> layerNames = new List<string>();
        for(int i = 0; i < 32; ++i)
        {
            if(LayerMask.LayerToName(i).Length != 0)
                layerNames.Add(LayerMask.LayerToName(i));
        }
        _pathLayerMask = EditorGUILayout.MaskField("Path",_pathLayerMask, layerNames.ToArray());
        _obstacleLayerMask = EditorGUILayout.MaskField("Obstacles", _obstacleLayerMask, layerNames.ToArray());

        if (GUILayout.Button("Scan the map"))
        {
            _mapSize = ZKit.PathFinder.UUtil.ScanMapSize(_pathLayerMask, _obstacleLayerMask);
        }
        EditorGUILayout.EndVertical();
    }
}