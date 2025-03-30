using Codice.Client.Common;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameFramework.Runtime;

public class RoomEntity : EntityBase
{
    public Door[] doors;
    private List<int> gridEntityIds;
    RoomData roomData;
    internal async UniTask SetData(RoomData room,Vector2Int curPos)
    {
        roomData = room;
        SerDoor(curPos + Vector2Int.up, 0);
        SerDoor(curPos + Vector2Int.down, 1);
        SerDoor(curPos + Vector2Int.left, 2);
        SerDoor(curPos + Vector2Int.right, 3);
        await CreateEntities();
    }

    private async UniTask CreateEntities()
    {
        if(gridEntityIds == null)
        {
            gridEntityIds = new List<int>(10);
        }
        else
        {
            return;
        }
        var offset = new Vector3(43.5f - 48f, 62.4f - 60f, 0);
        foreach (var info in roomData.Grids)
        {
            var item = info.Value;
            var entity = await item.OnEnter(transform.position + offset);
            gridEntityIds.Add(entity.Entity.Id);
        }
    }

    private void SerDoor(Vector2Int curPos,int index)
    {
        var show = GF.Floor.GetRoomShowType(curPos);
        doors[index].Show(show != RoomType.EMPTY);
    }

    public override void Leave()
    {
        foreach (var id in gridEntityIds)
        {
            var entity = GF.Entity.GetEntity<EntityLogic>(id);
            if (entity != null)
            {
                entity.Leave();
            }
        }
    }

    public override void Enter()
    {
        foreach (var id in gridEntityIds)
        {
            var entity = GF.Entity.GetEntity<EntityLogic>(id);
            if (entity != null)
            {
                entity.Enter();
            }
        }
    }

}

