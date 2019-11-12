using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User1
{
    public string userN;
    public int userScore;
    public string localId;

    public User1() 
    {
        userN = Databasemanagment.playerName;
       
        localId = Databasemanagment.localId;
    }
}
