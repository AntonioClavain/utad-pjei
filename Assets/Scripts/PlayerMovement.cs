using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public enum WorldReference { WORLD, LOCAL};
    public enum Axis { RIGHT, UP, FORWARD};

    public WorldReference worldReference;

    public Axis axis;

    [Header("Input")]
    public string inputMovementName = "Horizontal";
    public string inputJumpName = "Jump";
    public string inputCrouch = "Crouch";

    [Header("Grounded")]
    public float maxMovementSpeed = 5;
    public float movementAcceleration = 3;
    public float movementDeceleration = 4;
    public float movementDecelerationChangeDirection = 8;

    [Header("On Air")]
    public float maxAirMovementSpeed = 3;
    public float airMovementAcceleration = 1;
    public float airMovementDeceleration = 2;
    public float airMovementDecelerationChangeDirection = 4;

    [Header("Jump")]
    public float jumpForce = 10;
    public float jumpSpeedDecrease = 5;
    public float secondJumpForce = 5;
    public float maxJumpSpeed = 5;
    public float maxFallSpeed = -10;
    public float gravity = 9.81f;
    public LayerMask groundLayers;
    public float distanceToBeOnGround = 0.1f;

    [Header("Crouch")]
    public CrouchType crouchType;
    [Range(0, 1)]
    public float crouchHeight = 0.5f;
    public float crouchMaxMovementSpeed = 3;
    public enum CrouchType { Hold, Toggle };

    [Header("Wall Jump Detection")]
    public float wallDetectionDistance;
    public float wallSlideFallSpeedFactor;
    public LayerMask wallLayers;

    private float _originalHeight;

    private float _currentSpeed;
    private float _currentJumpSpeed;
    private bool _doubleJump;
    private bool _playerCrouch;

    private CharacterController _charController;

    private Ray _ray;

    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();

        _originalHeight = _charController.height;

        _ray = new Ray();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis(inputMovementName);
        bool jump = Input.GetButtonDown(inputJumpName);

        bool playerIsGrounded = IsGrounded();

        bool playerIsWallSliding = IsWallSliding(inputX);

        // Apply Crouch
        if(crouchType == CrouchType.Hold)
        {
            _playerCrouch = Input.GetButton(inputCrouch);

            if (_playerCrouch)
            {
                _charController.height = _originalHeight * crouchHeight;
            }
            else
            {
                _charController.height = _originalHeight;
            }
        }
        else if(crouchType == CrouchType.Toggle)
        {
            if (Input.GetButtonDown(inputCrouch))
            {
                _playerCrouch = true;

                _charController.height = _originalHeight * crouchHeight;
            }
            if (Input.GetButtonUp(inputCrouch))
            {
                _playerCrouch = false;

                _charController.height = _originalHeight;
            }
        }

        // Apply gravity if it's on air
        if (!playerIsGrounded)
        {
            if(_currentJumpSpeed > 0)
            {
                _currentJumpSpeed -= jumpSpeedDecrease * Time.deltaTime;
            }
            else
            {
                if (playerIsWallSliding)
                {
                    _currentJumpSpeed -= gravity * Time.deltaTime * wallSlideFallSpeedFactor;
                    _doubleJump = false;
                }
                else
                {
                    _currentJumpSpeed -= gravity * Time.deltaTime;
                }
            }
            
        }
        else
        {
            _currentJumpSpeed = 0;
            _doubleJump = false;
        }

        // Apply Jump
        if (jump)
        {
            if (playerIsGrounded)
            {
                _currentJumpSpeed = jumpForce;
            }
            else if (playerIsWallSliding)
            {
                _currentJumpSpeed = jumpForce;
                _currentSpeed = maxMovementSpeed * Mathf.Sign(-inputX);
            }
            else if (!_doubleJump)
            {
                _doubleJump = true;
                _currentJumpSpeed = secondJumpForce;
            }
        }

        // Calculate movement speed
        if(inputX != 0)
        {
            if(Mathf.Sign(_currentSpeed) == Mathf.Sign(inputX))
            {
                if (playerIsGrounded)
                {
                    _currentSpeed += inputX * movementAcceleration * Time.deltaTime;
                }
                else
                {
                    _currentSpeed += inputX * airMovementAcceleration * Time.deltaTime;
                }
            }
            else
            {
                if (playerIsGrounded)
                {
                    _currentSpeed += inputX * movementDecelerationChangeDirection * Time.deltaTime;
                }
                else
                {
                    _currentSpeed += inputX * airMovementDecelerationChangeDirection * Time.deltaTime;
                }
            }
        }
        else
        {
            if (playerIsGrounded)
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, movementDeceleration * Time.deltaTime);
            }
            else
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, airMovementDeceleration * Time.deltaTime);
            }
        }

        if (_playerCrouch)
        {
            _currentSpeed = Mathf.Min(_currentSpeed, crouchMaxMovementSpeed);
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, -maxMovementSpeed, maxMovementSpeed);

        Vector3 movementDirection = Vector3.zero;
        if(worldReference == WorldReference.LOCAL)
        {
            if(axis == Axis.RIGHT)
            {
                movementDirection = transform.right;
            }
            else if(axis == Axis.UP)
            {
                movementDirection = transform.up;
            }
            else
            {
                movementDirection = transform.forward;
            }
        }
        else
        {
            if (axis == Axis.RIGHT)
            {
                movementDirection = Vector3.right;
            }
            else if (axis == Axis.UP)
            {
                movementDirection = Vector3.up;
            }
            else
            {
                movementDirection = Vector3.forward;
            }
        }

        // Apply movement
        _charController.Move(Time.deltaTime * (_currentSpeed * movementDirection + Mathf.Clamp(_currentJumpSpeed, maxFallSpeed, maxJumpSpeed) * Vector3.up));
    }

    private bool IsGrounded()
    {
        _ray.origin = transform.position - Vector3.up * _charController.height / 2;
        _ray.direction = -Vector3.up;
        if (Physics.Raycast(_ray, distanceToBeOnGround, groundLayers))
        {
            return true;
        }
        return false;
    }

    private bool IsWallSliding(float inputX)
    {
        // Check if the player is sliding through a wall
        _ray.origin = transform.position;
        _ray.direction = transform.right;
        if (Physics.Raycast(_ray, wallDetectionDistance, wallLayers) && inputX > 0)
        {
            _currentSpeed = 0;
            return true;
        }
        else
        {
            _ray.origin = transform.position;
            _ray.direction = -transform.right;
            if (Physics.Raycast(_ray, wallDetectionDistance, wallLayers) && inputX < 0)
            {
                _currentSpeed = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
