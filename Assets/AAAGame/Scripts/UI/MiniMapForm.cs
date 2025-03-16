
using GameFramework.Event;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapForm:UIFormBase
{
    public Sprite NormalCell;
    public Sprite CurCell;
    public Sprite PassCell;
    public Sprite[] FloorSprites;
    public GameObject Cell;
    public Vector2 RoomSize;
    public Transform CellParent;
    Dictionary<int, GameObject> rooms = new();
    Vector2Int startPos;
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        GF.Event.Subscribe(SwitchRoomEventArgs.EventId, SwitchRoom);

    }

    private void SwitchRoom(object sender, GameEventArgs e)
    {
        var args = e as SwitchRoomEventArgs;
        var curPos = GF.Floor.GetCurRoomPos();
        var oldPos = args.oldPos;
        var dir = args.dir;
        var oldRoom = rooms[GF.Floor.GetRoomId(oldPos.x, oldPos.y)];
        oldRoom.GetComponent<Image>().sprite = PassCell;

        var curRoom = rooms[GF.Floor.GetRoomId(curPos.x, curPos.y)];
        GenRoom(curPos.x, curPos.y);
        curRoom.GetComponent<Image>().sprite = CurCell;
    }

    public void RefreshAll()
    {
        var FloorWidth = GF.Floor.FloorWidth;
        var FloorHeight = GF.Floor.FloorHeight;
        var map = GF.Floor.Rooms;
        startPos = GF.Floor.GetCurRoomPos();
        var offset = new Vector2(GetComponent<RectTransform>().rect.width / 2, GetComponent<RectTransform>().rect.height / 2);
        for (int i = 1; i <= FloorWidth; i++)
        {
            for (int j = 1; j <= FloorHeight; j++)
            {
                var room = map[i, j];
                if (room != null && room.RoomType != RoomType.EMPTY && room.Pass)
                {
                    GenRoom(i, j);
                }
            }
        }
        var curRoom = rooms[GF.Floor.GetRoomId(startPos.x, startPos.y)];
        curRoom.GetComponent<Image>().sprite = CurCell;
    }

    void GenRoom(int i, int j,bool form = false)
    {
        var map = GF.Floor.Rooms;
        var room = map[i, j];
        if(room == null || room.RoomType == RoomType.EMPTY || (form && room.Pass))
        {
            return;
        } 
        int id = GF.Floor.GetRoomId(i, j);
        if(!rooms.TryGetValue(id, out var obj))
        {
            obj = Instantiate(Cell, CellParent, false);
            obj.GetComponent<RectTransform>().localPosition = new Vector3((i - startPos.x) * RoomSize.x, (j - startPos.y) * RoomSize.y, 0);
            rooms.Add(id, obj);
            var icon = FloorSprites[(int)room.RoomType];
            var iconCom = obj.transform.GetChild(0).GetComponent<Image>();
            iconCom.enabled = icon != null;
            if (icon != null)
            {
                iconCom.sprite = icon;
            }
        }
        
        if (room.Pass)
        {
            obj.GetComponent<Image>().sprite = PassCell;
            GenRoom(i + 1, j,true);
            GenRoom(i - 1, j, true);
            GenRoom(i, j + 1, true);
            GenRoom(i, j - 1, true);
        }
        else
        {
            obj.GetComponent<Image>().sprite = NormalCell;
        }

       
    }

    public void RefreshRoom()
    {

    }
}
