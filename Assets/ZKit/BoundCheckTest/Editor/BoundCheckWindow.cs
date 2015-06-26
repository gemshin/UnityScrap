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

    private static Color _lineColor = Color.red;

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
            Handles.color = _lineColor;
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
                Handles.color = _lineColor;
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
            Gizmos.color = _lineColor;
            Gizmos.DrawLine(_prevClickedPos, _currentClickedPos);
        }
        else if (_testMode == TestMode.Ray)
        {
            Gizmos.color = _lineColor;
            Gizmos.DrawRay(_currentClickedPos, (_prevClickedPos - _currentClickedPos) * 100);
        }
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
        bool isIn = false;
        switch (_boundType)
        {
            case BoundType.Box:
                switch(_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", (isIn = _box.CollisionDetect2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        EditorGUILayout.LabelField("In ?", (isIn = _box.CollisionDetect2DLine(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray:
                        EditorGUILayout.LabelField("In ?", (isIn = _box.CollisionDetect2DRay(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Cube:
                switch (_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", (isIn = _cube.CollisionDetect2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        EditorGUILayout.LabelField("In ?", (isIn = _cube.CollisionDetect2DLine(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray:
                        //EditorGUILayout.LabelField("In ?", (isIn = _cube.CollisionDetect2DRay(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Circle:
                switch (_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", (isIn = _circle.CollisionDetect2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        EditorGUILayout.LabelField("In ?", (isIn = _circle.CollisionDetect2DLine(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray:
                        Vector2 dir = new Vector2(_prevClickedPos.x, _prevClickedPos.z) - new Vector2(_currentClickedPos.x, _currentClickedPos.z);
                        EditorGUILayout.LabelField("In ?", (isIn = _circle.CollisionDetect2DRay(new Vector2(_currentClickedPos.x, _currentClickedPos.z), dir)) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Capsule:
                switch (_testMode)
                {
                    case TestMode.Dot:
                        EditorGUILayout.LabelField("In ?", (isIn = _capsule.CollisionDetect2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line:
                        EditorGUILayout.LabelField("In ?", (isIn = _capsule.CollisionDetect2DLine(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray:
                        Vector2 dir = new Vector2(_prevClickedPos.x, _prevClickedPos.z) - new Vector2(_currentClickedPos.x, _currentClickedPos.z);
                        EditorGUILayout.LabelField("In ?", (isIn = _capsule.CollisionDetect2DRay(new Vector2(_currentClickedPos.x, _currentClickedPos.z), dir)) ? "Yes" : "No");
                        break;
                }
                break;
        }

        if (isIn) _lineColor = Color.green;
        else _lineColor = Color.red;

        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();
    }

}
