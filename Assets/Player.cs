using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Joystick joystick;
    public GameObject arrow;  // Assign your arrow in the Inspector
    public float arrowDistance = 1.5f;
    public float moveSpeed = 5f;

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 moveDir = GetJoystickDirection();
        MovePlayer(moveDir);
        UpdateArrow(moveDir);
    }
    
    // Returns a direction vector based on the joystick input, relative to the camera
    Vector3 GetJoystickDirection()
    {
        Vector3 forward = mainCam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        Vector3 right = mainCam.transform.right;
        right.y = 0;
        right.Normalize();

        return right * joystick.Horizontal + forward * joystick.Vertical;
    }
    
    void MovePlayer(Vector3 move)
    {
        transform.position += move * Time.deltaTime * moveSpeed;
    }
    
    void UpdateArrow(Vector3 direction)
    {
        if(direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            arrow.transform.position = transform.position + direction * arrowDistance;
            arrow.transform.position += Vector3.up * 0.03f;
            arrow.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            arrow.transform.eulerAngles = new Vector3(90f, arrow.transform.eulerAngles.y, arrow.transform.eulerAngles.z);
        }
    }
}