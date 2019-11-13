using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string userN;
    public int userScore;
    public string localId;

    private void Awake()
    {
        userN = Databasemanagment_Register.playerName;
       
        localId = Databasemanagment_Register.localId;
    }

    private void Start()
    {
        userN = Databasemanagment_Register.playerName;
       
        localId = Databasemanagment_Register.localId;
    }

    public UserInfo() 
    {
        userN = Databasemanagment_Register.playerName;
       
        localId = Databasemanagment_Register.localId;
    }
}
