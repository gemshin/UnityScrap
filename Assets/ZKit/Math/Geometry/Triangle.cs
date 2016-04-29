using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Triangle
{
    private Vector3[] _vertices;
    private Vector3 _normal;

    public Vector3 this [int i]
    {
        get { if (i >= 3) return new Vector3(); return _vertices[i]; }
        set { if (i >= 3) return; _vertices[i] = value; Recalculate(); }
    }

    public Vector3 Normal { get { return _normal; } }

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        // abc 순으로 그림.
        _vertices = new Vector3[3]{ a, b, c };
        Recalculate();
    }

    private void Recalculate()
    {
        _normal = Vector3.Cross(_vertices[1] - _vertices[0], _vertices[2] - _vertices[0]).normalized;
    }

    public IEnumerable<Vector3> Vertices
    {
        get
        {
            foreach (Vector3 point in _vertices)
            {
                yield return point;
            }
        }
    }

    public void DrawGizmo()
    {
        Gizmos.DrawLine(_vertices[0], _vertices[1]);
        Gizmos.DrawLine(_vertices[1], _vertices[2]);
        Gizmos.DrawLine(_vertices[2], _vertices[0]);
        Gizmos.DrawLine(_vertices[0], _vertices[0] + _normal);
    }
}
