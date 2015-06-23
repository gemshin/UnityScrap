using UnityEngine;
using UnityEditor;
using System.Collections;

using ZKit;

public class BoundCheckWindow : EditorWindow
{
    private enum BoundType
    {
        Box,
        Cube,
        Capsule,
        Circle
    }
    private enum TestMode
    {
        Dot,
        Line,
        Ray,
        None
    }
    private static BoundType _boundType = BoundType.Box;
    private static TestMode _testMode = TestMode.None;

    private static Vector3 _currentClickedPos;
    private static Vector3 _prevClickedPos;

    private static bool _clickMode_cur = false;
    private static bool _clickMode_prev = false;

    #region box
    private static Box _box = new Box();
    #endregion

    #region cube
    private static Cube _cube = new Cube();
    #endregion

    #region circle
    private static Circle _circle = new Circle(); 
    #endregion

    #region capsule
    private static Capsule _capsule = new Capsule();
    #endregion

    [MenuItem("TEST/BoundCheck")]
    static void Init()
    {
        BoundCheckWindow window = (BoundCheckWindow)EditorWindow.GetWindow(typeof(BoundCheckWindow), false, "BoundCheck");
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
        if (_testMode != TestMode.None)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            #region Mouse Click Event
            if (_testMode == TestMode.Dot && _clickMode_cur)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Vector2 MousePos = Event.current.mousePosition;
                    MousePos.y = sceneView.camera.pixelHeight - MousePos.y;

                    RaycastHit hit;
                    if (Physics.Raycast(sceneView.camera.ScreenPointToRay(MousePos), out hit, 10000, (1 << 8)))
                    {
                        _prevClickedPos = _currentClickedPos;
                        _currentClickedPos = hit.point;
                        Debug.Log(hit.point);
                    }
                    this.Repaint();
                }
            }
            #endregion

            #region Draw Click point
            bool buttonClicked = false;
            Handles.color = Color.red;
            if (Handles.Button(_currentClickedPos, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(_currentClickedPos), 0.1f, Handles.DotCap))
            {
                _clickMode_cur = !_clickMode_cur;
                buttonClicked = true;
                this.Repaint();
            }

            EditorGUI.BeginChangeCheck();
            if(_clickMode_cur)
                _currentClickedPos = Handles.PositionHandle(_currentClickedPos, Quaternion.identity);

            if (_testMode == TestMode.Line || _testMode == TestMode.Ray)
            {
                Handles.color = new Color(0.8f, 0f, 0f);
                if (Handles.Button(_prevClickedPos, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(_prevClickedPos), 0.1f, Handles.DotCap))
                {
                    _clickMode_prev = !_clickMode_prev;
                    buttonClicked = true;
                    this.Repaint();
                }
                if(_clickMode_prev)
                    _prevClickedPos = Handles.PositionHandle(_prevClickedPos, Quaternion.identity);
            }

            if (EditorGUI.EndChangeCheck())
                this.Repaint();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (!buttonClicked)
                {
                    _clickMode_cur = false;
                    _clickMode_prev = false;
                    this.Repaint();
                }
            }
            #endregion
        }
        sceneView.Repaint();
    }

    [DrawGizmo(GizmoType.NotSelected | GizmoType.Selected)]
    static void DrawGizmo(GizmoDummy dummy, GizmoType gizmoType)
    {
        Gizmos.color = Color.cyan;

        switch (_boundType)
        {
            case BoundType.Box:
                _box.DrawGizmo();
                break;
            case BoundType.Cube:
                _cube.DrawGizmo();
                break;
            case BoundType.Circle:
                _circle.DrawGizmo();
                break;
            case BoundType.Capsule:
                _capsule.DrawGizmo();
                break;
        }

        if (_testMode == TestMode.Line)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_prevClickedPos, _currentClickedPos);
        }
        else if (_testMode == TestMode.Ray)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_prevClickedPos, (_currentClickedPos - _prevClickedPos) * 100);
        }
    }

    private bool IsInZone(Vector3 pos, Vector3 boxPos, Vector3 boxScale, float boxRotation)
    {
        return false;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();
        _boundType = (BoundType)EditorGUILayout.EnumPopup("Bound Type", _boundType);

        switch (_boundType)
        {
            case BoundType.Box:
                _box.position = EditorGUILayout.Vector3Field("position", _box.position);
                _box.size = EditorGUILayout.Vector2Field("Scale", _box.size);
                _box.rotate.y = EditorGUILayout.Slider("rotate Y angle", _box.rotate.y, 0f, 360f);
                break;
            case BoundType.Cube:
                _cube.position = EditorGUILayout.Vector3Field("position", _cube.position);
                _cube.size = EditorGUILayout.Vector3Field("Scale", _cube.size);
                _cube.rotate.y = EditorGUILayout.Slider("rotate Y angle", _cube.rotate.y, 0f, 360f);
                break;
            case BoundType.Circle:
                _circle.position = EditorGUILayout.Vector3Field("position", _circle.position);
                _circle.radius = EditorGUILayout.FloatField("radius", _circle.radius);
                break;
            case BoundType.Capsule:
                _capsule.position = EditorGUILayout.Vector3Field("position", _capsule.position);
                _capsule.radius = EditorGUILayout.FloatField("radius", _capsule.radius);
                _capsule.height = EditorGUILayout.FloatField("height", _capsule.height);
                break;
        }
        if (EditorGUI.EndChangeCheck())
        {
            //Calculate();
            SceneView.RepaintAll();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        //EditorGUILayout.
        _testMode = (TestMode)EditorGUILayout.EnumPopup("Test Mode", _testMode);
        if (_testMode != TestMode.None)
        {
            EditorGUILayout.BeginHorizontal();
            _currentClickedPos = EditorGUILayout.Vector3Field("Clicked Pos A", _currentClickedPos);
            GUIStyle gs = new GUIStyle("button");
            if (_clickMode_cur) gs.normal.textColor = Color.green;
            else                gs.normal.textColor = Color.red;
            if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                _clickMode_cur = !_clickMode_cur;
            EditorGUILayout.EndHorizontal();
            if (_testMode == TestMode.Line || _testMode == TestMode.Ray)
            {
                EditorGUILayout.BeginHorizontal();
                _prevClickedPos = EditorGUILayout.Vector3Field("Clicked Pos B", _prevClickedPos);
                if (_clickMode_prev) gs.normal.textColor = Color.green;
                else                 gs.normal.textColor = Color.red;
                if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                    _clickMode_prev = !_clickMode_prev;
                EditorGUILayout.EndHorizontal();
            }
            
        }
        switch (_boundType)
        {
            case BoundType.Box:
                switch(_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", _box.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        EditorGUILayout.LabelField("In ?", _box.Check2DLine(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z)) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Cube:
                switch (_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", _cube.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        break;
                }
                break;
            case BoundType.Circle:
                switch (_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", _circle.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        break;
                }
                break;
            case BoundType.Capsule:
                switch (_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", _capsule.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        break;
                }
                break;
        }
        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();
    }

}
