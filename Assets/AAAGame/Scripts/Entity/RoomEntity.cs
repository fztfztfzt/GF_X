using System;
using UnityEngine;

public class RoomEntity : EntityBase
{
    public Door[] doors;
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);

    }

    internal void SetData(Vector2Int curPos)
    {
        SerDoor(curPos + Vector2Int.up,0);
        SerDoor(curPos + Vector2Int.down,1);
        SerDoor(curPos + Vector2Int.left,2);
        SerDoor(curPos + Vector2Int.right,3);
        var curRoomType = GF.Floor.GetRoomType(curPos);
    }

    private void SerDoor(Vector2Int curPos,int index)
    {
        var show = GF.Floor.GetRoomShowType(curPos);
        doors[index].Show(show != RoomType.EMPTY);
    }
}
