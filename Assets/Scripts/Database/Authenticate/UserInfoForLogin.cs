using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class UserInfoForLogin
{
    public string userN;
    public int userScore;
    public string localId;

    public UserInfoForLogin() 
    {
        userN = Databasemanagment_Register.playerName;
        localId = Databasemanagment_Register.localId;
    }
}
