using Godot;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
public class GameManager
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

   public static List<Player> CurrentPlayers = new List<Player>();

   public static void OnPlayerJoinedLobby(Friend player){
        Player p = new Player();
        p.FriendData = player;
        CurrentPlayers.Add(p);
   }

   public static void OnPlayerReady(Dictionary<string, string> dict){
        var players = CurrentPlayers;
        Player player = CurrentPlayers.Where(x => x.FriendData.Id.AccountId.ToString() == dict["PlayerName"]).FirstOrDefault();
        player.IsReady = bool.Parse(dict["IsReady"]);

        if(SteamManager.Manager.IsHost){

            SteamManager.Manager.Broadcast(JsonConvert.SerializeObject(dict));
            if(CurrentPlayers.Count(x => x.IsReady) == CurrentPlayers.Count){
                GD.Print("Everyone is Ready! Game Start!");

                Dictionary<string, string> readyPacket = new Dictionary<string, string>(){
                    {"DataType" , "StartGame"},
                    {"SceneToLoad", "res://GameScene"}
                };
            
                SteamManager.Manager.Broadcast(JsonConvert.SerializeObject(readyPacket));
            }
        }
   }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
