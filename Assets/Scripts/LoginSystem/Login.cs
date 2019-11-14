using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


public class Login : MonoBehaviour
{
	[SerializeField] private InputField userName;
	[SerializeField] private InputField passwordName;
	private GlobalManager globalCanvas;
	[SerializeField] private Text WarningMsg;
	private UserInfo user;
	private Chat chat;
    private DatabaseReference reference;
    Firebase.Auth.FirebaseAuth auth;

    // Start is called before the first frame update
    void Start () {		
		globalCanvas = gameObject.GetComponent<GlobalManager>();
		user = gameObject.GetComponent<UserInfo>();
		chat = gameObject.GetComponent<Chat>();
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mmo-spiel.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	public void LoginMethod(){
	if(userName.text != null && userName.text != ""){
		user.userN = userName.text;
		chat.EstablishConnection(user);
        globalCanvas.ToggleCanvas("chat");


        }
        else {
			
		WarningMsg.text = "Error no Username";
		}
	}


    public void LogoutMethod()
    {

        chat.Disconnection();
        globalCanvas.ToggleCanvas("login");

    }

    public void RegisterMethod()
    {
        //writeNewUser("1","name123456", "email@sdasd");
        globalCanvas.ToggleCanvas("login");


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
        public string username;
        public string email;
        public string password;

        public User()
        {
        }

        public User(string username, string email)
        {
            this.username = username;
            this.email = email;
        }
    }

    private void writeNewUser(string userId, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        reference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }
    // This Method is for Data change (password change)
    // reference.Child("users").Child(userId).Child("username").SetValueAsync(name);


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

    private void SignUpUser(string username,string password,string email)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });

    }

    private void SignInUser(string username, string password, string email)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }



    }
