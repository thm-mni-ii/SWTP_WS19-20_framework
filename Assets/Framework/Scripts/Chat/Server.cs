using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/**
 * MainServer configuration
 */
public class Server : MonoBehaviour
{

    /// <summary>
    /// make a new Telepathy.Server (responsible for chat)
    /// </summary>
    public Telepathy.Server server = new Telepathy.Server();

    /// <summary>
    /// set port of chat server
    /// </summary>
    public int port = 7777;

    /// <summary>
    /// Temp Variable (We don't need it anymore)
    /// </summary>
    public bool firststart = true;

    /// <summary>
    /// Client list: Iteration the list of clients
    /// </summary>
    private LinkedList<int> clienList = new LinkedList<int>();

    /// <summary>
    /// user connection ID  and his information (Using in database)
    /// </summary>
    Dictionary<int, string> userList = new Dictionary<int, string>();

    /// <summary>
    /// party list to save all partys
    /// </summary>
    Dictionary<string, party> partyList = new Dictionary<string, party>();


    /**
     * PartyPlayer class to manage users before a game starts /*hier kommt noch was*
     */
    public class PartyPlayer
    {
        public string playername;
        public bool isReady = false;
        public PartyPlayer(string name)
        {
            this.playername = name;
        }
    }

    /**
     * make a new party and save the names of the players in the list of players
     */
    public class party
    {
        /// <summary>
        /// hostname of party
        /// </summary>
        public string hostname;
        /*hier kommt noch was*/
        public bool gameStarted = false;
        /*hier kommt noch was*/
        public uint playersReady = 0;
        public string gameType;    /*hier kommt noch was*/

        /// <summary>
        /// players id and names, which are in the party
        /// </summary>
        public Dictionary<int, PartyPlayer> playersList = new Dictionary<int, PartyPlayer>();

        /**
         * add a new player the list of players in the party
         */
        public void addPlayer(int con, PartyPlayer player)
        {
            playersList.Add(con, player);
        }

        /**
        * remove a player from the list of players in the party
        */
        public void removPlayer(int con)
        {
            playersList.Remove(con);
        }

        /**
         * make a new party and save the names of the players in the list of players
         */
        public party(string hostname, string ptype)
        {
            this.hostname = hostname;
            this.gameType = ptype;
        }

        public void PlayerReady(int con)    /*hier kommt noch was*/
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

        public bool allPlayersReady()    /*hier kommt noch was*/
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

    void Awake()    /*hier kommt noch was*/
    {
        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }

    /**
     * Update is called once per frame
     * receive messages from clients
     * There are many types of messages:
     * 1. Connected: add client to the client list
     * 2. Data: send message to clients
     * 3. Disconnected: remove client from client list
     */
    void Update()
    {

        // server
        if (server.Active)
        {
            // if (Input.GetKeyDown(KeyCode.Space)){
            //   server.Send(1, new byte[]{0x2});
            //}

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
                     //   Debug.Log(msg.connectionId + " Data: " + BitConverter.ToString(msg.data));
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


    /**
     * stop server
     */
    void OnApplicationQuit()
    {
        server.Stop();
    }
    /*hier kommt noch was*/
    class ClientToken
    {
        public TcpClient client;

        public ClientToken(TcpClient client)
        {
            this.client = client;
        }
    }

    /**
     * handle the data, send to the server
     * /*hier kommt noch was*
     * Types of data are:
     * case 1: user information after connection
     * case 2: Global message
     * case 3: Private Message
     * case 4: handle a host party request
     * case 5: only for client should never be used here
     * case 7: cancel party request (sent from host)
     * case 8: player left a party
     */
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

    /**
     * Update the Party-List only to the Party Members
     * /*hier kommt noch was*
     */
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


    /**
     * /*hier kommt noch was*
     * Update the Host list for all players on the StartGame Menu
     */
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

    /**
     * send a message to all players
     */
    void SendToAll(Byte[] data)
    {
        if (clienList.Count > 0)
        {
            foreach (int i in clienList)
                server.Send(i, data);
        }
    }

    /**
     * Convert an object to a byte array
     */
    public byte[] ObjectToByteArray(MessageStruct obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    /**
     * Convert a byte array to an object
     */
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
}
