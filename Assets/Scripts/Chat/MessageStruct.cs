using Mirror;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class MessageStruct 
{
    public string senderName = null;
    public string Text = null;
	public int messagetype = 0;
    /* Type
     1 - login
     2 - chat message
     3 - start game
     */
    public Game gameinfo = null;
    public Dictionary<string, Game> list = null;

   public MessageStruct(String sender,String text , int typ, Dictionary<string, Game> list,Game info)
    {
        this.senderName = sender;
        this.Text = text;
        this.messagetype = typ;
        this.list = list;
        this.gameinfo = info;
    }
}