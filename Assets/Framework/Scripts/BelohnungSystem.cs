using System;
using UnityEngine;

/**
 * *hier kommt noch was*
 */
public class BelohnungSystem : MonoBehaviour
{
    /// <summary>
    /// score of player in whole of the play
    /// </summary>
    public int score;
    /// <summary>
    /// Name of player, who have the score
    /// </summary>
    public String username;
    /// <summary>
    /// which lever have the player
    /// </summary>
    public int level;
    
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// Take the score of the player from database
    /// </summary>
    void takeScoreOfPlayer()
    {
        //database instructions ...
    }
    
    /// <summary>
    /// put new score of the player from database
    /// </summary>
    void putNewScore()
    {
        //database instructions ...
    }

    void updateLevel(int score)
    {
        if (score < 3 && score >= 0)
        {
            level = 1;
        } else if (score < 6 && score >= 3)
        {
            level = 2;
        } else if (score < 9 && score >= 6)
        {
            level = 3;
        } else if (score < 12 && score >= 9)
        {
            level = 4;
        } else if (score < 15 && score >= 12)
        {
            level = 5;
        } else if (score < 18 && score >= 15)
        {
            level = 6;
        } else if (score < 21 && score >= 18)
        {
            level = 7;
        } else if (score < 27 && score >= 24)
        {
            level = 8;
        } else if (score < 30 && score >= 27)
        {
            level = 9;
        } else if (score < 33 && score >= 30)
        {
            level = 10;
        } else if (score < 3 && score >= 0)
        {
            level = 11;
        }
        // Other Things ...
    }
}
