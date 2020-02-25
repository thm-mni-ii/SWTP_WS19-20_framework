using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Table definition class
/// Contains Static Methods to The data in a Table
/// </summary>
public class Table : MonoBehaviour
{
    /// <summary>
    /// Adds a new Entry to the Table
    /// Used by the HostsTable and PartyTable
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    /// <param name="index3"></param>
    /// <param name="index4"></param>
    /// <param name="container"></param>
    /// <param name="entryTemplate"></param>
    /// <param name="transformList"></param>
    /// <param name="color"></param>
    public static void CreateEntryTransform(string index1, string index2, string index3,string index4, Transform container, Transform entryTemplate, List<Transform> transformList, Color color) {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);
        
        entryTransform.Find("index1").GetComponent<Text>().text = index1;
        entryTransform.Find("index2").GetComponent<Text>().text = index2;
        entryTransform.Find("index3").GetComponent<Text>().text = index3;

        entryTransform.Find("index1").GetComponent<Text>().color = color;
        entryTransform.Find("index2").GetComponent<Text>().color = color;
        entryTransform.Find("index3").GetComponent<Text>().color = color;

        if (index4 != null)
        {
            entryTransform.Find("index4").GetComponent<Text>().text = index4;
            entryTransform.Find("index4").GetComponent<Text>().color = color;
        }
        transformList.Add(entryTransform);
    }
    
    /// <summary>
    /// This Method should be called every time a Table is updated
    /// It cleans the Transform List and deletes each Entry/Object created on the Client
    /// </summary>
    /// <param name="transformList"></param>
    public static void ClearEntryList(List<Transform> transformList) {
        foreach (Transform transform in transformList)
        {
            Destroy(transform.gameObject);
        }
        transformList.Clear();
    }
}
