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
 * Game class to create a game and save the game information
 */
public class Game
{
    /// <summary>
    /// ip address of the Hosting player
    /// players can join to the game with this ip address
    /// </summary>
    public string ip;
    
    /// <summary>
    /// Hosting port
    /// </summary>
    public ushort port;
    
    /// <summary>
    /// game title
    /// </summary>
    public string title;
    
    /// <summary>
    /// number of players
    /// </summary>
    public ushort players;
    
    /// <summary>
    /// game capacity
    /// </summary>
    public ushort capacity;
    /*hier kommt noch was*/
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
