﻿using UnityEngine;

namespace Mirror
{
    [RequireComponent(typeof(CharacterController))]
    /**
     * PlayerMovement class to set the Movement of the player (class token from mirror)
     */
    public class PlayerMovement : NetworkBehaviour
    {
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

        /**
         * Used from Mirror
         */
        void SetColor(Color color)
        {
            if (cachedMaterial == null) cachedMaterial = GetComponent<Renderer>().material;
            cachedMaterial.color = color;
        }

        /**
         * Used from Mirror
         */
        void OnDisable()
        {
            if (isLocalPlayer)
            {
                Camera.main.transform.SetParent(null);
                Camera.main.transform.localPosition = new Vector3(0f, 50f, 0f);
                Camera.main.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
        }

        /**
         * Used from Mirror
         */
        void OnDestroy()
        {
            Destroy(cachedMaterial);
        }

        CharacterController characterController;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            characterController = GetComponent<CharacterController>();

            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0f, 3f, -8f);
            Camera.main.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
        }

        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        public float turnSpeedAccel = 10f;
        public float turnSpeedDecel = 10f;
        public float maxTurnSpeed = 150f;

        [Header("Jump Settings")]
        public float jumpSpeed = 0f;
        public float jumpFactor = .025F;

        [Header("Diagnostics")]
        public float horizontal = 0f;
        public float vertical = 0f;
        public float turn = 0f;
        public bool isGrounded = true;
        public bool isFalling = false;

        /**
         * Update player position 
         */
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

        /**
         * Used from Mirror
         */
        void FixedUpdate()
        {
            
            if (!isLocalPlayer || characterController == null) return;

            if (clientVar.clientMessageTF.isFocused) return; //stops the player from moving when writing on the chat

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

        GameObject controllerColliderHitObject;



        /**
         * override methode
         * 
         * When the player stand on the "Magic Circle" :
         * 
         * change the game type of in the client variable accordingly
         * and show the startgame canvas to allow players to join host a party and start a game
         */
       private void OnTriggerEnter(Collider other)
        {
            if (!isLocalPlayer || characterController == null) return;

            if (other.gameObject.tag.Equals("MATH") || other.gameObject.tag.Equals("NTG") || other.gameObject.tag.Equals("OOP"))
            {
                clientVar.setgameType(other.gameObject.tag);
                globalCanvas.ToggleCanvas("gameOn");
                clientVar.updateStartGameUI();
            }
        }


        /*
         * override methode
         * disable the startgame canvas when the player leave the "Magic Circle"
         * 
         */
    private void OnTriggerExit(Collider other)
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

