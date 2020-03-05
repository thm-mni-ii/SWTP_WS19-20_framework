using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Server class
///This class contains all the methods and variables for the handling of client requests on the main server
/// </summary>
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
    /// Party Map to store information about every current Party that is running on the server
    /// </summary>
    Dictionary<string, Party> partyList = new Dictionary<string, Party>();
    

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
                        clienList.AddLast(msg.connectionId);
                        server.Send(msg.connectionId, ObjectToByteArray(new MessageStruct("server", msg.connectionId.ToString(), 0, null)));
                        break;
                    case Telepathy.EventType.Data:
                        HandleMessage(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
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
    /// Handle the Data, sent to the Server from the Clients
    /// Each number represents a Request made from the Clients
    /// This Method recieves the Data and handles it based on the Request type 
    /// Types of Client Requests are:
    /// case 1: user information after connection
    /// case 2: Global message
    /// case 3: Private Message
    /// case 4: handle a host Party request
    /// case 5: only for client should never be used here
    /// case 6: join Party request
    /// case 7: cancel Party request (sent from host)
    /// case 8: player left a Party
    /// case 9: player is ready
    /// case 10: Update Partylist for Client
    /// case 11: Start game Request, sent from Host
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
                UpdateHostList();// update Hostslist for client Uponconnection
                break;
            case 2:    // Global message
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
            case 4:    //handle a host Party request
                partyList.Add(Smsg.senderName, new Party(Smsg.senderName, Smsg.Text,Smsg.reciever));
                Party temp = partyList[Smsg.senderName];
                temp.Maxplayers = Smsg.max;
                temp.Minplayers = Smsg.min;
                temp.addPlayer(Smsg.senderId, new PartyPlayer(Smsg.senderName));
                UpdateList(temp);
                UpdateHostList();
                break;
            case 5:    // only for client should never be used here
                break;
            case 6://join Party request
                if (!partyList.ContainsKey(Smsg.reciever))
                {
                    server.Send(Smsg.senderId, ObjectToByteArray(new MessageStruct("server: Host not found", null, 8, null)));
                    return;
                }
                Party temp2 = partyList[Smsg.reciever];
                if(!temp2.addPlayer(Smsg.senderId, new PartyPlayer(Smsg.senderName))){
                    server.Send(Smsg.senderId, ObjectToByteArray(new MessageStruct("server: The Party is full", null, 8, null)));
                    return;
                }
                UpdateList(temp2);
                UpdateHostList();
                break;
            case 7:    //cancel Party request (sent from host)
                Party temp3 = partyList[Smsg.senderName];
                //inform clients that host has disconnected and delete Party
                foreach (var entry in temp3.playersList)
                {
                    server.Send(entry.Key, ObjectToByteArray(new MessageStruct("server: Host has Disconnected", null, 7, null)));
                }
                partyList.Remove(Smsg.senderName);
                UpdateHostList();
                break;
            case 8:    // player left a Party
                Party temp4 = partyList[Smsg.reciever];
                temp4.removPlayer(Smsg.senderId);
                UpdateList(temp4);
                //clear list for player
                server.Send(Smsg.senderId, ObjectToByteArray(new MessageStruct("You left the Party", null, 7, null)));
                UpdateHostList();
                break;
            case 9://ready
                Party temp5 = partyList[Smsg.reciever];
                temp5.PlayerReady(Smsg.senderId);
                UpdateList(temp5);
                break;
            case 10:// Update Partylist for Client
                UpdateHostListforOneClient(Smsg.senderId);
                break;
            case 11://Start game Request, sent from Host
                Party GameSelected = partyList[Smsg.senderName];
                if (!GameSelected.checkPlayerNumber())
                {
                    //start failed
                    foreach (var entry in GameSelected.playersList)
                    {
                        server.Send(entry.Key, ObjectToByteArray(new MessageStruct("server: Player count must be between "+ GameSelected.Maxplayers+ " and " + GameSelected.Minplayers, null, 11, null)));
                    }

                }
                else if (GameSelected.allPlayersReady())
                {
                    //startgame
                    foreach (var entry in GameSelected.playersList)
                    {
                        server.Send(entry.Key, ObjectToByteArray(new MessageStruct("server", GameSelected.GameType, 10, null)));
                    }
                }
                else
                {
                    //start failed
                    foreach (var entry in GameSelected.playersList)
                    {
                        server.Send(entry.Key, ObjectToByteArray(new MessageStruct("server: Cannot start game until all players are ready", null, 11, null)));
                    }
                }
                break;
            default:

                break;
        }
    }

    /// <summary>
    /// Update the Party-List only to the Party Members
    /// A list of all player names and thier ready state is sent to all clients in the Party
    /// if the player is ready the Text will be Green else the Text will be Red
    /// </summary>
    /// <param name="temp">object from type Party it contains all information about the Party to which the player belongs</param>
    void UpdateList(Party temp)
    {
        String names = "";
        foreach (var entry in temp.playersList)
        {
            if (entry.Value.Playername != "")
                names += entry.Value.Playername + ":" + entry.Value.IsReady+ ";";
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
            hostlist += entry.Value.Module + ":" + entry.Key + ":" + entry.Value.playersList.Count + ":" + entry.Value.GameType + ";";
        }
        Byte[] data = ObjectToByteArray(new MessageStruct("server", hostlist, 9, null));

        SendToAll(data);
    }

    public void UpdateHostListforOneClient(int id)
    {
        string hostlist = "";
        foreach (var entry in partyList)
        {
            hostlist += entry.Value.Module + ":" + entry.Key + ":" + entry.Value.playersList.Count + ":" + entry.Value.GameType + ";";
        }
        Byte[] data = ObjectToByteArray(new MessageStruct("server", hostlist, 9, null));

        server.Send(id, data);
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
}
