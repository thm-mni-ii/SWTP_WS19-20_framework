using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
/**
 * Game class to save the game information
 */
public class Game
{
    public string ip;
    public ushort port;
    public string title;
    public ushort players;
    public ushort capacity;

    public int lastLatency = -1;

    /**
    * make a new Game and save the game information (ip, port, title, players, capacity)
    */
    public Game(string ip, ushort port, string title, ushort players, ushort capacity)
    {
        this.ip = ip;
        this.port = port;
        this.title = title;
        this.players = players;
        this.capacity = capacity;

    }
}
