using UnityEngine;
using UnityEngine.AI;

public class AttackState : IState
{
    readonly NavMeshAgent agent;
    readonly Transform self;
    readonly Transform post;
    readonly float radius;
    readonly float dwellTime;
    float timer;

    public string Name => "Attack";

    public AttackState(NavMeshAgent agent, Transform self, Transform post, float radius, float dwellTime)
    {
    }

    public void OnEnter() { timer = 0f; SetNewPoint(); agent.isStopped = false; }

    public void Tick(float dt)
    {
    }

    public void FixedTick(float fdt) { }
    public void OnExit() { }

    void SetNewPoint()
    {
    }
}