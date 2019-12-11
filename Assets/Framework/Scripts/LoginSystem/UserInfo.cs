using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
/**
 * data struct of user information
 */
public class UserInfo : MonoBehaviour
{
    /// <summary>
    /// Username 
    /// </summary>
    public string userN;
    
    /// <summary>
    /// User id: auxiliary variable for the database server
    /// </summary>
    public string Uid;
    
    /// <summary>
    /// User E-Mail
    /// </summary>
    public string email;

   public  UserInfo() { }

   /**
    * create new user information
    */
    public UserInfo(string userN, string Uid, string email)
    {
        this.userN = userN;
        this.Uid = Uid;
        this.email = email;
    }
}
