using System;
using System.Collections.Generic;

/// <summary>
/// A tiny, reusable Finite State Machine (FSM) that switches between IState objects
/// based on boolean conditions. Designed to be easy to read and drop into Unity.
/// 
/// Key ideas:
/// - The FSM holds exactly one "Current" state at a time.
/// - Each frame (Tick), it looks for a transition whose condition is true:
///     1) Check GLOBAL transitions (higher priority, can fire from any state)
///     2) Check LOCAL transitions (only those that start at the Current state)
/// - If a transition is found, we "change state": call OnExit() on the old state,
///   then OnEnter() on the new one.
/// - After handling transitions, we run Current.Tick(dt) to let the state do its work.
/// 
/// Design choices :
/// - Conditions are simple Func<bool> lambdas, so students can write () => distance < 3f, etc.
/// - We store transitions as tuples for brevity (from, to, condition).
/// - Global transitions allow "interrupts" like "if player is visible, chase" from any state.
/// </summary>
public class StateMachine
{
    /// <summary>
    /// The state currently active/running.
    /// - 'private set' means only this class can change it (encapsulation),
    ///   but other code can read it (useful for debugging/labels).
    /// </summary>
    public IState Current { get; private set; }

    /// <summary>
    /// LOCAL transitions:
    /// - Only considered if 'from' equals the Current state.
    /// - Tuple layout: (fromState, toState, conditionFunction).
    /// - Example: (Patrol, Chase, () => canSeePlayer)
    /// </summary>
    private readonly List<(IState from, IState to, Func<bool> cond)> _local = new();

    /// <summary>
    /// GLOBAL transitions:
    /// - Considered BEFORE local transitions (higher priority).
    /// - Do not depend on the Current state (fire from anywhere).
    /// - Tuple layout: (toState, conditionFunction).
    /// - Example: (Chase, () => canSeePlayer)
    /// 
    /// Why globals? Great for "interrupts" (e.g., spot the player ? chase now),
    /// emergency states, or default fallbacks.
    /// </summary>
    private readonly List<(IState to, Func<bool> cond)> _global = new();

    /// <summary>
    /// Sets the initial state and calls its OnEnter().
    /// Must be called before Tick() is used.
    /// </summary>
    public void SetInitial(IState state)
    {
        // Assign the starting state.
        Current = state;

        // Null-conditional operator (?.): only call OnEnter if Current is not null.
        Current?.OnEnter();
    }

    /// <summary>
    /// Adds a LOCAL transition that is only valid when 'from' is the Current state.
    /// Example usage:
    ///   fsm.AddTransition(patrol, chase, () => canSeePlayer);
    /// </summary>
    public void AddTransition(IState from, IState to, Func<bool> cond)
        => _local.Add((from, to, cond));

    /// <summary>
    /// Adds a GLOBAL transition that can fire from ANY state.
    /// Example usage:
    ///   fsm.AddGlobal(chase, () => canSeePlayer);
    /// </summary>
    public void AddGlobal(IState to, Func<bool> cond)
        => _global.Add((to, cond));

    /// <summary>
    /// Called once per frame (e.g., in MonoBehaviour.Update).
    /// Order of operations:
    ///   1) Look for a valid transition (globals first, then locals).
    ///   2) If found, change state.
    ///   3) Run the Current state's Tick(dt).
    /// </summary>
    public void Tick(float dt)
    {
        // Ask if any transition should fire right now.
        var next = FindTransition();

        // If a transition is available, switch states before ticking.
        // This ensures the new state's Tick runs on the same frame the condition became true.
        if (next != null) ChangeState(next);

        // Let the current state perform its per-frame logic.
        Current?.Tick(dt);
    }

    /// <summary>
    /// Called at fixed intervals (e.g., in MonoBehaviour.FixedUpdate) for physics-friendly work.
    /// Simply forwards the call to the current state.
    /// </summary>
    public void FixedTick(float fdt) => Current?.FixedTick(fdt);

    /// <summary>
    /// Finds the first transition whose condition is true.
    /// Priority:
    ///   1) GLOBAL transitions (checked in the order they were added)
    ///   2) LOCAL transitions for the Current state (in the order they were added)
    /// Returns the 'to' state if a transition should happen; otherwise null.
    /// </summary>
    private IState FindTransition()
    {
        // 1) Check GLOBAL transitions first (higher priority).
        foreach (var g in _global)
            if (g.cond())                 // If the condition returns true...
                return g.to;              // ...we want to transition to g.to immediately.

        // 2) Check LOCAL transitions that start from the Current state.
        foreach (var t in _local)
            // Only consider transitions whose 'from' matches the Current state
            if (t.from == Current && t.cond())
                return t.to;

        // No transition should fire this frame.
        return null;
    }

    /// <summary>
    /// Performs the actual state switch:
    ///   - Calls OnExit() on the old state
    ///   - Assigns the new state to Current
    ///   - Calls OnEnter() on the new state
    /// 
    /// Includes a guard to avoid redundant work if 'next' equals Current.
    /// </summary>
    private void ChangeState(IState next)
    {
        // If we're already in 'next', do nothing (prevents double-enter/exit issues).
        if (next == Current) return;

        // Give the old state a chance to clean up (stop animations, timers, etc.).
        Current?.OnExit();

        // Switch to the new state.
        Current = next;

        // Let the new state initialize (start animations, reset timers, etc.).
        Current?.OnEnter();
    }
}
