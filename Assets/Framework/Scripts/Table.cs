using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Table definition class
/// </summary>
public class Table : MonoBehaviour
{
    /// <summary>
    /// Adds a new Entry to the Table
    /// Used by the HostsTable and PartyTable
    /// </summary>
    /// <param name="host"></param>
    /// <param name="container"></param>
    /// <param name="transformList"></param>
    public void CreateEntryTransform(string type, string host, string players, Transform container, Transform entryTemplate, List<Transform> transformList, Color color) {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);
        
        entryTransform.Find("gameText").GetComponent<Text>().text = type;
        entryTransform.Find("hostText").GetComponent<Text>().text = host;
        entryTransform.Find("playersText").GetComponent<Text>().text = players;
        entryTransform.Find("gameText").GetComponent<Text>().color = color;
        entryTransform.Find("hostText").GetComponent<Text>().color = color;
        entryTransform.Find("playersText").GetComponent<Text>().color = color;
        transformList.Add(entryTransform);
    }
    
    /// <summary>
    /// This Method should be called every time a Table is updated
    /// It cleans the Transform List and deletes each Entry/Object created on the Client
    /// </summary>
    /// <param name="transformList"></param>
    public void ClearEntryList(List<Transform> transformList) {
        foreach (Transform transform in transformList)
        {
            Destroy(transform.gameObject);
        }
        transformList.Clear();
    }
}
