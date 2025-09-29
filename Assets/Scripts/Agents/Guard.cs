using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Transform guardPost;
    public FieldOfView fov;

    [Header("Tuning")]
    public float patrolRadius = 6f;
    public float waitAtPoint = 1.2f;
    public float loseSightAfter = 2.0f;
    public float attackRange = 1.8f;

    NavMeshAgent agent;
    StateMachine fsm;
    public string FsmCurrentName => fsm?.Current?.Name;


    // States
    PatrolState patrol;
    ChaseState chase;
    ReturnState ret;

    float lastSeenTime;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    void Start()
    {
        // instantiate states
        patrol = new PatrolState(agent, transform, guardPost, patrolRadius, waitAtPoint);
        chase = new ChaseState(agent, transform, () => player ? player.position : transform.position);
        ret = new ReturnState(agent, guardPost);

        // build FSM
        fsm = new StateMachine();
        fsm.SetInitial(patrol);

        fsm.AddTransition(patrol, chase, () => fov && fov.canSeePlayer);
        fsm.AddTransition(chase, ret, () => Time.time - lastSeenTime > loseSightAfter);
        fsm.AddTransition(ret, patrol, () => ret.AtPost);

        fsm.AddGlobal(chase, () => fov && fov.canSeePlayer);
    }

    void Update()
    {
        if (fov && fov.canSeePlayer) lastSeenTime = Time.time;
        fsm.Tick(Time.deltaTime);
    }

    void FixedUpdate() => fsm.FixedTick(Time.fixedDeltaTime);

    void OnGUI()
    {
        if (!Camera.main) return;
        var p = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
        if (p.z > 0)
        {
            string label = $"Guard | {fsm.Current?.Name}";
            var size = GUI.skin.label.CalcSize(new GUIContent(label));
            GUI.Label(new Rect(p.x - size.x / 2f, Screen.height - p.y - size.y, size.x, size.y), label);
        }
    }
}
