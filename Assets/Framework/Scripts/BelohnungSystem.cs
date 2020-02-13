using System;
using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine.UI;
using Object = System.Object;

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
    /// Create Dictionary of users and scores
    /// </summary>
    public Dictionary<string, int> usersScores = new Dictionary<string, int>();
    /// <summary>
    /// List of players. it use to find all players, who have any Information in the database
    /// </summary>
    public List<string> playerList = new List<string>();
    /// <summary>
    /// 
    /// </summary>
    public Transform entryContainer;
    /// <summary>
    /// 
    /// </summary>
    public Transform entryTemplate;
    /// <summary>
    /// 
    /// </summary>
    private List<Transform> highscoreEntryTransformList;

    public bool isTableValChanged = true;
    
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
    /// Awake is called when the script instance is being loaded
    /// </summary>
     private void Awake() {
         entryTemplate.gameObject.SetActive(false);
         updateUsersScores();
     }
     
     /// <summary>
     /// Update is called once per frame
     /// </summary>
     void Update()
     {
         if (isTableValChanged)
         {
             updateTable();
             isTableValChanged = false;
         }
         //takeScoreOfPlayer();
     }

     public void updateTable()
     {
         string jsonString;
         Highscores highscores;
         PlayerPrefs.DeleteKey("highscoreTable");
         foreach (KeyValuePair<string, int> userInMap in usersScores)
         {
             AddHighscoreEntry(userInMap.Value, userInMap.Key);
             //Debug.Log("KEY: " + userInMap.Key + "   VALUE: " + userInMap.Value);
         }
         Debug.Log("Initializing table with default values...");
         
         /*AddHighscoreEntry(100043000, "CdMK");
         AddHighscoreEntry(89764321, "JOfsE");
         AddHighscoreEntry(87294331, "DAfsV");
         AddHighscoreEntry(78543123, "CfsAT");
         AddHighscoreEntry(54243024, "MAfsX");
         AddHighscoreEntry(6824345, "AAfsA");
         AddHighscoreEntry(1000000, "CMK");
         AddHighscoreEntry(897621, "JOE");
         AddHighscoreEntry(872931, "DAV");
         AddHighscoreEntry(785123, "CAT");
         AddHighscoreEntry(542024, "MAX");
         AddHighscoreEntry(68245, "AAA");
         AddHighscoreEntry(1000064300, "CM6K");
         AddHighscoreEntry(897456621, "J6OE");
         AddHighscoreEntry(87264931, "D4AV");
         AddHighscoreEntry(786435123, "CA6T");
         AddHighscoreEntry(546432024, "MA4X");
         AddHighscoreEntry(6864245, "AA6A");*/

         // Reload
         jsonString = PlayerPrefs.GetString("highscoreTable");
         highscores = JsonUtility.FromJson<Highscores>(jsonString);

         //// Sort entry list by Score
        /*for (int i = 0; i < highscores.highscoreEntryList.Count; i++) {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++) {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score) {
                    // Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }*/

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList) {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
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
            //database (Firebase) save data instruction ...
            login.reference.Child("users").Child(userInfo.username).Child("score").SetValueAsync(newScore);
        }
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
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            // Do something with the data in args.Snapshot
            // Loop over items in collection of users.
            foreach (KeyValuePair<string, Object> users in (Dictionary<string, Object>) args.Snapshot.Value)
            {
                string tempScore = null; //Save a temporary score for later storage in the list
                //Debug.Log("KEY: " + users.Key + "   VALUE: " + users.Value);
                foreach (KeyValuePair<string, Object> userInformation in (Dictionary<string, Object>) users.Value)
                {
                    //take score
                    //be careful, it may be that the username is taken from the database (Firebase) before the score
                    if (userInformation.Key.Equals("score"))
                    {
                        tempScore = userInformation.Value.ToString();
                    }
                    //Debug.Log("KEY: " + userInformation.Key + "   VALUE: " + userInformation.Value);
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
                            // Add data to the Dictionary<string, int> usersScores
                            usersScores.Add((string) userInformation.Value, Convert.ToInt32(tempScore));
                            playerList.Add((string) userInformation.Value);
                        }
                    }
                }
            }
            
            //Debug: Check if every thing okay in the Dictionary
            foreach (KeyValuePair<string, int> userInMap in usersScores)
            {
                Debug.Log("KEY: " + userInMap.Key + "   VALUE: " + userInMap.Value);
            }
        }
       
        // Add some data.
        // usersScores.Add("pearl", 100);

        // Get value that exists.
        // int value1 = usersScores["diamond"];
        // Console.WriteLine("get DIAMOND: " + value1);

        // Get value that does not exist.
        // usersScores.TryGetValue("coal", out int value2);
        // Console.WriteLine("get COAL: " + value2);
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
    
     private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) {
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

        int score = highscoreEntry.score;

        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;
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
        //switch (rank) {
        //default:
        //    entryTransform.Find("trophy").gameObject.SetActive(false);
        //    break;
        //case 1:
        //    entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("FFD200");
        //    break;
        //case 2:
        //    entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("C6C6C6");
        //    break;
        //case 3:
        //    entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("B76F56");
        //    break;

        //}

        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string name) {
        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };
        
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null) {
            // There'scrollRect no stored table, initialize
            highscores = new Highscores() {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }

    /*
     * Represents a single High score entry
     * */
    [System.Serializable] 
    private class HighscoreEntry {
        public int score;
        public string name;
    }
}
