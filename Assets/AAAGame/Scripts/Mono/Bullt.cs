using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Bullt : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector3 Speed;
    public float LifeTime = 2.0f;
    public float g = 9.8f;
    private MaterialPropertyBlock propertyBlock;
    private float duration = 0.0f;
    private bool isDie = false;
    private bool isG = false;
    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(propertyBlock);

    }
    private Vector2 SquareTocircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }
    public void SetDirection(Vector2 dir)
    {
        Speed = Speed * dir;
        if(Speed.y == 0)
        {
            isG = true;
        }
        else 
        {
            isG = false;
        }
    }

    private void Update()
    {
        duration += Time.deltaTime;
        if (duration >= LifeTime)
        {
            if(isDie) return;
            isDie = true;
            StartCoroutine(Die());
            return;
        }

        transform.position += Speed * Time.deltaTime;
        Speed.y -= g * Time.deltaTime;
    }

    private IEnumerator Die()
    {
        PlayDieAnima();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void PlayDieAnima()
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_TimeStart", Time.time);
        propertyBlock.SetInt("_Anim", 1);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}
