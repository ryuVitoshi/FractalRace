using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class ShipControls : MonoBehaviour
{
    private PhotonView view;
    //[SerializeField] private GameObject _camera;

    private Rigidbody rigidBody;
    [Header("Forward / Backward movement")]
    [SerializeField] private float _accelForward;
    [SerializeField] private float _accelBackward;
    [SerializeField] private float _deceleration;
    [SerializeField] private float _forwardMaxSpeed;
    [SerializeField] private float _backwardMaxSpeed;
    private float _velocityPercentage = 0;

    [Header("Mouse movement")]
    [SerializeField] private float _mouseSpeed;
    [SerializeField] private float _mouseSmooth;
    [SerializeField] private float _mouseMaxSpeed;

    [Header("Rotation movement")]
    [SerializeField] private float _rotationMaxSpeed;
    [SerializeField] private float _rotationAccel;
    [SerializeField] private float _rotationDecel;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _stabilisationSpeed;
    private float _stableAngleZ = 0;
    private float _rotationVelocity = 0;

    private Vector2 _rotation = Vector2.zero;
    private Vector2 _mouseMovement = Vector2.zero;
    private Vector2 _rotaionSpeed = Vector2.zero;

    [Header("Test Fields")]
    [SerializeField] private float _angle1;
    [SerializeField] private float _angle2;

    private Vector3 _velocity = Vector3.zero;

    public bool IsMine()
    {
        return view.IsMine;
    }
    
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (view.IsMine)
        {
            _velocityPercentage = _velocity.magnitude/_forwardMaxSpeed;
            HandleMouseInputs();
            HandleMovementInputs();
            HandleRotationInputs();
            //RotationStabilisation();
            //DrawLine(transform.position, transform.forward*3);
        }
    }

    void HandleMouseInputs()
    {
        _mouseMovement.x = _mouseSpeed * Mathf.Clamp(Input.GetAxis("Mouse X"), -_mouseMaxSpeed, _mouseMaxSpeed) * Time.deltaTime;
        _mouseMovement.y = _mouseSpeed * Mathf.Clamp(Input.GetAxis("Mouse Y"), -_mouseMaxSpeed, _mouseMaxSpeed) * Time.deltaTime;

        // Lerp
        //_rotation = Vector2.Lerp(_rotation, _mouseMovement, _mouseSmooth);

        _rotation.x = Mathf.SmoothDampAngle(_rotation.x, _mouseMovement.x, ref _rotaionSpeed.x, _mouseSmooth);
        _rotation.y = Mathf.SmoothDampAngle(_rotation.y, _mouseMovement.y, ref _rotaionSpeed.y, _mouseSmooth);

        transform.RotateAround(transform.position, transform.right, -_rotation.y * _velocityPercentage);
        transform.RotateAround(transform.position, transform.up, _rotation.x * _velocityPercentage);
    }

    // Manages player's movement based on input, adjusting velocity and position
    void HandleMovementInputs()
    {
        Vector3 movementDirection = transform.forward;
        _velocity = movementDirection * _velocity.magnitude;

        // Handle forward acceleration/movement
        if (Input.GetKey(KeyCode.W))
        {
            _velocity += movementDirection * _accelForward * Time.deltaTime;
            _velocity = Vector3.ClampMagnitude(_velocity, _backwardMaxSpeed);
        }
        // Handle forward deceleration
        else if (Vector3.Angle(_velocity, movementDirection) < 90 && _velocity != Vector3.zero)
        {
            _velocity -= movementDirection * _deceleration * Time.deltaTime;
            if (Vector3.Angle(_velocity, movementDirection) > 90)
            {
                _velocity = Vector3.zero;
            }
        }

        // Handle backward acceleration/movement
        if (Input.GetKey(KeyCode.S))
        {
            _velocity -= movementDirection * _accelBackward * Time.deltaTime;
            _velocity = Vector3.ClampMagnitude(_velocity, _forwardMaxSpeed);
        }
        // Handle backward deceleration
        else if (Vector3.Angle(_velocity, -movementDirection) < 90 && _velocity != Vector3.zero)
        {
            _velocity += movementDirection * _deceleration * Time.deltaTime;
            if (Vector3.Angle(_velocity, -movementDirection) > 90)
            {
                _velocity = Vector3.zero;
            }
        }

        // Apply velocity
        transform.position += _velocity * Time.deltaTime;
    }

    // Controls the rotation of an object based on keyboard input for left and right rotation
    void HandleRotationInputs()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _rotationVelocity += _rotationAccel * Time.deltaTime;
            _rotationVelocity = Mathf.Clamp(_rotationVelocity, -_rotationMaxSpeed, _rotationMaxSpeed);
            transform.RotateAround(transform.position, transform.forward, _rotationVelocity * Time.deltaTime);
            _stableAngleZ = transform.eulerAngles.z;
        }
        else if (_rotationVelocity > 0)
        {
            _rotationVelocity -= _rotationDecel * Time.deltaTime;
            _rotationVelocity = Mathf.Clamp(_rotationVelocity, 0.0f, _rotationMaxSpeed);
            transform.RotateAround(transform.position, transform.forward, _rotationVelocity * Time.deltaTime);
            _stableAngleZ = transform.eulerAngles.z;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _rotationVelocity -= _rotationAccel * Time.deltaTime;
            _rotationVelocity = Mathf.Clamp(_rotationVelocity, -_rotationMaxSpeed, _rotationMaxSpeed);
            transform.RotateAround(transform.position, transform.forward, _rotationVelocity * Time.deltaTime);
            _stableAngleZ = transform.eulerAngles.z;
        }
        else if (_rotationVelocity < 0)
        {
            _rotationVelocity += _rotationDecel * Time.deltaTime;
            _rotationVelocity = Mathf.Clamp(_rotationVelocity, -_rotationMaxSpeed, 0.0f);
            transform.RotateAround(transform.position, transform.forward, _rotationVelocity * Time.deltaTime);
            _stableAngleZ = transform.eulerAngles.z;
        }
    }

    void RotationStabilisation()
    {
        //print("angl: "+transform.eulerAngles.z + "  stabl_angl: "+_stableAngleZ);
        //print(AngleDifference(transform.eulerAngles.z, _stableAngleZ));
        if (AngleDifference(transform.eulerAngles.z, _stableAngleZ) < 0)
        {
            transform.eulerAngles += new Vector3(0,0,1) * _stabilisationSpeed * Time.deltaTime;
            if (AngleDifference(transform.eulerAngles.z, _stableAngleZ) > 0)
            {
                transform.eulerAngles = new Vector3(0,0,1) * _stableAngleZ;
            }
        }

        if (AngleDifference(transform.eulerAngles.z, _stableAngleZ) > 0)
        {
            transform.eulerAngles -= new Vector3(0,0,1) * _stabilisationSpeed * Time.deltaTime;
            if (AngleDifference(transform.eulerAngles.z, _stableAngleZ) < 0)
            {
                transform.eulerAngles = new Vector3(0,0,1) * _stableAngleZ;
            }
        }
    }

    const int _dotsAmount = 30;
    GameObject[] _spheres = new GameObject[_dotsAmount];

    void DrawLine(Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < _dotsAmount; i++)
        {
            Destroy(_spheres[i]);
            _spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _spheres[i].transform.position = Vector3.Lerp(position, position+direction, i / (float)_dotsAmount);
            _spheres[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }

    float AngleDifference(float angle1, float angle2)
    {
        angle1 -= angle2;
        if (angle1 < 0)
        {
            angle1 += 360 * (int)(angle1/360);
        }
        if (angle1 > 0)
        {
            angle1 -= 360 * (int)(angle1/360);
        }
        if (angle1 <= -180)
        {
            angle1 = 360 + angle1;
        }
        if (angle1 > 180)
        {
            angle1 = -360 + angle1;
        }
        return angle1;
    }

}
