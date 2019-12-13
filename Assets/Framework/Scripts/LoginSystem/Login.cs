using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Mirror;

[RequireComponent(typeof(NetworkManager))]
/**
 * login class has many tasks:
 * 1. (sign in) verify if the access data are right and make a connection between client and server
 * 2. (sign up/register)  register a new data in the database
 * 3. (sign out) logout from the connection between client and server
 * 4. other functions like: forget the password
  */
public class Login : MonoBehaviour
{
    /// <summary>
    /// Take username from GUI (login portal)
    /// </summary>
    [SerializeField] private InputField userName;
    
    /// <summary>
    /// Take password from GUI (login portal)
    /// </summary>
    [SerializeField] private InputField passwordField;
    
    /// <summary>
    /// Take E-Mail address from GUI (register portal)
    /// </summary>
    [SerializeField] private InputField rEmail;
    
    /// <summary>
    /// Take desired username from GUI (register portal)
    /// </summary>
    [SerializeField] private InputField rUsername;
    
    /// <summary>
    /// Take desired password from GUI (register portal)
    /// </summary>
    [SerializeField] private InputField rPass1;
    
    /// <summary>
    /// confirmation phase: Take desired password from GUI (register portal) 
    /// </summary>
    [SerializeField] private InputField rPass2;
    
    /// <summary>
    /// manage the whole game. Hide and show the components.
    /// </summary>
    private GlobalManager globalCanvas;
    
    /// <summary>
    /// warning message (by login portal)
    /// </summary>
    [SerializeField] private Text WarningMsg;
    
    /// <summary>
    /// warning message from forgot password portal
    /// </summary>
    [SerializeField] private Text ResWarningMsg;
    
    /// <summary>
    /// Take E-Mail address from forgot password portal
    /// </summary>
    [SerializeField] private InputField ResEmail;
    
    /// <summary>
    /// warning message from register portal
    /// </summary>
    [SerializeField] private Text regWarningMsg;
    
    /// <summary>
    /// data struct of user information
    /// </summary>
    private UserInfo user;
    
    /// <summary>
    /// chat manager by client
    /// </summary>
    private Chat chat;
    
    /// <summary>
    /// DatabaseReference from Firebase packages
    /// </summary>
    private DatabaseReference reference;
    
    /// <summary>
    /// Database Authentication from Firebase packages
    /// </summary>
    private Firebase.Auth.FirebaseAuth auth;
    
    /// <summary>
    /// NetworkManager is responsible for the network connection.
    /// He has connection settings (network address, maxConnections, etc)
    /// </summary>
    NetworkManager manager;
    
    /// <summary>
    /// Answer from database server (to know if all thinks right or to do some think)
    /// </summary>
    private int report = 0;
    
    /// <summary>
    /// auxiliary variable to rest the password using E-Mail address
    /// </summary>
    private string resEmail = null;
    private bool connecting = false;
    
    /**
     * Start is called before the first frame update
     * connection to database (Firebase) configuration and show login portal
     */
    void Start()
    {
        globalCanvas = gameObject.GetComponent<GlobalManager>();
        user = gameObject.GetComponent<UserInfo>();
        chat = gameObject.GetComponent<Chat>();

        GameObject NM = GameObject.FindWithTag("NetworkManager");

        if (NM != null)
        {
            manager = NM.GetComponent<NetworkManager>();
        }

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mmo-spiel.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    /**
     * Update is called once per frame
     * display the answer or check 
     * display if there are any errors or if every think right
     * There are many types of answers:
     * case 1: Login Succesful
     * case 10: or case 11: Invalid username or password
     * case 12: Registration Succesful
     * case 13: Error Registration was canceled
     * case 14: Email is already registered
     * case 15: please confirm your email
     * case 16: Password reset email sent successfully
     * case 17: Error Send request was canceled
     * case 18: Could not send reset E-mail
     * case 19: check: username and password (by register)
     * case 20: Username is Taken
     * case 21: check: username and password (by login)
     */
    void Update()
    {

        /* Show the connection status after clicking the Login button */
        if (connecting)
        {

                if (NetworkClient.isConnected)
            {
                connecting = false;
                chat.EstablishConnection(user);
                globalCanvas.ToggleCanvas("chat");

                if (NetworkClient.isConnected && !ClientScene.ready)
                {

                    ClientScene.Ready(NetworkClient.connection);
                    if (ClientScene.localPlayer == null)
                    {
                        ClientScene.AddPlayer();

                    }

                }
            }
                else if(!NetworkClient.isConnected && !NetworkClient.active)
            {
                connecting = false;
                WarningMsg.text = "NetworkServer Offline!";
            }
        }



                /*
                 * Login-Menu messages are displayed from this switch-case becasue this action must be done from he main Thread
                 */
                if (report != 0)
        {
            switch (report) {
                case 1: //Connection Request
                    if (!NetworkClient.isConnected && !NetworkClient.active && !connecting)
                    {
                        connecting = true;
                        WarningMsg.text = "Connecting to Server..";
                        manager.StartClient();
                        
                    }
                    break;
                case 10:
                    WarningMsg.text = "Invalid Username or password #1";
                    break;
                case 11:
                    WarningMsg.text = "Invalid username or password";
                    break;
                case 12:
                    globalCanvas.ToggleCanvas("login");
                    WarningMsg.text = "Registration Succesful";
                    break;
                case 13:
                    regWarningMsg.text = "Error Registration was canceled)";
                    break;
                case 14:
                    regWarningMsg.text = "Email is already registered";
                    break;
                case 15:
                    WarningMsg.text = "please confirm your email";
                    break;
                case 16:
                    ResWarningMsg.text = "Password reset email sent successfully";
                    break;
                case 17:
                    ResWarningMsg.text = "Error Send request was canceled";
                    break;
                case 18:
                    ResWarningMsg.text = "Could not send reset E-mail";
                    break;
                case 19:
                    report = 0;
                    reg2();
                    break;
                case 20:
                    regWarningMsg.text = "Username is Taken";
                    break;
                case 21:
                    report = 0;
                    LoginMethod2(resEmail);
                    break;
                default:
                    Debug.Log("Error unkown Report (Login.class)");
                    break;
            }
            report = 0;
        }
    }

    /**
     * login methode checks if user is registered on the database and returns the result accordingly 
     */
    public void LoginMethod()
    {
        if (userName.text != null && userName.text != "" && passwordField.text != null && passwordField.text != "")
        {
            resEmail = null;
            FirebaseDatabase.DefaultInstance
          .GetReference("users/" + userName.text + "/email")
           .GetValueAsync().ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("reg task failed");
                   return;
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result;
                   resEmail = (string)snapshot.Value;
                   if (resEmail == null)
                   {
                       report = 11;
                       Debug.Log("not found");
                   } else
                   {
                       report = 21;
                   }
               }
           });
        }
    }
    /**
     * a helping login method to allow login with username instead of email
     */
    public void LoginMethod2(string resEmail)
    {
        if (resEmail != null && resEmail != "" && passwordField.text != null && passwordField.text != "")
        {
            auth.SignInWithEmailAndPasswordAsync(resEmail, passwordField.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    report = 10;
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    report = 11;
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                Firebase.Auth.FirebaseUser newUser = task.Result;
                if (newUser.IsEmailVerified)
                {
                    user.email = newUser.Email;
                    user.Uid = newUser.UserId;
                    user.userN = newUser.DisplayName;

                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);
                    
                    report = 1;
                    
                }
                else
                {
                    newUser.SendEmailVerificationAsync();
                    report = 15;
                }
            });
        }
        else
        {
            WarningMsg.text = "Please Enter a username and password";
        }
    }

    /**
     * logs out user and disconnects the connection
     */
    public void LogoutMethod()
    {
        chat.Disconnection();
        if (NetworkClient.active)
        {
            manager.StopClient();
        }
        globalCanvas.ToggleCanvas("login");
        passwordField.text = "";
    }

    /**
     * Registers new user to the Databank and handles the request if the user is already registered
     */
    public void RegisterMethod()
    {
        if (rUsername.text != null && rUsername.text != "" && rEmail.text != null && rEmail.text != "" && rPass1.text != null && rPass1.text != "" && rPass2.text != null && rPass2.text != "")
        {
            if (!rEmail.text.Contains("@"))
            {
                regWarningMsg.text = "Please enter your E-mail";
                return;
            }
            
            string u = null;
            FirebaseDatabase.DefaultInstance
                      .GetReference("users/" + rUsername.text + "/UserName")
                       .GetValueAsync().ContinueWith(task =>
                       {
                           if (task.IsFaulted)
                           {
                               Debug.Log("reg task failed");
                               return;
                           }
                           else if (task.IsCompleted)
                           {
                               DataSnapshot snapshot = task.Result;
                               u = (string)snapshot.Value;
                               if (u == null)
                               {

                                   Debug.Log("not found");
                                   report = 19;
                               }else
                               {

                                  
                                   report = 20;
                               }
                           }
                       });
        }
    }
    /**
     * helping register method to Registers new user to the Databank and handles the request if the user is already registered
     */
    public void reg2() { 
        if (rUsername.text != null && rUsername.text != "" && rEmail.text != null && rEmail.text != "" && rPass1.text != null && rPass1.text != "" && rPass2.text != null && rPass2.text != "")
        {
            if (rPass1.text.Length < 6 || rPass2.text.Length < 6)
            {
                regWarningMsg.text = "Password must contain at least 6 Charachters";
                return;
            }

            if (string.Compare(rPass1.text, rPass2.text) != 0)
            {
                regWarningMsg.text = "Passwords don't match";
                return;
            }

            auth.CreateUserWithEmailAndPasswordAsync(rEmail.text, rPass1.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    report = 13;
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    report = 14;
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;


                newUser.SendEmailVerificationAsync();


                setDisplayName(newUser);

                writeNewUser(newUser.UserId, rUsername.text, rEmail.text);

                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                                        newUser.DisplayName, newUser.UserId);
                report = 12;
            });
        }
        else
        {
            regWarningMsg.text = "Please fill in all the Fields";

        }
    }
    
    /**
     * Saved the username in the Databank because firebase saves only the email and password
     */
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
    
    /**
     * display "register" Canvas
     */
    public void RegisterButton()
    {
        globalCanvas.ToggleCanvas("register");
    }
    
    /**
     * display "forgot" Canvas
     */
    public void ResetButton()
    {
        globalCanvas.ToggleCanvas("forgot");
    }
    
    /**
     * display "login" Canvas
     */
    public void backButton()
    {
        globalCanvas.ToggleCanvas("login");
    }

    /**
     * sends a recovery Email to reset password
     */
    public void ResetPass()
    {
        if (ResEmail.text != null && ResEmail.text != "")
        {
            if (!ResEmail.text.Contains("@"))
            {
                ResWarningMsg.text = "Please enter your E-mail to reset password";
                return;
            }
            auth.SendPasswordResetEmailAsync(ResEmail.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    report = 17;
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    report = 18;
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }
                report = 16;
                Debug.Log("Password reset email sent successfully.");
            });
        }else
        {
            ResWarningMsg.text = "Please enter your E-mail to reset password";
        }
    }

    /**
     * Database for user definition
     */
    public class User
    {
        public string UserName;
        public string email;
        public int xp;
        public string id;
        public User() { }

        public User(string username, string email,string id)
        {
            this.UserName = username;
            this.email = email;
            this.xp = 0;
            this.id = id;
        }
    }

    /**
     * after register take the new user and include him in the database
     */
    private void writeNewUser(string userId, string name, string email)
    {
        User user = new User(name, email, userId);
        string json = JsonUtility.ToJson(user);

        reference.Child("users").Child(name).SetRawJsonValueAsync(json);
    }
    // Example Method for Data change 
    // reference.Child("users").Child(userId).Child("UserName").SetValueAsync(name);
    
    /**
     * Read Data from Database
     */
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