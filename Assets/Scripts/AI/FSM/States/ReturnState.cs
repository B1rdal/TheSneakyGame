using UnityEngine;
using UnityEngine.AI;

public class ReturnState : IState
{
    readonly NavMeshAgent agent;
    readonly Transform post;
    public bool AtPost { get; private set; }

    public string Name => "Return";

    public ReturnState(NavMeshAgent agent, Transform post) { this.agent = agent; this.post = post; }

    public void OnEnter()
    {
        AtPost = false;
        Vector3 p = post ? post.position : agent.transform.position;
        agent.isStopped = false;
        agent.SetDestination(p);
    }

    public void Tick(float dt)
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            AtPost = true;
    }

    public void FixedTick(float fdt) { }
    public void OnExit() { }
}
