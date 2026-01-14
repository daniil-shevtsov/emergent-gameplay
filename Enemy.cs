using Godot;
using System;
using emergentgameplay.core.debug;

public partial class Enemy : CharacterBody2D
{
	public Gun _gun;
	public Area2D RangeArea;
	public Area2D SightArea;
	public Timer _timer;
	public ColorRect DebugIndicator;
	public const float Speed = 300.0f;
	public string Id = "";
	
	public  bool IsPlayerInRange = false;

	public bool IsPlayerInSight = false;
	
	private Vector2? _shootingTarget = null;
	public Vector2? RunTargetPosition = null;

	public override void _Ready()
	{
		_gun = GetNode<Gun>("Gun");
		RangeArea = GetNode<Area2D>("RangeArea");
		SightArea = GetNode<Area2D>("SightArea");
		_timer = GetNode<Timer>("Timer");
		DebugIndicator = (ColorRect)FindChild("DebugIndicator");
	}

	public void SetRunTarget(Vector2? target)
	{
		RunTargetPosition = target;
	}

	public void OnPlayerInRange(bool isPlayerInRange)
	{
		IsPlayerInRange = isPlayerInRange;
	}
	
	public void UpdateShootingPosition(Vector2? globalPosition)
	{
		if (IsPlayerInRange)
		{
			_shootingTarget = globalPosition;
		}
		else
		{
			_shootingTarget = null;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Vector2.Zero;

		var distance =0f;
		if (RunTargetPosition != null)
		{
			direction = GlobalPosition.DirectionTo(RunTargetPosition.Value);
			distance = GlobalPosition.DistanceTo(RunTargetPosition.Value);
			DebugDrawSingleton.Instance.UpdateVectorToDraw($"enemy-direction {Id}", GlobalPosition, RunTargetPosition.Value);
		}
		
		
		if (direction != Vector2.Zero && distance >= 5f)
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

		if (_shootingTarget != null)
		{
			DebugDrawSingleton.Instance.UpdateVectorToDraw($"shooting-direction {Id}", GlobalPosition, _shootingTarget.Value, new Color(1f,0f,0f));
			DebugDrawSingleton.Instance.UpdateVectorToDraw($"shooting-direction 2 {Id}", GlobalPosition, (_shootingTarget.Value - GlobalPosition), new Color(1f,0f,1f));
			var rotationSpeed = 1.5f;
			var angle = (_shootingTarget.Value - GlobalPosition).Angle();
			GlobalRotation = Mathf.LerpAngle(GlobalRotation, angle, (float)delta * rotationSpeed);
		}
	}
}
