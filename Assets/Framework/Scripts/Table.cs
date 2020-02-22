using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Table definition class
/// </summary>
public class Table : MonoBehaviour
{

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

    public void updateTable(String[] text, Transform container, Transform template)
    {
        this.entryContainer = container;
        this.entryTemplate = template;
        Debug.Log("Initializing table Hostlist...");
        //(Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
        EntryTransformList = new List<Transform>();
        foreach (string Element in text)
        {
            CreateEntryTransform(Element, entryContainer, EntryTransformList);
        }
    }

    /// <summary>
    /// (Token from https://unitycodemonkey.com/video.php?v=iAbaqGYdnyI)
    /// </summary>
    /// <param name="host"></param>
    /// <param name="container"></param>
    /// <param name="transformList"></param>
    private void CreateEntryTransform(string host, Transform container, List<Transform> transformList)
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

       // int rank = transformList.Count + 1;

      //  string game = gameMap[host];
        entryTransform.Find("GameText").GetComponent<Text>().text = host;
        entryTransform.Find("HostText").GetComponent<Text>().text = host;
        //int players = partyMap[host];
        entryTransform.Find("PlayersText").GetComponent<Text>().text = host;
        // Set background visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(false);

        entryTransform.Find("GameText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("HostText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("PlayersText").GetComponent<Text>().color = Color.green;

        transformList.Add(entryTransform);
    }

}
