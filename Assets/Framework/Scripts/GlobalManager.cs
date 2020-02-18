using UnityEngine;
using UnityEngine.UI;

/**
 * GlobalManager class to manage the whole game (like: Canvases)
 * Hide and show the components
 */
public class GlobalManager : MonoBehaviour {
	
	/// <summary>
	/// RegisterCanvas manage the Register portal
	/// </summary>
	public Canvas RegisterCanvas;
	/// <summary>
	/// LoginCanvas manage the login portal
	/// </summary>
	public Canvas LoginCanvas;
	/// <summary>
	/// ChatCanvas manage Chat window
	/// </summary>
	public Canvas ChatCanvas;
	/// <summary>
	/// ForgotCanvas manage Forgot password window
	/// </summary>
    public Canvas ForgotCanvas;
	/// <summary>
    /// GameCanvas manage displays the Game
    /// </summary>
    public Canvas GameCanvas;
	/// <summary>
	/// TOPPlayerCanvas manage displays the Game
	/// </summary>
	public Canvas TOPPlayerCanvas;
	/// <summary>
	/// logout Button to sign out from the game
	/// </summary>
	public Button logout = null;
	/// <summary>
	/// auxiliary variable to know if the logout button has been hidden
	/// </summary>
	public bool hideLogoutButton = false;
	/// <summary>
	/// auxiliary variable to know if the chat canvas has been hidden
	/// </summary>
	public bool hideChatCanvas = false;
	/// <summary>
	/// auxiliary variable to know if the TOP10 Canvas has been hidden
	/// </summary>
	public bool hideTOP10Canvas = false;
	/// <summary>
	/// auxiliary variable to hide all windows on lobby
	/// </summary>
	public bool hideall = true;

    /// <summary>
    /// Use this for initialization canvases, which we need
    /// canvases are:
    /// 1. RegisterCanvas
    /// 2. LoginCanvas
    /// 3. ChatCanvas
    /// 4. ForgotCanvas
    /// 5. GameCanvas
    /// </summary>
    void Awake () {
		RegisterCanvas.enabled = false;
		LoginCanvas.enabled = true;
		ChatCanvas.enabled  = false;
        ForgotCanvas.enabled = false;
        GameCanvas.enabled = false;
        TOPPlayerCanvas.enabled = false;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
	    hideShowObjects();
    }

    /// <summary>
    /// hide/show Gameobjects
    /// </summary>
    private void hideShowObjects()
    {
	    // hide/show logout button by pressed on ESC
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
		    if (hideLogoutButton)
		    {
			    logout.gameObject.SetActive(true);
			    hideLogoutButton = false;
		    }
		    else
		    {
			    logout.gameObject.SetActive(false);
			    hideLogoutButton = true;
		    }
	    }
	    
	    // hide/show chat canvas by pressed on c
	    if (Input.GetKeyDown(KeyCode.C))
	    {
		    if (hideChatCanvas)
		    {
			    ChatCanvas.gameObject.SetActive(true);
			    hideChatCanvas = false;
		    }
		    else
		    {
			    ChatCanvas.gameObject.SetActive(false);
			    hideChatCanvas = true;
		    }
	    }
	    
	    // hide/show TOP10 Canvas by pressed on t
	    if (Input.GetKeyDown(KeyCode.T))
	    {
		    if (hideTOP10Canvas)
		    {
			    TOPPlayerCanvas.gameObject.SetActive(true);
			    hideTOP10Canvas = false;
		    }
		    else
		    {
			    TOPPlayerCanvas.gameObject.SetActive(false);
			    hideTOP10Canvas = true;
		    }
	    }
	    
	    // hide/show by pressed on h
	    if (Input.GetKeyDown(KeyCode.H))
	    {
		    if (hideall)
		    {
			    logout.gameObject.SetActive(true);
			    hideLogoutButton = false;
			    ChatCanvas.gameObject.SetActive(true);
			    hideChatCanvas = false;
			    TOPPlayerCanvas.gameObject.SetActive(true);
			    hideTOP10Canvas = false;
			    hideall = false;
		    }
		    else
		    {
			    logout.gameObject.SetActive(false);
			    hideLogoutButton = true;
			    ChatCanvas.gameObject.SetActive(false);
			    hideChatCanvas = true;
			    TOPPlayerCanvas.gameObject.SetActive(false);
			    hideTOP10Canvas = true;
			    hideall = true;
		    }
	    }
    }
    
    /// <summary>
    /// change between canvases
    /// </summary>
    /// <param name="open">name of the canvas, which we want to open</param>
	public void ToggleCanvas(string open){
        if (open == "login") {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = true;
            ChatCanvas.enabled = false;
            ForgotCanvas.enabled = false;
            GameCanvas.enabled = false;
            TOPPlayerCanvas.enabled = false;
        } else if (open == "register") {
            RegisterCanvas.enabled = true;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            ForgotCanvas.enabled = false;
            TOPPlayerCanvas.enabled = false;
        } else if (open == "chat") {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = true;
            ForgotCanvas.enabled = false;
            GameCanvas.enabled = false;
        }
        else if (open == "forgot")
        {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            ForgotCanvas.enabled = true;
            TOPPlayerCanvas.enabled = false;
        } else if (open == "toplist")
        {
	        TOPPlayerCanvas.enabled = true;
        }
        else if (open == "gameOn") {
            GameCanvas.enabled = true;
        }
        else if (open == "gameOff")
        {
            GameCanvas.enabled = false;
        }
    }
}
