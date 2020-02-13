using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{ 
    public Transform entryContainer; 
    public Transform entryTemplate;
    public static List<Transform> highscoreEntryTransformList = new List<Transform>();

    private List<HighscoreEntry> highscoreEntryList;
    static int counter = 0;
    static int counter2 = 0;
    static int count = 0;
    
    //Das wird spaeter geloescht .
    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    private class HighscoreEntry
    {
        public int score;
        public string name;
    }

    void render()
    {
        entryTemplate.gameObject.SetActive(false);

        // exampel entry
        highscoreEntryList = new List<HighscoreEntry>();
        highscoreEntryList.Add(new HighscoreEntry { score = 23, name = "eins" });
        highscoreEntryList.Add(new HighscoreEntry { score = 232, name = "hh" });
        highscoreEntryList.Add(new HighscoreEntry { score = 223, name = "asdf" });
        highscoreEntryList.Add(new HighscoreEntry { score = 657, name = "sadfad" });
        highscoreEntryList.Add(new HighscoreEntry { score = 23, name = "hallo" });
        highscoreEntryList.Add(new HighscoreEntry { score = 232, name = "hh" });
        highscoreEntryList.Add(new HighscoreEntry { score = 223, name = "asdf" });
        highscoreEntryList.Add(new HighscoreEntry { score = 657, name = "hallowasmachstdu" });
        highscoreEntryList.Add(new HighscoreEntry { score = 23, name = "Zwei" });
        highscoreEntryList.Add(new HighscoreEntry { score = 232, name = "qqqqqqq" });
        highscoreEntryList.Add(new HighscoreEntry { score = 223, name = "tttttt" });
        highscoreEntryList.Add(new HighscoreEntry { score = 657, name = "eeeeee" });
        highscoreEntryList.Add(new HighscoreEntry { score = 23, name = "werwerwe" });
        highscoreEntryList.Add(new HighscoreEntry { score = 232, name = "rrrrrr" });
        highscoreEntryList.Add(new HighscoreEntry { score = 223, name = "ffff" });
        highscoreEntryList.Add(new HighscoreEntry { score = 657, name = "mmmm" });
        highscoreEntryList.Add(new HighscoreEntry { score = 657, name = "mmmm" });
        Debug.Log(highscoreEntryList.Count);
        
        foreach (HighscoreEntry highscoreEntry in highscoreEntryList)
        {
            if ((counter < (counter2 * 8) + 8))
            {
                CreateHighscoreEntryTransform(highscoreEntryList[counter], entryContainer, HighscoreTable.highscoreEntryTransformList);
                counter++;
            }
        }
    }

    //hier kannst du ein Element zu der Tabelle hinzufügen
    public void addTableElement(string type, int host, int player)
    {
        count++;
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, entryContainer);    
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();   
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * count);
        entryTransform.gameObject.SetActive(true);
        
        // hier kannst du auf die Elemente in der Tabellen zugreifen 
        entryTransform.Find("posText").GetComponent<Text>().text = type;
        entryTransform.Find("scoreText").GetComponent<Text>().text = host.ToString();
        entryTransform.Find("nameText").GetComponent<Text>().text = player.ToString();
        entryTransform.Find("background").gameObject.SetActive(count % 2 == 1);

        // das dient einfach dazu, dass du die Farbe des textes aendern kannst
        entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        HighscoreTable.highscoreEntryTransformList.Add(entryTransform);
    }
    
    // muss noch programmiert werden 
    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {

        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * count);
        entryTransform.gameObject.SetActive(true);

        int rank = count + 1;
        string rankString;
        switch (rank)
        {
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
        entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
        entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        
        // transformList.Add(entryTransform);
        HighscoreTable.highscoreEntryTransformList.Add(entryTransform);
        count++;
        Debug.Log("highscoreEntryTransformList Count in the Backasdfasdf " + HighscoreTable.highscoreEntryTransformList.Count);
    }

    //muss noch programmiert werden
    public void showNextElements()
    {
        addTableElement("TEST", 23, 234);
        //count = 0;
        //cleartransformlist();
        //Debug.Log("highscoreEntryTransformList Count" + HighscoreTable.highscoreEntryTransformList.Count);

        //render();
        //counter2++;
    }
    
    //muss noch programmiert werden
    public void cleartransformlist()
    {
        Debug.Log("halloMthode");
        foreach (Transform highscoreEntry in HighscoreTable.highscoreEntryTransformList)
        {
            Destroy(highscoreEntry.gameObject);
        }

        Debug.Log("highscoreEntryTransformList Count in the Back hallo mein Lieber" + HighscoreTable.highscoreEntryTransformList.Count);
        if (highscoreEntryTransformList.Count > 0)
        {
            for (int i = 0; i <= count; i++)
            {
                if (HighscoreTable.highscoreEntryTransformList[i] != null)
                {
                    highscoreEntryTransformList.Remove(HighscoreTable.highscoreEntryTransformList[i]);
                }
            }
        }
    }

    //muss noch programmiert werden
    public void showBackElements()
    {
        cleartransformlist();
        Debug.Log("highscoreEntryTransformList Count in the Back" + HighscoreTable.highscoreEntryTransformList.Count);

        counter -= 7;
        counter2--;
        //render();
    }
}
