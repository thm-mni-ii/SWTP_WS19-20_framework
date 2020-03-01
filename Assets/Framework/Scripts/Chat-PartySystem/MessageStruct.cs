using System;

[Serializable]

/**
 * MessageStruct for transformation messages (include message information)
 * Message types:
 * 0 - Client id is sent from server
 * 1 - connection started with this message the user id and data will be sent to server and saved in a list
 * 2 - chat message
 * 3 - Private Message
 * 4 - Host request
 * 5 - update list from server
 * 6 - join a party
 * 7 - party is canceled
 * 8 - join party failed
 */
public class MessageStruct 
{
    /// <summary>
    /// sender username
    /// </summary>
    public string senderName = null;
    /// <summary>
    /// message
    /// </summary>
    public string Text = null;
    /// <summary>
    ///  Message types are:
    /// 0 - Client id is sent from server
    /// 1 - connection started with this message the user id and data will be sent to server and saved in a list
    /// 2 - chat message
    /// 3 - Private Message
    /// 4 - Host request
    /// 5 - update list from server
    /// 6 - join a party
    /// 7 - party is canceled
    /// 8 - join party failed
    /// </summary>
    public int messagetype = 0;
    /// <summary>
    /// Massage reciever
    /// </summary>
    public string reciever = null;
    /// <summary>
    /// Sender id to know, who sent the Massage
    /// </summary>
    public int senderId = 0;
    
    /// <summary>
    ///Min number of players when hosting a party 
    /// </summary>
    public int min=0;
    /// <summary>
    ///Max number of players when hosting a party 
    /// </summary>
    public int max=0;

    MessageStruct() { }

    /// <summary>
    /// MessageStruct for transformation messages (include message information)
    /// Message types:
    /// 0 - Client id is sent from server
    /// 1 - connection started with this message the user id and data will be sent to server and saved in a list
    /// 2 - chat message
    /// 3 - Private Message
    /// 4 - Host request
    /// 5 - update list from server
    /// 6 - join a party
    /// 7 - party is canceled
    /// 8 - join party failed
    /// </summary>
    /// <param name="sender">sender username</param>
    /// <param name="text">message</param>
    /// <param name="typ">Message type</param>
    /// <param name="rec">Massage reciever</param>
   public MessageStruct(String sender,String text , int typ, string rec)
    {
        this.senderName = sender;
        this.Text = text;
        this.messagetype = typ;
        this.reciever = rec;
    }
}