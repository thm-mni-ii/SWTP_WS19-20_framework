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
	public InputField clientMessageTF = null;
	public Text content = null;
	
    Telepathy.Client client = new Telepathy.Client();
    public int clientport= 7777;
	public string mainServerip = "localhost";
	public string userName = "User";
	private UserInfo Cuser;
	private bool firstConnect = true;
    public Transform Listcontent;
    public UIServerStatusSlot slotPrefab;

    Dictionary<string, Game> list = new Dictionary<string, Game>();

    void awake() {
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
                UpdateServerList();
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
			//MessageStruct Smsg = new MessageStruct(userName, clientMessageTF.text,2,null,null);
			byte[] bytes = ObjectToByteArray(new MessageStruct(userName, clientMessageTF.text, 2, null, null));
            clientMessageTF.text = string.Empty;
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

		case 3://Updated server List from Main server
			this.list = Smsg.list;
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
	// instantiate/remove enough prefabs to match amount
    public static void BalancePrefabs(GameObject prefab, int amount, Transform parent)
    {
        // instantiate until amount
        for (int i = parent.childCount; i < amount; ++i)
        {
            Instantiate(prefab, parent, false);
        }

        // delete everything that's too much
        // (backwards loop because Destroy changes childCount)
        for (int i = parent.childCount - 1; i >= amount; --i)
            Destroy(parent.GetChild(i).gameObject);
    }
    void UpdateServerList()
    {
        // instantiate/destroy enough slots
        BalancePrefabs(slotPrefab.gameObject, list.Count, Listcontent);

        // refresh all members
        for (int i = 0; i < list.Values.Count; ++i)
        {
            UIServerStatusSlot slot = Listcontent.GetChild(i).GetComponent<UIServerStatusSlot>();
            Game server = list.Values.ToList()[i];
            slot.titleText.text = server.title;
            slot.playersText.text = server.players + "/" + server.capacity;
            slot.latencyText.text = server.lastLatency != -1 ? server.lastLatency.ToString() : "...";
            slot.addressText.text = server.ip;
            slot.joinButton.interactable = true;
            slot.joinButton.gameObject.SetActive(server.players < server.capacity);
            // slot.joinButton.onClick.
        }
    }
}