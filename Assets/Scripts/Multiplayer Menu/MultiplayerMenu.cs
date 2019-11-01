
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    //Input
	public InputField ipAddress = null;
	public InputField portNumber = null;
    public InputField username = null;
    public InputField password = null;

    //Server Setting
	public string masterServerHost = string.Empty;
	public ushort masterServerPort = 7777;
	public string natServerHost = string.Empty;
	public ushort natServerPort = 7777;

    //Access conditions
    private bool getRightAccessInfo = false;

    public GameObject networkManager = null;
	public GameObject[] ToggledButtons;

	private List<Button> _uiButtons = new List<Button>();

	private void Start()
	{
		ipAddress.text = "localhost";
		portNumber.text = "7777";
	}

	public void Connect()
	{
		if (!getRightAccessInfo)
        {
            SceneManager.LoadScene("Chat", LoadSceneMode.Additive);
            foreach (GameObject g in SceneManager.GetSceneByName("MultiplayerMenu").GetRootGameObjects())
            {
                g.SetActive(false);
            }
        }
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
			Connect();
	}

	public void Connected()
	{
        
    }
}
