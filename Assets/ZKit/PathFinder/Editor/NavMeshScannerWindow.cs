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

    }

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
        }
        EditorGUILayout.EndVertical();
    }
}