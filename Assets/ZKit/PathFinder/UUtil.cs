using UnityEngine;
using System.Collections.Generic;

namespace ZKit.PathFinder
{
    public static class UUtil
    {
        public static Rect ScanMapSize(int pathLayerMask, int obstacleLayerMask)
        {
            TerrainCollider[] terrainColliders = (TerrainCollider[])GameObject.FindObjectsOfType(typeof(TerrainCollider));

            Rect result = new Rect(0f, 0f, 0f, 0f);
            Vector2 max = new Vector2(), min = new Vector2();

            if (terrainColliders.Length > 0)
            {
                max = terrainColliders[0].bounds.max;
                min = terrainColliders[0].bounds.min;

                #region 상하좌우 크기를 검색
                foreach(var collider in terrainColliders)
                {
                    if (result.xMax < collider.bounds.max.x) result.xMax = collider.bounds.max.x;
                    if (result.xMin > collider.bounds.min.x) result.xMin = collider.bounds.min.x;
                    if (result.yMax < collider.bounds.max.z) result.yMax = collider.bounds.max.z;
                    if (result.yMin > collider.bounds.min.z) result.yMin = collider.bounds.min.z;
                }
                #endregion
            }

            var objects = GameObject.FindObjectsOfType<GameObject>();
            if (objects.Length > 0 && terrainColliders.Length <= 0) max = min = objects[0].transform.position;
            int layerMask = (1 << pathLayerMask << obstacleLayerMask);
            foreach (GameObject go in objects)
            {
                if ((layerMask & (1 << go.layer)) == 0) continue;

                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (!mf) continue;
                if (!mf.sharedMesh) continue;

                foreach (var vertex in mf.sharedMesh.vertices)
                {
                    Vector3 v = vertex;
                    v.Scale(go.transform.localScale);
                    v = go.transform.rotation * v;
                    v += go.transform.position;

                    if (result.xMax < v.x) result.xMax = v.x;
                    if (result.xMin > v.x) result.xMin = v.x;
                    if (result.yMax < v.z) result.yMax = v.z;
                    if (result.yMin > v.z) result.yMin = v.z;
                }
            }

            return result;
        }

        public static Bounds ScanMapSize3D(int pathLayerMask, int obstacleLayerMask, float gap = 0f)
        {
            TerrainCollider[] terrainColliders = (TerrainCollider[])GameObject.FindObjectsOfType(typeof(TerrainCollider));

            Bounds result = new Bounds();
            Vector3 max = new Vector3(), min = new Vector3();

            if (/*terrainColliders != null && */terrainColliders.Length > 0)
            {
                max = terrainColliders[0].bounds.max;
                min = terrainColliders[0].bounds.min;

                #region 상하좌우 크기를 검색
                foreach(TerrainCollider collider in terrainColliders)
                {
                    if (max.x < collider.bounds.max.x) max.x = collider.bounds.max.x;
                    if (min.x > collider.bounds.min.x) min.x = collider.bounds.min.x;
                    if (max.y < collider.bounds.max.y) max.y = collider.bounds.max.y;
                    if (min.y > collider.bounds.min.y) min.y = collider.bounds.min.y;
                    if (max.z < collider.bounds.max.z) max.z = collider.bounds.max.z;
                    if (min.z > collider.bounds.min.z) min.z = collider.bounds.min.z;
                }
                result.SetMinMax(min, max);
                #endregion
            }

            var objects = GameObject.FindObjectsOfType<GameObject>();
            bool first = true;
            int layer = pathLayerMask | obstacleLayerMask;
            foreach (GameObject go in objects)
            {
                if ((layer & (1 << go.layer)) == 0) continue;

                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (!mf) continue;
                if (!mf.sharedMesh) continue;

                if(first)
                {
                    max = min = go.transform.position;
                    first = false;
                }

                foreach (var vertex in mf.sharedMesh.vertices)
                {
                    Vector3 v = vertex;
                    v.Scale(go.transform.localScale);
                    v = go.transform.rotation * v;
                    v += go.transform.position;

                    if (max.x < v.x) max.x = v.x;
                    if (min.x > v.x) min.x = v.x;

                    if (max.y < v.y) max.y = v.y;
                    if (min.y > v.y) min.y = v.y;

                    if (max.z < v.z) max.z = v.z;
                    if (min.z > v.z) min.z = v.z;
                }
            }
            min -= Vector3.one*gap;
            max += Vector3.one*gap;
            result.SetMinMax(min, max);
            return result;
        }

        public static void GetVoxel()
        {

        }
    }
}