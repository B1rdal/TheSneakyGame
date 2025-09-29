using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    readonly NavMeshAgent agent;
    readonly Transform self;
    readonly Transform post;
    readonly float radius;
    readonly float dwellTime;
    float timer;

    public string Name => "Patrol";

    public PatrolState(NavMeshAgent agent, Transform self, Transform post, float radius, float dwellTime)
    {
        this.agent = agent; this.self = self; this.post = post;
        this.radius = radius; this.dwellTime = dwellTime;
    }

    public void OnEnter() { timer = 0f; SetNewPoint(); agent.isStopped = false; }

    public void Tick(float dt)
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            timer += dt;
            if (timer >= dwellTime) { timer = 0f; SetNewPoint(); }
        }
    }

    public void FixedTick(float fdt) { }
    public void OnExit() { }

    void SetNewPoint()
    {
        Vector3 center = post ? post.position : self.position;
        for (int i = 0; i < 10; i++)
        {
            Vector2 c = Random.insideUnitCircle * radius;
            Vector3 pos = center + new Vector3(c.x, 0, c.y);
            if (NavMesh.SamplePosition(pos, out var hit, 1.5f, NavMesh.AllAreas))
            { agent.SetDestination(hit.position); return; }
        }
        agent.SetDestination(center);
    }
}
