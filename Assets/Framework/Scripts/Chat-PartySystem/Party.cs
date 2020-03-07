using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Party class
/// This class contains all the information and methods to manage a Party
/// </summary>
public class Party
{
    /// <summary>
    /// hostname of Party
    /// </summary>
    private string hostname;
    /// <summary>
    /// This variable is used to determine if the Party is in a game or not
    /// With the help of this variable the game will be started for all Party members
    /// The value is set to true of all Party players are ready and then the game starts
    /// when the game ends the value is set back to false
    /// </summary>
    private bool gameStarted = false;
    /// <summary>
    /// Number of ready players in the Party
    /// This variable is used to keep track of how many player are ready
    /// once all players are ready the game can be started
    /// </summary>
    private uint playersReady = 0;
    /// <summary>
    /// module { OOP, NTG, MATHE ...}
    /// It is used to set the module Type
    /// this variable is set once the Party is Hosted and can not be changed
    /// </summary>
    private string module;
    /// <summary>
    /// Game Type
    /// Game types include { Dixit }
    /// </summary>
    private string gameType;
    /// <summary>
    /// Map of player ids and names, which are currently in the Party
    /// </summary>
    public Dictionary<int, PartyPlayer> playersList = new Dictionary<int, PartyPlayer>();

    /// <summary>
    /// Maximum number of players allowed in the party
    /// </summary>
    private int maxplayers;
    /// <summary>
    /// Minimal number of players allowed in the party
    /// </summary>
    private int minplayers;

    public bool GameStarted { get => gameStarted; set => gameStarted = value; }
    public string Hostname { get => hostname; set => hostname = value; }
    public uint PlayersReady { get => playersReady; set => playersReady = value; }
    public string GameType { get => gameType; set => gameType = value; }
    public string Module { get => module; set => module = value; }
    public int Maxplayers { get => maxplayers; set => maxplayers = value; }
    public int Minplayers { get => minplayers; set => minplayers = value; }

    /// <summary>
    /// add a new player the Party
    /// return false if there is no space in the party
    /// return true if player added succesfully
    /// </summary>
    /// <param name="con">Connection id (client id/number on server)</param>
    /// <param name="player">PartyPlayer Object contains information about the player in the Party</param>
    public bool addPlayer(int con, PartyPlayer player)
    {
        if (playersList.Count >= Maxplayers)
            return false;
        else
        {
            playersList.Add(con, player);
        }
        return true;
    }

    /// <summary>
    /// Check if the player count is within the allowed range
    /// </summary>
    public bool checkPlayerNumber()
    {
        if (playersList.Count > Maxplayers || playersList.Count < Minplayers)
            return false;
        else
            return true;
    }

    /// <summary>
    /// remove a player from the Party
    /// </summary>
    /// <param name="con">Connection id (client id/number on server)</param>
    public void removPlayer(int con)
    {
        playersList.Remove(con);
    }

    /// <summary>
    /// constructor
    /// Create a new Party with Hostname, module and gameType
    /// The parameters are recieved from the client
    /// </summary>
    /// <param name="hostname">Hostname</param>
    /// <param name="mtype">module Type</param>
    /// <param name="gtype">Game Type</param>
    public Party(string hostname, string mtype, string gType)
    {
        this.Hostname = hostname;
        this.Module = mtype;
        this.GameType = gType;
    }

    /// <summary>
    /// check of the player is ready or not
    /// </summary>
    /// <param name="con">connection id (client id/number on the server)</param>
    public void PlayerReady(int con)
    {
        if (!playersList[con].IsReady)
        {
            playersList[con].IsReady = true;
            PlayersReady++;
        }
        else
        {
            playersList[con].IsReady = false;
            PlayersReady--;
        }
    }

    /// <summary>
    /// Check if all Party players are ready
    /// The return value determines if the Party can be started or not
    /// True game can be started
    /// False game canno't be started
    /// </summary>
    /// <returns> True if all players are ready, False if atleast one player is not ready</returns>
    public bool allPlayersReady()
    {
        if (PlayersReady == playersList.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}