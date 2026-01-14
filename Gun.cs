using Godot;
using System;

public partial class Gun : StaticBody2D
{

	public Marker2D _endMarker;
	public ProgressBar _progressBar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_endMarker = (Marker2D) GetNode("EndMarker");
		_progressBar = (ProgressBar)FindChild("ProgressBar");
	}

	public void SetProgress(long value)
	{
		_progressBar.Value = value;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
