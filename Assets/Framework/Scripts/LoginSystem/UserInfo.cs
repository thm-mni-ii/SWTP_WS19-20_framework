using UnityEngine;
using System;

[Serializable]
/**
 * Data struct of user information
 */
public class UserInfo : MonoBehaviour
{
    /// <summary>
    /// The username of the player. There are unique usernames.
    /// </summary>
    public string username;
    /// <summary>
    /// Player id (unique)
    /// </summary>
    public string id;
    /// <summary>
    /// Email address of player. There are unique emails.
    /// </summary>
    public string email;
    
    /// <summary>
    /// User construct to create a new user. 
    /// </summary>
    /// <param name="username">The username of the player</param>
    /// <param name="id">Player id</param>
    /// <param name="email">Email address of player</param>
    public UserInfo(string username, string id, string email)
    {
        this.username = username;
        this.id = id;
        this.email = email;
    }
}
