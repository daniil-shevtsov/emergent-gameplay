using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public Gun _gun;
	public Area2D _sightArea;
	public Area2D _runArea;
	public Timer _timer;
	public const float Speed = 300.0f;
	
	private Vector2? _lastKnownTargetPosition = null;
	private Vector2? _runTargetPosition = null;

	public override void _Ready()
	{
		_gun = GetNode<Gun>("Gun");
		_sightArea = GetNode<Area2D>("SightArea");
		_runArea = GetNode<Area2D>("RunArea");
		_timer = GetNode<Timer>("Timer");
	}

	public void SetRunTarget(Vector2 target)
	{
		_runTargetPosition = target;
	}

	public void onPlayerDetected(Vector2 globalPosition)
	{
		_lastKnownTargetPosition = globalPosition;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Vector2.Zero;

		if (_runTargetPosition != null)
		{
			direction = GlobalPosition.DirectionTo(_runTargetPosition.Value);
		}
		_runTargetPosition = null;
		
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}
		Velocity = velocity;
		MoveAndSlide();

		if (_lastKnownTargetPosition != null)
		{
			var rotationSpeed = 1.5f;
			var angle = (_lastKnownTargetPosition.Value - GlobalPosition).Angle();
			GlobalRotation = Mathf.LerpAngle(GlobalRotation, angle, (float)delta * rotationSpeed);
			// var targetDirection = _lastKnownTargetPosition - GlobalPosition;
			// var degreesToTurn = 0f;
			// RotationDegrees += degreesToTurn * (float)delta;
			//
			// LookAt(_lastKnownTargetPosition.Value);
		}
	}
}
