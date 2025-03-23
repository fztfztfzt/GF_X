using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 战斗单位实体
/// </summary>
public class CombatUnitEntity : EntityBase
{
    public enum CombatFlag
    {
        Player,
        Enemy
    }


    /// <summary>
    /// 阵营
    /// </summary>
    protected CombatFlag CampFlag { get; private set; }
    public CombatUnitTable CombatUnitRow { get; private set; }

    public virtual int Hp { get; protected set; }

    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
    }

    public virtual int ComputeDamage(CombatUnitEntity other)
    {
        return 1;//默认伤害值，半个心
    }

    public virtual bool Attack(CombatUnitEntity dest)
    {
        dest.ApplyDamage(1);
        return true;
    }
    public virtual bool ApplyDamage(int damgeValue)
    {
        if (Hp <= 0) return false;
        Hp -= damgeValue;
        if (Hp <= 0)
        {
            OnBeKilled();
            return true;
        }
        return false;
    }

    protected virtual void OnBeKilled()
    {
        GF.Entity.HideEntity(this.Entity);
    }

    internal void SetColor(Color green)
    {
        GetComponent<Renderer>().material.color = green;
    }
}
