using System;
using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine.UI;
using Object = System.Object;

/// <summary>
/// The reward system is a method of realizing the levels based on the score.
/// </summary>
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
    /// Create Dictionary of users and scores
    /// </summary>
    public Dictionary<string, int> usersScores = new Dictionary<string, int>();
    /// <summary>
    /// List of players. it use to find all players, who have any Information in the database
    /// </summary>
    public List<string> playerList = new List<string>();
    /// <summary>
    /// table template
    /// </summary>
    public Transform entryContainer;
    /// <summary>
    /// row in the table
    /// </summary>
    public Transform entryTemplate;
    /// <summary>
    /// list of rows
    /// </summary>
    private List<Transform> highscoreEntryTransformList;
    /// <summary>
    /// if playername exist -> getPlayerName = true
    /// </summary>
    private bool getPlayerName = false;
    /// <summary>
    /// Username of player will be show on the window
    /// </summary>
    [SerializeField] private Text usernameText = null;
    /// <summary>
    /// Score of player will be show on the window
    /// </summary>
    [SerializeField] private Text scoreText = null;
    /// <summary>
    /// level of player will be show on the window
    /// </summary>
    [SerializeField] private Text levelText = null;
    
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        //setup canvases and game objekts
        globalCanvas = gameObject.GetComponent<GlobalManager>();
        userInfo = globalCanvas.GetComponent<UserInfo>();
        login = globalCanvas.GetComponent<Login>();
        updateUsersScores();
    }
    
    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
     private void Awake() {
         entryTemplate.gameObject.SetActive(false);
    }
     
     /// <summary>
     /// Update is called once per frame
     /// </summary>
     void Update()
     {
         if (userInfo.username != "")
         {
             userInfo.score = usersScores[userInfo.username];
             updateLevel(userInfo.score);
             getPlayerName = true;
         }
         if (getPlayerName)
         {
             usernameText.text = userInfo.username;
             scoreText.text = "Score:" + userInfo.score;
             levelText.text = "Level:" + level;
         }
     }

     public void updateTable()
     {
         // Sort entry list by Score
        for (int i = 0; i < playerList.Count; i++) {
            for (int j = i + 1; j < playerList.Count; j++) {
                if (usersScores[playerList[j]] > usersScores[playerList[i]]) {
                    // Swap
                    string tmp = playerList[i];
                    playerList[i] = playerList[j];
                    playerList[j] = tmp;
                }
            }
        }
        //(Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
        highscoreEntryTransformList = new List<Transform>();
        foreach (string playername in playerList) {
            CreateHighscoreEntryTransform(playername, entryContainer, highscoreEntryTransformList);
        }
     }
     
    /// <summary>
    /// (Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
    /// </summary>
    /// <param name="playername"></param>
    /// <param name="container"></param>
    /// <param name="transformList"></param>
     private void CreateHighscoreEntryTransform(string playername, Transform container, List<Transform> transformList) {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank) {
        default:
            rankString = rank + "TH"; break;

        case 1: rankString = "1ST"; break;
        case 2: rankString = "2ND"; break;
        case 3: rankString = "3RD"; break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;
        int score = usersScores[playername];
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();
        string name = playername;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;
        // Set background visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);
        
        // Highlight First
      //  if (rank == 1) {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
      //  }

        // Set tropy
        switch (rank) {
        default:
            entryTransform.Find("trophy").gameObject.SetActive(false);
            break;
        case 1:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(0,0,1,1);
            break;
        case 2:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(0,1,0,1);
            break;
        case 3:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(1,0,0,1);
            break;
        case 4:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(0,1,1,1);
            break;
        case 5:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(1,0,1,1);
            break;
        case 6:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(1,1,0,1);
            break;
        case 7:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(1,1,1,1);
            break;
        case 8:
            entryTransform.Find("trophy").GetComponent<Image>().color = new Color(0,0,0, 1);
            break;
        }
        transformList.Add(entryTransform);
    }
     
    /// <summary>
    /// Scores are updated periodically. The information in the database is retrieved and then stored in the players list.
    /// </summary>
    public void updateUsersScores()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .ValueChanged += HandleValueChanged;

        void HandleValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                return;
            }
            // Do something with the data in args.Snapshot
            // Loop over items in collection of users.
            foreach (KeyValuePair<string, Object> users in (Dictionary<string, Object>) args.Snapshot.Value)
            {
                string tempScore = null; //Save a temporary score for later storage in the list
                foreach (KeyValuePair<string, Object> userInformation in (Dictionary<string, Object>) users.Value)
                {
                    //take score
                    //be careful, it may be that the username is taken from the database (Firebase) before the score
                    if (userInformation.Key.Equals("score"))
                    {
                        tempScore = userInformation.Value.ToString();
                    }

                    if (userInformation.Key.Equals("username"))
                    {
                        bool ifUserExist = false;
                        //add username just if the user not in the list of players.
                        foreach (string username in playerList)
                        {
                            if (username.Equals(userInformation.Value)) ifUserExist = true;
                        }
                        if (!ifUserExist)
                        {
                            // Add data to the Dictionary<string, int> partyMap
                            usersScores.Add((string) userInformation.Value, Convert.ToInt32(tempScore));
                            playerList.Add((string) userInformation.Value);
                        }
                    }
                }
            }
            updateTable();
        }
    }

     /// <summary>
    /// Read Data from Database.
    /// Take the score of the player from database.
    /// </summary>
    public int takeScoreOfPlayer()
     { 
         int score = -1;
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + userInfo.username + "/score")
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    userInfo.score = Convert.ToInt32(snapshot.Value);
                    score = userInfo.score;
                }
            });
        return userInfo.score;
     }
    
    /// <summary>
    /// put new score of the player on database
    /// </summary>
    /// <param name="newScore">new score, which the player have</param>
    void putNewScore(int newScore)
    {
        if (userInfo.username != "")
        {
            //database (Firebase) save data instruction ...
            login.reference.Child("users").Child(userInfo.username).Child("score").SetValueAsync(newScore);
        }
    }

    /// <summary>
    /// Update the player's level according to his score
    /// </summary>
    /// <param name="score">score of player</param>
    public void updateLevel(int score)
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
