using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    private Transform _target;
    private Transform _aim;
    [SerializeField] private float _distance;
    [SerializeField] private float _height;
    [SerializeField] private float _smoothSpeed;
    [SerializeField] private float _smoothRotation;
    private bool _lockedOnTarget = false;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _eulerVelocity = Vector3.zero;
    private Vector3 _eulerAngles = Vector3.zero;

    void Awake()
    {
        _eulerAngles = transform.eulerAngles;
    }

    void LateUpdate()
    {
        if (_lockedOnTarget)
        {
            Vector3 targetPosition = _target.position - _target.forward*_distance + _target.up*_height;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothSpeed);

            transform.eulerAngles = _target.eulerAngles;
            transform.RotateAround(transform.position, _target.right, 57.2957795f * Mathf.Asin(_distance / Vector3.Distance(targetPosition, _aim.position)) );
            _eulerAngles = SmoothDampAngle(_eulerAngles, transform.eulerAngles, ref _eulerVelocity, _smoothRotation);
            transform.eulerAngles = _eulerAngles;
        }
        else
        {
            print("SEARCHING FOR PLAYER");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetComponent<ShipControls>().IsMine())
                {
                    SetTarget(player);
                    break;
                }
            }
        }
    }

    Vector3 SmoothDampAngle(Vector3 current, Vector3 target, ref Vector3 velocity, float smooth)
    {
        current.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, smooth);
        current.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, smooth);
        current.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, smooth);
        return current;
    }

    public void SetTarget(GameObject target)
    {
        _target = target.GetComponent<Transform>();
        _aim = _target.GetChild(0).GetComponent<Transform>();
        _lockedOnTarget = true;
        print("FOUND AND LOCKED");
    }
}
