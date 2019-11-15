using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserInfo : MonoBehaviour
{

	public string userN;
    public string Uid;
    public string email;

   public  UserInfo() { }

    public UserInfo(string userN, string Uid, string email)
    {
        this.userN = userN;
        this.Uid = Uid;
        this.email = email;

    }

}
