using Godot;
using System;
using System.Collections.Generic;
using Steamworks.Data;
using Steamworks;
using Newtonsoft.Json;
public class SceneManager : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public PackedScene LobbyElement;
    [Export]
    public PackedScene LobbyPlayer;

    private bool isPlayerReady;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SteamManager.OnLobbyRefreshCompleted += OnLobbyRefreshCompletedCallback;
        SteamManager.OnPlayerJoinLobby += OnPlayerJoinLobbyCallback;
        SteamManager.OnPlayerLeftLobby += OnPlayerLeftLobbyCallback;
        DataParser.OnReadyMessage += OnPlayerReadyMessageCallback;
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
        GameManager.OnPlayerJoinedLobby(friend);
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
        isPlayerReady = !isPlayerReady;
        Dictionary<string, string> playerDict = new Dictionary<string, string>();
        playerDict.Add("DataType", "Ready");
        playerDict.Add("PlayerName", SteamManager.Manager.PlayerSteamID.AccountId.ToString());
        playerDict.Add("IsReady", isPlayerReady.ToString());// True or False
        string str = JsonConvert.SerializeObject(playerDict);
        OnPlayerReadyMessageCallback(playerDict);
        if(SteamManager.Manager.IsHost){
            SteamManager.Manager.Broadcast(str);
        }else{
            SteamManager.Manager.SteamConnectionManager.Connection.SendMessage(str);
        }
    }

    private void OnPlayerReadyMessageCallback(Dictionary<string, string> dict){
        GetNode<LobbyPlayer>($"Lobby Users/{dict["PlayerName"]}").SetReadyStatus(bool.Parse(dict["IsReady"]));
        GameManager.OnPlayerReady(dict);
    }

    public void OnStartGame(Dictionary<string, string> dict){
        GD.Print("Starting Game");
    }
}
