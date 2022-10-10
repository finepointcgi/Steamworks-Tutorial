using Godot;
using Steamworks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public Friend FriendData {get; set;}
   
   public bool IsReady;
    public bool Controlled;
    private int currentFrame = 0;
    private int targetFrame = 10;
    private Vector2 targetPosition;

    public override void _Ready(){
        DataParser.OnPlayerUpdate += OnPlayerUpdateCallback;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
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
            currentFrame += 1;
            if(currentFrame == targetFrame){
                updateRemoteLocation(Position, Rotation);
                currentFrame = 0;
            }
        }else{
            Position = Position.LinearInterpolate(targetPosition, delta * 15);
        }
    }

    private void updateRemoteLocation(Vector2 position, float rotation){
        var dict = new Dictionary<string, string>(){
            {"DataType", "UpdatePlayer"},
            {"PlayerID", SteamManager.Manager.PlayerSteamID.AccountId.ToString()},
            {"positionx", position.x.ToString()},
            {"positiony", position.y.ToString()},
            {"rotation", rotation.ToString()}
        };
        if(SteamManager.Manager.IsHost){
            SteamManager.Manager.Broadcast(JsonConvert.SerializeObject(dict));
        }else{
            SteamManager.Manager.SteamConnectionManager.Connection.SendMessage(JsonConvert.SerializeObject(dict));
        }
    }


    private void OnPlayerUpdateCallback(Dictionary<string, string> dict){
        if(dict["PlayerID"] == SteamManager.Manager.PlayerSteamID.AccountId.ToString()) 
            return;

        if(dict["PlayerID"] == FriendData.Id.AccountId.ToString()){
                targetPosition = new Vector2(float.Parse(dict["positionx"]),float.Parse(dict["positiony"]));
                Rotation = float.Parse(dict["rotation"]);
        }
    }
}
