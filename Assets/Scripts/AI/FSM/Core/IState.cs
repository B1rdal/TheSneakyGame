/// <summary>
/// The IState interface defines the "contract" that every state in our FSM must follow.
/// 
/// Why use an interface?
/// - It enforces consistency: all states must provide the same set of methods.
/// - It allows the StateMachine to treat all states the same way, no matter what
///   behaviour they implement (Patrol, Chase, Attack, etc.).
/// - It keeps states modular: you can create new states in separate scripts
///   without changing the StateMachine code.
/// 
/// Every method here represents a "lifecycle stage" of a state.
/// </summary>
public interface IState
{
    /// <summary>
    /// A short name for the state, used mostly for debugging/labels.
    /// Example: "Patrol", "Chase", "Return".
    /// 
    /// Note: It’s a property, not a method, so states can just write:
    ///   public string Name => "Patrol";
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Called once, immediately after the state becomes active.
    /// Good place to:
    /// - Reset timers
    /// - Play an animation
    /// - Set a NavMeshAgent destination
    /// </summary>
    void OnEnter();

    /// <summary>
    /// Called every frame (from Update in Unity).
    /// Use this for logic that should run as often as possible:
    /// - Checking distances
    /// - Updating AI decisions
    /// - Rotating towards a target
    /// 
    /// The parameter 'dt' is the frame’s deltaTime,
    /// so you can make movement or timers frame-rate independent.
    /// </summary>
    void Tick(float dt);

    /// <summary>
    /// Called at fixed time intervals (from FixedUpdate in Unity).
    /// Use this for physics-related code:
    /// - Applying forces
    /// - Rigidbody movement
    /// - Physics checks
    /// 
    /// Many AI states don’t need this, so it can be left empty.
    /// </summary>
    void FixedTick(float fdt);

    /// <summary>
    /// Called once, just before the state is exited.
    /// Good place to:
    /// - Stop animations
    /// - Clear flags
    /// - Reset temporary variables
    /// 
    /// Ensures a clean handoff between states.
    /// </summary>
    void OnExit();
}
