using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// PartyPlayer class to manage users before a game starts
/// it keeps track of the player name and his ready state
/// </summary>
public class PartyPlayer
{
    private string playername;
    private bool isReady = false;

    /// <summary>
    /// constructor to save the player name
    /// </summary>
    /// <param name="name"> The name of the player </param>
    public PartyPlayer(string name)
    {
        this.Playername = name;
    }

    public string Playername { get => playername; set => playername = value; }
    public bool IsReady { get => isReady; set => isReady = value; }
}
