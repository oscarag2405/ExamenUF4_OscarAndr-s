using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    [SerializeField] public Transform _target;
    [SerializeField] private float _distance;
    [SerializeField] private float _height;

    public int cameraInvert = 1;
    public float sensibilityX = 1;
    public float sensibilityY = 1;
    public bool enabled = true;

    private float _currentRotationAngle;
    private float _currentHeight;
    private Quaternion _currentRotation;
    private float _currentDistance;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(enabled) {
            if(Application.platform == RuntimePlatform.IPhonePlayer) {
                foreach(Touch t in Input.touches) {
                    if(t.phase == TouchPhase.Moved  && t.position.x > Screen.width / 3) {
                        _currentRotationAngle += t.deltaPosition.x * 0.4f * sensibilityX;
                        _currentHeight -= t.deltaPosition.y * 0.01f * cameraInvert * sensibilityY;
                        _currentHeight = Mathf.Min(Mathf.Max(_currentHeight, -1.5f), 1.5f);
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                foreach (Touch t in Input.touches)
                {
                    if (t.phase == TouchPhase.Moved && t.position.x > Screen.width / 3)
                    {
                        _currentRotationAngle += t.deltaPosition.x * 0.4f * sensibilityX;
                        _currentHeight -= t.deltaPosition.y * 0.01f * cameraInvert * sensibilityY;
                        _currentHeight = Mathf.Min(Mathf.Max(_currentHeight, -1.5f), 1.5f);
                    }
                }
            }
            else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
                
                if(Input.GetMouseButton(0)  && Input.mousePosition.x > Screen.width / 3) {
                    _currentRotationAngle += Input.GetAxis("Mouse X") * 6 * sensibilityX;
                    _currentHeight -= Input.GetAxis("Mouse Y") * 0.2f * cameraInvert * sensibilityY;
                    _currentHeight = Mathf.Min(Mathf.Max(_currentHeight, -1.5f), 1.5f);
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {

                if (Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 3)
                {
                    _currentRotationAngle += Input.GetAxis("Mouse X") * 6 * sensibilityX;
                    _currentHeight -= Input.GetAxis("Mouse Y") * 0.2f * cameraInvert * sensibilityY;
                    _currentHeight = Mathf.Min(Mathf.Max(_currentHeight, -1.5f), 1.5f);
                }
            }
        }
        //Calculate current rotation angle and height
        _currentRotation = Quaternion.Euler(0, _currentRotationAngle, 0);
        _currentHeight = Mathf.Clamp(_currentHeight, -20, 80);
        
        Vector3 idealPosition = _currentRotation * Vector3.forward * _distance;

        if(_target) {
            if(Physics.Raycast(_target.position + new Vector3(0, 0.5f, 0), -idealPosition,
                out RaycastHit hit, _distance) && hit.transform.tag != "Player" && hit.transform.tag != "Invisible") {
                _currentDistance = Vector3.Distance(_target.position, hit.point) - 0.2f;
            } else {
                _currentDistance = _distance;
            }

            //Calculate the current distance and height
            Vector3 targetPosition = _target.position - _currentRotation * Vector3.forward * _currentDistance;
            targetPosition.y = _target.position.y + _height + _currentHeight;

            //Set the camera position and rotation
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 20f);
            transform.LookAt(_target.position + Vector3.up * 1.2f);
        }
    }
}
