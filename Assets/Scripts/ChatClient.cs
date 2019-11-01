using System;
using UnityEngine;
using Mirror;
using System.Text;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class ChatClient : MonoBehaviour
{
    Telepathy.Client client = new Telepathy.Client();
	public int clientport= 7777;
	public string ip = "localhost";
	private String clientmsg = null;
	private String chatmsg = null;
	public int MaxMessages = 15;
	private int msgnum = 0;
	public string userName = "User";
	
	

    void Awake()
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
            //if (Input.GetKeyDown(KeyCode.Space))
              //  client.Send(new byte[]{0x1});

            // show all new messages
            Telepathy.Message msg;
            while (client.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Debug.Log("Client Connected on using ip: "+ ip);
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

    void OnGUI()
    {
        // client
		
		GUI.enabled = !client.Connected;
		if (GUI.Button(new Rect(0, 25, 120, 20), "LAN Client"))
		{
			client.Connect(ip, clientport);
		}
		ip = GUI.TextField(new Rect(260, 25, 120, 20),ip);

        

        GUI.enabled = client.Connected;
        if (GUI.Button(new Rect(130, 25, 120, 20), "Disconnect Client"))
            client.Disconnect();
		
		GUI.Box(new Rect(270, 100, 300, 300), chatmsg);
		clientmsg = GUI.TextField(new Rect(270, 410, 200, 20),clientmsg);
		if (GUI.Button(new Rect(470, 410, 100, 20), "send")){
			
			MessageStruct Smsg = new MessageStruct();
				   Smsg.senderName = userName;
					Smsg.Text = clientmsg;
					Smsg.messagetype = 2;
			
			byte[] bytes = ObjectToByteArray(Smsg);
			client.Send(bytes);	
		}

        GUI.enabled = true;
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
	chatmsg += "\n" + name + ": " + text;
	msgnum++;
	if(msgnum >= MaxMessages)
		chatmsg = chatmsg.Substring(chatmsg.IndexOf('\n') + 1);
		
	}

    void OnApplicationQuit()
    {
        client.Disconnect();
    }
	
 
}