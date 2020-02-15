using UnityEngine;

namespace Mirror
{
    [RequireComponent(typeof(CharacterController))]
    /**
     * PlayerMovement class to set the Movement of the player 
     * (class token from mirror: https://mirror-networking.com/docs/)
     */
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
        /// 
        /// </summary>
        CharacterController characterController;
        /// <summary>
        /// 
        /// </summary>
        GameObject controllerColliderHitObject;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private Animator m_animator;
        //[SerializeField] private Rigidbody m_rigidBody;
        
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
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            GameObject GM = GameObject.FindWithTag("GlobalManager");
            if (GM != null)
            {
                globalCanvas = GM.GetComponent<GlobalManager>();
            }
            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            clientVar = globalCanvas.GetComponent<Client>();
        }
        
        public void Initialize(GameObject character)
        {
            m_animator = character.GetComponent<Animator>();
            //m_rigidBody = character.GetComponent<Rigidbody>();
        }
        
        void Awake()
        {
            if(!m_animator) { gameObject.GetComponent<Animator>(); }
            //if(!m_rigidBody) { gameObject.GetComponent<Animator>(); }
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
                /*if (!wasGrounded && isGrounded)
                {
                    m_animator.SetTrigger("Land");
                }
                if (!isGrounded && wasGrounded)
                {
                    m_animator.SetTrigger("Jump");
                }*/
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
                //Vector3 direction = camera.forward * vertical + camera.right * horizontal;
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
        ///
        /// change the game type of in the client variable accordingly
        /// and show the startgame canvas to allow players to join host a party and start a game
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (!isLocalPlayer || characterController == null) return;
            if (other.gameObject.tag.Equals("MATH") || other.gameObject.tag.Equals("NTG") || other.gameObject.tag.Equals("OOP") || other.gameObject.tag.Equals("GDI") || other.gameObject.tag.Equals("RNAI"))
            {
                clientVar.setgameType(other.gameObject.tag);
                globalCanvas.ToggleCanvas("gameOn");
                clientVar.updateStartGameUI();
            }
        }
        
        /// <summary>
        /// override methode
        /// disable the startgame canvas when the player leave the "Magic Circle"
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (!isLocalPlayer || characterController == null) return;
            if (other.gameObject.tag.Equals("MATH") || other.gameObject.tag.Equals("NTG") || other.gameObject.tag.Equals("OOP") || other.gameObject.tag.Equals("GDI") || other.gameObject.tag.Equals("RNAI"))
            {
                clientVar.setgameType(null);
                globalCanvas.ToggleCanvas("gameOff");
            }
        }
    }
}