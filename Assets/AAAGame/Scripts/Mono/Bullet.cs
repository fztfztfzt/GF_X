using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : CombatUnitEntity
{
    public SpriteRenderer spriteRenderer;
    public GameObject Shadow;
    public Vector3 Speed;
    public float LifeTime = 2.0f;
    public float g = 9.8f;
    private MaterialPropertyBlock propertyBlock;
    private float duration = 0.0f;
    private bool isDie = false;
    private bool isG = false;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    public void Init(Vector2 dir)
    {
        rb.linearVelocity = dir * Speed;
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
            ToDie();
            return;
        }

        //transform.position += Speed * Time.deltaTime;
        //Speed.y -= g * Time.deltaTime;
    }

    void ToDie()
    {
        if (isDie) return;
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        isDie = true;
        rb.simulated = false;
        Shadow.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var combatUnit = collision.GetComponent<CombatUnitEntity>();
        if (combatUnit != null)
        {
            if (this.Attack(combatUnit))
            {
                ToDie();
            }
        }
        GF.LogInfo($"bullt OnTriggerEnter2D {collision.name}");
    }
}
