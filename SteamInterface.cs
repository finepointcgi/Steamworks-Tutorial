using Godot;
using System;
using Steamworks;
using Steamworks.Data;
using System.Text;

public class SteamInterface : Control
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

    public void _on_PrintFriends_button_down(){
        foreach(var friend in SteamFriends.GetFriends()){
            GD.Print($"{friend.Id}: {friend.Name}");

            if(friend.Name == "Hunter"){
                friend.SendMessage("Hello From Steamworks Tutorial!!");
            }
        }
    }

    public void _on_Get_Achevements_button_down(){
        var achevements = SteamUserStats.Achievements;
        foreach(var achevement in achevements){
            GD.Print($"{achevement.Name} {achevement.State}");
        }
    }

    public void _on_Acheve_Test_Achevements_button_down(){
        var testAchevement = new Achievement("TutorialAchevement");
        testAchevement.Trigger();
        //testAchevement.Clear();
    }

    private async void createLeaderboard(){
        Leaderboard? leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync($"Scores", LeaderboardSort.Ascending, LeaderboardDisplay.Numeric);

        if(leaderboard.HasValue){
            Leaderboard steamLeaderboard = leaderboard.Value;
            //LeaderboardUpdate? update =  await steamLeaderboard.SubmitScoreAsync(100);
            LeaderboardUpdate? update = await steamLeaderboard.ReplaceScore(100);
            if(update.HasValue){
                LeaderboardUpdate updateValue = update.Value;
                GD.Print("your global rank is: " + updateValue.NewGlobalRank.ToString());
            }else{
                GD.Print("Error cant submit leaderboard score!");
            }
        }
        else{
            GD.Print("Leaderboard Does Not Exist!");
        }
    }

    public void _on_CreateLeaderBoard_button_down(){
        createLeaderboard();
    }

    public void _on_FetchLeaderBoardScores_button_down(){
        getLeadboardScore();   
    }

    private async void getLeadboardScore(){
        Leaderboard? leaderboard = await SteamUserStats.FindLeaderboardAsync("Scores");
        if(leaderboard.HasValue){
            Leaderboard currentLeaderboard = leaderboard.Value;
            LeaderboardEntry[] entries = await currentLeaderboard.GetScoresAsync(10);

            foreach (var item in entries)
            {
                GD.Print($"{item.GlobalRank} {item.User} {item.Score}");   
            }
        }
    }

    private void _on_SaveToCloud_button_down(){
        var fileContents = Encoding.ASCII.GetBytes("test");
        SteamRemoteStorage.FileWrite("save.txt", fileContents);
    }

    private void _on_LoadFromCloud_button_down(){
        var fileContents = SteamRemoteStorage.FileRead("save.txt");
        GD.Print(Encoding.Default.GetString(fileContents));
    }
}
