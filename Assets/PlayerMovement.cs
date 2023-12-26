using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    bool readyToJump;
    bool isCrouching;
    bool isSprinting;

    [Header("Crouching")]
    public float playerScaleCrouch;
    public float crouchSpeed;
    private Vector3 playerScaleNormal = new Vector3(1f, 1f, 1f);

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool isAir;
    public float airDrag;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;
    private bool sliding;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public Transform orientation;
    bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public float SlopeMovementSpeed;
    private RaycastHit slopeHit;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    private void Start()
    {
        moveSpeed = walkSpeed;
        sprintSpeed = walkSpeed - walkSpeed + sprintSpeed;
        crouchSpeed = walkSpeed - walkSpeed + crouchSpeed;

        isSprinting = false;
        readyToJump = true;
        rb = GetComponent<Rigidbody>();

        // Freeze Player to not fall over
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
        HandleSlipperyMovement();
        Jump();
        Crouch();
        Sprint();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        // A and D keys
        horizontalInput = Input.GetAxisRaw("Horizontal");
        // W and S keys
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // Calculating Direction of Movement
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope())
        {

            rb.AddForce(SlopeMovementSpeed * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);

            // turn gravity off while on slope
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // in air
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    // Limit speed
    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        
        // Gets only horizontal components of Speed
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if necessary
        if (flatVel.magnitude > moveSpeed)
        {
            // Wenn Geschwindigkeit moveSpeed überschreitet, begrenzt sie diese auf moveSpeed und behält dabei die Richtung bei
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);

        }
    }

    private void HandleSlipperyMovement()
    {
        // ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // handle drag
        if (isGrounded)
            // rb.drag gets variable groundDrag (reason: groundDrag is automatically 0 and can be changed in inspector) 
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void HandleAirDrag()
    {
        
    }

    private void Jump()
    {
        // wenn springen
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;

            //reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            // nur 1 mal tun
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    // Serves the Jump function
    private void ResetJump()
    {
        readyToJump = true;
    }


    private void Crouch()
    {
        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            isCrouching = true;

            transform.localScale = new Vector3(transform.localScale.x, playerScaleCrouch, transform.localScale.z);
            rb.AddForce(Vector3.down * 15f, ForceMode.Impulse);

            moveSpeed = crouchSpeed;
        }

        if (Input.GetKeyUp(crouchKey) && isGrounded)
        {
            isCrouching = false;

            transform.localScale = playerScaleNormal;
        }
    }

    private void Sprint()
    {
        if (!isCrouching && Input.GetKeyDown(sprintKey) && isGrounded)
        {
            // Necessary for Slide function
            isSprinting = true;

            moveSpeed = sprintSpeed;
        }
        if (!isCrouching && Input.GetKeyUp(sprintKey) && isGrounded)
        {
            // Necessary for Slide function
            isSprinting = false;

            moveSpeed = walkSpeed;
        }
    }

    private bool OnSlope()
    {

        // slopeHit stores information of the object we hit 
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), Color.red);
            // calculate steepness of slope
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            // bool should return true if angle smaller then our maxSlopeAngle
            return angle < maxSlopeAngle && angle != 0;
        }
        // if raycast doesnt hit anything return false
        return false;
    }

    // now we can find correct direction relative to the slope
    private Vector3 GetSlopeMoveDirection()
    {
        // with ProjectOnPlane we project normal move direction onto slope
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    
}