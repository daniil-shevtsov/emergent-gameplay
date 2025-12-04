using Godot;
using System;

public partial class Game : Node2D
{
	private Player _player;

	private Enemy _enemy;

	private bool _isPlayerInSight = false;

	private bool _isPlayerInRun = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_enemy = GetNode<Enemy>("Enemy");

		_enemy._sightArea.ProcessMode = ProcessModeEnum.Disabled;
		_enemy._sightArea.BodyEntered += body =>
		{
			GD.Print("body entered sight");
			if (body is Player)
			{
				_isPlayerInSight = true;
			}
		};
		_enemy._sightArea.BodyExited += body =>
		{
			GD.Print("body exited sign");
			if (body is Player)
			{
				_isPlayerInSight = false;
			}
		};
		_enemy._runArea.BodyEntered += body =>
		{
			GD.Print("body entered run");
			if (body is Player)
			{
				_isPlayerInRun = true;
			}
		};
		_enemy._runArea.BodyExited += body =>
		{
			GD.Print("body exited run");
			if (body is Player)
			{
				_isPlayerInRun = false;
			}
		};
		_enemy._sightArea.ProcessMode = ProcessModeEnum.Inherit;
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
	}
}
