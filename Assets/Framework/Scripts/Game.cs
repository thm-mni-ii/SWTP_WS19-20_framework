using System;

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
    /// <summary>
    /// *hier kommt noch was*
    /// </summary>
    public int lastLatency = -1;

    /// <summary>
    /// make a new Game and save the game information (ip, port, title, players, capacity)
    /// </summary>
    /// <param name="ip">ip address of the Hosting player</param>
    /// <param name="port">Hosting port</param>
    /// <param name="title">game title</param>
    /// <param name="players">number of players</param>
    /// <param name="capacity">game capacity</param>
    public Game(string ip, ushort port, string title, ushort players, ushort capacity)
    {
        this.ip = ip;
        this.port = port;
        this.title = title;
        this.players = players;
        this.capacity = capacity;
    }
}
