using UnityEngine;
using System.Collections;
using System.IO;

using ZKit;
using ZKit.PathFinder;

public class CellLoader : MonoBehaviour
{
    void Awake()
    {
        LoadFromFile("Assets/Scenes/CellData/" + Application.loadedLevelName);
        JPS.Instance.SetMap();
    }
	
    void LoadFromFile(string filePath)
    {
        var cells = DataCon.Instance.CellDatas;
        filePath = Path.ChangeExtension(filePath, ".cel");
        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        if (fs != null)
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                br.ReadString(); // SceneName
                cells.CellSize = br.ReadSingle(); // CellSize
                cells.HeightLimit = br.ReadSingle(); // HeightLimit
                Vector2 sp;
                sp.x = br.ReadSingle();
                sp.y = br.ReadSingle();
                cells.StartingPoint = sp;
                int width = br.ReadInt32();
                int height = br.ReadInt32();
                cells.Create(width, height);

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        cells[y, x].Height = br.ReadSingle();
                        cells[y, x].Type = EnumHelper.Parse<CellType, int>(br.ReadInt32());
                    }
                }
            }
            fs.Close();
        }
    }
}
