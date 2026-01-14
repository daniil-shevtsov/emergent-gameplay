using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using emergentgameplay.core.debug;

public partial class Game : Node2D
{
	private Player _player;

	private List<Enemy> _enemies = new();

	private Enemy _enemy;
	private Enemy _enemy2;
	
	private DebugOverlay _debugOverlay;

	private PackedScene? _bulletResource = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_enemy = GetNode<Enemy>("Enemy");
		_enemy2 = GetNode<Enemy>("Enemy2");
		_bulletResource = GD.Load<PackedScene>("res://bullet.tscn");
		_debugOverlay = GetNode<DebugOverlay>("DebugOverlay");
		_debugOverlay.debugDraw.IsDebugEnabled = true;

		_enemies.Add(_enemy);
		_enemies.Add(_enemy2);
		var count = 0;
		_enemies.ForEach(enemy =>
		{
			enemy.Id = $"enemy_{count++}";
			SetCallbackSafe(enemy.RangeArea, (area2D) =>
			{
				area2D.BodyEntered += body =>
				{
					if (body is Player)
					{
						enemy.OnPlayerInRange(true);
					}
				};
				area2D.BodyExited += body =>
				{
					if (body is Player)
					{
						enemy.OnPlayerInRange(false);

					}
				};
				return true;
			});
		
			SetCallbackSafe(enemy.SightArea, (area2D) =>
			{
				area2D.BodyEntered += body =>
				{
					if (body is Player)
					{
						enemy.IsPlayerInSight = true;
					}
				};
				area2D.BodyExited += body =>
				{
					if (body is Player)
					{
						enemy.IsPlayerInSight = false;
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
		bullet.QueueFree();
		var bulletDamage = 10f;
		_player.UpdateDamage(bulletDamage);
	}
	
	private void SetCallbackSafe(Area2D area2D, Func<Area2D, Boolean> callbackSetter)
	{
		area2D.ProcessMode = ProcessModeEnum.Disabled;
		callbackSetter(area2D);
		area2D.ProcessMode = ProcessModeEnum.Inherit;
	}

	private bool isFrontOccupied = false;
	private bool isRightOccupied = false;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_enemies.ForEach((enemy) =>
		{
			if (enemy.IsPlayerInSight)
			{
				if (!isFrontOccupied)
				{
					enemy.SetRunTarget(_player._frontMarker.GlobalPosition);
					enemy.DebugIndicator.Color = _player._frontMarker.GetNode<ColorRect>("ColorRect").Color;
					isFrontOccupied = true;
				} else if (!isRightOccupied)
				{
					enemy.SetRunTarget(_player.RightMarker.GlobalPosition);
					enemy.DebugIndicator.Color = _player.RightMarker.GetNode<ColorRect>("ColorRect").Color;

					isRightOccupied = true;
				}
			}
			else
			{
				enemy.SetRunTarget(null);
			}
			enemy.UpdateShootingPosition(_player.GlobalPosition);
			DebugDrawSingleton.Instance.UpdateVectorToDraw(enemy.Id, _player.GlobalPosition, enemy.GlobalPosition);

			var isTimeToShoot = enemy._timer.TimeLeft == 0f;
			if (isTimeToShoot && enemy.IsPlayerInRange)
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
		isFrontOccupied = false;
		isRightOccupied = false;
		
		_debugOverlay.debugDraw.Redraw();
	}
}
