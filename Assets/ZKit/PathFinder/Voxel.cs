﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZKit.PathFinder
{
    public struct SimpleMesh
    {
        //public MeshFilter original;

        public int _area;
        public Vector3[] _vertices;
        public int[] _triangles;

        /** World bounds of the mesh. Assumed to already be multiplied with the matrix */
        public Bounds _bounds;

        public Matrix4x4 _matrix;

        public SimpleMesh(Vector3[] vertices, int[] triangles, Bounds bounds)
        {
            _matrix = Matrix4x4.identity;
            _vertices = vertices;
            _triangles = triangles;
            _bounds = bounds;
            //original = null;
            _area = 0;
        }

        public SimpleMesh(Vector3[] vertices, int[] triangles, Bounds bounds, Matrix4x4 matrix)
        {
            this._matrix = matrix;
            _vertices = vertices;
            _triangles = triangles;
            _bounds = bounds;
            //original = null;
            _area = 0;
        }

        /** Recalculate the bounds based on vertices and matrix */
        public void RecalculateBounds()
        {
            Bounds b = new Bounds(_matrix.MultiplyPoint3x4(_vertices[0]), Vector3.zero);

            for (int i = 1; i < _vertices.Length; i++)
            {
                b.Encapsulate(_matrix.MultiplyPoint3x4(_vertices[i]));
            }
            //Assigned here to avoid changing bounds if vertices would happen to be null
            _bounds = b;
        }
    }

    public class VoxelSpace : NonPubSingleton<VoxelSpace>
    {
        private bool _setDone = false;
        private Bounds _space;
        private float _cellSize;
        private float _cellHeight;
        private VoxelSpan[] _voxelSpawn;

        private List<SimpleMesh> _inObjects;

        private int _pathLayerMask = 0;
        private int _obstacleLayerMask = 0;

        public void CollectObjects()
        {
            if (_setDone) return;
            int layerMask = (1 << _pathLayerMask << _obstacleLayerMask);

            //var cachedVertices = new Dictionary<Mesh, Vector3[]>();
            //var cachedTris = new Dictionary<Mesh, int[]>();

            foreach (MeshFilter filter in GameObject.FindObjectsOfType<MeshFilter>())
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (filter.sharedMesh == null || !renderer.enabled) continue;
                if ((layerMask & (1 << filter.gameObject.layer)) == 0) continue;
                
                if( _space.Intersects(renderer.bounds) )
                {
                    Mesh mesh = filter.sharedMesh;
                    SimpleMesh smesh = new SimpleMesh();

                    smesh._matrix = renderer.localToWorldMatrix;

                    //if (cachedVertices.ContainsKey(mesh))
                    //{
                    //    smesh._vertices = cachedVertices[mesh];
                    //    smesh._triangles = cachedTris[mesh];
                    //}
                    //else
                    {
                        smesh._vertices = mesh.vertices;
                        smesh._triangles = mesh.triangles;
                        //cachedVertices[mesh] = smesh._vertices;
                        //cachedTris[mesh] = smesh._triangles;
                    }
                    smesh._bounds = renderer.bounds;

                    _inObjects.Add(smesh);
                }
            }
            
        }
    }

    public class VoxelSpan
    {
        private uint _spawn;
        //private VoxelCell _voxelCell;
        private VoxelCell[] _voxelCell;
    }

    public class VoxelCell
    {
        private uint index;
    }
}