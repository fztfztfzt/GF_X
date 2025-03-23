using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;
using static GridItemConfigs;
using Random = UnityEngine.Random;

//负责楼层创建相关
public partial class FloorComponent : GameFrameworkComponent
{
    Queue<Vector2Int> nextFind = new();
    Stack<Vector2Int> endRooms = new();
    public RoomType[,] map;
    int minRoomNumber = 10;
    int roomNumber = 0;
    int MinDeadEnds = 3;

    public void GenFloor()
    {
        map = new RoomType[FloorWidth + 1, FloorHeight + 1];
        RefreshFloor();
    }

    void RefreshFloor()
    {
        Array.Clear(map, 0, map.Length);
        nextFind.Clear();
        endRooms.Clear();
        roomNumber = 1;
        GenRoomNumber();
        startPos = new Vector2Int(FloorWidth / 2, FloorWidth / 2);
        nextFind.Enqueue(startPos);
        map[(int)startPos.x, (int)startPos.y] = RoomType.ROOM;
        while (nextFind.Count > 0)
        {
            var pos = nextFind.Dequeue();
            int hasRoom = GenRoom(pos + Vector2Int.up);
            hasRoom += GenRoom(pos + Vector2Int.down);
            hasRoom += GenRoom(pos + Vector2Int.left);
            hasRoom += GenRoom(pos + Vector2Int.right);
            if (hasRoom == 0)
            {
                endRooms.Push (pos);
            }
        }
        Debug.Log($"RoomNum:{roomNumber},MaxRoomNum:{minRoomNumber}");
        // 检查房间数据
        if (!CheckFloor())
        {
            RefreshFloor();
        }
        else
        {
            GenRoom();
        }
    }


    void ClearShowFloor()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    void GenRoomNumber()
    {
        minRoomNumber = (int)(Random.Range(0, 2) + 5 + FloorLevel * 2.6f);
        MinDeadEnds = 5;
        if(FloorLevel != 1)
        {
            MinDeadEnds += 1;
        }
    }

    bool CheckFloor()
    {
        if (roomNumber < minRoomNumber) return false;
        if (endRooms.Count < MinDeadEnds) return false;
        return true;
    }

    int GenRoom(Vector2Int pos)
    {
        if (pos.x <= 0 || pos.x >= FloorWidth || pos.y <= 0 || pos.y >= FloorHeight)
        {
            return 0;
        }
        //如果我们已经有足够的房间，那就放弃
        if (roomNumber > minRoomNumber)
        {
            return 0;
        }
        //随机 50% 几率，放弃
        if (Random.Range(1, 100) <= 50)
        {
            return 0;
        }

        //如果相邻单元已被占用，放弃
        if (map[(int)pos.x, (int)pos.y] != 0)
        {
            return 0;
        }

        //如果相邻单元格本身有大于1个填充的相邻单元格，放弃
        if (NeighborHasRoom(pos))
        {
            return 0;
        }

        map[(int)pos.x, (int)pos.y] = RoomType.ROOM;
        roomNumber++;
        nextFind.Enqueue(pos);
        return 1;
    }

    private bool NeighborHasRoom(Vector2Int pos)
    {
        return (int)map[(int)pos.x + 1, (int)pos.y] + (int)map[(int)pos.x - 1, (int)pos.y] + (int)map[(int)pos.x, (int)pos.y + 1] + (int)map[(int)pos.x, (int)pos.y - 1] > 1;
    }



    void GenRoom()
    {
        CreateEndRoom();
        Rooms.Clear();
        for (int i = 1; i <= FloorWidth; i++)
        {
            for (int j = 1; j <= FloorHeight; j++)
            {
                if (map[i, j] != RoomType.EMPTY)
                {
                    var room = new RoomData();
                    Rooms.Add(GetRoomId(i, j),room);
                    room.Init(map[i, j],i,j);
                    //根据房间类型获取布局
                    LoadRoomLayout(room);
                }
            }
        }
        curRoom = Rooms[GetRoomId(startPos.x, startPos.y)];
        curRoom.Pass = true;
    }

    private void CreateEndRoom()
    {
        if(FloorLevel == 1) SetRoomType(startPos, RoomType.START);
        //生成隐藏房
        //boss房生成在最后一个endRooms中
        SetRoomType(endRooms.Pop(), RoomType.BOSS);
        // 必定生成一个商品房
        if(FloorLevel < 7) SetRoomType(endRooms.Pop(), RoomType.SHOP);
        // 宝藏房
        if(FloorLevel < 7) SetRoomType(endRooms.Pop(), RoomType.TREASURE);
        // 牺牲房
        if (Random.Range(1, 7) == 1) SetRoomType(endRooms.Pop(), RoomType.SACRIFICE);
        //判断是否生成诅咒房
        if(Random.Range(1,2) == 1) SetRoomType(endRooms.Pop(), RoomType.CURSE);
        //其他都是普通房间

    }

    void SetRoomType(Vector2Int pos, RoomType roomType)
    {
        map[pos.x,pos.y] = roomType;
    }

    async void LoadRoomLayout(RoomData roomData)
    {
        var configAsset = UtilityBuiltin.AssetsPath.GetScriptableAsset("RoomLayout/1");
        var layout = await GFBuiltin.Resource.LoadAssetAwait<RoomLayoutAsset>(configAsset);
        int id = 0;
        foreach(var item in layout.items)
        {
            var pos = item.GetPos();
            switch(item.type)
            {
                case 1:
                    var gridItem = new DestroyableItemGridData();
                    gridItem.y = pos.y;
                    gridItem.x = pos.x;
                    var gridItemInfo = GridItemConfigs.Instance.Items[item.dataId];
                    gridItem.hp = gridItemInfo.sprites.Length - 1;
                    gridItem.dataId = item.dataId;
                    gridItem.id = id;
                    roomData.Grids.Add(id++,gridItem);
                    break;
            }
        }
    }
}
