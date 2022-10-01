using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ChatBox : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void _on_Button_button_down(){
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("DataType:","ChatMessage");
        dict.Add("UserID", SteamManager.Manager.PlayerName);
        dict.Add("Message", GetNode<LineEdit>("LineEdit").Text);

        string json = JsonConvert.SerializeObject(dict);


        SteamManager.Manager.SteamConnectionManager.Connection.SendMessage(json);
    }
}
