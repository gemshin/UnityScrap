using UnityEngine;
using System.Collections.Generic;
using ZKit.Math.Geometry;

namespace ZKit.PathFinder
{
    /// <summary>
    /// 복셀구조를 뽑아준다.
    /// </summary>
    public class Voxelizer
    {
        private VoxelArea _area;

        private List<SimpleMesh> _inObjects = new List<SimpleMesh>();
        private List<SimpleMesh> _exceptObjects = new List<SimpleMesh>();

        private int _pathLayerMask = 0;
        private int _exceptLayerMask = 0;

        public VoxelArea VoxelArea { get { return _area; } }

        public void InitVoxelArea(float cellSize, float cellHeight, int pathLayerMask, int exceptLayerMask)
        {
            _pathLayerMask = pathLayerMask;
            _exceptLayerMask = exceptLayerMask;
            _area = new VoxelArea(UUtil.ScanMapSize3D(_pathLayerMask, cellSize), cellSize, cellHeight);
            _inObjects.Clear();
            _exceptObjects.Clear();
        }

        public void ScanVoxelSpace()
        {
            CollectObjects();
            Voxelize();
        }

        private void CollectObjects()
        {
            foreach (MeshFilter filter in GameObject.FindObjectsOfType<MeshFilter>())
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (filter.sharedMesh == null || !renderer.enabled) continue;
                if (((_pathLayerMask | _exceptLayerMask) & (1 << filter.gameObject.layer)) == 0) continue;

                if (_area.AreaBound.Intersects(renderer.bounds))
                {
                    Mesh mesh = filter.sharedMesh;
                    SimpleMesh smesh = new SimpleMesh();
                    smesh._matrix = renderer.localToWorldMatrix;

                    // 중복 버티스 제거 코드인데 더 느리다. 삭제준비중.
                    //var cacheVertices = new List<Vector3>();
                    //var cacheTris = new List<int>(mesh.triangles);
                    //foreach(var vertice in mesh.vertices)
                    //    if (!cacheVertices.Contains(vertice))
                    //        cacheVertices.Add(vertice);
                    //for (int i = 0; i < cacheTris.Count; ++i)
                    //{
                    //    cacheTris[i] = cacheVertices.FindIndex(x => x == mesh.vertices[cacheTris[i]]);
                    //}
                    //smesh._vertices = cacheVertices.ToArray();
                    //smesh._triangles = cacheTris.ToArray();

                    smesh._vertices = mesh.vertices;
                    smesh._triangles = mesh.triangles;

                    smesh._bounds = renderer.bounds;

                    if ((_exceptLayerMask & (1 << filter.gameObject.layer)) != 0)
                        _exceptObjects.Add(smesh);
                    else
                        _inObjects.Add(smesh);
                }
            }
        }
        private void Voxelize()
        {
            //foreach(var obj in _exceptObjects)
            //{
            //    RegistExceptVoxel(obj);
            //}
            foreach (var obj in _inObjects)
            {
                VoxelizeObject(obj);
            }

            //FilterFirstLedge();

            //FilterLowCeilingPlace();
            //FilterLedge();

            //BuildConnection();
            //BuildDistanceField();
        }

        //private void FilterLedge()
        //{
        //    BuildConnection();
        //    var va = VoxelArea;
        //    List<uint> deletingReservations = new List<uint>();
        //    foreach (var ele in va.WalkableCells)
        //    {
        //        if (!ele.ConnectionBackward || !ele.ConnectionForward || !ele.ConnectionLeft || !ele.ConnectionRight)
        //            deletingReservations.Add(ele.Index);
        //    }
        //    foreach (var ele in deletingReservations)
        //    {
        //        va.RemoveWalkableCell(ele);
        //        var ledge = va.GetCell(ele);
        //        if (ledge != null)
        //        {
        //            va.AddLedge(ledge);
        //            ledge.CellType = VOXEL_NAVI_TYPE.Ledge;
        //        }
        //    }
        //}

        //private void BuildDistanceField()
        //{
        //    var va = VoxelArea;
        //    var wc = VoxelArea.WalkableCells;
        //    foreach(var ele in wc)
        //    {

        //    }
        //}

        /// <summary>
        /// 일단 정방(AABB)만 처리한다.
        /// </summary>
        /// <param name="mesh"></param>
        private void RegistExceptVoxel(SimpleMesh mesh)
        {
            // 쎌 처리 되어 있는데 성능상 바운드 체크만 하는걸로 변경하자.
            // 모델 안쪽도 다 넣어줘야 한다.
            // ex 모델의 경우 높이에서 바운드 밖으로 나갈수 있으니 수정요망.
            VoxelArea va = VoxelArea;

            for (int i = 0; i < mesh._triangles.Length; i += 3)
            {
                uint min_x, min_y, min_z, max_x, max_y, max_z;

                Vector3 tri1 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i]]);
                Vector3 tri2 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i + 1]]);
                Vector3 tri3 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i + 2]]);

                Vector3 minPos = Vector3.Min(Vector3.Min(tri1, tri2), tri3);
                Vector3 maxPos = Vector3.Max(Vector3.Max(tri1, tri2), tri3);

                if (!va.GetCellCount(minPos, out min_x, out min_y, out min_z))
                {
                    Debug.Log("bound min err");
                    return;
                }
                if (!va.GetCellCount(maxPos, out max_x, out max_y, out max_z))
                {
                    Debug.Log("bound max err");
                    return;
                }

                for (uint z = min_z; z <= max_z; ++z)
                {
                    for (uint y = min_y; y <= max_y; ++y)
                    {
                        for (uint x = min_x; x <= max_x; ++x)
                        {
                            // 일단 정방만 처리.
                            uint index;
                            if (!va.GetCellIndex(out index, x, y, z)) continue;

                            var vox = new Voxel(index);
                            vox._ex = true;
                            va.Voxels[index] = vox;
                        }
                    }
                }
            }
        }

        private void BuildSpan(uint index)
        {
            var va = _area;

            if (!va.Voxels.ContainsKey(index)) return;

            var upper = va.GetCell(VoxelArea.DIRECTION.Upper, index);
            var lower = va.GetCell(VoxelArea.DIRECTION.Lower, index);

            if (upper != null) // let's span.
            {
                va.Voxels[index].IsSpan = true;
                va.Voxels[index].SpanTop = upper.SpanTop;
                upper.SpanNext = va.Voxels[index].Index;
            }

            if (lower != null)
            {
                va.Voxels[index].IsSpan = true;
                va.Voxels[index].SpanNext = lower.Index;
                for (Voxel vox = lower; !vox.IsBottom; vox = va.GetCell(vox.SpanNext))
                {
                    vox.SpanTop = va.Voxels[index].SpanTop;
                }
            }
        }

        private void VoxelizeObject(SimpleMesh mesh)
        {
            VoxelArea va = VoxelArea;

            for (int i = 0; i < mesh._triangles.Length; i += 3)
            {
                uint min_x, min_y, min_z, max_x, max_y, max_z;

                Vector3 tri1 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i]]);
                Vector3 tri2 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i + 1]]);
                Vector3 tri3 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i + 2]]);

                Vector3 minPos = Vector3.Min(Vector3.Min(tri1, tri2), tri3);
                Vector3 maxPos = Vector3.Max(Vector3.Max(tri1, tri2), tri3);

                if (!va.GetCellCount(minPos, out min_x, out min_y, out min_z))
                {
                    Debug.Log("bound min err");
                    return;
                }
                if (!va.GetCellCount(maxPos, out max_x, out max_y, out max_z))
                {
                    Debug.Log("bound max err");
                    return;
                }

                for (uint z = (uint)min_z; z <= max_z; ++z)
                {
                    for (uint y = (uint)min_y; y <= max_y; ++y)
                    {
                        for (uint x = (uint)min_x; x <= max_x; ++x)
                        {
                            uint index;
                            if (!va.GetCellIndex(out index, x, y, z)) continue;

                            // 이미 검출되었고, 위를 향한 면이라서 더 검출을 할 필요가 없다.
                            if (va.Voxels.ContainsKey(index))
                            {
                                BuildSpan(index);
                                continue;
                            }

                            Vector3 position;
                            if (!va.GetCellPosition(x, y, z, out position)) continue;

                            AABox box = new AABox(position, new Vector3(va.CellSize, va.CellHeight, va.CellSize));
                            Triangle triangle = new Triangle(tri1, tri2, tri3);
                            bool reverseSide = false;
                            if (Collision3D.CollisionDetectTriangle(triangle, box, out reverseSide, Vector3.up))
                            //if(Collision3D.triBoxOverlap(position, new Vector3(va.CellSize, va.CellHeight, va.CellSize)*0.5f, triangle)) // 이게 더 빠르다.. 근데 아직 미완성.
                            {
                                va.Voxels[index] = new Voxel(index);
                                if (reverseSide) va.Voxels[index].FaceReverse = true;
                                BuildSpan(index);
                            }
                        }
                    }
                }
            }
        }
    }
}