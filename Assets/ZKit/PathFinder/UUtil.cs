using UnityEngine;
using System.Collections.Generic;

namespace ZKit.PathFinder
{
    public static class UUtil
    {
        public static Rect ScanMapSize(int pathLayerMask, int obstacleLayerMask)
        {
            TerrainCollider[] terrain = (TerrainCollider[])GameObject.FindObjectsOfType(typeof(TerrainCollider));

            Rect result = new Rect(0f, 0f, 0f, 0f);

            if (terrain != null)
            {
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

            List<string> layerNames = new List<string>();
            for (int i = 0; i < 32; ++i)
            {
                if (LayerMask.LayerToName(i).Length != 0)
                    layerNames.Add(LayerMask.LayerToName(i));
            }
            List<string> selectedLayers = new List<string>();
            for (int i = 0; i < layerNames.Count; ++i)
            {
                if (((pathLayerMask & (1 << i)) != 0) || ((obstacleLayerMask & (1 << i)) != 0))
                    selectedLayers.Add(layerNames[i]);
            }
            int selectedLayerMask = LayerMask.GetMask(selectedLayers.ToArray());

            foreach (GameObject go in (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if ((selectedLayerMask & (1 << go.layer)) == 0) continue;

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
                    else if (result.xMin > v.x) result.xMin = v.x;

                    if (result.yMax < v.z) result.yMax = v.z;
                    else if (result.yMin > v.z) result.yMin = v.z;
                }
            }

            return result;
        }

        public static Cuboid ScanMapSize3D(int pathLayerMask, int obstacleLayerMask)
        {
            TerrainCollider[] terrain = (TerrainCollider[])GameObject.FindObjectsOfType(typeof(TerrainCollider));

            Cuboid result = new Cuboid(0f, 0f, 0f, 0f, 0f, 0f);

            if (terrain != null)
            {
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

            List<string> layerNames = new List<string>();
            for (int i = 0; i < 32; ++i)
            {
                if (LayerMask.LayerToName(i).Length != 0)
                    layerNames.Add(LayerMask.LayerToName(i));
            }
            List<string> selectedLayers = new List<string>();
            for (int i = 0; i < layerNames.Count; ++i)
            {
                if (((pathLayerMask & (1 << i)) != 0) || ((obstacleLayerMask & (1 << i)) != 0))
                    selectedLayers.Add(layerNames[i]);
            }
            int selectedLayerMask = LayerMask.GetMask(selectedLayers.ToArray());

            foreach (GameObject go in (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if ((selectedLayerMask & (1 << go.layer)) == 0) continue;

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
                    else if (result.xMin > v.x) result.xMin = v.x;

                    if (result.yMax < v.y) result.yMax = v.y;
                    else if (result.yMin > v.y) result.yMin = v.y;

                    if (result.zMax < v.z) result.zMax = v.z;
                    else if (result.zMin > v.z) result.zMin = v.z;
                }
            }

            return result;
        }
    }
}