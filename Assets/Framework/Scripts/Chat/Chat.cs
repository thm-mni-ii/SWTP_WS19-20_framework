using System;
using UnityEngine;
using Mirror;
using System.Text;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Net;



public class Chat : MonoBehaviour
{
 	//Input varaibles
	public InputField clientMessageTF = null;
	public Text content = null;
    //party vaiables
    public InputField partyTextField = null;
    public Text PartycontentField = null;

    /* client variables */
    Telepathy.Client client = new Telepathy.Client();
	
	public int clientport= 7777;
	public string mainServerip = "localhost";
	public string userName = "ILLEGAL USER";
	private UserInfo Cuser;
	private bool firstConnect = true;
    private int clientId = 0;
    private bool isHost = false;
    private bool inParty = false;
    private string partyhostname = "";



    void awake()
		{
			
		// update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
		
		}

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
                        Debug.Log("Client Connected on using ip: "+ mainServerip);
                        break;
                    case Telepathy.EventType.Data:
                        Debug.Log("Data: " + BitConverter.ToString(msg.data));
						HandleData(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log("Disconnected");
                        break;
                }
            }
        }
    }
    /*Connect user to the global chat (called after login)*/
		public void EstablishConnection(UserInfo user)
	{
		Cuser = user;
		userName = Cuser.userN;

        if (firstConnect)
        {
			if(mainServerip != null && (mainServerip != "") && (userName != "") && userName != null){
			client.Connect(mainServerip, clientport);
            }
		}
	}


   public void leaveParty()
    {
        if (isHost && inParty)
        {

            //inform server that host left to close party

            client.Send(ObjectToByteArray(new MessageStruct(userName, null, 7, null)));

            isHost = false;
            inParty = false;

        }else if (inParty)
        { //inform serer that client has left
            if (partyTextField.text != null && partyTextField.text != "")
            {
                MessageStruct Smsg = new MessageStruct(userName, null, 8, partyTextField.text);
                Smsg.senderId = clientId;
                client.Send(ObjectToByteArray(Smsg));//change
            }
            //inParty = false;
        }
    }
    public void Disconnection()
    {
        Cuser = null;
        userName = null;
        content.text = "";
        leaveParty();
    client.Disconnect();
    }

    /* send a message to all clients or a private message*/
    public void clientSendMessage(){
		if(clientMessageTF.text != null){

            string[] tokens = clientMessageTF.text.Split(new char[] { ':' },2);
            int lenth = 0;
            foreach(string t in tokens){
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
	
	public void HandleData(Byte[] data){
	MessageStruct Smsg = ByteArrayToObject(data);		
		switch(Smsg.messagetype){
            case 0:
                clientId = Int32.Parse(Smsg.Text);
                client.Send(ObjectToByteArray(new MessageStruct(userName, Smsg.Text, 1, null)));
                break;
            case 1: //only for server should never be used here
		
		break;
		
		case 2: //message recieved
		UpdateChat(Smsg.Text,Smsg.senderName);
		break;

         case 3:// Private Message
                UpdateChat(Smsg.Text, "[Private]"+Smsg.senderName+":");

                break;
            case 4:// Host a party

                CreatePartyButton();

         break;
            case 5:// updated list from server
                string[] names = Smsg.Text.Split(new char[] { ';' });
                //Debug.Log(Smsg.Text + "\n");
                PartycontentField.text = "";
                for (int i=0; i< names.Length - 1; i++) { //might cause errors
                    UpdateChatP(names[i], "[Member]"+ i+" ");
                }
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
        }
	}



    /* Host a party */
    public void CreatePartyButton() {
        if (inParty && isHost)
        {
            PartycontentField.text += "\n you are already in a party please leave in order to create a new one";
            return;
        }
        MessageStruct Smsg = new MessageStruct(userName, null, 4, null);
        Smsg.senderId = clientId;
        byte[] bytes = ObjectToByteArray(Smsg);
        client.Send(bytes);

        inParty = true;
        isHost = true;

    }
    /*Join an existing Party*/
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
        }else
        {
            PartycontentField.text = "Enter the Host name";
        }


    }

    // Convert an object to a byte array
    public byte[] ObjectToByteArray(MessageStruct obj)
	{
		BinaryFormatter bf = new BinaryFormatter();
		using (var ms = new MemoryStream())
		{
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}
	}
	
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

	void UpdateChat(String text,String name){
	content.text += "\n" + name + ": " + text;
	}
    void UpdateChatP(String text, String name)
    {
        PartycontentField.text += "\n" + name + ": " + text;
    }

    void OnApplicationQuit()
    {
        content.text = "";
        client.Disconnect();
    }
	
	    public void ValueChanged()
    {
        if (clientMessageTF.text.Contains("\n"))
        {
			clientSendMessage();
        }
    }

}