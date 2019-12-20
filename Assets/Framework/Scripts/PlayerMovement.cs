using UnityEngine;

namespace Mirror
{
    [RequireComponent(typeof(CharacterController))]
    /**
     * PlayerMovement class to set the Movement of the player (class token from mirror)
     */
    public class PlayerMovement : NetworkBehaviour
    {
        /// <summary>
        /// *hier kommt noch was*
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
        /// *hier kommt noch was*
        /// a reference to the client class which contains all of the client information it is used here to change the scene/canvas of the player,
        /// and to fix the problem where the player moves automaticly, when he types in the chat
        /// </summary>
        private Client clientVar;

        /// <summary>
        /// *hier kommt noch was*
        /// </summary>
        void Start()
        {
            //globalCanvas = gameObject.GetComponent<GlobalManager>();
            GameObject GM = GameObject.FindWithTag("GlobalManager");
            if (GM != null)
            {
                globalCanvas = GM.GetComponent<GlobalManager>();
            }
            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            clientVar = globalCanvas.GetComponent<Client>();
        }

        /// <summary>
        /// Used from Mirror /*hier kommt noch was*
        /// </summary>
        /// <param name="color"></param>
        void SetColor(Color color)
        {
            if (cachedMaterial == null) cachedMaterial = GetComponent<Renderer>().material;
            cachedMaterial.color = color;
        }
        
        /// <summary>
        /// Used from Mirror /*hier kommt noch was*
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
        /// Used from Mirror /*hier kommt noch was*
        /// </summary>
        void OnDestroy()
        {
            Destroy(cachedMaterial);
        }
        /// <summary>
        /// *hier kommt noch was*
        /// </summary>
        CharacterController characterController;
        
        /// <summary>
        /// 
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            characterController = GetComponent<CharacterController>();
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0f, 3f, -8f);
            Camera.main.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
        }

        /// <summary>
        /// 
        /// </summary>
        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        /// <summary>
        /// 
        /// </summary>
        public float turnSpeedAccel = 10f;
        /// <summary>
        /// 
        /// </summary>
        public float turnSpeedDecel = 10f;
        /// <summary>
        /// 
        /// </summary>
        public float maxTurnSpeed = 150f;

        /// <summary>
        /// 
        /// </summary>
        [Header("Jump Settings")]
        public float jumpSpeed = 0f;
        /// <summary>
        /// 
        /// </summary>
        public float jumpFactor = .025F;

        /// <summary>
        /// 
        /// </summary>
        [Header("Diagnostics")]
        public float horizontal = 0f;
        /// <summary>
        /// 
        /// </summary>
        public float vertical = 0f;
        /// <summary>
        /// 
        /// </summary>
        public float turn = 0f;
        /// <summary>
        /// 
        /// </summary>
        public bool isGrounded = true;
        /// <summary>
        /// 
        /// </summary>
        public bool isFalling = false;

        /// <summary>
        /// Update player position /*hier kommt noch was*
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
                jumpSpeed += jumpFactor;
            else if (isGrounded)
                isFalling = false;
            else
            {
                isFalling = true;
                jumpSpeed = 0;
            }
        }

        /// <summary>
        /// Used from Mirror /*hier kommt noch was*
        /// </summary>
        void FixedUpdate()
        {
            if (!isLocalPlayer || characterController == null) return;
            if (clientVar.clientMessageTF.isFocused) return;
            transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);
            Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction = transform.TransformDirection(direction);
            direction *= moveSpeed;

            if (jumpSpeed > 0)
                characterController.Move(direction * Time.fixedDeltaTime);
            else
                characterController.SimpleMove(direction);

            isGrounded = characterController.isGrounded;
        }
        
        /// <summary>
        /// 
        /// </summary>
        GameObject controllerColliderHitObject;
        
        /// <summary>
        /// override methode
        /// When the player stand on the "Magic Circle" :
        ///
        /// change the game type of in the client variable accordingly
        /// and show the startgame canvas to allow players to join host a party and start a game
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other) /*hier kommt noch was*/
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
        private void OnTriggerExit(Collider other) /*hier kommt noch was*/
        {
            if (!isLocalPlayer || characterController == null) return;

            if (other.gameObject.tag.Equals("MATH") || other.gameObject.tag.Equals("NTG") || other.gameObject.tag.Equals("OOP"))
            {
                clientVar.setgameType(null);
                globalCanvas.ToggleCanvas("gameOff");
            }
        }
    }
}