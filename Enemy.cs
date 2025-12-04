using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public Gun _gun;
	public Area2D _sightArea;
	public const float Speed = 300.0f;
	
	private Vector2? _lastKnownTargetPosition = null;

	public override void _Ready()
	{
		_gun = GetNode<Gun>("Gun");
		_sightArea = GetNode<Area2D>("SightArea");
	}

	public void onPlayerDetected(Vector2 globalPosition)
	{
		_lastKnownTargetPosition = globalPosition;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Vector2.Zero;
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
			var targetDirection = _lastKnownTargetPosition - GlobalPosition;
			var degreesToTurn = 0f;
			RotationDegrees += degreesToTurn;
			
			LookAt(_lastKnownTargetPosition.Value);
		}
	}
}
