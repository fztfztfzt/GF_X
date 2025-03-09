using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerSpriteSetter playerSpriteSetter;
    public Rigidbody2D rb;
    public float speed;
    public int HeadState = 0;


    private Vector2 moveDir;
    private Vector2 attackDir;
    private Vector2 SquareTocircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }
    public void OnMove(InputValue value)
    {
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

    public void OnAttack(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        attackDir = input;
        UpdateAction();
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
