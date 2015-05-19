using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ScannerWindow : EditorWindow
{
    private static bool _showGrid = true;
    private static bool _showIndex = false;
    private static float _viewDistance = 20f;
    private static float _gridGap = 0.1f;

    private static bool _testMode = false;
    private static Vector3 _testAsHero = new Vector3();
    public Transform _testHero;
//    private static Vector3 _testJpsHero = new Vector3();
    private static int _testHeroCurIndex = 0;
    private static bool _testHeroMoveMode = false;
    private float _testHeroMoveTime = 0f;
    private float _testHeroSpeed = 1f;
    private Point _testClickedIndex = new Point();

    #region A*
    private bool _testShowAsPath = false;
    private bool _testShowAClosedPoint = false;
    private bool _testShowAValue = false;
    #endregion

    #region JPS
    private bool _testShowJumpPointPath = false;
    private bool _testShowJumpPoint = false;
    private bool _testShowJClosedPoint = false;
    private bool _testShowJValue = false;
    #endregion

    List<Point> _asPath = null;
    List<Point> _jpsPath = null;

    [MenuItem("TEST/Scanner")]
    static void Init()
    {
        ScannerWindow window = (ScannerWindow)EditorWindow.GetWindow(typeof(ScannerWindow), false, "Scanner");
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
        if (_testMode) HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (DataCon.Instance.CellDatas.IsEmpty) return;
        var cells = DataCon.Instance.CellDatas;

        #region Draw Map Bound
        Handles.color = Color.red;
        for (int y = 0; y < 3; ++y)
        {
            Handles.DrawLine(new Vector3(cells.MapBound.xMax, y, cells.MapBound.yMax), new Vector3(cells.MapBound.xMin, y, cells.MapBound.yMax));
            Handles.DrawLine(new Vector3(cells.MapBound.xMin, y, cells.MapBound.yMax), new Vector3(cells.MapBound.xMin, y, cells.MapBound.yMin));
            Handles.DrawLine(new Vector3(cells.MapBound.xMin, y, cells.MapBound.yMin), new Vector3(cells.MapBound.xMax, y, cells.MapBound.yMin));
            Handles.DrawLine(new Vector3(cells.MapBound.xMax, y, cells.MapBound.yMin), new Vector3(cells.MapBound.xMax, y, cells.MapBound.yMax));
        }
        #endregion

        if (_testMode && _testShowAClosedPoint)
        {
            for (int y = 0; y < cells.CountY; ++y)
            {
                for (int x = 0; x < cells.CountX; ++x)
                {
                    if (PathFinder.AStar.Instance._map[y, x].F != 0)
                    {
                        Handles.color = Color.cyan;
                        Handles.DotCap(0, cells.GetPosVec3(cells[y, x].Index, cells[y, x].Height), Quaternion.identity, 0.4f);
                    }
                }
            }
        }

        #region Draw JPS ClosedPoint
        if (_testMode && _testShowJClosedPoint)
        {
            if (PathFinder.JPS.Instance._closedList != null)
            {
                for (int i = 0; i < PathFinder.JPS.Instance._closedList.Length; ++i)
                {
                    if (PathFinder.JPS.Instance._closedList[i] != null)
                    {
                        Handles.color = Color.magenta;
                        Handles.DotCap(0, cells.GetPosVec3(PathFinder.JPS.Instance._closedList[i].Index, PathFinder.JPS.Instance._closedList[i].Height), Quaternion.identity, 0.3f);
                    }
                }
            }
        } 
        #endregion

        #region Draw JumpPoint
        if (_testMode && _testShowJumpPoint)
        {
            foreach (PathFinder.Node n in PathFinder.JPS.Instance.TEST)
            {
                Handles.color = Color.blue;
                Handles.DotCap(0, cells.GetPosVec3(n.Index, n.Height), Quaternion.identity, 0.3f);
            }
        } 
        #endregion

        #region Index
        if (_showIndex || _testShowAValue || _testShowJValue)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperCenter;
            for (int y = 0; y < cells.CountY; ++y)
            {
                for (int x = 0; x < cells.CountX; ++x)
                {
                    if (cells[y, x].Type == CellType.Normal)
                    {
                        // 퍼포먼스를 위해 가까운것만 그리자.
                        if (Vector3.Distance(cells.GetPosVec3(cells[y, x].Index, cells[y, x].Height), sceneView.camera.transform.position) < _viewDistance)
                        {
                            string label = "";
                            if( _showIndex )
                                label += string.Format("{0}:{1}", (cells.CountX * y + x), cells[y, x].Index.ToString());

                            if (_testShowAValue && PathFinder.AStar.Instance._map[y, x].F != 0)
                                label += string.Format("\nF={0}-G={1}-H={2}", PathFinder.AStar.Instance._map[y, x].F, PathFinder.AStar.Instance._map[y, x].G, PathFinder.AStar.Instance._map[y, x].H);

                            if (_testShowJValue && PathFinder.JPS.Instance._map[y, x].F != 0)
                                label += string.Format("\nF={0}-G={1}-H={2}", PathFinder.JPS.Instance._map[y, x].F, PathFinder.JPS.Instance._map[y, x].G, PathFinder.JPS.Instance._map[y, x].H);

                            Handles.Label(cells.GetPosVec3(cells[y, x].Index, cells[y, x].Height), label, style);
                        }
                    }
                }
            }
        }
        #endregion

        if (_testMode)
        {
            #region Mouse Click Event
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 MousePos = Event.current.mousePosition;
                MousePos.y = sceneView.camera.pixelHeight - MousePos.y;

                RaycastHit hit;
                if (Physics.Raycast(sceneView.camera.ScreenPointToRay(MousePos), out hit, 10000, (1 << 8)))
                {
                    _testClickedIndex = cells.GetNearIndex(hit.point);
                    _asPath = PathFinder.AStar.Instance.Find(cells.GetNearIndex(_testAsHero), _testClickedIndex);
                    //_jpsPath = PathFinder.JPS.Instance.Find(cells.GetNearIndex(_testJpsHero), _testClickedIndex);
                    _jpsPath = PathFinder.JPS.Instance.Find(cells.GetNearIndex(_testAsHero), _testClickedIndex);

                    _testHeroCurIndex = 1;
                    _testHeroMoveTime = 0f;
                }
                sceneView.Repaint();
            }
            #endregion

            #region Draw Path
            if (_testShowAsPath && _asPath != null)
            {
                Handles.color = Color.red;
                for (int i = 0; i < _asPath.Count; ++i)
                {
                    Vector3 point = cells.GetPosVec3(_asPath[i], cells[_asPath[i].y, _asPath[i].x].Height);
                    Handles.DotCap(0, point, Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(point));

                    if (i + 1 >= _asPath.Count) continue;
                    Handles.DrawLine(point, cells.GetPosVec3(_asPath[i + 1], cells[_asPath[i + 1].y, _asPath[i + 1].x].Height));
                }
            }

            if (_testShowJumpPointPath && _jpsPath != null)
            {
                Handles.color = Color.yellow;
                for (int i = 0; i < _jpsPath.Count; ++i)
                {
                    Vector3 point = cells.GetPosVec3(_jpsPath[i], cells[_jpsPath[i].y, _jpsPath[i].x].Height);
                    Handles.DotCap(0, point, Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(point));

                    if (i + 1 >= _jpsPath.Count) continue;
                    Handles.DrawLine(point, cells.GetPosVec3(_jpsPath[i + 1], cells[_jpsPath[i + 1].y, _jpsPath[i + 1].x].Height));
                }
            }
            #endregion

            #region Draw Destination Point
            Handles.color = Color.green;
            Vector3 clickedPoint = cells.GetPosVec3(_testClickedIndex, cells[_testClickedIndex.y, _testClickedIndex.x].Y + _gridGap);
            Handles.DotCap(0, clickedPoint, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(clickedPoint));
            #endregion
        }
    }

    void Update()
    {
        if (DataCon.Instance.CellDatas.IsEmpty) return;

        #region test Hero 를 움직인다.
        if (_testMode && _testHeroMoveMode )
        {
            _testHeroMoveTime += (Time.deltaTime * (1000f * _testHeroSpeed) * (1 / DataCon.Instance.CellDatas.CellSize));
            if (_testHeroMoveTime >= 1f)
            {
                _testHeroMoveTime = (_testHeroMoveTime - 1f) >= 1f ? 0f : (_testHeroMoveTime - 1f);
                _testHeroCurIndex++;
            }

            if (_asPath != null && _testHeroCurIndex < _asPath.Count)
            {
                var cells = DataCon.Instance.CellDatas;

                Vector3 f = cells.GetPosVec3(_asPath[_testHeroCurIndex - 1], cells[_asPath[_testHeroCurIndex - 1].y, _asPath[_testHeroCurIndex - 1].x].Height);
                Vector3 t = cells.GetPosVec3(_asPath[_testHeroCurIndex], cells[_asPath[_testHeroCurIndex].y, _asPath[_testHeroCurIndex].x].Height);

                _testAsHero = Vector3.Lerp(f, t, _testHeroMoveTime);
            }

            //if (_jpsPath != null && _testHeroCurIndex < _jpsPath.Count)
            //{
            //    var cells = DataCon.Instance.CellDatas;

            //    Vector3 f = cells.GetPosVec3(_jpsPath[_testHeroCurIndex - 1], cells[_jpsPath[_testHeroCurIndex - 1].y, _jpsPath[_testHeroCurIndex - 1].x].Height);
            //    Vector3 t = cells.GetPosVec3(_jpsPath[_testHeroCurIndex], cells[_jpsPath[_testHeroCurIndex].y, _jpsPath[_testHeroCurIndex].x].Height);

            //    _testJpsHero = Vector3.Lerp(f, t, _testHeroMoveTime);
            //}
        }
        #endregion

        SceneView.lastActiveSceneView.Repaint();
    }

    [DrawGizmo(GizmoType.NotSelected | GizmoType.Selected)]
    static void DrawGizmo(GizmoDummy dummy, GizmoType gizmoType)
    {
        #region Draw Cell Grid
        if (_showGrid)
        {
            var cells = DataCon.Instance.CellDatas;
            for (int y = 0; y < cells.CountY; ++y)
            {
                for (int x = 0; x < cells.CountX; ++x)
                {
                    Gizmos.color = Color.white;
                    if (cells[y, x].Type != CellType.Normal)
                        continue;
                    CellData currentCell = cells[y, x];
                    // right
                    if (cells.IsExist(x + 1, y))
                    {
                        CellData nextCell = cells[y, x + 1];
                        if (nextCell.Type == CellType.Normal)
                        {
                            if (UnityEngine.Mathf.Abs(currentCell.Height - nextCell.Height) < cells.HeightLimit)
                            {
                                Vector3 from = cells.GetPosVec3(new Point(x, y), currentCell.Height + _gridGap);
                                Vector3 to = cells.GetPosVec3(new Point(x + 1, y), nextCell.Height + _gridGap);
                                Gizmos.DrawLine(from, to);
                            }
                        }
                    }
                    // up
                    if (cells.IsExist(x, y + 1))
                    {
                        CellData nextCell = cells[y + 1, x];
                        if (nextCell.Type == CellType.Normal)
                        {
                            if (UnityEngine.Mathf.Abs(currentCell.Height - nextCell.Height) < cells.HeightLimit)
                            {
                                Vector3 from = cells.GetPosVec3(new Point(x, y), currentCell.Height + _gridGap);
                                Vector3 to = cells.GetPosVec3(new Point(x, y + 1), nextCell.Height + _gridGap);
                                Gizmos.DrawLine(from, to);
                            }
                        }
                    }
                    Gizmos.color = Color.cyan;
                    // 퍼포먼스를 위해 가까운것만 그리자.
                    if (Vector3.Distance(cells.GetPosVec3(currentCell.Index, currentCell.Height), SceneView.lastActiveSceneView.camera.transform.position) < _viewDistance)
                    {
                        // right up
                        if (cells.IsExist(x + 1, y + 1))
                        {
                            CellData nextCell = cells[y + 1, x + 1];
                            if (nextCell.Type == CellType.Normal)
                            {
                                if (UnityEngine.Mathf.Abs(currentCell.Height - nextCell.Height) < cells.HeightLimit)
                                {
                                    Vector3 from = cells.GetPosVec3(new Point(x, y), currentCell.Height + _gridGap);
                                    Vector3 to = cells.GetPosVec3(new Point(x + 1, y + 1), nextCell.Height + _gridGap);
                                    Gizmos.DrawLine(from, to);
                                }
                            }
                        }
                        // left up
                        if (cells.IsExist(x - 1, y + 1))
                        {
                            CellData nextCell = cells[y + 1, x - 1];
                            if (nextCell.Type == CellType.Normal)
                            {
                                if (UnityEngine.Mathf.Abs(currentCell.Height - nextCell.Height) < cells.HeightLimit)
                                {
                                    Vector3 from = cells.GetPosVec3(new Point(x, y), currentCell.Height + _gridGap);
                                    Vector3 to = cells.GetPosVec3(new Point(x - 1, y + 1), nextCell.Height + _gridGap);
                                    Gizmos.DrawLine(from, to);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Draw TestHero
        if (_testMode)
        {
            Gizmos.color = Color.blue;
            Vector3 pos = _testAsHero; pos.y += 1f + _gridGap;
            Gizmos.DrawWireCube(pos, new Vector3(1f, 2f, 1f));
            Gizmos.DrawWireCube(pos, new Vector3(0.7f, 1.8f, 0.7f));
            Color c = Color.yellow; c.a = 0.5f;
            Gizmos.color = c;
            Gizmos.DrawCube(pos, new Vector3(1f, 2f, 1f));
            c.a = 1.0f;
            Gizmos.color = c;
            Gizmos.DrawCube(pos, new Vector3(0.7f, 1.8f, 0.7f));

            //Gizmos.color = Color.blue;
            //pos = _testJpsHero; pos.y += 1f + _gridGap;
            //Gizmos.DrawWireCube(pos, new Vector3(1f, 2f, 1f));
            //Gizmos.DrawWireCube(pos, new Vector3(0.7f, 1.8f, 0.7f));
            //c = Color.red; c.a = 0.5f;
            //Gizmos.color = c;
            //Gizmos.DrawCube(pos, new Vector3(1f, 2f, 1f));
            //c.a = 1.0f;
            //Gizmos.color = c;
            //Gizmos.DrawCube(pos, new Vector3(0.7f, 1.8f, 0.7f));
        }
        #endregion
    }

    void OnGUI()
    {
        var cells = DataCon.Instance.CellDatas;

        EditorGUILayout.BeginVertical();

        #region 1st Step - Map Scan
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("1st Step - Scan");
        EditorGUILayout.EndVertical();
        cells.CellSize = EditorGUILayout.FloatField("CellSize", cells.CellSize);
        cells.HeightLimit = EditorGUILayout.FloatField("Height Limit", cells.HeightLimit);
        if (GUILayout.Button("Scan the map"))
        {
            cells.Clear();
            _testMode = false;
            _testClickedIndex = new Point();

            cells.MapBound = ScanMapSize();

            int layerMask = 1 << 8;

            int height = (int)((cells.MapBound.yMax - cells.MapBound.yMin) / cells.CellSize);
            int width = (int)((cells.MapBound.xMax - cells.MapBound.xMin) / cells.CellSize);

            cells.Create(width, height);
            cells.StartingPoint = new Vector2(cells.MapBound.xMin, cells.MapBound.yMax);

            foreach (CellData ele in cells)
            {
                Vector3 rayStart = cells.GetPosVec3(ele.Index, 100f);
                RaycastHit hit;
                if (Physics.Raycast(rayStart, Vector3.down, out hit, 2000f, layerMask))
                {
                    ele.Type = CellType.Normal;
                    ele.Height = hit.point.y;

                    if (ele.Height > 0.1f || ele.Height < -0.1f) ele.Type = CellType.CantMove;
                }
                else
                {
                    ele.Type = CellType.CantMove;
                }

                if (Physics.Raycast(rayStart, Vector3.down, 2000f, (1 << 0)))
                {
                    ele.Type = CellType.CantMove;
                }
            }

            PathFinder.AStar.Instance.SetMap();
            PathFinder.JPS.Instance.SetMap();
            _asPath = null;
        }
        #endregion

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("00000000000000");
        EditorGUILayout.ObjectField("Target", _testHero, typeof(Transform));

        if (!cells.IsEmpty)
        {
            #region View Options
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("View Options");
            _showGrid = EditorGUILayout.Toggle("- Show Grid", _showGrid);
            _showIndex = EditorGUILayout.Toggle("- Show Index", _showIndex);
            _viewDistance = EditorGUILayout.Slider("- View Distance", _viewDistance, 10f, 100f);
            EditorGUILayout.LabelField("- Map Index Width", ((int)(cells.MapBound.width / cells.CellSize)).ToString());
            EditorGUILayout.LabelField("- Map Index Height", ((int)(cells.MapBound.height / cells.CellSize)).ToString());
            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.Separator();

            #region 2nd Step - Test & Review
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("2nd Step - Test & Review");
            EditorGUILayout.EndVertical();
            bool pastTestMode = _testMode;
            _testMode = EditorGUILayout.Toggle("Test Mode", _testMode);
            if (_testMode != pastTestMode && _testMode)
            {
                foreach (CellData element in cells)
                {
                    if (element.Type == CellType.Normal)
                    {
                        _testAsHero = cells.GetPosVec3(element.Index, element.Y);
                        //_testJpsHero = cells.GetPosVec3(element.Index, element.Y);
                        break;
                    }
                }
            }
            if (_testMode)
            {
                _testShowAsPath = EditorGUILayout.Toggle("- Show A* Line", _testShowAsPath);
                if (_testShowAsPath)
                {
                    _testShowAClosedPoint = EditorGUILayout.Toggle("-- Show ClosedPoint", _testShowAClosedPoint);
                    _testShowAValue = EditorGUILayout.Toggle("-- Show A Star Value", _testShowAValue);
                }
                _testShowJumpPointPath = EditorGUILayout.Toggle("- Show JumpPoint Line", _testShowJumpPointPath);
                if (_testShowJumpPointPath)
                {
                    _testShowJumpPoint = EditorGUILayout.Toggle("-- Show JumpPoint", _testShowJumpPoint);
                    _testShowJClosedPoint = EditorGUILayout.Toggle("-- Show ClosedPoint", _testShowJClosedPoint);
                    _testShowJValue = EditorGUILayout.Toggle("-- Show JumpPoint Value", _testShowJValue);
                }
                _testHeroMoveMode = EditorGUILayout.Toggle("- Hero Move", _testHeroMoveMode);
                if (_testHeroMoveMode)
                {
                    _testHeroSpeed = EditorGUILayout.Slider("- Hero Speed", _testHeroSpeed, 0f, 10f);
                }
            }
            #endregion
        }
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

        int layerMask = 1 << 8;
        #region 상하좌우 크기를 검색
        for (int i = 0; i < o.Length; ++i)
        {
            Vector3 point = o[i].transform.position; point.y += 100.0f;
            if (Physics.Raycast(point, Vector3.down, 2000, layerMask))
            {
                if (result.xMax < point.x) result.xMax = point.x;
                else if (result.xMin > point.x) result.xMin = point.x;
                if (result.yMax < point.z) result.yMax = point.z;
                else if (result.yMin > point.z) result.yMin = point.z;
            }
        }
        #endregion

        #region 혹시 모를 자투리 여백을 검색 ( 100정도 크기 확인해보고 줄일 수 있음 더 줄이자. )
        for (float i = result.xMin; i < result.xMax; ++i)
        {
            float yMax = result.yMax;
            float yMin = result.yMin;
            for (int j = 1; j < 100; ++j)
            {
                if (Physics.Raycast(new Vector3(i, 100f, yMax + j), Vector3.down, 2000, layerMask))
                    result.yMax = yMax + j;
                if (Physics.Raycast(new Vector3(i, 100f, yMin - j), Vector3.down, 2000, layerMask))
                    result.yMin = yMin - j;
            }
        }

        for (float i = result.yMin; i < result.yMax; ++i)
        {
            float xMax = result.xMax;
            float xMin = result.xMin;
            for (int j = 1; j < 100; ++j)
            {
                if (Physics.Raycast(new Vector3(xMax + j, 100f, i), Vector3.down, 2000, layerMask))
                    result.xMax = xMax + j;
                if (Physics.Raycast(new Vector3(xMin - j, 100f, i), Vector3.down, 2000, layerMask))
                    result.xMin = xMin - j;
            }
        }
        #endregion

        // 2칸정도 여백을 추가.
        result.xMax += 2f;
        result.xMin -= 2f;
        result.yMax += 2f;
        result.yMin -= 2f;

        return result;
    }
}
