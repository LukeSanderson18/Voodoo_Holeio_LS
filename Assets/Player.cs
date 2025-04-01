using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private Hole hole;
    public GameObject arrow; // Assign your arrow in the Inspector
    public float arrowDistance = 1.5f;

    private Camera mainCam;
    private Rigidbody holeRb;
    Vector3 moveDir;
    bool gameStarted = false;

    void Awake()
    {
        mainCam = Camera.main;
        if (hole != null)
        {
            holeRb = hole.GetComponent<Rigidbody>();
        }
    }
    
    public void Start()
    {
        GameManager.Instance.OnGameStarted += HandleGameStarted;
        GameManager.Instance.OnGameEnded += HandleGameEnded;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted -= HandleGameStarted;
            GameManager.Instance.OnGameEnded -= HandleGameEnded;
        }
    }

    private void HandleGameStarted()
    {
        gameStarted = true;
    }

    private void HandleGameEnded()
    {
        gameStarted = false;
    }
    
    void Update()
    {
        if (!gameStarted)
            return;

        moveDir = GetJoystickDirection();
        UpdateArrow(moveDir);
    }

    void FixedUpdate()
    {
        if (!gameStarted)
            return;

        MoveHole(moveDir);
    }

    // Returns a direction vector based on joystick input relative to the cam
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

    void MoveHole(Vector3 move)
    {
        Vector3 newPosition = holeRb.position + move * (Time.fixedDeltaTime * hole.MovementSpeed);
        holeRb.MovePosition(newPosition);
    }

    void UpdateArrow(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            // Position arrow relative to the hole's current scale.
            arrow.transform.position =
                hole.transform.position + direction * (hole.Size * arrowDistance);
            arrow.transform.position += Vector3.up * 0.03f;
            arrow.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            arrow.transform.eulerAngles =
                new Vector3(90f, arrow.transform.eulerAngles.y, arrow.transform.eulerAngles.z);
        }
    }
}