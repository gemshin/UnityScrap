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
        Circle,
        Sector
    }
    private enum TestMode
    {
        Dot2D,
        Line2D,
        Ray2D,
        Box2D,
        LerpBox2D,
        Circle2D,
        Sector2D,
        None
    }
    private static BoundType _boundType = BoundType.Sector;
    private static TestMode _testMode = TestMode.Circle2D;

    private static Vector3 _currentClickedPos;
    private static Vector3 _prevClickedPos;

    private static bool _clickMode_cur = false;
    private static bool _clickMode_prev = false;

    #region lerp
    private static float _lerpTweenFactor = 0f;
    private static float _lerpElapsedTime = 1f;
    #endregion

    #region lerpBox
    private static Vector2 _lerpBoxSize = Vector2.one;
    #endregion

    #region testArc
    private static Sector _testSector = new Sector(); 
    #endregion

    #region testBox
    private static Box _testBox = new Box();
    #endregion

    #region testCircle
    private static Circle _testCircle = new Circle();
    #endregion

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

    #region sector
    private static Sector _sector = new Sector(); 
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
            if (_testMode == TestMode.Dot2D && _clickMode_cur)
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
            if (_clickMode_cur)
            {
                _currentClickedPos = Handles.PositionHandle(_currentClickedPos, Quaternion.identity);
                Handles.Label(_currentClickedPos + Vector3.back * 0.2f, "Pos A");
            }

            if (_testMode == TestMode.Line2D || _testMode == TestMode.Ray2D || _testMode == TestMode.LerpBox2D)
            {
                Handles.color = _lineColor;
                if (Handles.Button(_prevClickedPos, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(_prevClickedPos), 0.1f, Handles.DotCap))
                {
                    _clickMode_prev = !_clickMode_prev;
                    buttonClicked = true;
                    this.Repaint();
                }
                if (_clickMode_prev)
                {
                    _prevClickedPos = Handles.PositionHandle(_prevClickedPos, Quaternion.identity);
                    Handles.Label(_prevClickedPos + Vector3.back * 0.2f, "Pos B");
                }
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
            case BoundType.Sector:
                _sector.DrawGizmo();
                break;
        }

        Gizmos.color = _lineColor;
        switch (_testMode)
        {
            case TestMode.Line2D:
                Gizmos.DrawLine(_prevClickedPos, _currentClickedPos);
                break;
            case TestMode.Ray2D:
                Gizmos.DrawRay(_currentClickedPos, (_prevClickedPos - _currentClickedPos) * 100);
                break;
            case TestMode.Box2D:
                _testBox.DrawGizmo();
                break;
            case TestMode.LerpBox2D:
                Gizmos.DrawLine(_prevClickedPos, _currentClickedPos);
                _testBox.DrawGizmo();
                break;
            case TestMode.Sector2D:
                _testSector.DrawGizmo();
                break;
            case TestMode.Circle2D:
                _testCircle.DrawGizmo();
                break;
        }

        if (_boundType == BoundType.Circle && _testMode == TestMode.Sector2D)
        {
            Vector2 lineR, lineL;
            ZKit.Math.Geometry.GetTangentOnCircle(_circle.position2D, _circle.radius, _testSector.position2D, out lineR, out lineL);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_testSector.position, new Vector3(lineR.x, _circle.position.y, lineR.y));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_testSector.position, new Vector3(lineL.x, _circle.position.y, lineL.y));
        }
        if (_boundType == BoundType.Sector && _testMode == TestMode.Circle2D)
        {
            Vector2 lineR, lineL;
            ZKit.Math.Geometry.GetTangentOnCircle(_testCircle.position2D, _testCircle.radius, _sector.position2D, out lineR, out lineL);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_sector.position, new Vector3(lineR.x, _testCircle.position.y, lineR.y));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_sector.position, new Vector3(lineL.x, _testCircle.position.y, lineL.y));
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
                _box.rotate_y = EditorGUILayout.Slider("rotate Y angle", _box.rotate_y, 0f, 360f);
                break;
            case BoundType.Cube:
                _cube.position = EditorGUILayout.Vector3Field("position", _cube.position);
                _cube.size = EditorGUILayout.Vector3Field("Scale", _cube.size);
                _cube.rotate.y = EditorGUILayout.Slider("Rotate Y angle", _cube.rotate.y, 0f, 360f);
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
            case BoundType.Sector:
                _sector.position = EditorGUILayout.Vector3Field("position", _sector.position);
                _sector.radius = EditorGUILayout.FloatField("radius", _sector.radius);
                _sector.angle = EditorGUILayout.Slider("Sector Angle", _sector.angle, 0f, 360f);
                _sector.rotate_y = EditorGUILayout.Slider("Rotate Y angle", _sector.rotate_y, 0f, 360f);
                break;
        }
        if (EditorGUI.EndChangeCheck())
        {
            //Calculate();
            SceneView.RepaintAll();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical("box");
        _testMode = (TestMode)EditorGUILayout.EnumPopup("Test Mode", _testMode);
        //if (_testMode != TestMode.None)
        if (_testMode == TestMode.Dot2D || _testMode == TestMode.Line2D || _testMode == TestMode.Ray2D)
        {
            EditorGUILayout.BeginHorizontal();
            _currentClickedPos = EditorGUILayout.Vector3Field("Clicked Pos A", _currentClickedPos);
            GUIStyle gs = new GUIStyle("button");
            if (_clickMode_cur) gs.normal.textColor = Color.green;
            else gs.normal.textColor = Color.red;
            if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                _clickMode_cur = !_clickMode_cur;
            EditorGUILayout.EndHorizontal();
            if (_testMode == TestMode.Line2D || _testMode == TestMode.Ray2D)
            {
                EditorGUILayout.BeginHorizontal();
                _prevClickedPos = EditorGUILayout.Vector3Field("Clicked Pos B", _prevClickedPos);
                if (_clickMode_prev) gs.normal.textColor = Color.green;
                else gs.normal.textColor = Color.red;
                if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                    _clickMode_prev = !_clickMode_prev;
                EditorGUILayout.EndHorizontal();
            }
        }
        else if (_testMode == TestMode.Box2D)
        {
            EditorGUILayout.BeginHorizontal();
            _currentClickedPos = EditorGUILayout.Vector3Field("Box Pos", _currentClickedPos);
            GUIStyle gs = new GUIStyle("button");
            if (_clickMode_cur) gs.normal.textColor = Color.green;
            else gs.normal.textColor = Color.red;
            if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                _clickMode_cur = !_clickMode_cur;
            EditorGUILayout.EndHorizontal();
            _testBox.position = _currentClickedPos;
            _testBox.size = EditorGUILayout.Vector2Field("Box Size", _testBox.size);
            _testBox.rotate_y = EditorGUILayout.Slider("Box Rotate Y angle", _testBox.rotate_y, 0f, 360f);
        }
        else if (_testMode == TestMode.LerpBox2D)
        {
            EditorGUILayout.BeginHorizontal();
            _currentClickedPos = EditorGUILayout.Vector3Field("Clicked Pos A", _currentClickedPos);
            GUIStyle gs = new GUIStyle("button");
            if (_clickMode_cur) gs.normal.textColor = Color.green;
            else gs.normal.textColor = Color.red;
            if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                _clickMode_cur = !_clickMode_cur;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            _prevClickedPos = EditorGUILayout.Vector3Field("Clicked Pos B", _prevClickedPos);
            if (_clickMode_prev) gs.normal.textColor = Color.green;
            else gs.normal.textColor = Color.red;
            if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                _clickMode_prev = !_clickMode_prev;
            EditorGUILayout.EndHorizontal();

            _lerpBoxSize = EditorGUILayout.Vector2Field("Box Size  (X:width  Y:height)", _lerpBoxSize);
            _lerpTweenFactor = EditorGUILayout.Slider("Tween Factor", _lerpTweenFactor, 0f, 1f);
            _lerpElapsedTime = EditorGUILayout.FloatField("Elapsed Time (millisec)", _lerpElapsedTime);

            Vector3 direction = (_prevClickedPos - _currentClickedPos).normalized;

            Vector3 end = _prevClickedPos - (direction * _lerpBoxSize.y * 0.5f);
            Vector3 lerpPosition = Vector3.Lerp(_currentClickedPos + (direction * _lerpBoxSize.y * 0.5f), end, _lerpTweenFactor);

            float totLength = (lerpPosition - end).magnitude;
            float elapsedLength = (_lerpElapsedTime * 0.001f) > totLength ? totLength  : _lerpElapsedTime * 0.001f;
            Vector3 elapsedPosition = lerpPosition + (direction * elapsedLength);
            float lerpLength = elapsedLength + _lerpBoxSize.y;

            _testBox.position = Vector3.Lerp(lerpPosition, elapsedPosition, 0.5f);
            _testBox.size.x = _lerpBoxSize.x;
            _testBox.size.y = _lerpBoxSize.y > lerpLength ? _lerpBoxSize.y : lerpLength;
            _testBox.rotate_y = Mathf.Acos(Vector3.Dot(direction, Vector3.forward)) * Mathf.Rad2Deg * (direction.x > 0f ? 1 : -1);
            EditorGUILayout.LabelField("angle", _testBox.rotate_y.ToString());
        }
        else if (_testMode == TestMode.Sector2D)
        {
            EditorGUILayout.BeginHorizontal();
            _currentClickedPos = EditorGUILayout.Vector3Field("Sector Pos", _currentClickedPos);
            GUIStyle gs = new GUIStyle("button");
            if (_clickMode_cur) gs.normal.textColor = Color.green;
            else gs.normal.textColor = Color.red;
            if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                _clickMode_cur = !_clickMode_cur;
            EditorGUILayout.EndHorizontal();
            _testSector.position = _currentClickedPos;
            _testSector.radius = EditorGUILayout.FloatField("Sector Radius", _testSector.radius);
            _testSector.angle = EditorGUILayout.Slider("Sector Angle", _testSector.angle, 0f, 360f);
            _testSector.rotate_y = EditorGUILayout.Slider("Sector Rotate Y Angle", _testSector.rotate_y, 0f, 360f);
        }
        else if (_testMode == TestMode.Circle2D)
        {
            EditorGUILayout.BeginHorizontal();
            _currentClickedPos = EditorGUILayout.Vector3Field("Circle Pos", _currentClickedPos);
            GUIStyle gs = new GUIStyle("button");
            if (_clickMode_cur) gs.normal.textColor = Color.green;
            else gs.normal.textColor = Color.red;
            if (GUILayout.Button("Edit", gs, GUILayout.Height(32f)))
                _clickMode_cur = !_clickMode_cur;
            EditorGUILayout.EndHorizontal();
            _testCircle.position = _currentClickedPos;
            _testCircle.radius = EditorGUILayout.FloatField("Circle Size", _testCircle.radius);
        }
        EditorGUILayout.EndVertical();

        bool isIn = false;
        switch (_boundType)
        {
            case BoundType.Box:
                switch (_testMode)
                {
                    case TestMode.Dot2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DDot(_box, new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DLine(_box, new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray2D:
                        Vector2 dir = new Vector2(_prevClickedPos.x, _prevClickedPos.z) - new Vector2(_currentClickedPos.x, _currentClickedPos.z);
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DRay(_box, new Vector2(_currentClickedPos.x, _currentClickedPos.z), dir)) ? "Yes" : "No");
                        break;
                    case TestMode.Box2D:
                    case TestMode.LerpBox2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DBox(_box, _testBox)) ? "Yes" : "No");
                        break;
                    case TestMode.Circle2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DCircle(_box, _testCircle)) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Cube:
                switch (_testMode)
                {
                    case TestMode.Dot2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DDot(_cube.Get2Dbox(), new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DLine(_cube.Get2Dbox(), new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray2D:
                        Vector2 dir = new Vector2(_prevClickedPos.x, _prevClickedPos.z) - new Vector2(_currentClickedPos.x, _currentClickedPos.z);
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DRay(_cube.Get2Dbox(), new Vector2(_currentClickedPos.x, _currentClickedPos.z), dir)) ? "Yes" : "No");
                        break;
                    case TestMode.Box2D:
                    case TestMode.LerpBox2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DBox(_cube.Get2Dbox(), _testBox)) ? "Yes" : "No");
                        break;
                    case TestMode.Circle2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DCircle(_cube.Get2Dbox(), _testCircle)) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Circle:
                switch (_testMode)
                {
                    case TestMode.Dot2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DDot(_circle, new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DLine(_circle, new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray2D:
                        Vector2 dir = new Vector2(_prevClickedPos.x, _prevClickedPos.z) - new Vector2(_currentClickedPos.x, _currentClickedPos.z);
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DRay(_circle, new Vector2(_currentClickedPos.x, _currentClickedPos.z), dir)) ? "Yes" : "No");
                        break;
                    case TestMode.Box2D:
                    case TestMode.LerpBox2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DBox(_circle, _testBox)) ? "Yes" : "No");
                        break;
                    case TestMode.Circle2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DCircle(_circle, _testCircle)) ? "Yes" : "No");
                        break;
                    case TestMode.Sector2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DSector(_circle, _testSector)) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Capsule:
                switch (_testMode)
                {
                    case TestMode.Dot2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DDot(_capsule.GetCircle(), new Vector2(_currentClickedPos.x, _currentClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Line2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DLine(_capsule.GetCircle(), new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z))) ? "Yes" : "No");
                        break;
                    case TestMode.Ray2D:
                        Vector2 dir = new Vector2(_prevClickedPos.x, _prevClickedPos.z) - new Vector2(_currentClickedPos.x, _currentClickedPos.z);
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DRay(_capsule.GetCircle(), new Vector2(_currentClickedPos.x, _currentClickedPos.z), dir)) ? "Yes" : "No");
                        break;
                    case TestMode.Box2D:
                    case TestMode.LerpBox2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DBox(_capsule.GetCircle(), _testBox)) ? "Yes" : "No");
                        break;
                    case TestMode.Circle2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DCircle(_capsule.GetCircle(), _testCircle)) ? "Yes" : "No");
                        break;
                    case TestMode.Sector2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DSector(_capsule.GetCircle(), _testSector)) ? "Yes" : "No");
                        break;
                }
                break;
            case BoundType.Sector:
                switch (_testMode)
                {
                    case TestMode.Dot2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DSector(new Vector2(_currentClickedPos.x, _currentClickedPos.z), _sector)) ? "Yes" : "No");
                        break;
                    case TestMode.Line2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DSector(new Vector2(_currentClickedPos.x, _currentClickedPos.z), new Vector2(_prevClickedPos.x, _prevClickedPos.z), _sector)) ? "Yes" : "No");
                        break;
                    case TestMode.Box2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DSector(_testBox, _sector)) ? "Yes" : "No");
                        break;
                    case TestMode.Circle2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DSector(_testCircle, _sector)) ? "Yes" : "No");
                        break;
                    case TestMode.Sector2D:
                        EditorGUILayout.LabelField("In ?", (isIn = ZKit.Math.Geometry.CollisionDetect2DSector(_testSector, _sector)) ? "Yes" : "No");
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
