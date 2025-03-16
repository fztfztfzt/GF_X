using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = UnityEngine.Random;

public enum RoomType
{
    EMPTY,
    ROOM,
    BOSS,
    START,
    MONSTER,
    TREASURE,
    SHOP,
    EVENT,
}

public class FloorComponent: GameFrameworkComponent
{
    public int FloorWidth = 10;
    public int FloorHeight = 10;
    public int FloorLevel = 1;
    Queue<Vector2Int> nextFind = new();
    List<Vector2Int> endRooms = new();
    public RoomType[,] map;
    public RoomData[,] Rooms;
    int maxRoomNumber = 10;
    int roomNumber = 0;
    Vector2Int startPos;

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
                endRooms.Add(pos);
            }
        }
        Debug.Log($"RoomNum:{roomNumber},MaxRoomNum:{maxRoomNumber}");
        // 检查房间数据
        if (roomNumber < maxRoomNumber)
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
        maxRoomNumber = (int)(Random.Range(0, 2) + 5 + FloorLevel * 2.6f);
    }

    int GenRoom(Vector2Int pos)
    {
        if (pos.x <= 0 || pos.x >= FloorWidth || pos.y <= 0 || pos.y >= FloorHeight)
        {
            return 0;
        }
        //如果我们已经有足够的房间，那就放弃
        if (roomNumber > maxRoomNumber)
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

    internal Vector2Int GetCurRoomPos()
    {
        return startPos;
    }

    internal RoomType GetRoomType(Vector2Int pos)
    {
        return map[pos.x, pos.y];
    }

    internal RoomType GetRoomShowType(Vector2Int vector2Int)
    {
        var rt = GetRoomType(vector2Int); 
        return rt;
    }

    void GenRoom()
    {
        map[(int)startPos.x, (int)startPos.y] = RoomType.START;
        var bossRoom = endRooms.Last();
        map[bossRoom.x, bossRoom.y] = RoomType.BOSS;
        Rooms = new RoomData[FloorWidth + 1, FloorHeight + 1];
        for (int i = 1; i <= FloorWidth; i++)
        {
            for (int j = 1; j <= FloorHeight; j++)
            {
                if (map[i, j] != RoomType.EMPTY)
                {
                    var room = new RoomData();
                    Rooms[i, j] = room;
                    room.RoomType = map[i, j];
                    room.Pass = false;
                }
            }
        }
        Rooms[startPos.x, startPos.y].Pass = true;
    }

    internal void EnterRoom(Vector2Int dir)
    {
        GF.Event.Fire(this, SwitchRoomEventArgs.Create(startPos,dir));
        startPos += dir;
        Rooms[startPos.x, startPos.y].Pass = true;
    }

    public int GetRoomId(int i,int j)
    {
        return i * FloorWidth + j;
    }
}
