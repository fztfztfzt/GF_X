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
        gameObject.layer = LayerMask.NameToLayer(CampFlag == CombatFlag.Player ? "Player" : "Enemy");
    }


    public virtual bool ApplyDamage(CombatUnitEntity attacker, int damgeValue)
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
