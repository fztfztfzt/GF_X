using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEntity : EntityBase
{
    public Collider2D m_collider;
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
    }

    private void LifeTimeOver()
    {
        List<Collider2D> results = new();
        int count = Physics2D.OverlapCollider(m_collider, results);
        foreach(var collision in results)
        {
            var combatUnit = collision.transform.GetComponent<CombatUnitEntity>();
            if (combatUnit != null)
            {
                combatUnit.ApplyDamage(100);
            }
            GF.LogInfo($"BombEntity OnTriggerEnter2D {collision.name}");
        }

        GF.Entity.HideEntity(this.Entity);
    }
}
