using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameFramework.Runtime;

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
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            var eParams = EntityParams.Create(Vector3.zero, Vector3.zero);
            eParams.Set<VarInt32>(GridEntity.P_GridID, 1);
            int entityId = GF.Entity.ShowEntity<GridEntity>("grid", Const.EntityGroup.Level, eParams);
        }
    }
}
