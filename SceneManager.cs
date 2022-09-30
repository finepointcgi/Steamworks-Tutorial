using Godot;
using System;
using System.Collections.Generic;
using Steamworks.Data;
using Steamworks;

public class SceneManager : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public PackedScene LobbyElement;
        [Export]
    public PackedScene LobbyPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SteamManager.OnLobbyRefreshCompleted += OnLobbyRefreshCompletedCallback;
        SteamManager.OnPlayerJoinLobby += OnPlayerJoinLobbyCallback;
        SteamManager.OnPlayerLeftLobby += OnPlayerLeftLobbyCallback;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void OnPlayerJoinLobbyCallback(Friend friend){
        var element = LobbyPlayer.Instance() as LobbyPlayer;
        element.Name = friend.Id.AccountId.ToString(); 
        element.SetPlayerInfo(friend.Name);
        GetNode<VBoxContainer>("Lobby Users").AddChild(element);

    }

    private void OnPlayerLeftLobbyCallback(Friend friend){
        GetNode<LobbyPlayer>($"Lobby Users/{friend.Id.AccountId.ToString()}").QueueFree();
    }

    private void OnLobbyRefreshCompletedCallback(List<Lobby> lobbies){
        foreach (var item in lobbies)
        {
            var element  = LobbyElement.Instance() as LobbyElement;
            element.SetLabels(item.Id.ToString(), item.GetData("ownerNameDataString") + " Lobby" , item);
            GetNode<VBoxContainer>("Lobby Container").AddChild(element);
        }
    }

    private void _on_InviteFriend_button_down(){
        SteamManager.Manager.OpenFriendOverlayForInvite();
    }

    private void _on_CreateLobby_button_down(){
        SteamManager.Manager.CreateLobby();
    }

    private void _on_GetLobbies_button_down(){
        SteamManager.Manager.GetMultiplayerLobbies();
    }

    private void _on_LobbyButton_button_down(){
        SteamManager.Manager.SteamConnectionManager.Connection.SendMessage("test");
    }
}
