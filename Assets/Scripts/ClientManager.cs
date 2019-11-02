using System;
using UnityEngine;
using Mirror;
using System.Text;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;



public class ClientManager : MonoBehaviour
{
	
	    //Input
	public InputField ipAddressTF = null;
	public InputField portNumberTF = null;
    public InputField usernameTF = null;
    public InputField passwordTF = null;
	public InputField clientMessageTF = null;
	private bool firstConnect = true;
	public GameObject Canvas2 = null;
	public GameObject Canvas1 = null;
	public Text content = null;
	
    Telepathy.Client client = new Telepathy.Client();
	public int clientport= 7777;
	public string ip = "localhost";
	public string userName = "User";
	private bool hideCanvas2 = true;
	
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
        }else if(hideCanvas2){
					Canvas2.SetActive(false);
				hideCanvas2=false;
		}else if(!client.Connected && !firstConnect){
			Canvas1.SetActive(true);
			Canvas2.SetActive(false);
			firstConnect = true;
		}

    }

		public void EstablishConnection()
	{
		ip = ipAddressTF.text;
		userName = usernameTF.text;

		if (firstConnect)
        {
			if(ip != null && (ip != "") && (userName != "") && userName != null){
			client.Connect(ip, clientport);
				for(int i =0;i<50000;i++){} // Waiting loop
            
			if(client.Connected){
			GameObject.Find("Canvas").SetActive(false);
			Canvas2.SetActive(true);
			firstConnect = false;
			}else {
				
				Debug.Log("Connection Failed");
			}
			}else {
			Debug.Log("client ERROR ip/username is null");
			}
        }
		//Debug.Log("client.connect is " + client.Connected);
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