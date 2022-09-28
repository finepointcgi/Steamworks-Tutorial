using Godot;
using System;
using Steamworks.Data;

public class LobbyElement : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Lobby lobby{get; set;}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void SetLabels(string id, string name, Lobby lobby){
        GetNode<RichTextLabel>("ID").Text = id;
        GetNode<RichTextLabel>("LobbyName").Text = name;
        this.lobby = lobby;
    }


    public void _on_JoinButton_button_down(){
        lobby.Join();
    }
}
