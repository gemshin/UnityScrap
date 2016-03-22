using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using ZKit;

public class NavMeshScannerWindow : EditorWindow
{
    static Cuboid _mapSize = new Cuboid();

    static int _pathLayerMask;
    static int _obstacleLayerMask;

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
        Handles.DrawLine(new Vector3(_mapSize.xMax, _mapSize.yMax, _mapSize.zMax), new Vector3(_mapSize.xMin, _mapSize.yMax, _mapSize.zMax));
        Handles.DrawLine(new Vector3(_mapSize.xMin, _mapSize.yMax, _mapSize.zMax), new Vector3(_mapSize.xMin, _mapSize.yMax, _mapSize.zMin));
        Handles.DrawLine(new Vector3(_mapSize.xMin, _mapSize.yMax, _mapSize.zMin), new Vector3(_mapSize.xMax, _mapSize.yMax, _mapSize.zMin));
        Handles.DrawLine(new Vector3(_mapSize.xMax, _mapSize.yMax, _mapSize.zMin), new Vector3(_mapSize.xMax, _mapSize.yMax, _mapSize.zMax));

        Handles.DrawLine(new Vector3(_mapSize.xMax, _mapSize.yMin, _mapSize.zMax), new Vector3(_mapSize.xMin, _mapSize.yMin, _mapSize.zMax));
        Handles.DrawLine(new Vector3(_mapSize.xMin, _mapSize.yMin, _mapSize.zMax), new Vector3(_mapSize.xMin, _mapSize.yMin, _mapSize.zMin));
        Handles.DrawLine(new Vector3(_mapSize.xMin, _mapSize.yMin, _mapSize.zMin), new Vector3(_mapSize.xMax, _mapSize.yMin, _mapSize.zMin));
        Handles.DrawLine(new Vector3(_mapSize.xMax, _mapSize.yMin, _mapSize.zMin), new Vector3(_mapSize.xMax, _mapSize.yMin, _mapSize.zMax));

        Handles.DrawLine(new Vector3(_mapSize.xMax, _mapSize.yMax, _mapSize.zMax), new Vector3(_mapSize.xMax, _mapSize.yMin, _mapSize.zMax));
        Handles.DrawLine(new Vector3(_mapSize.xMin, _mapSize.yMax, _mapSize.zMax), new Vector3(_mapSize.xMin, _mapSize.yMin, _mapSize.zMax));
        Handles.DrawLine(new Vector3(_mapSize.xMin, _mapSize.yMax, _mapSize.zMin), new Vector3(_mapSize.xMin, _mapSize.yMin, _mapSize.zMin));
        Handles.DrawLine(new Vector3(_mapSize.xMax, _mapSize.yMax, _mapSize.zMin), new Vector3(_mapSize.xMax, _mapSize.yMin, _mapSize.zMin));
        #endregion

        #region Draw Cell Size
        Handles.color = Color.white;
        uint xCount = (uint)System.Math.Truncate((_mapSize.xMax - _mapSize.xMin) / _cellSize);
        uint yCount = (uint)System.Math.Truncate((_mapSize.yMax - _mapSize.yMin) / _cellHeight);
        uint zCount = (uint)System.Math.Truncate((_mapSize.zMax - _mapSize.zMin) / _cellSize);
        ////for (int i = 1; i <= yCount; ++i)
        //{
        //    //float y = _mapSize.yMin + (i * _cellHeight);
        //    float y = _mapSize.yMin;
        //    for (int j = 1; j <= zCount; ++j)
        //    {
        //        for (int k = 1; k < xCount; ++k)
        //        {
        //            Handles.DrawLine(new Vector3(_mapSize.xMin + (k * _cellSize), y, _mapSize.zMin), new Vector3(_mapSize.xMin + (k * _cellSize), y, _mapSize.zMax));
        //        }
        //        Handles.DrawLine(new Vector3(_mapSize.xMin, y, _mapSize.zMin + (j * _cellSize)), new Vector3(_mapSize.xMax, y, _mapSize.zMin + (j * _cellSize)));
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
        //uint xCount = (uint)System.Math.Truncate((_mapSize.xMax - _mapSize.xMin) / _cellSize);
        //uint yCount = (uint)System.Math.Truncate((_mapSize.yMax - _mapSize.yMin) / _cellHeight);
        //uint zCount = (uint)System.Math.Truncate((_mapSize.zMax - _mapSize.zMin) / _cellSize);
        //for (int i = 1; i <= zCount; ++i)
        //{
        //    for (int j = 1; j <= xCount; ++j)
        //    {
        //        Gizmos.DrawCube(new Vector3(_mapSize.xMin + (j * _cellSize) - (_cellSize * 0.5f), 0f, _mapSize.zMin + (i * _cellSize) - (_cellSize * 0.5f)), new Vector3(_cellSize, _cellHeight, _cellSize));
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
            if(LayerMask.LayerToName(i).Length != 0)
                layerNames.Add(LayerMask.LayerToName(i));
        }
        _pathLayerMask = EditorGUILayout.MaskField("Path",_pathLayerMask, layerNames.ToArray());
        _obstacleLayerMask = EditorGUILayout.MaskField("Obstacles", _obstacleLayerMask, layerNames.ToArray());

        if (GUILayout.Button("Scan the map"))
        {
            _mapSize = ZKit.PathFinder.UUtil.ScanMapSize3D(_pathLayerMask, _obstacleLayerMask);
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
        root.hideFlags = HideFlags.HideAndDontSave;

        Vector3[] vertexList = new Vector3[]
        {
            //new Vector3(-_cellSize, -_cellSize, -_cellSize),
            //new Vector3(-_cellSize, _cellSize, -_cellSize),
            //new Vector3(_cellSize, _cellSize, -_cellSize),
            //new Vector3(_cellSize, -_cellSize, -_cellSize),
            //new Vector3(_cellSize, -_cellSize, _cellSize),
            //new Vector3(_cellSize, _cellSize, _cellSize),
            //new Vector3(-_cellSize, _cellSize, _cellSize),
            //new Vector3(-_cellSize, -_cellSize, _cellSize)
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f)
        };

        int[] faceList = new int[]
        {
            0, 1, 3, //   1: face arrière
            0, 2, 3,
            3, 2, 5, //   2: face droite
            3, 5, 4,
            5, 2, 1, //   3: face dessue
            5, 1, 6,
            3, 4, 7, //   4: face dessous
            3, 7, 0,
            0, 7, 6, //   5: face gauche
            0, 6, 1,
            4, 5, 6, //   6: face avant
            4, 6, 7
         };

        Vector2[] textureCoordinate = new Vector2[] {
             new Vector2(0.0f, 0.0f),
             new Vector2(1.0f, 0.0f),
             new Vector2(0.0f, 1.0f),
             new Vector2(1.0f, 0.0f)
        };

        uint xCount = (uint)System.Math.Truncate((_mapSize.xMax - _mapSize.xMin) / _cellSize);
        uint yCount = (uint)System.Math.Truncate((_mapSize.yMax - _mapSize.yMin) / _cellHeight);
        uint zCount = (uint)System.Math.Truncate((_mapSize.zMax - _mapSize.zMin) / _cellSize);
        for (int i = 1; i <= zCount; ++i)
        {
            for (int j = 1; j <= xCount; ++j)
            {
                GameObject newMesh = new GameObject((j * i).ToString());

                MeshFilter mf = newMesh.AddComponent<MeshFilter>();
                MeshRenderer mr = newMesh.AddComponent<MeshRenderer>();

                Mesh mesh = new Mesh();
                mesh.vertices = vertexList;
                mesh.triangles = faceList;
                mesh.RecalculateNormals();
                mf.mesh = mesh;

                Material material = new Material(Shader.Find("Diffuse"));
                material.color = Color.red;
                mr.material = material;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.useLightProbes = false;
                mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                newMesh.transform.parent = root.transform;
                newMesh.transform.position = new Vector3(_mapSize.xMin + (j * _cellSize) - (_cellSize * 0.5f), 0f, _mapSize.zMin + (i * _cellSize) - (_cellSize * 0.5f));
                newMesh.transform.localScale = new Vector3(_cellSize, _cellHeight, _cellSize);
            }
        }
    }
}