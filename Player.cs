using Godot;
using Steamworks;
using System;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public Friend FriendData {get; set;}
   
   public bool IsReady;
    public bool Controlled;
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(Controlled){
            Vector2 movementVector = Vector2.Zero;
            if(Input.IsActionPressed("ui_up")){
                movementVector += Vector2.Up;
            }
            if(Input.IsActionPressed("ui_down")){
                movementVector += Vector2.Down;
            }
            if(Input.IsActionPressed("ui_left")){
                movementVector += Vector2.Left;
            }
            if(Input.IsActionPressed("ui_right")){
                movementVector += Vector2.Right;
            }
            MoveAndSlide(movementVector * 100, Vector2.Up);
        }else{
            //controlled by steam networking
        }
    }
}
