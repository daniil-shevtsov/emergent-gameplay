using Godot;
using System;
using System.Linq.Expressions;

public partial class Game : Node2D
{
	private Player _player;

	private Enemy _enemy;

	private bool _isPlayerInSight = false;

	private bool _isPlayerInRun = false;

	private PackedScene? _bulletResource = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_enemy = GetNode<Enemy>("Enemy");
		_bulletResource = GD.Load<PackedScene>("res://bullet.tscn");

		SetCallbackSafe(_enemy._sightArea, (area2D) =>
		{
			area2D.BodyEntered += body =>
			{
				GD.Print("body entered sight");
				if (body is Player)
				{
					_isPlayerInSight = true;
				}
			};
			area2D.BodyExited += body =>
			{
				GD.Print("body exited sight");
				if (body is Player)
				{
					_isPlayerInSight = false;
				}
			};
			return true;
		});
		
		SetCallbackSafe(_enemy._runArea, (area2D) =>
		{
			area2D.BodyEntered += body =>
			{
				GD.Print("body entered run");
				if (body is Player)
				{
					_isPlayerInRun = true;
				}
			};
			area2D.BodyExited += body =>
			{
				GD.Print("body exited run");
				if (body is Player)
				{
					_isPlayerInRun = false;
				}
			};
			return true;
		});
	}
	
	private void SetCallbackSafe(Area2D area2D, Func<Area2D, Boolean> callbackSetter)
	{
		area2D.ProcessMode = ProcessModeEnum.Disabled;
		callbackSetter(area2D);
		area2D.ProcessMode = ProcessModeEnum.Inherit;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isPlayerInSight)
		{
			_enemy.onPlayerDetected(_player.GlobalPosition);
		}

		if (_isPlayerInRun && !_isPlayerInSight)
		{
			_enemy.SetRunTarget(_player._frontMarker.GlobalPosition);
		}

		var isTimeToShoot = _enemy._timer.TimeLeft == 0f;
		GD.Print($"time {_enemy._timer.TimeLeft}");
		if (isTimeToShoot)
		{
			var bullet = (Bullet) _bulletResource.Instantiate().Duplicate();
			var bulletSpawnPosition = _enemy._gun._endMarker.GlobalPosition;
			var direction = (bulletSpawnPosition - _enemy.GlobalPosition).Normalized();
			
			AddChild(bullet);
			bullet.GlobalPosition = bulletSpawnPosition;
			_enemy._timer.Start();
		}
		
	}
}
