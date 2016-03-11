using UnityEngine;
using UnityEditor;

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
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    static void DrawGizmo(GizmoDummy dummy, GizmoType gizmoType)
    {
        
    }

    static Rect _testSize = new Rect();

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("1st Step - Scan");
        EditorGUILayout.EndVertical();
        //cells.CellSize = EditorGUILayout.FloatField("CellSize", cells.CellSize);
        //cells.HeightLimit = EditorGUILayout.FloatField("height Limit", cells.HeightLimit);
        if (GUILayout.Button("Scan the map"))
        {
            _testSize = ScanMapSize();
        }
        EditorGUILayout.EndVertical();
    }

    private Rect ScanMapSize()
    {
        TerrainCollider[] terrain = (TerrainCollider[])GameObject.FindObjectsOfType(typeof(TerrainCollider));
        // TODO : o 의 크기가 0이면 건너뛴다.

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

        GameObject[] o = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));

        //int layerMask = 1 << 8;
        //#region 상하좌우 크기를 검색
        //for (int i = 0; i < o.Length; ++i)
        //{
        //    Vector3 point = o[i].transform.position; point.y += 100.0f;
        //    if (Physics.Raycast(point, Vector3.down, 2000, layerMask))
        //    {
        //        if (result.xMax < point.x) result.xMax = point.x;
        //        else if (result.xMin > point.x) result.xMin = point.x;
        //        if (result.yMax < point.z) result.yMax = point.z;
        //        else if (result.yMin > point.z) result.yMin = point.z;
        //    }
        //}
        //#endregion

        //#region 혹시 모를 자투리 여백을 검색 ( 100정도 크기 확인해보고 줄일 수 있음 더 줄이자. )
        //for (float i = result.xMin; i < result.xMax; ++i)
        //{
        //    float yMax = result.yMax;
        //    float yMin = result.yMin;
        //    for (int j = 1; j < 100; ++j)
        //    {
        //        if (Physics.Raycast(new Vector3(i, 100f, yMax + j), Vector3.down, 2000, layerMask))
        //            result.yMax = yMax + j;
        //        if (Physics.Raycast(new Vector3(i, 100f, yMin - j), Vector3.down, 2000, layerMask))
        //            result.yMin = yMin - j;
        //    }
        //}

        //for (float i = result.yMin; i < result.yMax; ++i)
        //{
        //    float xMax = result.xMax;
        //    float xMin = result.xMin;
        //    for (int j = 1; j < 100; ++j)
        //    {
        //        if (Physics.Raycast(new Vector3(xMax + j, 100f, i), Vector3.down, 2000, layerMask))
        //            result.xMax = xMax + j;
        //        if (Physics.Raycast(new Vector3(xMin - j, 100f, i), Vector3.down, 2000, layerMask))
        //            result.xMin = xMin - j;
        //    }
        //}
        //#endregion

        // 2칸정도 여백을 추가.
        result.xMax += 2f;
        result.xMin -= 2f;
        result.yMax += 2f;
        result.yMin -= 2f;

        return result;
    }
}