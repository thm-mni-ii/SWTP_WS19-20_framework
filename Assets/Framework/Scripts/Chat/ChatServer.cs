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
using System.Net.Sockets;


public class ChatServer : MonoBehaviour
{

    public Telepathy.Server server = new Telepathy.Server();
	public int port= 7777;
    public bool firststart = true;

    private LinkedList<int> clienList = new LinkedList<int>();
    Dictionary<int, string> userList = new Dictionary<int, string>(); // user connection ID  and info



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
                        server.Send(msg.connectionId, ObjectToByteArray(new MessageStruct("server", msg.connectionId.ToString(), 0,null)));
                        break;
                    case Telepathy.EventType.Data:
                        Debug.Log(msg.connectionId + " Data: " + BitConverter.ToString(msg.data));
                        HandleMessage(msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log(msg.connectionId + " Disconnected");
                        clienList.Remove(msg.connectionId);
                        userList.Remove(msg.connectionId);
                        break;
                }
               
      
            }
        }
    }


    void OnApplicationQuit()
    {
        server.Stop();
    }

    class ClientToken
    {
        public TcpClient client;

        public ClientToken(TcpClient client)
        {
            this.client = client;
        }
    }

    void HandleMessage(Byte[] data){
	MessageStruct Smsg = ByteArrayToObject(data);

        switch (Smsg.messagetype) {
            case 1://user information after connection

                // when id is found add it to the list on our server with the User Information
                int id = Int32.Parse(Smsg.Text);
                userList.Add(id, Smsg.senderName);
                Debug.Log("Added user " + Smsg.senderName + " id: " + id);
                 


                break;
		
		
		case 2:// Global message
		Debug.Log("Message from : "+ Smsg.senderName);
		SendToAll(data);
		break;
        case 3:// Private Message
                if(userList.Values.Contains(Smsg.reciever)){

                    int to = userList.FirstOrDefault(x => x.Value == Smsg.reciever).Key;
                    
                    server.Send(to, ObjectToByteArray(new MessageStruct(Smsg.senderName, Smsg.Text, 3, Smsg.reciever)));
                }
                else
                {
                    server.Send(Smsg.senderId, ObjectToByteArray(new MessageStruct("Server:", "User unknown/offline", 2, null)));
                }
                
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
