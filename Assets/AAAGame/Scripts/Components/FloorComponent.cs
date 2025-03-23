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
    SACRIFICE,
    CURSE,
    EVENT,
}
//楼层数据管理
public partial class FloorComponent: GameFrameworkComponent
{
    public int FloorWidth = 10;
    public int FloorHeight = 10;
    public int FloorLevel = 1;
    public Dictionary<int, RoomData> Rooms = new(10);
    Vector2Int startPos;
    public RoomData curRoom;
    internal Vector2Int GetCurRoomPos()
    {
        return startPos;
    }

    internal RoomType GetRoomType(in Vector2Int pos)
    {
        return map[pos.x, pos.y];
    }

    internal RoomType GetRoomShowType(in Vector2Int vector2Int)
    {
        var rt = GetRoomType(vector2Int); 
        return rt;
    }

    internal void EnterRoom(in Vector2Int dir)
    {
        GF.Event.Fire(this, SwitchRoomEventArgs.Create(startPos,dir));
        startPos += dir;
        curRoom = Rooms[GetRoomId(startPos.x, startPos.y)];
        curRoom.Pass = true;
    }

    public int GetRoomId(int i,int j)
    {
        return i * FloorWidth + j;
    }

    public RoomData GetRoomData(int i,int j)
    {
        return Rooms[GetRoomId(i, j)];
    }

    public GridData GetRoomGridData(int id)
    {
        //return curRoom.GetGridData(id);
        return null;
    }
}
