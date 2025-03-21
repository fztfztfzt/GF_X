﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerSpriteSetter playerSpriteSetter;
    public Rigidbody2D rb;
    public GameObject bulletPrefab;
    public float speed;
    public int HeadState = 0;
    public float attackCD = 0.5f;

    private Vector2 moveDir;
    private Vector2 attackDir;
    private float lastAttackTime = 0.0f;

    public void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;
        moveDir = Vector2.zero;
        attackDir = Vector2.zero;
        lastAttackTime = 0.0f;
        playerSpriteSetter.FaceNormal();
        playerSpriteSetter.BodyNormal();
    }

    public void OnEnable()
    {
        rb.linearVelocity = Vector2.zero;
        moveDir = Vector2.zero;
        attackDir = Vector2.zero;
        lastAttackTime = 0.0f;
    }
    private Vector2 SquareTocircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }
    public void OnMove(InputValue value)
    {
        if(enabled == false) return;
        Vector2 input = value.Get<Vector2>();
        input = SquareTocircle(input);
        moveDir = input;
        rb.linearVelocity = new Vector3(input.x, input.y, 0) * speed;
        UpdateAction(); 
    }

    private void SetFace(Vector2 input)
    {
        if (input.y > 0)
        {
            playerSpriteSetter.FaceBack();
            playerSpriteSetter.BodyWalkNormal();
        }
        else if (input.y < 0)
        {
            playerSpriteSetter.FaceNormal();
            playerSpriteSetter.BodyWalkNormal();
        }
        else if (input.x > 0)
        {
            playerSpriteSetter.FaceRight();
            playerSpriteSetter.BodyWalkRight();
        }
        else if (input.x < 0)
        {
            playerSpriteSetter.FaceLeft();
            playerSpriteSetter.BodyWalkLeft();
        }
        else
        {
            playerSpriteSetter.FaceNormal();
            playerSpriteSetter.BodyNormal();
        }
    }
    private void Update()
    {
        if (enabled == false) return;
        GenBullet();
    }
    void GenBullet()
    {
        if (attackDir.x != 0 || attackDir.y != 0)
        {
            if (Time.time - lastAttackTime >= attackCD)
            {
                lastAttackTime = Time.time;
                var dir = attackDir;
                if (moveDir != Vector2.zero)
                {
                    dir += attackDir.x == 0 ? new Vector2(moveDir.x/10, 0) : new Vector2(0, moveDir.y / 10);
                }
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Bullt>().SetDirection((dir));
            }
        }
    }

    public void OnAttack(InputValue value)
    {
        if (enabled == false) return;
        Vector2 input = value.Get<Vector2>();
        attackDir = input;
        UpdateAction();
        GenBullet();
    }

    private void UpdateAttackAction(Vector2 input)
    {
        if (input.x == 0 && input.y == 0)
        {
            playerSpriteSetter.PlayHeadAnim(false);
            SetFace(moveDir);
            return;
        }
        playerSpriteSetter.PlayHeadAnim(true);
        if (input.y > 0)
        {
            playerSpriteSetter.FaceBack();
        }
        else if (input.y < 0)
        {
            playerSpriteSetter.FaceNormal();
        }
        else if (input.x > 0)
        {
            playerSpriteSetter.FaceRight();
        }
        else if (input.x < 0)
        {
            playerSpriteSetter.FaceLeft();
        }
    }

    public void UpdateAction()
    {
        SetFace(moveDir);
        UpdateAttackAction(attackDir);

    }
    public void OnClick(InputValue value)
    {
        Debug.Log("OnClick");
    }
}
