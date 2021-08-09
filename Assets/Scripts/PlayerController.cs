using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController Player;
    #endregion

    public float speed = 6.0F;
    public float runMultiplier = 0.5f;
    public float jumpHeight = 5f;
    public float gravity = -50.0f;
    public float floatGravity = -75.0f;
    public float floatAirtimeLoss = 0.1f;
    public int maxCoyote = 5;
    public float maxAngle = 0.25f;
    public float flowStrength = 20f;

    [Header("- Player Model -")]
    public Transform playerModel;
    public Animator animator;
    public VisualEffect dustVFX;

    [Header("- Collectibles Indicator -")]
    public CollectiblesController collectiblesController;

    [Header("- Audio Clips -")]
    public AudioClip[] footstepClips;
    public AudioClip[] swimSound;
    public AudioClip jumpSound;
    public AudioClip landSound;


    [Header("- Debug Variables -")]
    public int coyoteTimer;
    public bool _hasJumped = false;
    public bool _grounded;
    public bool _allowJump = true;
    public bool _climbing = false;
    public bool _floating = false;
    public bool _inWater = false;
    //public bool _onMoss = false;
    public float tempFloatGrav;
    CharacterController character;
    Transform cam;
    AudioSource playerAudio;
    Vector3 move = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector3 collisionDirection = Vector3.zero;

    private void Awake()
    {
        Player = this;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    private void Start()
    {
        character = this.GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerAudio = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Quit"))
        {
            Application.Quit();
        }

        _grounded = character.isGrounded;
        if (_grounded && !_inWater)
        {
            animator.ResetTrigger("Jump");
            dustVFX.SetFloat("Velocity", move.magnitude);

            if (_hasJumped || character.velocity.y < gravity * 0.25f) //checks if on landing frame
            {
                playerAudio.PlayOneShot(landSound);
                dustVFX.SendEvent("Burst");
            }

            coyoteTimer = 0;
            _hasJumped = false;

            if (moveDirection.y < 0)
            {
                moveDirection.y = 0f;
            }
        }
        else
        {
            dustVFX.SetFloat("Velocity", 0);
        }

        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = Quaternion.Euler(0, cam.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        playerModel.LookAt(new Vector3(playerModel.position.x + move.x, playerModel.position.y, playerModel.position.z + move.z));
        move = Vector3.ClampMagnitude(move, 1.0f);        //normalise diagonal movement

        if (!_climbing) //disallow running while climbing or midair
        {
            move *= 1 + Input.GetAxis("Run") * (runMultiplier - 1); //apply dynamic run speed
        }


        move += collisionDirection * 0.5f;

        //playerModel.LookAt(new Vector3(playerModel.position.x + move.x, playerModel.position.y, playerModel.position.z + move.z));
        character.Move(move * Time.deltaTime * speed);

        if (Input.GetButtonDown("Jump") && coyoteTimer <= maxCoyote && !_hasJumped && _allowJump)
        {
            moveDirection.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            _hasJumped = true;
            playerAudio.PlayOneShot(jumpSound);
            animator.SetTrigger("Jump");
            dustVFX.SendEvent("Burst");
        }
        else if (!_grounded && Input.GetButtonDown("Jump")) //Midair button Press, start floating
        {
            _floating = true;
            tempFloatGrav = floatGravity;
        }
        else if (Input.GetButton("Jump") && !_grounded && _floating)
        {
            moveDirection.y = tempFloatGrav * Time.deltaTime;
            tempFloatGrav *= 1.0f + (floatAirtimeLoss * Time.deltaTime);
        }
        else
        {
            moveDirection.y += gravity * Time.deltaTime;
            _floating = false;
        }

        character.Move(moveDirection * Time.deltaTime);
        coyoteTimer += 1;

        animator.SetFloat("velocity", move.magnitude);
        animator.SetFloat("running", Input.GetAxis("Run"));
        animator.SetBool("gliding", _floating);
        animator.SetBool("swimming", _inWater);
        animator.SetBool("climbing", _climbing);
        animator.SetBool("grounded", _grounded);

        //if (character.isGrounded)
        //{
        //    moveDirection = Quaternion.Euler(0, cam.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //    //playerAnimator.SetFloat("Dir X", Input.GetAxis("Horizontal"));
        //    //playerAnimator.SetFloat("Dir Y", Input.GetAxis("Vertical"));
        //    //if (moveDirection != Vector3.zero)
        //    //{
        //    //    playerAnimator.SetBool("walking", true);
        //    //    //print("walking");
        //    //}
        //    //else
        //    //{
        //    //    playerAnimator.SetBool("walking", false);
        //    //}
        //    //print((int)moveDirection.normalized.x + ", " + (int)moveDirection.normalized.z);   
        //    moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);
        //    if (Input.GetButton("Run"))
        //    {
        //        moveDirection *= runSpeed;
        //    }
        //    moveDirection = transform.TransformDirection(moveDirection);

        //    moveDirection *= speed;
        //}
        //moveDirection.y -= gravity * Time.deltaTime;
        //character.Move(moveDirection * Time.deltaTime);
        //playerModel.LookAt(new Vector3(playerModel.position.x + moveDirection.x, playerModel.position.y, playerModel.position.z + moveDirection.z));
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //print(hit.normal + ", Mag: " + hit.normal.magnitude);
        if (hit.normal.y < maxAngle && hit.normal.y >= 0 && !_hasJumped)
        {
            if (!hit.collider.CompareTag("Moss"))
            {
                collisionDirection = hit.normal;
                collisionDirection.y = 0f;
                //_onMoss = true;
            }
            //print("too steep!!");
            _allowJump = false;
            //character.slopeLimit = slopeLimit;
            _climbing = true;
        }
        else if (hit.collider.CompareTag("Water"))
        {
            //print("Hehe sploosh");
            //print(hit.normal + ", Mag: " + hit.normal.magnitude);
            collisionDirection = hit.normal * flowStrength;
            collisionDirection.y = 0f;
            _allowJump = false;
            _inWater = true;
        }
        else
        {
            //_onMoss = false;
            _inWater = false;
            _allowJump = true;
            _climbing = false;
            //character.slopeLimit = 65;
            collisionDirection = Vector3.zero;
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _allowJump = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _allowJump = true;
        }
    }
    */
}
