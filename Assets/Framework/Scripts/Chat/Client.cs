using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/**
 *  Chat Class to set Chat client configuration
 */
public class Client : MonoBehaviour
{
    /// <summary>
    /// logout Button to sign out from the game
    /// </summary>
    public Button logout = null;
    /// <summary>
    /// auxiliary variable to know if the logout button has been hidden
    /// </summary>
    public bool hideLogoutButton = false;
    /// <summary>
    /// Take message text from client
    /// </summary>
    public InputField clientMessageTF = null;

    /// <summary>
    /// show received messages 
    /// </summary>
    public Text content = null;

    /// <summary>
    /// party variable /*hier kommt noch was*/
    /// </summary>
    public InputField partyTextField = null;

    /// <summary>
    /// Title    /*hier kommt noch was*/
    /// </summary>
    public Text startgameTitle = null;

    /// <summary>
    /// party variable    /*hier kommt noch was*/
    /// </summary>
    public Text PartycontentField = null;

    /// <summary>
    /// GameHosts Text Field    /*hier kommt noch was*/
    /// </summary>
    public Text GameHostsField = null;

    /// <summary>
    /// make a new Telepathy.Client (responsible for chat)
    /// </summary>
    Telepathy.Client client = new Telepathy.Client();

    /// <summary>
    /// set port of chat client
    /// </summary>
    public int clientport = 7777;

    /// <summary>
    /// Server ip address
    /// </summary>
    public string mainServerip = "localhost";
    public string userName = "ILLEGAL USER";

    /// <summary>
    /// save data information in data struct Cuser
    /// </summary>
    private UserInfo Cuser;

    /// <summary>
    /// auxiliary variable to make connection between client and server
    /// </summary>
    private bool firstConnect = true;

    /// <summary>
    /// The id of client
    /// It is used for easier communication with the server
    /// </summary>
    private int clientId = 0;

    /// <summary>
    /// Party variable
    /// If the client is a party host then isHost = true
    /// </summary>
    private bool isHost = false;

    /// <summary>
    /// party variable
    /// If the player is in a party then inParty = true
    /// </summary>
    private bool inParty = false;

    /// <summary>
    /// party variable
    /// used to save the name of the party host when a player joins a party
    /// </summary>
    private string partyhostname = "";

    /// <summary>
    /// The game type that the player is playing
    /// /*hier kommt noch was*/
    /// It is changed from the class PlayerMovement upon Trigger
    /// </summary>
    private string gameType = null;

    /*
     *  Get methode of the gameType 
     */

    public string getgameType()    /*hier kommt noch was*/
    {
        return this.gameType;
    }

    /*
*  Set methode of the gameType 
*/
    public void setgameType(string type)    /*hier kommt noch was*/
    {
        this.gameType = type;
    }

    /// <summary>
    /// The scrollRect of the Chat UI 
    /// It is used to allow auto-scrolling
    /// </summary>
    public ScrollRect ChatSR;    /*hier kommt noch was*/
   // public GameObject scrollView;

    void awake()    /*hier kommt noch was*/
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
     * receive messages from server
     * There are many types of messages:
     * 1. Connected
     * 2. Data: receive message from server
     * 3. Disconnected
     *
     * and hide/show logout button by pressed on ESC
     */
    void Update()
    {
        // client
        if (client.Connected)
        {
            // show all new messages
            Telepathy.Message msg;
            while (client.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Debug.Log("Client Connected on using ip: " + mainServerip);
                        break;
                    case Telepathy.EventType.Data:
                       // Debug.Log("Data: " + BitConverter.ToString(msg.data));
                        HandleData(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log("Disconnected");
                        break;
                }
            }
        }
        
        // hide/show logout button by pressed on ESC
        if (Input.GetKey(KeyCode.Escape))
        {
            if (hideLogoutButton)
            {
                logout.gameObject.SetActive(true);
                hideLogoutButton = false;
            }
            else
            {
                logout.gameObject.SetActive(false);
                hideLogoutButton = true;
            }
        }
    }

    /**
     * Connect user (client) with server to open global chat (called after login)
     * The methode is used from the login methdode 
     * UserInfo contains user iformation after login, it is then gived to the client to start the connection with the server
     */
    public void EstablishConnection(UserInfo user)
    {
        Cuser = user;
        userName = Cuser.userN;

        if (firstConnect)
        {
            if (mainServerip != null && (mainServerip != "") && (userName != "") && userName != null)
            {
                client.Connect(mainServerip, clientport);
            }
        }
    }

    /**
     * Send a leave party request to the server 
     * if the player is the host then the party is cancel and all party players are disconnected 
     */
    public void leaveParty()
    {
        if (isHost && inParty)
        {

            //inform server that host left to close party
            client.Send(ObjectToByteArray(new MessageStruct(userName, null, 7, null)));

            isHost = false;
            inParty = false;

        }
        else if (inParty)
        {         //inform serer that client has left
            if (partyTextField.text != null && partyTextField.text != "")
            {
                MessageStruct Smsg = new MessageStruct(userName, null, 8, partyTextField.text);
                Smsg.senderId = clientId;
                client.Send(ObjectToByteArray(Smsg));//change
            }
            //inParty = false;
        }
    }

    /**
     * disconnect the client socket
     */
    public void Disconnection()
    {
        Cuser = null;
        userName = null;
        content.text = "";
        leaveParty();
        client.Disconnect();
    }

    /**
     * send a message to all clients or a private message
     * this methode is used to send messages on the global chat
     */
    public void clientSendMessage()    /*hier kommt noch was*/
    {
        if (clientMessageTF.text != null)
        {

            string[] tokens = clientMessageTF.text.Split(new char[] { ':' }, 2);
            int lenth = 0;

            foreach (string t in tokens)
            {
                lenth++;
            }

            if (lenth > 1)
            {
                MessageStruct Smsg = new MessageStruct(userName, tokens[1], 3, tokens[0]);
                Smsg.senderId = clientId;
                byte[] bytes = ObjectToByteArray(Smsg);

                clientMessageTF.text = string.Empty;
                client.Send(bytes);
            }
            else
            {

                byte[] bytes = ObjectToByteArray(new MessageStruct(userName, clientMessageTF.text, 2, null));
                clientMessageTF.text = string.Empty;
                client.Send(bytes);
            }
        }
    }

    /**
     * handle the data, send from the server
     * Typs of data are:
     * case 1: data are only for server should never be used here
     * case 2: message recieved
     * case 3: Private Message for special client
     * case 4: Host a party to create a new party system
     * case 5: updated list from server
     * case 6: join a party
     * case 7: party canceled
     * case 8: join failed 
     */
    public void HandleData(Byte[] data)
    {
        MessageStruct Smsg = ByteArrayToObject(data);
        switch (Smsg.messagetype)
        {
            case 0:
                clientId = Int32.Parse(Smsg.Text);
                client.Send(ObjectToByteArray(new MessageStruct(userName, Smsg.Text, 1, null)));
                break;
            case 1: //only for server should never be used here

                break;

            case 2: //message recieved
                UpdateChat(Smsg.Text, Smsg.senderName);
                break;

            case 3:// Private Message
                UpdateChat(Smsg.Text, "[Private]" + Smsg.senderName + ":");
                break;
            case 4:// Host a party
                CreatePartyButton();
                break;
            case 5:// updated list from server
                string[] names = Smsg.Text.Split(new char[] { ';' });

                RenderPartyList(names);

                break;
            case 6://join a party
                if (partyTextField.text != null && partyTextField.text != "")
                {
                    JoinPartyButton();
                }
                break;
            case 7://party canceled
                PartycontentField.text = Smsg.senderName;
                inParty = false;
                break;
            case 8://join failed
                PartycontentField.text = Smsg.senderName;
                inParty = false;
                break;
            case 9://update host list
                string[] hlist = Smsg.Text.Split(new char[] { ';' });

                RenderHosts(hlist);
                break;
        }
    }

    /**
     * update the StartGame UI variables according to the game Module
     */
    public void updateStartGameUI()
    {
        startgameTitle.text = "Welcome to " + gameType + " Module";

    }

    /**
     * Ready Player
     */
    public void ReadyButton()    /*hier kommt noch was*/
    {
        if (!inParty)
        {
            PartycontentField.text += "\n you are Not in Party";
            return;
        }

        MessageStruct Smsg = new MessageStruct(userName, null, 9, partyhostname);
        Smsg.senderId = clientId;
        byte[] bytes = ObjectToByteArray(Smsg);
        client.Send(bytes);
    }
    
     /**
     * Start the Game by the Host
     */
    public void StartGame()
    {
        if (!isHost)
        {
            //report error here "Only the a Host can Start a Game"
            return;
        }
    }
    
    /**
     * Host a party    /*hier kommt noch was*
     */
    public void CreatePartyButton()
    {
        if (inParty && isHost)
        {
            PartycontentField.text += "\n you are already in a party please leave in order to create a new one";
            return;
        }
        MessageStruct Smsg = new MessageStruct(userName, gameType, 4, null);
        Smsg.senderId = clientId;
        byte[] bytes = ObjectToByteArray(Smsg);
        client.Send(bytes);
        partyhostname = userName;
        inParty = true;
        isHost = true;
    }

    /**
     * Join an existing Party    /*hier kommt noch was*
     */
    public void JoinPartyButton()
    {
        if (inParty)
        {
            PartycontentField.text += "\n you are already in a party please leave in order to join a new one";
            return;
        }

        if (partyTextField.text != null && partyTextField.text != "")
        {
            MessageStruct Smsg = new MessageStruct(userName, null, 6, partyTextField.text);
            Smsg.senderId = clientId;
            client.Send(ObjectToByteArray(Smsg));
            partyhostname = partyTextField.text;
            inParty = true;
        }
        else
        {
            PartycontentField.text = "Enter the Host name";
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

    /**
     * to update the new incoming messages
     */
    void UpdateChat(String text, String name)
    {
        content.text += "\n" + name + ": " + text;
        ChatSR.verticalNormalizedPosition = 0f;
    }

    /**
     * to update the party list    /*hier kommt noch was*
     */
    void RenderPartyList(String[] text)
    {
        PartycontentField.text = "";
        for (int i = 0; i < text.Length - 1; i++)
        {
            PartycontentField.text += "\n" + "Player[" + i + 1 + "]:" + text[i];
        }
    }

    /**
    * to update the Hostlist    /*hier kommt noch was*
    */
    void RenderHosts(String[] text)
    {
        GameHostsField.text = "\n Type     Host     Players";

        for (int i = 0; i+3 < text.Length; i += 3)
        {
            GameHostsField.text += "\n" + text[i] + "     " + text[i + 1] + "     " + text[i + 2];
        }
    }

    /**
     * Disconnect client
     */
    void OnApplicationQuit()
    {
        content.text = "";
        client.Disconnect();
    }

    /**
     * client send a message by Value change
     */
    public void ValueChanged()
    {
        if (clientMessageTF.text.Contains("\n"))
        {
            clientSendMessage();
        }
    }
}