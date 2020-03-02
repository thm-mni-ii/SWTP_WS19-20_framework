using System;

/// <summary>
/// MessageStruct Contains the Data when sending a Message
/// </summary>
[Serializable]
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
    /// A Message type is the Variable which determines that use of a Message
    /// each type describes a different request
    /// for more information about message types check the Client/Server classes
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
    /// MessageStruct Constructor
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