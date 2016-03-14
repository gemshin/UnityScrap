using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using ZKit;

public class NavMeshScannerWindow : EditorWindow
{
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
            Handles.DrawLine(new Vector3(_testSize.xMax, y, _testSize.yMax), new Vector3(_testSize.xMin, y, _testSize.yMax));
            Handles.DrawLine(new Vector3(_testSize.xMin, y, _testSize.yMax), new Vector3(_testSize.xMin, y, _testSize.yMin));
            Handles.DrawLine(new Vector3(_testSize.xMin, y, _testSize.yMin), new Vector3(_testSize.xMax, y, _testSize.yMin));
            Handles.DrawLine(new Vector3(_testSize.xMax, y, _testSize.yMin), new Vector3(_testSize.xMax, y, _testSize.yMax));
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

    static Rect _testSize = new Rect();

    static int _pathLayerMask;
    static int _obstacleLayerMask;

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
            _testSize = ScanMapSize();
        }
        EditorGUILayout.EndVertical();
    }

    private Rect ScanMapSize()
    {
        TerrainCollider[] terrain = (TerrainCollider[])GameObject.FindObjectsOfType(typeof(TerrainCollider));

        Rect result = new Rect(0f, 0f, 0f, 0f);

        if (terrain != null && terrain.Length != 0)
        {
            result.xMax = terrain[0].bounds.max.x;
            result.xMin = terrain[0].bounds.min.x;
            result.yMax = terrain[0].bounds.max.z;
            result.yMin = terrain[0].bounds.min.z;

            #region 상하좌우 크기를 검색
            for (int i = 0; i < terrain.Length; ++i)
            {
                if (result.xMax < terrain[i].bounds.max.x) result.xMax = terrain[i].bounds.max.x;
                else if (result.xMin > terrain[i].bounds.min.x) result.xMin = terrain[i].bounds.min.x;
                if (result.yMax < terrain[i].bounds.max.z) result.yMax = terrain[i].bounds.max.z;
                else if (result.yMin > terrain[i].bounds.min.z) result.yMin = terrain[i].bounds.min.z;
            }
            #endregion
        }

        List<string> layerNames = new List<string>();
        for (int i = 0; i < 32; ++i)
        {
            if (LayerMask.LayerToName(i).Length != 0)
                layerNames.Add(LayerMask.LayerToName(i));
        }
        List<string> selectedLayers = new List<string>();
        for (int i = 0; i < layerNames.Count; ++i)
        {
            if (((_pathLayerMask & (1 << i)) != 0) || ((_obstacleLayerMask & (1 << i)) != 0))
                selectedLayers.Add(layerNames[i]);
        }
        int selectedLayerMask = LayerMask.GetMask(selectedLayers.ToArray());

        foreach (GameObject go in (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if ((selectedLayerMask & (1<<go.layer)) == 0) continue;

            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (!mf) continue;
            if (!mf.sharedMesh) continue;
            foreach(var vertex in mf.sharedMesh.vertices)
            {
                Vector3 v = vertex;
                v.Scale(go.transform.localScale);
                v = go.transform.rotation * v;
                v += go.transform.position;

                if (result.xMax < v.x) result.xMax = v.x;
                else if (result.xMin > v.x) result.xMin = v.x;

                if (result.yMax < v.z) result.yMax = v.z;
                else if (result.yMin > v.z) result.yMin = v.z;
            }
        }

        return result;
    }
}