using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class MineStat
{
    const ushort dataSize = 512; // this will hopefully suffice since the MotD should be <=59 characters
    const ushort numFields = 6;  // number of values expected from server
    private string address;
    private ushort port;
    private bool serverUp;
    private string motd;
    private string version;
    private string currentPlayers;
    private string maximumPlayers;
    private string ping;

    public MineStat(string address, ushort port)
    {
        byte[] rawServerData = new byte[dataSize];
        string[] serverData;

        SetAddress(address);
        SetPort(port);

        try
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            TcpClient tcp = new TcpClient();
            stopwatch.Start();
            var result = tcp.BeginConnect(address, port, null, null);
            stopwatch.Stop();
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(50));
            if (!success) throw new Exception("Failed to connect.");
            Stream stream = tcp.GetStream();
            byte[] payload = { 0xFE, 0x01 };
            stream.Write(payload, 0, payload.Length);
            stream.Read(rawServerData, 0, dataSize);
            stream.Close();
            tcp.Close();
            SetPing(stopwatch.Elapsed.TotalMilliseconds.ToString());
        }
        catch (Exception)
        {
            serverUp = false;
            return;
        }

        if (rawServerData == null || rawServerData.Length == 0)
            serverUp = false;
        else
        {
            serverData = Encoding.Unicode.GetString(rawServerData).Split("\u0000\u0000\u0000".ToCharArray());
            if (serverData != null && serverData.Length >= numFields)
            {
                serverUp = true;
                SetVersion(serverData[2]);
                SetMotd(serverData[3]);
                SetCurrentPlayers(serverData[4]);
                SetMaximumPlayers(serverData[5]);
            }
            else
                serverUp = false;
        }
    }

    public string GetPing()
    {
        return ping;
    }

    public void SetPing(string p)
    {
        this.ping = p;
    }

    public string GetAddress()
    {
        return address;
    }

    public void SetAddress(string address)
    {
        this.address = address;
    }

    public ushort GetPort()
    {
        return port;
    }

    public void SetPort(ushort port)
    {
        this.port = port;
    }

    public string GetMotd()
    {
        return motd;
    }

    public void SetMotd(string motd)
    {
        this.motd = motd;
    }

    public string GetVersion()
    {
        return version;
    }

    public void SetVersion(string version)
    {
        this.version = version;
    }

    public string GetCurrentPlayers()
    {
        return currentPlayers;
    }

    public void SetCurrentPlayers(string currentPlayers)
    {
        this.currentPlayers = currentPlayers;
    }

    public string GetMaximumPlayers()
    {
        return maximumPlayers;
    }

    public void SetMaximumPlayers(string maximumPlayers)
    {
        this.maximumPlayers = maximumPlayers;
    }

    public bool IsServerUp()
    {
        return serverUp;
    }
}