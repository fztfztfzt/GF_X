using UnityEngine;
using UnityEngine.AI;

public class MonsterEntity : CombatUnitEntity
{
    public NavMeshAgent agent;
    public Transform target;
    private void Awake()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected override void OnShow(object userData)
    {
        base.OnShow(userData);

    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        if (target != null)
        {
            agent.destination = target.position;
        }
    }

    public override void Enter()
    {
        base.Enter();
        agent.enabled = true;
        target = GF.Floor.PlayerEntity.transform;
    }

    public override void Leave()
    {
        base.Leave();
        agent.enabled = false;
        target = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var combatUnit = collision.transform.GetComponent<CombatUnitEntity>();
        if (combatUnit != null)
        {
            this.Attack(combatUnit);
        }
        GF.LogInfo($"bullt OnTriggerEnter2D {collision.collider.name}");
    }
}
