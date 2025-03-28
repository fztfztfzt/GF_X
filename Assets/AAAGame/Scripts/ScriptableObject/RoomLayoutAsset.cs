// RoomLayoutAsset.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using static GridItemConfigs;
[Serializable]
public class RoomGridDef
{
    [SerializeField] public int id;
    [SerializeField]public int type;
    [SerializeField]public int dataId;
    public Texture2D sprite;
    public string name;
    public void Copy(RoomGridDef other,int id)
    {
        this.type = other.type;
        this.dataId = other.dataId;
        this.id = id;
    }
    public Vector2Int GetPos()
    {
        return new Vector2Int(id%100,id/100);
    }
}

[CreateAssetMenu(fileName = "New Room Layout", menuName = "Room Layout/New Layout")]
public class RoomLayoutAsset : ScriptableObject
{
    [SerializeField] public int width = 10;
    [SerializeField] public int height = 6;
    [SerializeField] public List<RoomGridDef> items;

    public Dictionary<int, RoomGridDef> layoutData = new();

    public void Initialize()
    {
        layoutData.Clear();
        if(items != null)
        {
            foreach (var item in items)
            {
                layoutData[item.id] = item;
            }
        }
    }

    public void SetObjectAtPosition(int x, int y, RoomGridDef obj)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (obj == null)
            {
                layoutData.Remove(GetId(x, y));
            }
            else
            {
                layoutData[GetId(x, y)] = obj;
            }
        }
    }

    public RoomGridDef GetObjectAtPosition(int x, int y)
    {
        if (layoutData.TryGetValue(GetId(x, y),out var ans))
        {
            return ans;
        }
        return null;
    }

    public int GetId(int x,int y)
    {
        return y * 100 + x;
    }

    public void SaveLayout()
    {
        // 保存逻辑可以在这里实现，或者在编辑器中处理
        items = new();
        foreach(var item in layoutData)
        {
            var data = new RoomGridDef();
            data.Copy(item.Value,item.Key);
            items.Add(data);
        }
    }

    public void LoadLayout()
    {
        // 加载逻辑可以在这里实现，或者在编辑器中处理
    }
}