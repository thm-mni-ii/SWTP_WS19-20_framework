using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Table : MonoBehaviour
{

    /* Variablen u. Methoden für die Tabelle */
    /// <summary>
    /// Create Dictionary of hosts and players
    /// </summary>
    public Dictionary<string, int> partyMap = new Dictionary<string, int>();
    /// <summary>
    /// Create Dictionary of hosts and games
    /// </summary>
    public Dictionary<string, string> gameMap = new Dictionary<string, string>();
    /// <summary>
    /// List of players. it use to find all players, who host a party
    /// </summary>
    public List<string> hostsList = new List<string>();
    /// <summary>
    /// (Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
    /// </summary>
    public Transform entryContainer;
    /// <summary>
    /// (Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
    /// </summary>
    public Transform entryTemplate;
    /// <summary>
    /// (Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
    /// </summary>
    private List<Transform> EntryTransformList;

    public void updateTable(String[] text)
    {
        Debug.Log("Initializing table Hostlist...");
        //(Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
        EntryTransformList = new List<Transform>();
        foreach (string Element in text)
        {
            CreateHighscoreEntryTransform(Element, entryContainer, EntryTransformList);
        }
    }

    /// <summary>
    /// (Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
    /// </summary>
    /// <param name="host"></param>
    /// <param name="container"></param>
    /// <param name="transformList"></param>
    private void CreateHighscoreEntryTransform(string host, Transform container, List<Transform> transformList)
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        /*string rankString;
        switch (rank) {
        default:
            rankString = rank + "TH"; break;

        case 1: rankString = "1ST"; break;
        case 2: rankString = "2ND"; break;
        case 3: rankString = "3RD"; break;
        }*/
        string game = gameMap[host];
        entryTransform.Find("GameText").GetComponent<Text>().text = game;
        entryTransform.Find("HostText").GetComponent<Text>().text = host;
        int players = partyMap[host];
        entryTransform.Find("PlayersText").GetComponent<Text>().text = players.ToString();
        // Set background visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        // Highlight First
        //  if (rank == 1) {
        entryTransform.Find("GameText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("HostText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("PlayersText").GetComponent<Text>().color = Color.green;
        //  }
        transformList.Add(entryTransform);
    }

}
