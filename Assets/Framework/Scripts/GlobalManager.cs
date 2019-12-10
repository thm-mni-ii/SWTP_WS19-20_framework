using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Firebase;
using Firebase.Unity.Editor;

/**
 * GlobalManager class to manage the whole game
 */
public class GlobalManager : MonoBehaviour {
	public Canvas RegisterCanvas;
	public Canvas LoginCanvas;
	public Canvas ChatCanvas;
    public Canvas PartyCanvas;
    public Canvas ForgotCanvas;

    /**
     * Use this for initialization
     */
    void Awake () {
		RegisterCanvas.enabled = false;
		LoginCanvas.enabled = true;
		ChatCanvas.enabled  = false;
        PartyCanvas.enabled = false;
        ForgotCanvas.enabled = false;
    }
	void Start () { }
	
	/**
	 * Update is called once per frame
	 */
	void Update () { }
	
	/**
	 * change between canvases
	 */
	public void ToggleCanvas(string open){
		if (open == "login") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = true;
            ChatCanvas.enabled = false;
            PartyCanvas.enabled = false;
            ForgotCanvas.enabled = false;
		} else if (open == "register") {
			RegisterCanvas.enabled = true;
			LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            PartyCanvas.enabled = false;
            ForgotCanvas.enabled = false;
		} else if (open == "chat") {
			RegisterCanvas.enabled = false;
			LoginCanvas.enabled = false;
			ChatCanvas.enabled  = true;
            PartyCanvas.enabled = true;
            ForgotCanvas.enabled = false;
		}
        else if (open == "party")
        {
            PartyCanvas.enabled = true;
        } else if (open == "forgot")
        {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            PartyCanvas.enabled = false;
            ForgotCanvas.enabled = true;
        }
	}
}
