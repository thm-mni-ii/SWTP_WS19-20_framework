
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ServerChatTextField : MonoBehaviour
{
    public ServerConfiguration instanceOfServer;
    private Text content;
    private string tempText;
    int i = 1;

    private void Start()
    {
        content = GetComponent<Text>();
    }

    private void Update()
    {
        this.content.text = tempText;
        //i++;
    }

    public void appendText(string newText)
    {
       tempText  = string.Format(tempText, newText);
    }
}
