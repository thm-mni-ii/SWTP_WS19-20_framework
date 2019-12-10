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
    public string userN;
    public string Uid;
    public string email;

   public  UserInfo() { }

   /**
    * data struct of user information
    */
    public UserInfo(string userN, string Uid, string email)
    {
        this.userN = userN;
        this.Uid = Uid;
        this.email = email;
    }
}
