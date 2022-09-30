using Godot;
using System;
using System.Collections.Generic;
using Steamworks.Data;
public class SceneManager : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public PackedScene LobbyElement;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SteamManager.OnLobbyRefreshCompleted += OnLobbyRefreshCompletedCallback;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void OnLobbyRefreshCompletedCallback(List<Lobby> lobbies){
        foreach (var item in lobbies)
        {
            var element  = LobbyElement.Instance() as LobbyElement;
            element.SetLabels(item.Id.ToString(), item.GetData("ownerNameDataString") + " Lobby" , item);
            GetNode<VBoxContainer>("Lobby Container").AddChild(element);
        }
    }

    private void _on_InviteFriend_button_down(){
        
    }

    private void _on_CreateLobby_button_down(){
        SteamManager.Manager.CreateLobby();
    }

    private void _on_GetLobbies_button_down(){
        SteamManager.Manager.GetMultiplayerLobbies();
    }
}
