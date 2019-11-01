
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Mirror;
using System.Text;

public class ServerConfiguration : MonoBehaviour
{
    //add Server
    Telepathy.Server server = new Telepathy.Server();
    public int port = 7777;

    //Text Field
    private ServerChatTextField serverChatTextField;

    //List of clients
    private LinkedList<int> clienList = new LinkedList<int>();

    private List<Button> _uiButtons = new List<Button>();

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
        if (Input.GetKeyDown(KeyCode.S))
            startServer();
        else if (Input.GetKeyDown(KeyCode.C))
        {
            stopServer();
        }

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
                        serverChatTextField.appendText(msg.connectionId + " Connected");
                        clienList.AddLast(msg.connectionId);
                        break;
                    case Telepathy.EventType.Data:
                        Debug.Log(msg.connectionId + " Data: " + BitConverter.ToString(msg.data));
                        serverChatTextField.appendText(msg.connectionId + " Data: " + BitConverter.ToString(msg.data));
                        SendToAll(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log(msg.connectionId + " Disconnected");
                        serverChatTextField.appendText(msg.connectionId + " Disconnected");
                        clienList.Remove(msg.connectionId);
                        break;
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        server.Stop();
    }

    public void startServer()
    {
        server.Start(port);
    }

    public void stopServer()
    {
        server.Stop();
    }

    public void SendToAll(Byte[] data)
    {
        if (clienList.Count > 0)
        {
            foreach (int i in clienList)
                server.Send(i, data);
        }
    }
}
