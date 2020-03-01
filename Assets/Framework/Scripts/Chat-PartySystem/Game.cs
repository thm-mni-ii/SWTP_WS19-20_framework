using System;
using System.Collections.Generic;
using Mirror;

/// <summary>
/// Game Class for the Database Definition
/// </summary>
public class Game
{


    public int Minplayers;
    /// <summary>
    /// game capacity (How many player can in one room play)
    /// </summary>
    public int Maxplayers;

    /// <summary>
    /// // Default constructor required for calls to DataSnapshot.getValue(User.class)
    /// </summary>
    public Game()
    {
        
    }

    /// <summary>
    /// Contains Game Information
    /// </summary>
    /// <param name="Minplayers"> min number of players</param>
    /// <param name="Maxplayers">game capacity max number of players</param>
    public Game(int players, int capacity)
    {
        this.Minplayers = players;
        this.Maxplayers = capacity;
    }
}
