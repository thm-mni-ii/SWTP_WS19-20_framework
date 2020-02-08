using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/**
 * Server class
 * This class contains all the methods and variables for the handling of client requests on the main server
 */
public class Server : MonoBehaviour
{
    /// <summary>
    /// Server variable of type Telepathy
    /// </summary>
    public Telepathy.Server server = new Telepathy.Server();
    /// <summary>
    /// set port of chat server
    /// </summary>
    public int port = 7777;
    /// <summary>
    /// This list contains all the client ids
    /// </summary>
    private LinkedList<int> clienList = new LinkedList<int>();
    /// <summary>
    /// Map to store user connection ID and username
    /// </summary>
    Dictionary<int, string> userList = new Dictionary<int, string>();
    /// <summary>
    /// party Map to store information about every current party that is running on the server
    /// </summary>
    Dictionary<string, party> partyList = new Dictionary<string, party>();
    
    /// <summary>
    /// PartyPlayer class to manage users before a game starts
    /// it keeps track of the player name and his ready state
    /// </summary>
    public class PartyPlayer
    {
        public string playername;
        public bool isReady = false;

        /// <summary>
        /// constructor to save the player name
        /// </summary>
        /// <param name="name"> The name of the player </param>
        public PartyPlayer(string name)
        {
            this.playername = name;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()  
    {
        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }
    
    /// <summary>
    /// Update is called once per frame
    /// receive messages from clients
    /// There are many types of messages:
    /// 1. Connected: add client to the client list
    /// 2. Data: send message to clients
    /// 3. Disconnected: remove client from client list
    /// </summary>
    void Update()
    {
        // server
        if (server.Active)
        {
            // show all new messages
            Telepathy.Message msg;
            while (server.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Debug.Log(msg.connectionId + " Connected");
                        clienList.AddLast(msg.connectionId);
                        server.Send(msg.connectionId, ObjectToByteArray(new MessageStruct("server", msg.connectionId.ToString(), 0, null)));
                        break;
                    case Telepathy.EventType.Data:
                        //Debug.Log(msg.connectionId + " Data: " + BitConverter.ToString(msg.data));
                        HandleMessage(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log(msg.connectionId + " Disconnected");
                        clienList.Remove(msg.connectionId);
                        userList.Remove(msg.connectionId);
                        break;
                }
            }
        }
    }
    
    /// <summary>
    /// stop server
    /// </summary>
    void OnApplicationQuit()
    {
        server.Stop();
    }

    /// <summary>
    /// handle the data, sent to the serve from the clients
    /// Each number represents a Request made from the clients
    /// This Method recieves the data and handles it based on the Request type 
    /// Types of Requests are:
    /// case 1: user information after connection
    /// case 2: Global message
    /// case 3: Private Message
    /// case 4: handle a host party request
    /// case 5: only for client should never be used here
    /// case 6: join party request
    /// case 7: cancel party request (sent from host)
    /// case 8: player left a party
    /// case 9: player is ready
    /// </summary>
    /// <param name="data"> Byte data recieved from the server (Telepathy.EventType.Data) </param>
    void HandleMessage(Byte[] data)
    {
        MessageStruct Smsg = ByteArrayToObject(data);
        switch (Smsg.messagetype)
        {
            case 1:    //user information after connection
                       // when id is found add it to the list on our server with the User Information
                int id = Int32.Parse(Smsg.Text);
                userList.Add(id, Smsg.senderName);
                Debug.Log("Added user " + Smsg.senderName + " id: " + id);
                UpdateHostList();// update Hostslist for client Uponconnection
                break;
            case 2:    // Global message
                Debug.Log("Message from : " + Smsg.senderName);
                SendToAll(data);
                break;
            case 3:    // Private Message
                if (userList.Values.Contains(Smsg.reciever))
                {
                    int to = userList.FirstOrDefault(x => x.Value == Smsg.reciever).Key;
                    server.Send(to, ObjectToByteArray(new MessageStruct(Smsg.senderName, Smsg.Text, 3, Smsg.reciever)));
                }
                else
                {
                    server.Send(Smsg.senderId, ObjectToByteArray(new MessageStruct("Server:", "User unknown/offline", 2, null)));
                }
                break;
            case 4:    //handle a host party request
                partyList.Add(Smsg.senderName, new party(Smsg.senderName, Smsg.Text));
                party temp = partyList[Smsg.senderName];
                temp.addPlayer(Smsg.senderId, new PartyPlayer(Smsg.senderName));
                UpdateList(temp);
                UpdateHostList();
                break;
            case 5:    // only for client should never be used here
                break;
            case 6://join party request
                if (!partyList.ContainsKey(Smsg.reciever))
                {
                    server.Send(Smsg.senderId, ObjectToByteArray(new MessageStruct("server: Host not found", null, 8, null)));
                    return;
                }
                party temp2 = partyList[Smsg.reciever];
                temp2.addPlayer(Smsg.senderId, new PartyPlayer(Smsg.senderName));
                UpdateList(temp2);
                UpdateHostList();
                break;
            case 7:    //cancel party request (sent from host)
                party temp3 = partyList[Smsg.senderName];
                //inform clients that host has disconnected and delete party
                foreach (var entry in temp3.playersList)
                {
                    server.Send(entry.Key, ObjectToByteArray(new MessageStruct("server: Host has Disconnected", null, 7, null)));
                }
                partyList.Remove(Smsg.senderName);
                UpdateHostList();
                break;
            case 8:    // player left a party
                party temp4 = partyList[Smsg.reciever];
                temp4.removPlayer(Smsg.senderId);
                UpdateList(temp4);
                //clear list for player
                server.Send(Smsg.senderId, ObjectToByteArray(new MessageStruct("You left the Party", null, 7, null)));
                UpdateHostList();
                break;
            case 9://ready
                party temp5 = partyList[Smsg.reciever];
                temp5.PlayerReady(Smsg.senderId);
                UpdateList(temp5);
                break;
            default:
                Debug.Log("msg Error unknown command");
                break;
        }
    }

    /// <summary>
    /// Update the Party-List only to the Party Members
    /// A list of all player names and thier ready state is sent to all clients in the party
    /// if the player is ready the Text will be Green else the Text will be Red
    /// </summary>
    /// <param name="temp">object from type party it contains all information about the party to which the player belongs</param>
    void UpdateList(party temp)
    {
        String names = "";
        foreach (var entry in temp.playersList)
        {
            if (entry.Value.playername != "")
                if (entry.Value.isReady)
                {
                    names += "<color=#00ff00ff>" + entry.Value.playername + "</color>;";//Color Green if player is ready
                }
                else
                {
                    names += "<color=#ff0000ff>" + entry.Value.playername + "</color>;";//Color Red if player is not ready
                }
        }
        Byte[] data = ObjectToByteArray(new MessageStruct("server", names, 5, null));
        foreach (var entry in temp.playersList)
        {
            server.Send(entry.Key, data);
        }
    }
    
    /// <summary>
    /// Update the Host list for all players on the StartGame Menu
    /// A list of all host name on the server is sent to all game players
    /// on the client this list will be rendered on the Game Hosts Field and it will be filtered on the Partycontent Field 
    /// depending on which Magic Circle the player is standing on
    /// </summary>
    public void UpdateHostList()
    {
        string hostlist = "";
        foreach (var entry in partyList)
        {
            hostlist += entry.Value.gameType + ";" + entry.Key + ";" + entry.Value.playersList.Count + ";";
        }
        Byte[] data = ObjectToByteArray(new MessageStruct("server", hostlist, 9, null));

        SendToAll(data);
    }

    /// <summary>
    /// send a message to all players
    /// </summary>
    /// <param name="data">Byte data Array</param>
    void SendToAll(Byte[] data)
    {
        if (clienList.Count > 0)
        {
            foreach (int i in clienList)
                server.Send(i, data);
        }
    }

    /// <summary>
    /// Convert an object to a byte array
    /// </summary>
    /// <param name="obj"> An object from type MessageStruct wich will be sent to the server </param>
    /// <returns> A byte array - This is the Data that will be sent to the server </returns>
    public byte[] ObjectToByteArray(MessageStruct obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Convert a byte array to an object
    /// </summary>
    /// <param name="arrBytes"> Byte array -  data sent from the server </param>
    /// <returns> The original Object format (MessageStruct) </returns>
    public MessageStruct ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return (MessageStruct)obj;
        }
    }
    
    /// <summary>
    /// Party class
    /// This class contains all the information and methods to manage a party
    /// </summary>
    public class party
    {
        /// <summary>
        /// hostname of party
        /// </summary>
        public string hostname;
        /// <summary>
        /// This variable is used to determine if the party is in a game or not
        /// With the help of this variable the game will be started for all party members
        /// The value is set to true of all party players are ready and then the game starts
        /// when the game ends the value is set back to false
        /// </summary>
        public bool gameStarted = false;
        /// <summary>
        /// Number of ready players in the party
        /// This variable is used to keep track of how many player are ready
        /// once all players are ready the game can be started
        /// </summary>
        public uint playersReady = 0;
        /// <summary>
        /// Game type { OOP, NTG, MATHE ...}
        /// It is used to set the Game type
        /// this variable is set once the Party is Hosted and can not be changed
        /// </summary>
        public string gameType;  
        /// <summary>
        /// Map of player ids and names, which are currently in the party
        /// </summary>
        public Dictionary<int, PartyPlayer> playersList = new Dictionary<int, PartyPlayer>();

        /// <summary>
        /// add a new player the party
        /// </summary>
        /// <param name="con">Connection id (client id/number on server)</param>
        /// <param name="player">PartyPlayer Object contains information about the player in the party</param>
        public void addPlayer(int con, PartyPlayer player)
        {
            playersList.Add(con, player);
        }

        /// <summary>
        /// remove a player from the party
        /// </summary>
        /// <param name="con">Connection id (client id/number on server)</param>
        public void removPlayer(int con)
        {
            playersList.Remove(con);
        }

        /// <summary>
        /// constructor
        /// Create a new party with hostname and game type
        /// The parameters are recieved from the client
        /// </summary>
        /// <param name="hostname">Hostname</param>
        /// <param name="ptype">Game Type</param>
        public party(string hostname, string ptype)
        {
            this.hostname = hostname;
            this.gameType = ptype;
        }

        /// <summary>
        /// check of the player is ready or not
        /// </summary>
        /// <param name="con">connection id (client id/number on the server)</param>
        public void PlayerReady(int con)
        {
            if (!playersList[con].isReady)
            {
                playersList[con].isReady = true;
                playersReady++;
            }
            else
            {
                playersList[con].isReady = false;
                playersReady--;
            }
        }
        
        /// <summary>
        /// Check if all party players are ready
        /// The return value determines if the party can be started or not
        /// True game can be started
        /// False game canno't be started
        /// </summary>
        /// <returns> True if all players are ready, False if atleast one player is not ready</returns>
        public bool allPlayersReady() 
        {
            if (playersReady == playersList.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
