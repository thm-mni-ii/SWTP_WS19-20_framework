using System;
using UnityEngine;
using Mirror;
using System.Text;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;



public class Chat : MonoBehaviour
{
 	    //Input
	public InputField clientMessageTF = null;
	public Text content = null;
	
    Telepathy.Client client = new Telepathy.Client();
	
	public int clientport= 7777;
	public string mainServerip = "localhost";
	public string userName = "User";
	private UserInfo Cuser;
	private bool firstConnect = true;
	
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
    public void Disconnection()
    {
        Cuser = null;
        userName = null;
        content.text = "";
        client.Disconnect();
    }


    public void clientSendMessage(){
		if(clientMessageTF.text != null){
			MessageStruct Smsg = new MessageStruct();
		   Smsg.senderName = userName;
			Smsg.Text = clientMessageTF.text;
			Smsg.messagetype = 2;
			clientMessageTF.text = string.Empty;
			byte[] bytes = ObjectToByteArray(Smsg);
			client.Send(bytes);	
		}
	}
	
	public void HandleData(Byte[] data){
	MessageStruct Smsg = ByteArrayToObject(data);		
		switch(Smsg.messagetype){
		case 1: //login reqeust result
		
		break;
		
		case 2: //message recieved
		UpdateChat(Smsg.Text,Smsg.senderName);
		break;
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