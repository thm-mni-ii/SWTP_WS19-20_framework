using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections.Generic;
using Mirror;



[RequireComponent(typeof(CharacterController))]


/// <summary>
/// PlayerMovement class to Control the Movement of the player 
/// For more info https://mirror-networking.com/docs/
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
 
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    [SyncVar]
    public int index;
    /// <summary>
    /// manage the whole game. Hide and show the components.
    /// </summary>
    private GlobalManager globalCanvas;
    /// <summary>
    /// Player score (helpful for the reward system)
    /// </summary>
    [SyncVar]
    public uint score;
    /// <summary>
    /// playerColor: to distinguish between player levels
    /// </summary>
    [SyncVar(hook = nameof(SetColor))]
    public Color playerColor = Color.black;
    /// <summary>
    /// Unity clones the material when GetComponent<Renderer>().material is called
    /// Cache it here and destroy it in OnDestroy to prevent a memory leak
    /// </summary>
    Material cachedMaterial;
    /// <summary>
    /// a reference to the client class which contains all of the client information it is used here to change the scene/canvas of the player,
    /// and to fix the problem where the player moves automaticly, when he types in the chat
    /// </summary>
    private Client clientVar;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    CharacterController characterController;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    GameObject controllerColliderHitObject;
    /// <summary>
    /// Token from Supercyan Character Pack: https://assetstore.unity.com/packages/3d/characters/humanoids/character-pack-free-sample-79870
    /// </summary>
    [Header("Components")]
    public Animator m_animator;


    /****** MovementVariables *******/
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public float turnSpeedAccel = 10f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public float turnSpeedDecel = 10f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public float maxTurnSpeed = 150f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    [Header("Jump Settings")]
    public float jumpSpeed = 0f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public float jumpFactor = .025F;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public bool wasGrounded = false;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    [Header("Diagnostics")]
    public float horizontal = 0f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public float vertical = 0f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public float turn = 0f;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public bool isGrounded = true;
    /// <summary>
    /// Token from Mirror: https://mirror-networking.com/docs/ 
    /// </summary>
    public bool isFalling = false;
    /// <summary>
    /// Reference to the Database
    /// It is needed to get the name of the Modules
    /// </summary>
    public DatabaseReference reference;

    /// <summary>
    ///List of all the Modules 
    /// The List is read from Database
    /// </summary>
    List<String> Modules = new List<String>();

    /// <summary>
    /// List of All Game 
    /// It's read form the Database
    /// </summary>
    Dictionary<string,Game> Games = new Dictionary<string,Game>();




    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mmo-spiel-1920.firebaseio.com");
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance.GetReference("Modules").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (KeyValuePair<string, object> Module in (Dictionary<string, object>)snapshot.Value)
                {
                    Modules.Add((String)Module.Key);
                }
            }
        });

        FirebaseDatabase.DefaultInstance.GetReference("Games").GetValueAsync().ContinueWith(task => {
        if (task.IsFaulted)
        {
            // Handle the error...
        }
        else if (task.IsCompleted)
        {

                DataSnapshot snapshot = task.Result;
    
                foreach (KeyValuePair<string, object> GameName in (Dictionary<string, object>)snapshot.Value)
                    {
                    int max=0;
                    int min=0;
                    foreach (KeyValuePair<string, object> num in (Dictionary<string, object>)GameName.Value)
                    {
                        if (num.Key.Equals("Maxplayers"))
                        {
                            max = Int32.Parse(num.Value.ToString());

                        }
                        else if (num.Key.Equals("Minplayers"))
                        {
                                min = Int32.Parse(num.Value.ToString());
                        }
                    }

                    Game game = new Game(min,max);
                    Games.Add(GameName.Key, game);
                }

                }
            });

            GameObject GM = GameObject.FindWithTag("GlobalManager");
            if (GM != null)
            {
                globalCanvas = GM.GetComponent<GlobalManager>();
            }
            clientVar = globalCanvas.GetComponent<Client>();
        }


        /// <summary>
        /// Update player position - Token from Mirror: https://mirror-networking.com/docs/
        /// </summary>
        void Update()
        {
            if (!isLocalPlayer) return;
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.Q) && (turn > -maxTurnSpeed))
                turn -= turnSpeedAccel;
            else if (Input.GetKey(KeyCode.E) && (turn < maxTurnSpeed))
                turn += turnSpeedAccel;
            else if (turn > turnSpeedDecel)
                turn -= turnSpeedDecel;
            else if (turn < -turnSpeedDecel)
                turn += turnSpeedDecel;
            else
                turn = 0f;

            if (!isFalling && Input.GetKey(KeyCode.Space) && (isGrounded || jumpSpeed < 1))
            {
                jumpSpeed += jumpFactor;

                m_animator.SetTrigger("Jump");
            }
            else if (isGrounded)
            {
                isFalling = false;
            }
            else
            {
                isFalling = true;
                jumpSpeed = 0;
                m_animator.SetTrigger("Land");
            }
        }

        /// <summary>
        /// Used from Mirror - Token from Mirror: https://mirror-networking.com/docs/ 
        /// </summary>
        void FixedUpdate()
        {
            if (!isLocalPlayer || characterController == null) return;
            if (clientVar.clientMessageTF.isFocused) return;
            if (clientVar.partyTextField.isFocused && globalCanvas.GameCanvas.enabled ) return;

            m_animator.SetBool("Grounded", isGrounded);
            transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);
            Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction = transform.TransformDirection(direction);
            direction *= moveSpeed;

            if (jumpSpeed > 0)
                characterController.Move(direction * Time.fixedDeltaTime);
            else
            {
                m_animator.SetFloat("MoveSpeed", direction.magnitude);
                characterController.SimpleMove(direction);
            }
            isGrounded = characterController.isGrounded;
        }

        /// <summary>
        /// Token from Mirror: https://mirror-networking.com/docs/ 
        /// </summary>
        /// <param name="color"></param>
        void SetColor(Color color)
        {
            if (cachedMaterial == null) cachedMaterial = GetComponent<Renderer>().material;
            cachedMaterial.color = color;
        }
        /// <summary>
        /// Token from Mirror: https://mirror-networking.com/docs/ 
        /// </summary>
        void OnDisable()
        {
            if (isLocalPlayer)
            {
                Camera.main.transform.SetParent(null);
                Camera.main.transform.localPosition = new Vector3(0f, 50f, 0f);
                Camera.main.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
        }
        /// <summary>
        /// Token from Mirror: https://mirror-networking.com/docs/ 
        /// </summary>
        void OnDestroy()
        {
            Destroy(cachedMaterial);
        }
        /// <summary>
        /// Token from Mirror: https://mirror-networking.com/docs/ 
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            characterController = GetComponent<CharacterController>();
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0f, 3f, -7f);
            Camera.main.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
        }

        /// <summary>
        /// override methode
        /// When the player stand on the "Magic Circle" :
        /// change the game type of in the client variable accordingly
        /// and show the startgame canvas to allow players to join host a party and start a game
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (!isLocalPlayer || characterController == null) return;

            clientVar.setGamesRef(this.Games);
            if (isAvaiableModule(other.gameObject.tag))
            {
                clientVar.setgameType(other.gameObject.tag);
                globalCanvas.ToggleCanvas("gameOn");
                clientVar.updateStartGameUI();
                clientVar.partyTextField.DeactivateInputField();
            }
        }

        /// <summary>
        /// Check if the Module exists on the Database this Methode is needed to find out which Magic circle the player is standing on
        /// </summary>
        private bool isAvaiableModule(string moduleName) {
            if (Modules.Contains(moduleName))
            {   
                return true;
            }
            return false;
        }

        /// <summary>
        /// Override Method
        /// Disables the startgame canvas when the player leave the "Magic Circle"
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (!isLocalPlayer || characterController == null) return;
            if (isAvaiableModule(other.gameObject.tag))
            {
                clientVar.setgameType(null);
                globalCanvas.ToggleCanvas("gameOff");
            }
        }
    }
