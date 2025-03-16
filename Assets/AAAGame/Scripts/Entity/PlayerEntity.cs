using Cysharp.Threading.Tasks;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityGameFramework.Runtime;

public class PlayerEntity : EntityBase
{
    PlayerInputManager inputManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Door")
        {
            var door = collision.gameObject.GetComponent<Door>();
            // ÇÐ·¿¼ä
            GF.Floor.EnterRoom(door.Dir);
        }
    }

    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        inputManager = GetComponent<PlayerInputManager>();
        GF.Event.Subscribe(SwitchRoomEventArgs.EventId, SwitchRoom);

    }

    private void SwitchRoom(object sender, GameEventArgs e)
    {
        inputManager.enabled = false;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        inputManager.enabled = true;
    }
}
