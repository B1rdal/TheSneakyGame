The Sneaky Game – AI FSM Prototype

This Unity project is a teaching scaffold for learning AI scripting for games.
It provides a simple top-down stealth scenario where guards patrol, chase the player, and return to their posts.
Your task is to extend the AI with an Attack state.

Project Overview

Included in this repo:

Core FSM

IState.cs – interface every state must implement

StateMachine.cs – lightweight finite state machine engine

States

PatrolState.cs – guard wanders around its post

ChaseState.cs – guard chases the player when seen

ReturnState.cs – guard returns to its guard post after losing the player

(Student task: add AttackState.cs)

AI Controller

GuardSimpleAI.cs – sets up states and transitions

Other Systems

FieldOfView.cs – vision cone & player detection

PlayerTopDown.cs – simple WASD/arrow-key player movement

Health.cs + IDamageable.cs – example system for receiving damage

Setup Instructions

Clone this repo

git clone https://github.com/fjharri/TheSneakyGame.git
cd TheSneakyGame


Open in Unity Hub

Recommended version: Unity 2022.3 LTS

Click Add Project from Disk → select this folder

Open the Scene

Assets/Scenes/Main.unity

Press Play

Use arrow keys to move the player

Avoid the guards’ vision cones

Watch them switch between Patrol, Chase, and Return states

Student Task – Implement Attack State
Goal

Make guards attack the player when they are close enough.
The attack should have a short windup, apply damage once, then enter a cooldown before resuming chase.

Steps (broad outline)

Create AttackState.cs

Implements IState

OnEnter: stop moving, reset timer

Tick: face player, after windup → apply damage once, wait cooldown

OnExit: re-enable movement

Wire into FSM (GuardSimpleAI)

Transition Chase → Attack when in range

Transition Attack → Chase when cooldown is done

Damage Application

Use IDamageable.TakeDamage() on the Player’s Health component

Show result in Console log

Feedback (optional but encouraged)

Change cone color during Attack

Play a sound or animation

Flash player’s screen on hit

 Acceptance Criteria

Guard attacks when close to player

Damage is applied only once per attack

After cooldown, guard resumes chase (or other FSM transitions)

Player’s Health decreases correctly

Stretch Goals

Prevent attack if player leaves range during windup

Flash the cone color (yellow → red) while winding up

Add knockback or animation events

Show HP on UI instead of Console

Contributing

Fork this repo and work on your branch

Commit often with meaningful messages

Share your AttackState implementation in class!
