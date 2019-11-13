using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalManager : MonoBehaviour {
	public Canvas RegisterCanvas;
	public Canvas LoginCanvas;
	public Canvas ChatCanvas;
	public Canvas LobbyCanvas;

	// Use this for initialization
	void Awake () {
		RegisterCanvas.enabled = false;
		LoginCanvas.enabled = true;
		ChatCanvas.enabled  = false;
        LobbyCanvas.enabled = false;
	}
	public void ToggleCanvas(string open){
		if (open == "login") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = true;
            LobbyCanvas.enabled = false;
            ChatCanvas.enabled = false;
		} else if (open == "register") {
			RegisterCanvas.enabled = true;
			LoginCanvas.enabled = false;
            LobbyCanvas.enabled = false;
		} else if (open == "chat") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			ChatCanvas.enabled  = true;
            LobbyCanvas.enabled = false;
		} else if (open == "lobby") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
            LobbyCanvas.enabled = true;
            ChatCanvas.enabled = true;
		} else if (open == "logout") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
            LobbyCanvas.enabled = false;
		} else if (open == "active") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
            LobbyCanvas.enabled = false;
		}
	}
}
