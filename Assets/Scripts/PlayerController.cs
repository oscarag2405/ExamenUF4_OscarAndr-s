/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private bool isGrounded;

    private float maxFallSpeed = 20f;
    private float currentFallSpeed = 0f;

    private CharacterController _playerController;
    private Vector3 _moveDirection = Vector3.zero;
    private float _vertical;
    private float _horizontal;

    private void Start()
    {
        _playerController = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        _vertical = _joystick.Vertical + Input.GetAxis("Vertical");
        _horizontal = _joystick.Horizontal; //es mou a on mira la camera.

        //vector direcció. direcció en X i Z. 
        Vector3 cameraForward = Vector3.Scale(_cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirection = (_vertical * cameraForward) + (_horizontal * _cameraTransform.right);

        //El player es mou en eix x i z
        _moveDirection.x = moveDirection.x *moveSpeed;
        _moveDirection.z = moveDirection.z * moveSpeed;

        // el character controller ja té un isGrounded!
        if (_playerController.isGrounded)
        {
            currentFallSpeed = 0f; //si està al terra.
            // Saltar per PC
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("SALTA");
                Jump();
            }
            //Saltar per Android/iOS
            foreach(Touch t in Input.touches)
            {
                if(t.tapCount == 2)
                {
                    Jump();
                }
            }
        }
        else //si el jugador NO està a terra
        {
            moveDirection.x *= 0.8f; //que vagi més lent al saltar. 
            moveDirection.z *= 0.8f;
            //si la velocitat del personatge supera la velocitat de caiguda, tornem a agafar la velocitat. 
            if(moveDirection.y < -maxFallSpeed)
            {
                moveDirection.y = -maxFallSpeed;
            }
            else //si no ha arribat al límit del salt
            {
                currentFallSpeed += Time.deltaTime * 20.0f; //augmenti el salt.
                moveDirection.y -= currentFallSpeed * Time.deltaTime; 
            }
        }

        //si toquem el joystick en qualsevol moment
        if (_horizontal != 0 || _vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        }

        //Fem que es mogui el personatge amb la funció Move() del Character Controller
        _playerController.Move(_moveDirection * Time.deltaTime);


        AnimatePlayer();


    }
    void AnimatePlayer()
    {
        _animator.SetBool("isGrounded", _playerController.isGrounded);

        if (_playerController.isGrounded)
        {
            _animator.SetBool("isJumping", false);

            if (_horizontal != 0 || _vertical != 0)
            {
                //controlar la velocitat d'animació a l'animació amb el joystick
                _animator.speed = Mathf.Min(Mathf.Max(Mathf.Abs(_vertical), Mathf.Abs(_horizontal)) + 0.1f, 1);
                // per a que miri 
                _animator.SetBool("isRunning", true);
            }
            else
            {
                _animator.speed = 20;
                _animator.SetBool("isRunning", false);

            }
        }
    }
    void Jump()
    {
        currentFallSpeed = 3; //velocitat de caiguda
        _moveDirection.y = jumpForce;
        _animator.SetBool("isJumping", true);
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Vector3 externalMoveSpeed;

    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private bool Grounded;

    [SerializeField] private AudioSource walk;

    private float _maxFallSpeed = 20f;
    private float _currentFallSpeed = 0f;

    private CharacterController _controller;
    [SerializeField] private Vector3 _moveDirection = Vector3.zero;
    public float _vertical;
    public float _horizontal;

    public bool inWater;

    private float _coyoteTime = 0.4f;
    private float _jumpingTime = 0f;
    private float _timeSinceGrounded;
    private bool slope = false;

    public float input;
    private bool jumping;



    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        jumping = false;

    }

    private void FixedUpdate()
    {
        Grounded = _controller.isGrounded;

        _vertical = _joystick.Vertical + Input.GetAxis("Vertical");
        _horizontal = _joystick.Horizontal;

        input = Mathf.Abs(_vertical) + Mathf.Abs(_horizontal);

        Vector3 cameraForward = Vector3.Scale(_cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirection = _vertical * cameraForward + _horizontal * _cameraTransform.right;
        _moveDirection.x = moveDirection.x * _moveSpeed; _moveDirection.z = moveDirection.z * _moveSpeed;

        if (_controller.isGrounded)
        {
            if (_jumpingTime < 0) jumping = false;
            _currentFallSpeed = 0f;
            _timeSinceGrounded = 0f;

        }
        else
        {
            _timeSinceGrounded += Time.deltaTime;
            _moveDirection.x *= 0.7f; _moveDirection.z *= 0.7f;

            if (_moveDirection.y < -_maxFallSpeed) _moveDirection.y = -_maxFallSpeed;
            else
            {
                _currentFallSpeed += Time.deltaTime * 20f;
                _moveDirection.y -= _currentFallSpeed * Time.deltaTime;
            }
        }

        if ((_horizontal != 0 || _vertical != 0))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z));
        }

        //Fall out with angle
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 1f))
        {
            if (hit.normal.y < Mathf.Cos(50 * Mathf.Deg2Rad))
            {
                var slideDirection = new Vector3(hit.normal.x, -hit.normal.y, hit.normal.z);
                slope = true;
                _controller.Move(slideDirection * Time.fixedDeltaTime);
            }
            else
            {
                slope = false;
            }
        }
        _controller.Move(_moveDirection * Time.deltaTime + externalMoveSpeed * Time.deltaTime);

       // if (transform.position.y < -100) Control.Death();

    }
    private void Update()
    {
        if (_jumpingTime > 0) _jumpingTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && !jumping) jump();
        foreach (Touch t in Input.touches) if (t.tapCount == 2 && !jumping) jump();
       //CheckPlayerTouch();
    }

    private void LateUpdate()
    {
        animate();
    }
    private void animate()
    {

        _animator.SetBool("Grounded", !jumping && (_controller.isGrounded || _timeSinceGrounded < _coyoteTime));
        _animator.SetBool("InWater", inWater);

        if (_controller.isGrounded)
        {

            if (_jumpingTime < 0) _animator.SetBool("Jump", false);

            if (_horizontal != 0 || _vertical != 0)
            {
                _animator.speed = Mathf.Min(Mathf.Max(Mathf.Abs(_vertical), Mathf.Abs(_horizontal)) + 0.1f, 1);
                _animator.SetBool("Running", true);
            }
            else
            {
                _animator.speed = 1;
                _animator.SetBool("Running", false);
            }

        }
        else
        {
            _animator.speed = 1;
            //jumping = true;
        }
    }
    private void jump()
    {

        if (slope) return;
        if (inWater) return;
        if (_timeSinceGrounded > _coyoteTime) return;

        //GetComponent<Animator>().Play("Jumping", -1, 0);
        jumping = true;
        _timeSinceGrounded = 1;
        _currentFallSpeed = 3;
        _moveDirection.y = _jumpForce;
        _jumpingTime = 0.5f;
        //AudioManager.PlayJump();
        _animator.SetBool("Jump", true);
    }
}