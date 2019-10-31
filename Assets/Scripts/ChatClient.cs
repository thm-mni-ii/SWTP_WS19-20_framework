using System;
using UnityEngine;
using Mirror;
using System.Text;
using UnityEngine.UI;


public class ChatClient : MonoBehaviour
{
    Telepathy.Client client = new Telepathy.Client();
	public int clientport= 7777;
	public string ip = "localhost";
	private String clientmsg = null;
	private String chatmsg = null;
	public int MaxMessages = 15;
	private int msgnum = 0;
	
	

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
						UpdateChat(msg.data);
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
		
		GUI.Box(new Rect(270, 100, 300, 400), chatmsg);
		clientmsg = GUI.TextField(new Rect(130, 410, 120, 20),clientmsg);
		if (GUI.Button(new Rect(300, 410, 120, 20), "send")){
			
			byte[] bytes = Encoding.ASCII.GetBytes(clientmsg);
			client.Send(bytes);
			
		}


        GUI.enabled = true;
    }
	
	void UpdateChat(Byte[] data){
	chatmsg += "\n" + BitConverter.ToString(data);
	msgnum++;
	if(msgnum >= MaxMessages)
		chatmsg = chatmsg.Substring(chatmsg.IndexOf('\n') + 1);
		
	}

    void OnApplicationQuit()
    {
        client.Disconnect();
    }
	
 
}