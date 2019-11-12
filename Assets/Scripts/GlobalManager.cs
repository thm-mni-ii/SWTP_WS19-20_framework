using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalManager : MonoBehaviour {
	public Canvas RegisterCanvas;
	public Canvas LoginCanvas;
	public Canvas ChatCanvas;
<<<<<<< HEAD
	//public Canvas ForgetCanvas;
=======
	public Canvas LobbyCanvas;
>>>>>>> chat
	//public Canvas ResetCanvas;
	//public Canvas ActiveCanvas;

	// Use this for initialization
	void Awake () {
		RegisterCanvas.enabled = false;
		LoginCanvas.enabled = true;
		ChatCanvas.enabled  = false;
<<<<<<< HEAD
	//	ResetCanvas.enabled = false;
=======
        LobbyCanvas.enabled = false;
>>>>>>> chat
		//ForgetCanvas.enabled = false;
		//ActiveCanvas.enabled = false;

	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ToggleCanvas(string open){
		if (open == "login") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = true;
<<<<<<< HEAD
		//	ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = false;

		} else if (open == "register") {
			RegisterCanvas.enabled = true;
			LoginCanvas.enabled = false;
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = false;
=======
            LobbyCanvas.enabled = false;
            ChatCanvas.enabled = false;


        } else if (open == "register") {
			RegisterCanvas.enabled = true;
			LoginCanvas.enabled = false;
            LobbyCanvas.enabled = false;

>>>>>>> chat

		} else if (open == "chat") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			ChatCanvas.enabled  = true;
<<<<<<< HEAD
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = true;
			//ActiveCanvas.enabled = false;

		} else if (open == "forget") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = true;
			//ActiveCanvas.enabled = false;

		} else if (open == "reset") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			//ResetCanvas.enabled = true;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = false;
=======
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
>>>>>>> chat

		} else if (open == "active") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
<<<<<<< HEAD
			//ResetCanvas.enabled = false;
			//ForgetCanvas.enabled = false;
			//ActiveCanvas.enabled = true;
=======
            LobbyCanvas.enabled = false;
>>>>>>> chat
		}
	
	}

}
