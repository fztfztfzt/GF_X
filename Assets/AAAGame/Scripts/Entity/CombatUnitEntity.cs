using cfg;
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

    public const string P_CombatFlag = "P_CombatFlag";
    public const string P_DataTableRow = "DataTableRow";

    /// <summary>
    /// 阵营
    /// </summary>
    protected CombatFlag CampFlag { get; private set; }
    public combat_unit CombatUnitRow { get; private set; }
    public int MaxHp;

    public bool IsFullHp => Hp == MaxHp;
    public virtual int Hp { get; protected set; }

    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        CampFlag = (CombatFlag)Params.Get<VarInt32>(P_CombatFlag).Value;
        gameObject.layer = LayerMask.NameToLayer(CampFlag == CombatFlag.Player ? "Player" : "Enemy");
        CombatUnitRow = Params.Get(P_DataTableRow) as combat_unit;
        if (CombatUnitRow != null)
        {
            Hp = CombatUnitRow.Hp;
        }
        MaxHp = Hp;
    }

    public virtual int ComputeDamage(CombatUnitEntity other)
    {
        return 1;//默认伤害值，半个心
    }

    public virtual bool Attack(CombatUnitEntity dest)
    {
        int damage = 1;
        if (dest.CampFlag == CombatFlag.Player)
        {
            if (GF.Floor.FloorLevel == 1)
            {
                damage = 1;
            }
            else
            {
                damage = 2;
            }
        }
        else
        {
            damage = (int)(3.5 * Mathf.Sqrt(1.2f*1 + 1));
        }
        dest.ApplyDamage(damage);
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
    public virtual void ApplyHeal(int healValue)
    {
        Hp += healValue;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
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
