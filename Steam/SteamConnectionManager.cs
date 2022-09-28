using Steamworks;
using Steamworks.Data;
using System;
using Godot;

public class SteamConnectionManager : ConnectionManager{
    public override void OnConnected(ConnectionInfo info)
    {
        base.OnConnected(info);
        GD.Print("on Connection");
    }

    public override void OnConnecting(ConnectionInfo info)
    {
        base.OnConnecting(info);
        GD.Print("on Connecting");
    }

    public override void OnDisconnected(ConnectionInfo info)
    {
        base.OnDisconnected(info);
        GD.Print("on Disconnection");
    }
    
    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        base.OnMessage(data, size, messageNum, recvTime, channel);
        GD.Print("got a message!");
    }
}