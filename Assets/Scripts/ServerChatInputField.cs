
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ServerChatInputField : MonoBehaviour
{
    public ServerConfiguration instanceOfServer;
    private InputField inputField;
    public ServerChatTextField serverChatTextField;

    private void Start()
    {
        inputField = GetComponent<InputField>();
    }

    public void ValueChanged()
    {
        if (inputField.text.Contains("\n"))
        {
            Debug.Log("Server ->" + " Data: " + inputField.text);
            serverChatTextField.appendText("Server ->" + " Data: " + inputField.text);
            instanceOfServer.SendToAll(Encoding.ASCII.GetBytes(inputField.ToString()));
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }
    }
}
