using Godot;
using System;
using Steamworks;
using Steamworks.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class SteamManager : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";'
    public static SteamManager Manager {get; set;}
    private static uint gameAppId {get; set;} = 2145350;
    public string PlayerName {get; set;}
    public SteamId PlayerSteamID {get; set;}
    private bool connectedToSteam {get; set;}
    private Lobby hostedLobby {get; set;}
    private List<Lobby> availableLobbies {get; set;} = new List<Lobby>();
    public static event Action<List<Lobby>> OnLobbyRefreshCompleted;
    public static event Action<Friend> OnPlayerJoinLobby;
    public static event Action<Friend> OnPlayerLeftLobby;
    public SteamSocketManager SteamSocketManager;
    public SteamConnectionManager SteamConnectionManager;
    public bool IsHost;
    public SteamManager(){
        if (Manager == null){
            Manager = this;
            try
            {
                SteamClient.Init(gameAppId, true);

                if(!SteamClient.IsValid){
                    GD.Print("Something went wrong Steam Client Is Not Valid!!");
                    throw new Exception();
                }

                PlayerName = SteamClient.Name;
                PlayerSteamID = SteamClient.SteamId;
                connectedToSteam = true;
                GD.Print("Steam Is Connected! playerName:" + PlayerName);
            }
            catch (System.Exception e)
            {
                connectedToSteam = false;
                GD.Print("Error Connecting To Steam: " + e.Message);
            }
        }
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreatedCallback;
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnectedCallback;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEnteredCallback;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequestedCallback;
    }

    private void OnLobbyMemberDisconnectedCallback(Lobby lobby, Friend friend){
        GD.Print("User Has Disconnectd left lobby: " + friend.Name);
        OnPlayerLeftLobby(friend);
    }

    private void OnLobbyMemberLeaveCallback(Lobby lobby, Friend friend){
        GD.Print("User Has left Disconnectd from lobby: " + friend.Name);
        OnPlayerLeftLobby(friend);
    }

    private void OnLobbyMemberJoinedCallback(Lobby lobby, Friend friend){
        GD.Print("User has joined the Lobby: " + friend.Name);
        OnPlayerJoinLobby(friend);
    }

    private void OnLobbyCreatedCallback(Result result, Lobby lobby){
        if (result != Result.OK){
            GD.Print("Lobby creation result was not ok");
        }else{
            GD.Print($"Created Lobby! id = {lobby.Id}");
        }

        CreateSteamSocketServer();
    }

    private void OnLobbyGameCreatedCallback(Lobby lobby, uint id, ushort port, SteamId steamId){
        GD.Print("firing callback for lobby game created!");
    }

    private void OnLobbyEnteredCallback(Lobby lobby){
        if(lobby.MemberCount > 0){
            GD.Print($"You joined {lobby.Owner.Name}'s lobby");
            hostedLobby = lobby;
            foreach (var item in lobby.Members)
            {
                OnPlayerJoinLobby(item);
            }
            lobby.SetGameServer(lobby.Owner.Id);
        }else{
            GD.Print("You have joined your own lobby");
        }
        JoinSteamSocketServer(lobby.Owner.Id);
    }

    public async Task<bool> CreateLobby(){
        try
        {
            GD.Print("Creating Lobby");
            Lobby? createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync(20);

            if(!createLobbyOutput.HasValue){
                GD.Print("lobby created by didnt instance correctly!");
                throw new Exception();
            }

            hostedLobby = createLobbyOutput.Value;
            hostedLobby.SetPublic();
            hostedLobby.SetJoinable(true);
            hostedLobby.SetData("ownerNameDataString", PlayerName);
            
            GD.Print("lobby Created!");
            return true;


        }
        catch (System.Exception  e)
        {
            GD.Print("Failed to create lobby " + e.Message);
            return false;
        }
    }

    private async void OnGameLobbyJoinRequestedCallback(Lobby lobby, SteamId id){
        RoomEnter joinSuccessful = await lobby.Join();
        if(joinSuccessful != RoomEnter.Success){
            GD.Print("Failed to Join Lobby");
        }
        else{
            hostedLobby = lobby;

            foreach (var item in lobby.Members)
            {
                OnPlayerJoinLobby(item);
            }


        }
    }

    public void OpenFriendOverlayForInvite(){
        SteamFriends.OpenGameInviteOverlay(hostedLobby.Id);
    }
    public async Task<bool> GetMultiplayerLobbies(){
        try
        {
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithMaxResults(10).RequestAsync();
            if(lobbies != null){
                foreach (var item in lobbies)
                {
                    GD.Print("lobby: " + item.Id);
                    availableLobbies.Add(item);
                }
            }

            OnLobbyRefreshCompleted.Invoke(availableLobbies);
            return true;
        }
        catch (System.Exception e)
        {
            GD.Print("Error fetching lobbies!: " + e.Message);
            return false;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        SteamClient.RunCallbacks();

        try
        {
            if(SteamSocketManager != null){
                SteamSocketManager.Receive();
            }
            if(SteamConnectionManager != null && SteamConnectionManager.Connected){
                SteamConnectionManager.Receive();
            }
        }
        catch (System.Exception e)
        {
            GD.Print("error reciving data: " + e.Message);
            throw;
        }
    }

    public override void _Notification(int what)
    {
        base._Notification(what);
        if(what == MainLoop.NotificationWmQuitRequest){
            SteamClient.Shutdown();
            GetTree().Quit();
        }
    }

    public void CreateSteamSocketServer(){
        SteamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>(0);

        SteamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(PlayerSteamID, 0);
        IsHost = true;
        GD.Print("We created our socket server!");
    }

    public void JoinSteamSocketServer(SteamId host){
        if(!IsHost){
            GD.Print("Joining Socket Server!");
            SteamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(host, 0);
        }
    }

    public void Broadcast(string data){
        foreach (var item in SteamSocketManager.Connected.ToArray())
        {
            item.SendMessage(data);
        }
    }
}
