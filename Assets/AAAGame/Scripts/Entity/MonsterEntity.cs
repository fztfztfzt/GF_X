using UnityEngine;
using UnityEngine.AI;

public class MonsterEntity : EntityBase
{
    public NavMeshAgent agent;
    public Transform target;
    private void Awake()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void Update()
    {
        if(target!= null)
        {
            agent.destination = target.position;
        }
    }
}
