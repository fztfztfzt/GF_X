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
    private float lastAttackTime = 0.0f;
    public float attackCD = 0.5f;
    public GameObject bulletPrefab;
    private void Awake()
    {
        GF.LogInfo("123");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Door")
        {
            var door = collision.gameObject.GetComponent<Door>();
            // ÇÐ·¿¼ä
            GF.Floor.EnterRoom(door.Dir);
        }
    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        GenBullet();
    }
    void GenBullet()
    {
        var attackDir = inputManager.AttackDir;
        var moveDir = inputManager.MoveDir;
        if (attackDir.x != 0 || attackDir.y != 0)
        {
            if (Time.time - lastAttackTime >= attackCD)
            {
                lastAttackTime = Time.time;
                var dir = attackDir;
                if (moveDir != Vector2.zero)
                {
                    dir += attackDir.x == 0 ? new Vector2(moveDir.x / 10, 0) : new Vector2(0, moveDir.y / 10);
                }
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Bullet>().Init(dir);
            }
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
