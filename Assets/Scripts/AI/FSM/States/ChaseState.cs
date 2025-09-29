using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The "Chase" state for a guard AI.
/// 
/// Responsibilities:
/// - Continuously update the guard’s NavMeshAgent destination
///   towards the target (usually the player).
/// - Smoothly rotate the guard so it faces the direction it is moving.
/// - Stop chasing when the StateMachine transitions out (handled by conditions).
/// 
/// This state doesn’t decide *when* to chase—that’s the job of the FSM.
/// It only defines *what happens* while in chase mode.
/// </summary>
public class ChaseState : IState
{
    // The NavMeshAgent moves the guard around the NavMesh.
    // It handles pathfinding around obstacles automatically.
    readonly NavMeshAgent agent;

    // The Transform of the guard itself (used for rotation).
    readonly Transform self;

    // A function that returns the current position of the chase target.
    // Using Func<Vector3> instead of a fixed Transform makes this state more flexible:
    // - You can pass in () => player.position
    // - Or () => lastKnownPlayerPos
    // - Or even dynamically switch targets
    readonly System.Func<Vector3> getTargetPos;

    /// <summary>
    /// Name of the state (for debugging/UI display).
    /// </summary>
    public string Name => "Chase";

    /// <summary>
    /// Constructor for the state.
    /// Takes references to the NavMeshAgent, the guard’s Transform,
    /// and a delegate for the target’s position.
    /// </summary>
    public ChaseState(NavMeshAgent agent, Transform self, System.Func<Vector3> getTargetPos)
    {
        this.agent = agent;
        this.self = self;
        this.getTargetPos = getTargetPos;
    }

    /// <summary>
    /// Called when the state is first entered.
    /// Ensures the NavMeshAgent is active and ready to move.
    /// </summary>
    public void OnEnter() => agent.isStopped = false;

    /// <summary>
    /// Called every frame while chasing.
    /// Responsibilities:
    /// 1) Continuously update the NavMeshAgent’s destination to the target’s position.
    /// 2) Rotate the guard smoothly towards the direction it is moving.
    /// </summary>
    public void Tick(float dt)
    {
        // 1) Tell the NavMeshAgent where to go.
        agent.SetDestination(getTargetPos());

        // 2) Rotate smoothly towards the velocity (movement direction).
        Vector3 v = agent.velocity;
        v.y = 0; // ignore vertical component (we only rotate on the Y axis).

        // Only rotate if moving (velocity > small threshold).
        if (v.sqrMagnitude > 0.01f)
        {
            // Create a rotation that looks in the movement direction.
            Quaternion targetRot = Quaternion.LookRotation(v);

            // Slerp (spherical interpolate) blends smoothly between current and target rotation.
            // dt * 8f makes the turning speed frame-rate independent.
            self.rotation = Quaternion.Slerp(self.rotation, targetRot, dt * 8f);
        }
    }

    /// <summary>
    /// Called every FixedUpdate (not needed here, so left empty).
    /// This state doesn’t directly handle physics.
    /// </summary>
    public void FixedTick(float fdt) { }

    /// <summary>
    /// Called when leaving the state.
    /// Currently empty, but could stop animations, reset flags, etc.
    /// </summary>
    public void OnExit() { }
}
