using UnityEngine;
using UnityEditor;
using System.Collections;

using ZKit;

public class BoundCheckWindow : EditorWindow
{
    private static bool _testMode = false;
    private static Vector3 _clickedPos;

    #region squar
    private const float _halfSquarX = 0.5f;
    private const float _halfSquarZ = 0.5f;
    private readonly Vector3[] _squar = new Vector3[4];
    #endregion

    #region box
    private static Box _boxx = new Box();

    private static Vector3[] _box = new Vector3[4];
    private static Vector2 _boxPos = new Vector3(3, 5);
    private static Vector2 _boxScale = new Vector3(2, 1);
    #endregion

    #region capsule
    private static Vector3 _capsuleLocalCenter = new Vector3();
    private static float _capsuleRadius = 0.5f;
    private static float _capsuleHeight = 2f; 
    #endregion

    //private static Matrix4x4 _mat4Custom;
    //private static Matrix4x4 _mat4TRS;
    private static Matrix4x4 _matbox;

    private static float _ang = 0f;

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

        _squar[0] = new Vector3(-_halfSquarX,0f,_halfSquarZ);
        _squar[1] = new Vector3(_halfSquarX,0f,_halfSquarZ);
        _squar[2] = new Vector3(-_halfSquarX,0f,-_halfSquarZ);
        _squar[3] = new Vector3(_halfSquarX,0f,-_halfSquarZ);

        _boxx.Rotate = new Vector3(0f, 90f, 0f);

        _testMode = true;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;

        _testMode = false;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (_testMode) HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

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
                    _clickedPos = hit.point;
                    Debug.Log(hit.point);
                }
                this.Repaint();
            }
            #endregion

            #region Draw Click point
            Handles.color = Color.red;
            Handles.DotCap(0, _clickedPos, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(_clickedPos)); 
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
        //Gizmos.matrix = _mat4Custom;
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        //Gizmos.matrix = _mat4TRS;
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_box[0], _box[1]);
        Gizmos.DrawLine(_box[1], _box[3]);
        Gizmos.DrawLine(_box[3], _box[2]);
        Gizmos.DrawLine(_box[2], _box[0]);

        float ang = _ang * Mathf.Deg2Rad * -1f;
        _boxx.Rotate = new Vector3(0f, _ang, 0f);
        _boxx.DrawGizmo(true);
        DebugExtension.DrawCapsule(_capsuleLocalCenter, Color.cyan, _capsuleHeight, _capsuleRadius);
    }

    private bool IsInZone(Vector3 pos, Vector3 boxPos, Vector3 boxScale, float boxRotation)
    {
        return false;
    }

    void Update()
    {
    }

    private bool Check()
    {
        Vector2 pp = new Vector2(_clickedPos.x - _boxPos.x, _clickedPos.z - _boxPos.y);

        float radius = Mathf.Sqrt((_boxScale.x*_boxScale.x) + (_boxScale.y*_boxScale.y)) * 0.5f;

        if (pp.magnitude <= radius)
        {
            Matrix4x4 mat = new Matrix4x4();

            mat.SetRow(0, Vector4.zero);
            mat.SetRow(1, Vector4.zero);
            mat.SetRow(2, Vector4.zero);
            mat.SetRow(3, Vector4.zero);
            mat.m00 = 1f; mat.m11 = 1f; mat.m22 = 1f; mat.m33 = 1f;

            float ang = _ang * Mathf.Deg2Rad * -1f;
            mat.m00 = Mathf.Cos(ang); mat.m02 = -Mathf.Sin(ang);
            mat.m20 = Mathf.Sin(ang); mat.m22 = Mathf.Cos(ang);

            float xx = (pp.x * mat.m00) + (pp.y * mat.m02);
            //float yy = (pp.x * mat.m10) + (pp.y * mat.m12);
            float zz = (pp.x * mat.m20) + (pp.y * mat.m22);

            pp = new Vector3(xx, 0f, zz);

            if( (_boxScale.x * 0.5f) >= xx && (_boxScale.x * -0.5f) <= xx
                && (_boxScale.y * 0.5f) >= zz && (_boxScale.y * -0.5f) <= zz)
                return true;
        }

        return false;
    }

    private void Calculate()
    {
        //Vector3 pos = new Vector3(_boxPos.x, 1f, _boxPos.y);
        Vector3 scale = new Vector3(_boxScale.x, 0f, _boxScale.y);
        float ang = _ang * Mathf.Deg2Rad;

        #region TRS
        //_mat4TRS = Matrix4x4.TRS(pos + Vector3.back * 4, Quaternion.AngleAxis(_ang, Vector3.down), scale); 
        #endregion

        #region Custom
        //_mat4Custom.SetRow(0, Vector4.zero);
        //_mat4Custom.SetRow(1, Vector4.zero);
        //_mat4Custom.SetRow(2, Vector4.zero);
        //_mat4Custom.SetRow(3, Vector4.zero);
        //_mat4Custom.m00 = 1; _mat4Custom.m11 = 1; _mat4Custom.m22 = 1; _mat4Custom.m33 = 1f;
        //_mat4Custom.SetColumn(3, pos + Vector3.back * 2); // transpose

        //_mat4Custom.m00 = Mathf.Cos(ang); _mat4Custom.m02 = -Mathf.Sin(ang);
        //_mat4Custom.m20 = Mathf.Sin(ang); _mat4Custom.m22 = Mathf.Cos(ang);
        //_mat4Custom.m00 *= scale.x; _mat4Custom.m01 *= scale.y; _mat4Custom.m02 *= scale.z;
        //_mat4Custom.m10 *= scale.x; _mat4Custom.m11 *= scale.y; _mat4Custom.m12 *= scale.z;
        //_mat4Custom.m20 *= scale.x; _mat4Custom.m21 *= scale.y; _mat4Custom.m22 *= scale.z; 
        #endregion

        _matbox.SetRow(0, Vector4.zero);
        _matbox.SetRow(1, Vector4.zero);
        _matbox.SetRow(2, Vector4.zero);
        _matbox.SetRow(3, Vector4.zero);
        _matbox.m00 = 1; _matbox.m11 = 1; _matbox.m22 = 1; _matbox.m33 = 1f;
        _matbox.m00 = Mathf.Cos(ang); _matbox.m02 = -Mathf.Sin(ang);
        _matbox.m20 = Mathf.Sin(ang); _matbox.m22 = Mathf.Cos(ang);
        _matbox.m00 *= scale.x; _matbox.m01 *= scale.y; _matbox.m02 *= scale.z;
        _matbox.m10 *= scale.x; _matbox.m11 *= scale.y; _matbox.m12 *= scale.z;
        _matbox.m20 *= scale.x; _matbox.m21 *= scale.y; _matbox.m22 *= scale.z;

        for (int i = 0; i < 4; ++i)
        {
            float xx = (_squar[i].x * _matbox.m00) + (_squar[i].y * _matbox.m01) + (_squar[i].z * _matbox.m02);
            //float yy = (_squar[i].x * _matbox.m10) + (_squar[i].y * _matbox.m11) + (_squar[i].z * _matbox.m12);
            float zz = (_squar[i].x * _matbox.m20) + (_squar[i].y * _matbox.m21) + (_squar[i].z * _matbox.m22);
            _box[i] = new Vector3(xx + _boxPos.x, 1f, zz + _boxPos.y);
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();
        _ang = EditorGUILayout.Slider("Rotate angle", _ang, 0f, 360f);

        _boxPos = EditorGUILayout.Vector2Field("Box Pos", _boxPos);
        _boxScale = EditorGUILayout.Vector2Field("Box Scale", _boxScale);
        if (EditorGUI.EndChangeCheck())
        {
            Calculate();
            SceneView.RepaintAll();
        }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Click Pos", _clickedPos.ToString());
        EditorGUILayout.LabelField("In?", Check() == true ? "Yes" : "No");

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("2D matrix 0", string.Format("{0}", _matbox.GetRow(0)));
        EditorGUILayout.LabelField("2D matrix 1", string.Format("{0}", _matbox.GetRow(1)));
        EditorGUILayout.LabelField("2D matrix 2", string.Format("{0}", _matbox.GetRow(2)));

        EditorGUILayout.Separator();

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
