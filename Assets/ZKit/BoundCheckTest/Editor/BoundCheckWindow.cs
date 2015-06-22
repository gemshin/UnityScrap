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

    //private static Matrix4x4 _mat4Custom;
    //private static Matrix4x4 _mat4TRS;
    private static Matrix4x4 _matbox;

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
            Handles.color = Color.red;
            Handles.DotCap(0, _currentClickedPos, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(_currentClickedPos));
            if(_clickMode_cur)
                _currentClickedPos = Handles.PositionHandle(_currentClickedPos, Quaternion.identity);

            if (_testMode == TestMode.Line || _testMode == TestMode.Ray)
            {
                Handles.color = new Color(0.8f, 0f, 0f);
                Handles.DotCap(0, _prevClickedPos, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(_prevClickedPos));
                if(_clickMode_prev)
                    _prevClickedPos = Handles.PositionHandle(_prevClickedPos, Quaternion.identity);
            }
            #endregion

            //Handles.color = Color.blue;
            //Handles.matrix = _mat4Custom;
            //Handles.CubeCap(0, Vector3.zero, Quaternion.identity, 1f);

            //Handles.color = Color.green;
            //Handles.matrix = _mat4TRS;
            //Handles.CubeCap(0, Vector3.zero, Quaternion.identity, 1f);

            //float radius = Mathf.Sqrt((_boxScale.x * _boxScale.x) + (_boxScale.z * _boxScale.z)) * 0.5f;
            //Handles.CircleCap(0, _boxPos, Quaternion.AngleAxis(90f, Vector3.right), radius);
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

    //private bool Check()
    //{
    //    Vector2 pp = new Vector2(_clickedPos.x - _boxPos.x, _clickedPos.z - _boxPos.y);

    //    float radius = Mathf.Sqrt((_boxScale.x*_boxScale.x) + (_boxScale.y*_boxScale.y)) * 0.5f;

    //    if (pp.magnitude <= radius)
    //    {
    //        Matrix4x4 mat = new Matrix4x4();

    //        mat.SetRow(0, Vector4.zero);
    //        mat.SetRow(1, Vector4.zero);
    //        mat.SetRow(2, Vector4.zero);
    //        mat.SetRow(3, Vector4.zero);
    //        mat.m00 = 1f; mat.m11 = 1f; mat.m22 = 1f; mat.m33 = 1f;

    //        float ang = _ang * Mathf.Deg2Rad * -1f;
    //        mat.m00 = Mathf.Cos(ang); mat.m02 = -Mathf.Sin(ang);
    //        mat.m20 = Mathf.Sin(ang); mat.m22 = Mathf.Cos(ang);

    //        float xx = (pp.x * mat.m00) + (pp.y * mat.m02);
    //        //float yy = (pp.x * mat.m10) + (pp.y * mat.m12);
    //        float zz = (pp.x * mat.m20) + (pp.y * mat.m22);

    //        pp = new Vector3(xx, 0f, zz);

    //        if( (_boxScale.x * 0.5f) >= xx && (_boxScale.x * -0.5f) <= xx
    //            && (_boxScale.y * 0.5f) >= zz && (_boxScale.y * -0.5f) <= zz)
    //            return true;
    //    }

    //    return false;
    //}

    //private void Calculate()
    //{
    //    //Vector3 pos = new Vector3(_boxPos.x, 1f, _boxPos.y);
    //    Vector3 scale = new Vector3(_boxScale.x, 0f, _boxScale.y);
    //    float ang = _ang * Mathf.Deg2Rad;

    //    #region TRS
    //    //_mat4TRS = Matrix4x4.TRS(pos + Vector3.back * 4, Quaternion.AngleAxis(_ang, Vector3.down), scale); 
    //    #endregion

    //    #region Custom
    //    //_mat4Custom.SetRow(0, Vector4.zero);
    //    //_mat4Custom.SetRow(1, Vector4.zero);
    //    //_mat4Custom.SetRow(2, Vector4.zero);
    //    //_mat4Custom.SetRow(3, Vector4.zero);
    //    //_mat4Custom.m00 = 1; _mat4Custom.m11 = 1; _mat4Custom.m22 = 1; _mat4Custom.m33 = 1f;
    //    //_mat4Custom.SetColumn(3, pos + Vector3.back * 2); // transpose

    //    //_mat4Custom.m00 = Mathf.Cos(ang); _mat4Custom.m02 = -Mathf.Sin(ang);
    //    //_mat4Custom.m20 = Mathf.Sin(ang); _mat4Custom.m22 = Mathf.Cos(ang);
    //    //_mat4Custom.m00 *= scale.x; _mat4Custom.m01 *= scale.y; _mat4Custom.m02 *= scale.z;
    //    //_mat4Custom.m10 *= scale.x; _mat4Custom.m11 *= scale.y; _mat4Custom.m12 *= scale.z;
    //    //_mat4Custom.m20 *= scale.x; _mat4Custom.m21 *= scale.y; _mat4Custom.m22 *= scale.z; 
    //    #endregion

    //    _matbox.SetRow(0, Vector4.zero);
    //    _matbox.SetRow(1, Vector4.zero);
    //    _matbox.SetRow(2, Vector4.zero);
    //    _matbox.SetRow(3, Vector4.zero);
    //    _matbox.m00 = 1; _matbox.m11 = 1; _matbox.m22 = 1; _matbox.m33 = 1f;
    //    _matbox.m00 = Mathf.Cos(ang); _matbox.m02 = -Mathf.Sin(ang);
    //    _matbox.m20 = Mathf.Sin(ang); _matbox.m22 = Mathf.Cos(ang);
    //    _matbox.m00 *= scale.x; _matbox.m01 *= scale.y; _matbox.m02 *= scale.z;
    //    _matbox.m10 *= scale.x; _matbox.m11 *= scale.y; _matbox.m12 *= scale.z;
    //    _matbox.m20 *= scale.x; _matbox.m21 *= scale.y; _matbox.m22 *= scale.z;

    //    for (int i = 0; i < 4; ++i)
    //    {
    //        float xx = (_squar[i].x * _matbox.m00) + (_squar[i].y * _matbox.m01) + (_squar[i].z * _matbox.m02);
    //        //float yy = (_squar[i].x * _matbox.m10) + (_squar[i].y * _matbox.m11) + (_squar[i].z * _matbox.m12);
    //        float zz = (_squar[i].x * _matbox.m20) + (_squar[i].y * _matbox.m21) + (_squar[i].z * _matbox.m22);
    //        _box[i] = new Vector3(xx + _boxPos.x, 1f, zz + _boxPos.y);
    //    }
    //}

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
                EditorGUILayout.LabelField("In?", _box.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                break;
            case BoundType.Cube:
                EditorGUILayout.LabelField("In?", _cube.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                break;
            case BoundType.Circle:
                EditorGUILayout.LabelField("In?", _circle.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                break;
            case BoundType.Capsule:
                EditorGUILayout.LabelField("In?", _capsule.Check2DDot(new Vector2(_currentClickedPos.x, _currentClickedPos.z)) ? "Yes" : "No");
                break;
        }

        EditorGUILayout.Separator();

        //EditorGUILayout.LabelField("2D matrix 0", string.Format("{0}", _matbox.GetRow(0)));
        //EditorGUILayout.LabelField("2D matrix 1", string.Format("{0}", _matbox.GetRow(1)));
        //EditorGUILayout.LabelField("2D matrix 2", string.Format("{0}", _matbox.GetRow(2)));

        //EditorGUILayout.Separator();

        //EditorGUILayout.LabelField("custom matrix 0", string.Format("{0}", _mat4Custom.GetRow(0)));
        //EditorGUILayout.LabelField("custom matrix 1", string.Format("{0}", _mat4Custom.GetRow(1)));
        //EditorGUILayout.LabelField("custom matrix 2", string.Format("{0}", _mat4Custom.GetRow(2)));
        //EditorGUILayout.LabelField("custom matrix 3", string.Format("{0}", _mat4Custom.GetRow(3)));

        EditorGUILayout.Separator();

        //EditorGUILayout.LabelField("TRS matrix 0", string.Format("{0}", _mat4TRS.GetRow(0)));
        //EditorGUILayout.LabelField("TRS matrix 1", string.Format("{0}", _mat4TRS.GetRow(1)));
        //EditorGUILayout.LabelField("TRS matrix 2", string.Format("{0}", _mat4TRS.GetRow(2)));
        //EditorGUILayout.LabelField("TRS matrix 3", string.Format("{0}", _mat4TRS.GetRow(3)));

        EditorGUILayout.EndVertical();
    }

}
