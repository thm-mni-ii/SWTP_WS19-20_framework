using UnityEngine;
using Firebase.Database;

/**
 * The reward system is a method of realizing the levels based on the score.
 */
public class BelohnungSystem : MonoBehaviour
{
    /// <summary>
    /// which level have the player
    /// </summary>
    public int level;
    /// <summary>
    /// manage the whole game. It used to take information from the game objects (like username of player)
    /// </summary>
    private GlobalManager globalCanvas;
    /// <summary>
    /// data struct of user information. The struct include: username, email, id, score
    /// </summary>
    private UserInfo userInfo;
    /// <summary>
    /// instance of the class Login to access on the database
    /// </summary>
    private Login login;
    
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        //setup canvases and game objekts
        globalCanvas = gameObject.GetComponent<GlobalManager>();
        userInfo = globalCanvas.GetComponent<UserInfo>();
        login = globalCanvas.GetComponent<Login>();
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        takeScoreOfPlayer();
        Debug.Log("Level: " + level);
    }

    /// <summary>
    /// Read Data from Database.
    /// Take the score of the player from database.
    /// </summary>
    public void takeScoreOfPlayer()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + userInfo.username + "/score")
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("score of player " + userInfo.username +" not found");
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    userInfo.score = (int) snapshot.Value;
                    updateLevel(userInfo.score);
                }
            });
    }
    
    /// <summary>
    /// put new score of the player on database
    /// </summary>
    /// <param name="newScore">new score, which the player have</param>
    void putNewScore(int newScore)
    {
        if (userInfo.username != null || userInfo.username != "")
        {
            //database (Firebase) save data instructions ...
            login.reference.Child("users").Child(userInfo.username).Child("score").SetValueAsync(newScore);
        }
    }

    /// <summary>
    /// Update the player's level according to his score
    /// </summary>
    /// <param name="score">score of player</param>
    void updateLevel(int score)
    {
        if (score < 5 && score > 0)
        {
            level = 1;
        } else if (score < 11 && score >= 5)
        {
            level = 2;
        } else if (score < 19 && score >= 11)
        {
            level = 3;
        } else if (score < 30 && score >= 19)
        {
            level = 4;
        } else if (score < 35 && score >= 30)
        {
            level = 5;
        } else if (score < 50 && score >= 35)
        {
            level = 6;
        } else if (score < 70 && score >= 50)
        {
            level = 7;
        } else if (score < 95 && score >= 70)
        {
            level = 8;
        } else if (score < 123 && score >= 95)
        {
            level = 9;
        } else if (score < 155 && score >= 123)
        {
            level = 10;
        } else if (score < 190 && score >= 155)
        {
            level = 11;
        } else if (score < 190 && score >= 155)
        {
            level = 12;
        } else if (score < 250 && score >= 190)
        {
            level = 13;
        } else if (score < 350 && score >= 250)
        {
            level = 14;
        } else if (score >= 350)
        {
            level = 15;
        }
    }
}
