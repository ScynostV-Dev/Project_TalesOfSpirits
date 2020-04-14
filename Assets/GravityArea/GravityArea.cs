using Godot;
using System;

public class GravityArea : Area2D
{
	private KinematicBody2D player = null;
	private AnimationPlayer rotateplayer = null;
	private Timer timer = null;
	private Area2D area = null;

	private bool isPlayerIn = false;
	private bool isEvent = false;
	private int anim = -1;

	[Export]
	float degree = 180f;
	[Export]
	float targetdegree = 360f;

	private float var1 = 0;
	private float var2 = 0;

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");

		if (degree > 360) degree = degree % 360;
		if (targetdegree > 360) targetdegree = targetdegree % 360;

		var1 = degree / 360f;
		var2 = targetdegree / 360f;
	}

	public override void _PhysicsProcess(float delta)
	{
		if (player != null)
		{
			if (rotateplayer.CurrentAnimation != "")
			{
				if (anim == 0 && rotateplayer.CurrentAnimationPosition > var1)
				{
					rotateplayer.Stop(true);
					player.RotationDegrees = degree;
				}
				else if (anim == 1 && rotateplayer.CurrentAnimationPosition > var2)
				{
					rotateplayer.Stop(true);
					player.RotationDegrees = targetdegree;
				}
			}

			if (player.IsOnFloor() && OverlapsArea(area) && isEvent && !isPlayerIn)
			{
				isPlayerIn = true;
			}
			if (player.IsOnFloor() && !OverlapsArea(area) && !isEvent && isPlayerIn)
			{
				isPlayerIn = false;
			}
		}
	}

	[Signal]
	public delegate void call_player_animation(String animation, bool backwarts, float position);

	private void _on_Player_enter_area(KinematicBody2D player)
	{
		Console.Out.WriteLine("Dieser name hiere das ist das wichtige dingens::: " + this.Name);

		if (!isPlayerIn)
		{
			if (this.player == null)
			{
				this.player = player;
				foreach (Node cache in player.GetChildren())
				{
					if (cache.Name.Equals("RotatePlayer")) {
						this.rotateplayer = (AnimationPlayer) cache;
					}
					if (cache.Name.Equals("Area2D"))
					{
						this.area = (Area2D) cache;
					}
				}
			}
			//player.RotationDegrees = 180f;
			isEvent = true;
			timer.Start();
		}
	}


	private void _on_Player_exit_area(KinematicBody2D player)
	{
		if (isPlayerIn)
		{
			//player.RotationDegrees = 0f;
			isEvent = false;
			timer.Start();
		}
	}

	private void _on_Timer_timeout()
	{
		if (isPlayerIn)
		{
			rotateplayer.Play("gravityRotate");
			rotateplayer.Advance(0.1f);
			anim = 0;
			timer.Stop();
		}
		else
		{
			rotateplayer.Play("gravityRotate");
			rotateplayer.Advance(var1);
			anim = 1;
			timer.Stop();
		}
	}


}
