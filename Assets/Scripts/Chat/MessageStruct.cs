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
    public string reciever = null;
    /* Type
     1 - login
     2 - chat message
     3 - Private Message
     */

   public MessageStruct(String sender,String text , int typ, string rec)
    {
        this.senderName = sender;
        this.Text = text;
        this.messagetype = typ;
        this.reciever = rec;
    }

}