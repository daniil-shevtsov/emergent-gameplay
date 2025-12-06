using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

public partial class Game : Node2D
{
	private Player _player;

	private List<Enemy> _enemies = new();

	private Enemy _enemy;
	private Enemy _enemy2;

	private bool _isPlayerInSight = false;

	private bool _isPlayerInRun = false;

	private PackedScene? _bulletResource = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_enemy = GetNode<Enemy>("Enemy");
		_enemy2 = GetNode<Enemy>("Enemy2");
		_bulletResource = GD.Load<PackedScene>("res://bullet.tscn");

		_enemies.Add(_enemy);
		_enemies.Add(_enemy2);
		
		_enemies.ForEach(enemy =>
		{
			SetCallbackSafe(enemy._sightArea, (area2D) =>
			{
				area2D.BodyEntered += body =>
				{
					if (body is Player)
					{
						_isPlayerInSight = true;
					}
				};
				area2D.BodyExited += body =>
				{
					if (body is Player)
					{
						_isPlayerInSight = false;
					}
				};
				return true;
			});
		
			SetCallbackSafe(enemy._runArea, (area2D) =>
			{
				area2D.BodyEntered += body =>
				{
					if (body is Player)
					{
						_isPlayerInRun = true;
					}
				};
				area2D.BodyExited += body =>
				{
					if (body is Player)
					{
						_isPlayerInRun = false;
					}
				};
				return true;
			});
		});

		
		
		SetCallbackSafe(_player._damageArea, (area2D) =>
		{
			area2D.BodyEntered += body =>
			{
				if (body is Bullet bullet)
				{
					HandlePlayerHit(bullet);
				}
			};
			return true;
		});
	}

	private void HandlePlayerHit(Bullet bullet)
	{
		GD.Print("Player hit");
		bullet.QueueFree();
		var bulletDamage = 10f;
		_player.Health -= bulletDamage;
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
		_enemies.ForEach((enemy) =>
		{
			if (_isPlayerInSight)
			{
				enemy.onPlayerDetected(_player.GlobalPosition);
			}

			if (_isPlayerInRun && !_isPlayerInSight)
			{
				enemy.SetRunTarget(_player._frontMarker.GlobalPosition);
			}

			var isTimeToShoot = enemy._timer.TimeLeft == 0f;
			if (isTimeToShoot)
			{
				var bullet = (Bullet) _bulletResource.Instantiate().Duplicate();
				var bulletSpawnPosition = enemy._gun._endMarker.GlobalPosition;
				var direction = (bulletSpawnPosition - enemy.GlobalPosition).Normalized();
				var bulletSpeed = 500f;
				var bulletVelocity = direction * bulletSpeed;
			
				AddChild(bullet);
				bullet.GlobalPosition = bulletSpawnPosition;
				bullet.Rotation = bulletVelocity.Angle();
				bullet.LinearVelocity = bulletVelocity;
				enemy._timer.Start();
			}
		});
	}
}
