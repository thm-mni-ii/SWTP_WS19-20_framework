using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Client Class Contains all Methods and Variables to serve the Client while in the Game.
/// </summary>
public class Client : MonoBehaviour
{
    /// <summary>
    /// Take message text from client
    /// </summary>
    public InputField clientMessageTF = null;

    /// <summary>
    /// show received messages 
    /// </summary>
    public Text content = null;
    
    /// <summary>
    /// A Text Object used for Error Messages
    /// Error Messages are cloned from this Object in the Methode ShowErrorMessage(...)
    /// </summary>
    public Text ErrorTextField = null;

    /// <summary>
    /// The Input Text Field in the Game Canvas where the Host names are Entered
    /// </summary>
    public InputField partyTextField = null;

    /// <summary>
    /// The Title of the Game Canvas it is changed every time a player enters a Magic Circle
    /// </summary>
    public Text startgameTitle = null;

    /// <summary>
    /// make a new Telepathy.Client resposible for the communication with the server
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

    /// <summary>
    /// The name of the user is read from the database and is saved on the client for easier communication with the server
    /// </summary>
    public string userName = "";

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
    /// This variable is used once a player Hosts a game, this value will be sent to the server
    /// to determine which game type is being hosted, also it is used to filter the Hosts list for client on the Game menu
    /// It is changed from the class PlayerMovement upon Trigger
    /// </summary>
    private string gameType = null;

    /// <summary>
    /// The scrollRect of the Chat UI 
    /// It is used to allow auto-scrolling
    /// </summary>
    public ScrollRect ChatSR;

    /// <summary>
    /// Game type Selection like: Dexit..
    /// </summary>
    public Dropdown GameSelect = null;

    /// <summary>
    /// Variable to avoid Reading the Gamelist  multiple times
    /// It is set to true, in the Method setGamesRef.
    /// </summary>
    private bool isGamesRefset = false;


    /// <summary>
    /// List of All Game 
    /// It References the Games list from PlayerMovement Class
    /// Games Contains the name of all Game types that are avaiable like Dexit..
    /// </summary>
    Dictionary<string,Game> Games;

    /* Table Definitions */
    /// <summary>
    /// the table inside the party system
    /// The Text Field where on the GameCanvas where the Hosts or the party players are displayed
    /// </summary>
    public Transform entryContainerInGame;
    /// <summary>
    /// row in the table inside the party system
    /// </summary>
    public Transform entryTemplateInGame;
    /// <summary>
    /// list of table rows (table inside the party system)
    /// </summary>
    private List<Transform> EntryTransformListInGame = new List<Transform>();
    /// <summary>
    /// the table inside lobby
    /// The GameHosts Field is displayed on the upper left corner of the screen
    /// It shows the current Hosts of each game this way the player will know which games are being hosted
    /// </summary>
    public Transform entryContainerHosts;
    /// <summary>
    /// row in the table inside lobby
    /// </summary>
    public Transform entryTemplateHosts;
    /// <summary>
    /// list of table rows (table inside lobby)
    /// </summary>
    private List<Transform> EntryTransformListHosts = new List<Transform>();



    /// <summary>
    /// Method to set the Games Reference from the PlayerMovement Class
    /// It also fills the Game types in the GameSelect Dropdown object
    /// </summary>
    /// <param name="list"></param>
    public void setGamesRef(Dictionary<string, Game> list)
    {
        if(isGamesRefset) { return; }
        this.Games = list;


        foreach (KeyValuePair<string, Game> game in this.Games)
        {

            if (game.Key != "" || game.Key != null)
            {
                Dropdown.OptionData m_NewData = new Dropdown.OptionData();
                m_NewData.text = game.Key;
                GameSelect.options.Add(m_NewData);
            }
        }
        GameSelect.RefreshShownValue();
        isGamesRefset = true;
    }

    /// <summary>
    /// Get methode of the gameType
    /// </summary>
    /// <returns> The game type which is the Modules example: OOP,GDI... </returns>
    public string getgameType()
    {
        return this.gameType;
    }
    
    /// <summary>
    /// Set methode of the gameType
    /// </summary>
    /// <param name="type"> The game type example: OOP,GDI... </param>
    public void setgameType(string type)  
    {
        this.gameType = type;
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        entryTemplateHosts.gameObject.SetActive(false);
        entryTemplateInGame.gameObject.SetActive(false);
        
        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }
    
    /// <summary>
    /// Update is called once per frame
    /// receive messages from server
    /// There are many types of messages:
    /// 1. Connected
    /// 2. Data: receive message from server
    /// 3. Disconnected
    /// and hide/show logout button by pressed on ESC
    /// </summary>
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
                        break;
                    case Telepathy.EventType.Data:
                        HandleData(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        break;
                }
            }
        }
    }
    
    /// <summary>
    /// Connect user (client) with server to open global chat (called after login)
    /// The methode is used from the login methdode
    /// UserInfo contains user iformation after login, it is then gived to the client to start the connection with the server
    /// </summary>
    /// <param name="user"> The user object from UserInfo class it contains all the information from the database </param>
    public void EstablishConnection(UserInfo user)
    {
        Cuser = user;
        userName = Cuser.username;

        if (firstConnect)
        {
            if (mainServerip != null && (mainServerip != "") && (userName != "") && userName != null)
            {
                client.Connect(mainServerip, clientport);
            }
        }
    }

    /// <summary>
    /// Send a leave party request to the server
    /// if the player is the host then the party is cancel and all party players are disconnected 
    /// </summary>
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
        {        
            //inform serer that client has left
            if (partyTextField.text != null && partyTextField.text != "")
            {
                MessageStruct Smsg = new MessageStruct(userName, null, 8, partyTextField.text);
                Smsg.senderId = clientId;
                client.Send(ObjectToByteArray(Smsg));//change
            }
            //inParty = false;
        }
    }

    /// <summary>
    /// disconnect the client socket
    /// </summary>
    public void Disconnection()
    {
        Cuser = null;
        userName = null;
        content.text = "";
        leaveParty();
        client.Disconnect();
    }

    /// <summary>
    /// send a message to all clients or a private message
    /// this methode is used to send messages on the global chat
    /// The Text (from clientMessageTF Field) is sent to the server from the client once the user presses the 'ENTER' button
    /// 
    /// </summary>
    public void clientSendMessage()
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

    /// <summary>
    /// Handle the Data, recieved from the server
    /// Typs of recieved Data are:
    /// case 1: data are only for server should never be used here
    /// case 2: message recieved
    /// case 3: Private Message for special client
    /// case 4: Host a party to create a new party system
    /// case 5: updated list from server
    /// case 6: join a party
    /// case 7: party canceled
    /// case 8: join failed 
    /// case 9: update host list
    /// case 10: start game for client
    /// case 11: failed start game request
    /// </summary>
    /// <param name="data"> Byte data recieved from the server (Telepathy.EventType.Data)  </param>
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
                ShowErrorMessage(Smsg.senderName);
                inParty = false;
                break;
            case 8://join failed
                ShowErrorMessage(Smsg.senderName);
                inParty = false;
                break;
            case 9://update host list
                string[] hlist = Smsg.Text.Split(new char[] { ';' });

                RenderHosts(hlist);
                RenderHostsInGameMenu(hlist);
                break;
            case 10://start game for client
                InstanceStarter.RunFile(Smsg.Text);
                break;
            case 11://failed start game request
                ShowErrorMessage(Smsg.senderName);
                break;
        }
    }

    /// <summary>
    /// update the StartGame UI variables according to the game Module
    /// </summary>
    public void updateStartGameUI()
    {
        startgameTitle.text = "Welcome to " + gameType + " Module";
        updatePartyList();
    }
    
    /// <summary>
    /// Send a request to the server to update the Partylist for the Client only.
    /// This Methode is called when the client stands on a Magic-circle.
    /// </summary>
    public void updatePartyList()
    {
        MessageStruct listReq = new MessageStruct(userName, null, 10, null);
        listReq.senderId = this.clientId;
        client.Send(ObjectToByteArray(listReq));
    }

    /// <summary>
    /// Ready Player
    /// Once the ready button is clicked, a message will be sent to inform the server that the player is ready
    /// </summary>
    public void ReadyButton()
    {
        if (!inParty)
        {
            ShowErrorMessage("you are Not in Party");
            return;
        }

        MessageStruct Smsg = new MessageStruct(userName, null, 9, partyhostname);
        Smsg.senderId = clientId;
        byte[] bytes = ObjectToByteArray(Smsg);
        client.Send(bytes);
    }

     /// <summary>
     /// Start the Game by the Host
     /// </summary>
    public void StartGame()
    {
        if (!isHost)
        {
            ShowErrorMessage("Only a Host can Start the game");
            return;
        }
        else
        {
            MessageStruct Smsg = new MessageStruct(userName, null, 11, null);
            Smsg.senderId = clientId;
            byte[] bytes = ObjectToByteArray(Smsg);
            client.Send(bytes);
        }
    }
     
    /// <summary>
    /// Host a party
    /// when the player is not in a party or is not a Host already then a Host party request will be sent to the server
    /// if succesful the isHost and inParty variables will be set to True
    /// also the server will respond with the new party list, the list will be rendered in another method 
    /// </summary>
    public void CreatePartyButton()
    {
        if (inParty && isHost)
        {
            ShowErrorMessage("You are already in a party please leave in order to create a new one");
            return;
        }
        else if (GameSelect.options[GameSelect.value].text == null || GameSelect.options[GameSelect.value].text == "")
        {
            ShowErrorMessage("please Select a game type");
            return;
        }
        MessageStruct Smsg = new MessageStruct(userName, gameType, 4, GameSelect.options[GameSelect.value].text);
        Smsg.senderId = clientId;
        Smsg.max = this.Games[GameSelect.options[GameSelect.value].text].Maxplayers;
        Smsg.min = this.Games[GameSelect.options[GameSelect.value].text].Minplayers;
        byte[] bytes = ObjectToByteArray(Smsg);
        client.Send(bytes);
        partyhostname = userName;
        inParty = true;
        isHost = true;
    }

    /// <summary>
    /// Join an existing Party
    /// If the Player is not in a party/host a request to create a party will be sent to the server
    /// if succesful the variable inParty will be set to True and the server will respond with the player list of the Party which will be Rendered in another Method
    /// if unsuccesful the player will be informed with a message in the chat
    /// </summary>
    public void JoinPartyButton()
    {
        if (inParty)
        {
            ShowErrorMessage("You are already in a party please leave in order to join a new one");
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
            ShowErrorMessage("Please enter the Hosts name");
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
    ///  Update the new incoming messages from all Players
    ///  The message and the name of the Sender will be displayed in the chat.
    /// </summary>
    /// <param name="text"> string variable - The text message </param>
    /// <param name="name"> string variable - The name of the Sender </param>
    void UpdateChat(String text, String name)
    {
        content.text += "\n" + name + ": " + text;
        ChatSR.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// This methods updates the party list for the player when a new member joins/ leaves the party,
    /// also when a Player hosts a Party his list is automaticly updated.
    /// The List is sent directly from the Server to all Players who are in the Party.
    /// </summary>
    /// <param name="text"> string array - List of the Party players </param>
    void RenderPartyList(String[] text)
    {
        if (!inParty || (getgameType() == null))
        {
            return;
        }
        Table.ClearEntryList(EntryTransformListInGame);
        int i = 1;

        foreach (string player in text)
        {
            if (player != "" && player != null)
            {

                string[] playerStatus = player.Split(new char[] { ':' });
                if (playerStatus[1].Equals("True"))
                {
                    Table.CreateEntryTransform(playerStatus[0], i.ToString(), "Ready",null, entryContainerInGame, entryTemplateInGame, EntryTransformListInGame, Color.green);
                }
                else if (playerStatus[1].Equals("False"))
                {
                    Table.CreateEntryTransform(playerStatus[0], i.ToString(), "Not Ready",null, entryContainerInGame, entryTemplateInGame, EntryTransformListInGame, Color.red);
                }
                i++;
            }
        }
    }

    /// <summary>
    /// This method recieves a List of all hosts fromt the server and displays them in the Host Canvas
    /// The list elements are the Module,Name of the Host, the amount of players in the party and the Gametype.
    /// </summary>
    /// <param name="text"> String array List of Hosts </param>
    void RenderHosts(String[] text)
    {
        Table.ClearEntryList(EntryTransformListHosts);

        foreach (string entry in text)
        {
            if (entry != "" && entry != null)
            {
                string[] tempHostsList = entry.Split(new char[] { ':' });
                Table.CreateEntryTransform(tempHostsList[0], tempHostsList[1], tempHostsList[2], tempHostsList[3], entryContainerHosts, entryTemplateHosts, EntryTransformListHosts, Color.green);
            }
        }
    }

    /// <summary>
    /// This methode recieves Message and diplays the Message on the screen.
    /// The Message cloned from the "ErrorTextField" Object and is displayed for 3 seconds before it is destroyed.
    /// </summary>
    /// <param name="ErrorTextField"> The Error Message as string</param>
    public void ShowErrorMessage(string textMessage)
    {
        Transform ErorrMessage = Instantiate(ErrorTextField.GetComponent<RectTransform>(), ErrorTextField.GetComponent<RectTransform>());
        ErorrMessage.gameObject.GetComponent<Text>().text = textMessage;
        ErorrMessage.gameObject.SetActive(true);
        Destroy(ErorrMessage.gameObject,3); //display time
    }

    /// <summary>
    /// This method recieves a List of all hosts fromt the server and displays them in the Host Canvas
    /// The list elements are the Module,Name of the Host, the amount of players in the party and the Gametype.
    /// </summary>
    /// <param name="text"> String array List of Hosts </param>
    void RenderHostsInGameMenu(String[] text)
    {
        if (isHost || inParty || (getgameType() == null)) {
            return;
        }

        Table.ClearEntryList(EntryTransformListInGame);

        foreach (string entry in text)
        {
            if (entry != "" && entry != null)
            {
                
                string[] tempHostsList = entry.Split(new char[] { ':' });

                if (tempHostsList[0].Equals(getgameType())){ //Filter

                    Table.CreateEntryTransform(tempHostsList[0], tempHostsList[1], tempHostsList[2], tempHostsList[3], entryContainerInGame, entryTemplateInGame, EntryTransformListInGame, Color.green);
                }
            }
        }
    }

    /// <summary>
    /// Disconnect client
    /// </summary>
    void OnApplicationQuit()
    {
        content.text = "";
        client.Disconnect();
    }

    /// <summary>
    /// client send a message by Value change
    /// </summary>
    public void ValueChanged()
    {
        if (clientMessageTF.text.Contains("\n"))
        {
            clientSendMessage();
        }
    }
}