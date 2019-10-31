using System;
using UnityEngine;
using Mirror;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChatServer : MonoBehaviour
{

    Telepathy.Server server = new Telepathy.Server();
	public int port= 7777;
	private LinkedList<int> clienList = new LinkedList<int>();
	//public int MaxMessages = 15;
	
	

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
                        break;
                    case Telepathy.EventType.Data:
                        Debug.Log(msg.connectionId + " Data: " + BitConverter.ToString(msg.data));
						SendToAll(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log(msg.connectionId + " Disconnected");
						clienList.Remove(msg.connectionId);
                        break;
                }
            }
        }
    }

    void OnGUI()
    {

        // server
        GUI.enabled = !server.Active;
        if (GUI.Button(new Rect(0, 50, 120, 20), "Start Server"))
            server.Start(port);

        GUI.enabled = server.Active;
        if (GUI.Button(new Rect(130, 50, 120, 20), "Stop Server"))
            server.Stop();

        GUI.enabled = true;
    }

    void OnApplicationQuit()
    {
        server.Stop();
    }
	void SendToAll(Byte[] data){
		if(clienList.Count>0){
		foreach(int i in clienList)
		server.Send(i,data);
		}
		}
	
}
