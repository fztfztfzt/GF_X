using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GenFloor : MonoBehaviour
{
    public GameObject Cell;
    public int FloorWidth = 10;
    public int FloorHeight = 10;
    public int FloorLevel = 1;
    public float RoomSize = 1.0f;
    public Sprite[] FloorSprites;
    Queue<Vector2Int> nextFind = new ();
    List<Vector2Int> endRooms = new ();
    int[,] map;
    int maxRoomNumber = 10;
    int roomNumber = 0;
    Vector2Int startPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void RefreshFloor()
    {
        map = new int[FloorWidth+1, FloorHeight+1];
        nextFind.Clear();
        endRooms.Clear();
        roomNumber = 1;
        GenRoomNumber();
        startPos = new Vector2Int(FloorWidth/2, FloorWidth/2);
        nextFind.Enqueue(startPos);
        map[(int)startPos.x, (int)startPos.y] = 1;
        while (nextFind.Count > 0)
        {
            var pos = nextFind.Dequeue();
            int hasRoom = GenRoom(pos + Vector2Int.up);
            hasRoom += GenRoom(pos + Vector2Int.down);
            hasRoom += GenRoom(pos + Vector2Int.left);
            hasRoom += GenRoom(pos + Vector2Int.right);
            if(hasRoom == 0)
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
            map[(int)startPos.x, (int)startPos.y] = 2;
            var bossRoom = endRooms.Last();
            map[bossRoom.x, bossRoom.y] = 3;
        }
    }

    private void ShowFloor()
    {
        var offset = new Vector2(GetComponent<RectTransform>().rect.width / 2, GetComponent<RectTransform>().rect.height / 2);
        for (int i = 1; i <= FloorWidth; i++)
        {
            for (int j = 1; j <= FloorHeight; j++)
            {
                if (map[i, j] != 0)
                {
                    var obj = Instantiate(Cell,transform,false);
                    Debug.Log($"x:{i - startPos.x},y:{j - startPos.y}");
                    obj.GetComponent<RectTransform>().localPosition = new Vector3((i - startPos.x) * RoomSize, (j - startPos.y) * RoomSize, 0);
                    obj.GetComponent<Image>().sprite = FloorSprites[map[i, j] - 1];
                }
            }
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
        if(pos.x <= 0 || pos.x >= FloorWidth || pos.y <= 0 || pos.y >= FloorHeight)
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
        
        map[(int)pos.x, (int)pos.y] = 1;
        roomNumber++;
        nextFind.Enqueue(pos);
        return 1;
    }

    private bool NeighborHasRoom(Vector2Int pos)
    {
        return map[(int)pos.x + 1, (int)pos.y] + map[(int)pos.x - 1, (int)pos.y] + map[(int)pos.x, (int)pos.y + 1] + map[(int)pos.x, (int)pos.y - 1] > 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            ClearShowFloor();
            RefreshFloor();
            ShowFloor();
        }
    }
}
