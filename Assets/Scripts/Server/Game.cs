using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Game
{
    public string ip;
    public ushort port;
    public string title;
    public ushort players;
    public ushort capacity;

    public int lastLatency = -1;

    public Game(string ip, ushort port, string title, ushort players, ushort capacity)
    {
        this.ip = ip;
        this.port = port;
        this.title = title;
        this.players = players;
        this.capacity = capacity;
    }
}
