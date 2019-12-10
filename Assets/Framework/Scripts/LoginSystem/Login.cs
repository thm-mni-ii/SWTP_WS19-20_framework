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
    /*Variables*/
    [SerializeField] private InputField userName;
    [SerializeField] private InputField passwordField;
    [SerializeField] private InputField rEmail;
    [SerializeField] private InputField rUsername;
    [SerializeField] private InputField rPass1;
    [SerializeField] private InputField rPass2;
    private GlobalManager globalCanvas;
    [SerializeField] private Text WarningMsg;
    [SerializeField] private Text ResWarningMsg;
    [SerializeField] private InputField ResEmail;
    [SerializeField] private Text regWarningMsg;
    private UserInfo user;
    private Chat chat;
    private DatabaseReference reference;
    private Firebase.Auth.FirebaseAuth auth;
    NetworkManager manager;
    private int report = 0;
    private string resEmail = null;


    /* Start is called before the first frame update */
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

    /*  Update is called once per frame */
    void Update()
    {


        /* Login-Menu messages are displayed from this switch-case becasue this action must be done from he main Thread */
        if (report != 0)
        {
            switch (report) {

                case 1: //Login Succesful
                    if (!NetworkClient.isConnected)
                    {
                        if (NetworkServer.active)
                        {
                            manager.StartClient();
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
                        else
                        {
                            WarningMsg.text = "Networkserver Offline";
                        }
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

    /* login methode checks if user is registered on the database and returns the result accordingly 
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
    /* a helping login method to allow login with username instead of email*/
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


    /*  logs out user and disconnects the connection */
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

    /*
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
    /*  helping register method*/
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
    /* Saved the username in the Databank because firebase saves only the email and password  */
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
    /* display "register" Canvas */
    public void RegisterButton()
    {

        globalCanvas.ToggleCanvas("register");


    }
    /* display "forgot" Canvas */
    public void ResetButton()
    {

        globalCanvas.ToggleCanvas("forgot");


    }
    /* display "login" Canvas */
    public void backButton()
    {

        globalCanvas.ToggleCanvas("login");


    }

    /*
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

    /* Database Definitions */

    public class User
    {
        public string UserName;
        public string email;
        public int xp;
        public string id;
        public User()
        {
        }

        public User(string username, string email,string id)
        {
            this.UserName = username;
            this.email = email;
            this.xp = 0;
            this.id = id;

        }
    }

    private void writeNewUser(string userId, string name, string email)
    {
        User user = new User(name, email, userId);
        string json = JsonUtility.ToJson(user);

        reference.Child("users").Child(name).SetRawJsonValueAsync(json);
    }
    // Example Method for Data change 
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