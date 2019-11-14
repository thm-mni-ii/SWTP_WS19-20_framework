using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Mirror;




[RequireComponent(typeof(NetworkManager))]
public class Login : MonoBehaviour
{
	[SerializeField] private InputField userName;
	[SerializeField] private InputField passwordField;
    [SerializeField] private InputField rEmail;
    [SerializeField] private InputField rUsername;
    [SerializeField] private InputField rPass1;
    [SerializeField] private InputField rPass2;
    private GlobalManager globalCanvas;
	[SerializeField] private Text WarningMsg;
    [SerializeField] private Text regWarningMsg;
    private UserInfo user;
	private Chat chat;
    private DatabaseReference reference;
    private Firebase.Auth.FirebaseAuth auth;
    public bool openscene = false;
    public bool loginfailed = false;
    public bool registration = false;
    public bool regfailed = false;
   // public gameObject netmanagerCanvas; 
    NetworkManager manager;


    // Start is called before the first frame update
    void Start () {		
		globalCanvas = gameObject.GetComponent<GlobalManager>();
		user = gameObject.GetComponent<UserInfo>();
		chat = gameObject.GetComponent<Chat>();

       

        GameObject  NM = GameObject.FindWithTag("NetworkManager");

        if (NM != null)
        {
            manager = NM.GetComponent<NetworkManager>();
        }

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mmo-spiel.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

    }

    // Update is called once per frame
    void Update()
    {

        if (openscene)
        {
            if (!NetworkClient.isConnected)
            {
                if (NetworkServer.active)
                {
                    manager.StartClient();
                    globalCanvas.ToggleCanvas("chat");
                    if (NetworkClient.isConnected && !ClientScene.ready)
                    {

                        ClientScene.Ready(NetworkClient.connection);

                            if (ClientScene.localPlayer == null)
                            {
                                ClientScene.AddPlayer();
                            }
                        
                    }
                    
                }else
                {
                    WarningMsg.text = "Networkserver Offline";
                }
            }
            
                
            openscene = false;
        }

        if (loginfailed)
        {
            WarningMsg.text = "Invalid Email or password";
            loginfailed = false;
        }

        if (registration)
        {

            globalCanvas.ToggleCanvas("login");
            WarningMsg.text = "Registration Succesful";
            registration = false;
        }
        if (regfailed)
        {
            regWarningMsg.text = "Registration failed";
            regfailed = false;
        }
    }
	
	
	public void LoginMethod(){
	if(userName.text != null && userName.text != "" && passwordField.text != null && passwordField.text != "")
        {
            auth.SignInWithEmailAndPasswordAsync(userName.text, passwordField.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                   loginfailed = true;
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                   loginfailed = true;
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }


               Firebase.Auth.FirebaseUser newUser = task.Result;

                user.email = newUser.Email;
                user.Uid = newUser.UserId;
                user.userN = newUser.DisplayName;



                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
               chat.EstablishConnection(user);
               openscene = true;
           });


        }
        else {
			
		WarningMsg.text = "Please Enter a username and password";
		}
	}



    public void LogoutMethod()
    {

        chat.Disconnection();
        globalCanvas.ToggleCanvas("login");

    }

    public void RegisterMethod()
    {
        if (rUsername.text != null && rUsername.text != "" && rEmail.text != null && rEmail.text != "" && rPass1.text != null && rPass1.text != "" && rPass2.text != null && rPass2.text != "")
        {

            if(string.Compare(rPass1.text, rPass2.text) != 0)
            {
                regWarningMsg.text = "Password don't match";
                return;
            }

            auth.CreateUserWithEmailAndPasswordAsync(rEmail.text, rPass1.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    regfailed = true;
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    regfailed = true;
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;

                setDisplayName(newUser);
                writeNewUser(newUser.UserId,rUsername.text, rEmail.text);

                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                                        newUser.DisplayName, newUser.UserId);
                registration = true;
            });

        }else
        {
            regWarningMsg.text = "Please fill in all the Fields";

        }

    }

    public void setDisplayName(Firebase.Auth.FirebaseUser newUser)
    {
        if (newUser != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = rUsername.text,
            };
            newUser.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });
        }
    }

    public void RegisterButton()
    {
    
        globalCanvas.ToggleCanvas("register");


    }

    public void ResetPass()
    {

        globalCanvas.ToggleCanvas("reset");


    }

    /* Database Definitions */
    public class User
    {
        public string UserName;
        public string email;
        public int xp;

        public User()
        {
        }

        public User(string username, string email)
        {
            this.UserName = username;
            this.email = email;
            this.xp = 0;

    }
    }

    private void writeNewUser(string userId, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        reference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }
    // This Method is for Data change (password change)
    // reference.Child("users").Child(userId).Child("UserName").SetValueAsync(name);


    /* Read Data from Database */
    public void readdata()
    {
        FirebaseDatabase.DefaultInstance
         .GetReference("users")
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              // Handle the error...
          }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
              // Do something with snapshot...
          }
          });
    }



}
