﻿using System;
using UnityEngine;
using Mirror;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Net;

public class ChatServer : MonoBehaviour
{

    public Telepathy.Server server = new Telepathy.Server();
	public int port= 7777;
    public bool firststart = true;

    private LinkedList<int> clienList = new LinkedList<int>();

    //Dictionary<int, UserInfo> userLisr = new Dictionary<int, UserInfo>(); // user connection ID  and info



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
                        HandleMessage(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log(msg.connectionId + " Disconnected");
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
	
	void HandleMessage(Byte[] data){
	MessageStruct Smsg = ByteArrayToObject(data);
		
		switch(Smsg.messagetype){
		case 1://login request

		break;
		
		
		case 2:// Global message
		Debug.Log("Message from : "+ Smsg.senderName);
		SendToAll(data);
		break;
        case 3:// Private Message
        
        break;
            
			default:
                Debug.Log("msg Error unknown command");
				break;
        }
		
	}
	
	void SendToAll(Byte[] data){
		if(clienList.Count>0){
		foreach(int i in clienList)
		server.Send(i,data);
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



}