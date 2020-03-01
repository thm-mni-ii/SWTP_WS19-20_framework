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
    /// GameCanvas manage Start of Game (like: making partys)
    /// </summary>
    public Canvas GameCanvas;
	/// <summary>
	/// TOPPlayerCanvas show TOP player
	/// </summary>
	public Canvas TOPPlayerCanvas;
	/// <summary>
	/// TOPPlayerCanvas show, which partys are open
	/// </summary>
	public Canvas PartysListCanvas;
    /// <summary>
    /// HelpMenu Canvas shows which the Keyboard shortcut Buttons
    /// </summary>
    public Canvas HelpMenu;
    /// <summary>
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
        PartysListCanvas.enabled = false;
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

        // hide/show HelpMenu by pressing F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (HelpMenu.enabled)
            {
                HelpMenu.enabled = false;
            }
            else
            {
                HelpMenu.enabled = true;
            }
        }

        // hide/show TOP10 Canvas by pressing Tab
        if (Input.GetKeyDown(KeyCode.Tab))
	    {
		    if (TOPPlayerCanvas.enabled)
		    {
                TOPPlayerCanvas.enabled = false;
		    }
		    else
		    {
                TOPPlayerCanvas.enabled = true;
		    }
	    }

        // hide/show by pressing Escape
        if (Input.GetKeyDown(KeyCode.Escape))
	    {
		    if (hideall)
		    {
                ChatCanvas.enabled = true;
                TOPPlayerCanvas.enabled = true;
                PartysListCanvas.enabled = true;
                hideall = false;
		    }
		    else
		    {
                ChatCanvas.enabled = false;
                TOPPlayerCanvas.enabled = false;
                PartysListCanvas.enabled = false;
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
            PartysListCanvas.enabled = false;
        } else if (open == "register") {
            RegisterCanvas.enabled = true;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            ForgotCanvas.enabled = false;
            TOPPlayerCanvas.enabled = false;
            PartysListCanvas.enabled = false;
        } else if (open == "chat") {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = true;
            ForgotCanvas.enabled = false;
            TOPPlayerCanvas.enabled = false;
            GameCanvas.enabled = false;
        }
        else if (open == "forgot")
        {
            RegisterCanvas.enabled = false;
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = false;
            ForgotCanvas.enabled = true;
            TOPPlayerCanvas.enabled = false;
        } else if (open == "partylist")
        {
	        PartysListCanvas.enabled = true;
        }
        else if (open == "gameOn") {
            GameCanvas.enabled = true;
            PartysListCanvas.enabled = false;
        }
        else if (open == "gameOff")
        {
            GameCanvas.enabled = false;
            PartysListCanvas.enabled = true;
        }
        else if (open =="help"){

            if (HelpMenu.enabled)
            {
                HelpMenu.enabled = false;
            }
            else
            {
                HelpMenu.enabled = true;
            }
        }
    }
}
