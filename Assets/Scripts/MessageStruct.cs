using Mirror;
using System;

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

}