using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

using ZKit;
using ZKit.PathFinder;

public class PackCellData : IEnumerable//, IEnumerator
{
    private Vector2 _startingPoint = new Vector2();
    public Vector2 StartingPoint { get { return _startingPoint; } set { _startingPoint = value; } }
    private CellData[,] _cellDatas = new CellData[0, 0];
    public CellData[,] GetCellData { get { return _cellDatas; } }
    private int _countX = 0;
    private int _countY = 0;
    public int CountX { get { return _countX; } }
    public int CountY { get { return _countY; } }

    ~PackCellData() { Clear(); }

    public void Clear() { _cellDatas = new CellData[0, 0]; _countX = _countY = 0; }

    public CellData this[int y, int x] { get { return _cellDatas[y, x]; } }
    public bool IsEmpty
    {
        get
        {
            if (_cellDatas.Length > 0) return false;
            else return true;
        }
    }
    public bool IsExist(Point index)
    {
        return IsExist(index.x, index.y);
    }
    public bool IsExist(int x, int y)
    {
        if (x < 0 || y < 0) return false;
        if (x >= _countX) return false;
        if (y >= _countY) return false;
        return true;
    }
    public void Create(int x, int y)
    {
        Clear();
        _cellDatas = new CellData[y, x];
        for (int i = 0; i < y; ++i)
        {
            for (int j = 0; j < x; ++j)
            {
                _cellDatas[i, j] = new CellData(j, i);
            }
        }
        _countX = x;
        _countY = y;
    }
    
    [Range(0, 10)]
    public float CellSize = 1f;
    public float HeightLimit = 0.5f;
    public Rect MapBound = new Rect();

    public Vector2 GetPosVec2(Point index)
    {
        return new Vector2(_startingPoint.x + (index.x * (CellSize)), _startingPoint.y - (index.y * (CellSize)));
    }
    public Vector3 GetPosVec3(Point index, float y = 0f)
    {
        return new Vector3(_startingPoint.x + (index.x * (CellSize)), y, _startingPoint.y - (index.y * (CellSize)));
    }

    public Point GetNearIndex(Vector3 pos)
    {
        Vector3 rb = pos - new Vector3(_startingPoint.x, 0, _startingPoint.y);
        if (rb.z < 0f) rb.z *= -1f; //UnityEngine.Mathf.Abs(rb.z);
        Point index = new Point((int)(rb.x / CellSize), (int)(rb.z / CellSize));
        if ((float)(rb.x - (index.x * CellSize)) > (CellSize * 0.5f)) ++index.x;
        if ((float)(rb.z - (index.y * CellSize)) > (CellSize * 0.5f)) ++index.y;
        return index;
    }

    #region IEnumerable 구현
    public IEnumerator GetEnumerator()
    {
        foreach (var element in _cellDatas)
        {
            yield return element;
        }
    }
    #endregion

}

public class DataCon
{
    private static DataCon _instance;
    private static object _lock = new object();

    public static DataCon Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    ConstructorInfo constructor = null;

                    try
                    {
                        constructor = typeof(DataCon).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[0], null);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Get Constructor Failed", e);
                    }

                    if (constructor == null || constructor.IsAssembly)
                        throw new Exception(string.Format("constructor is missing {0}", typeof(DataCon).Name));
                    _instance = (DataCon)constructor.Invoke(null);
                }
            }
            return _instance;
        }
    }

    #region CellData
    private PackCellData _cellDatas = new PackCellData();
    public PackCellData CellDatas { get { return _cellDatas; } }
    #endregion

}
