using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Login : MonoBehaviour
{
	[SerializeField] private InputField userName;
	[SerializeField] private InputField passwordName;
	private GlobalManager globalCanvas;
	[SerializeField] private Text WarningMsg;
	private UserInfo user;
	private Chat chat;
	//
    // Start is called before the first frame update
	void Start () {		
		globalCanvas = gameObject.GetComponent<GlobalManager>();
		user = gameObject.GetComponent<UserInfo>();
		chat = gameObject.GetComponent<Chat>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	public void LoginMethod(){
	if(userName.text != null && userName.text != ""){
		user.userN = userName.text;
		chat.EstablishConnection();
		globalCanvas.ToggleCanvas("chat");
		
		}else {
			
		WarningMsg.text = "Error no Username";
		}
	}
	

	
}
