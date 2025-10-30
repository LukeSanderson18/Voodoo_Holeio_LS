using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform playerHole;
    public float baseDistance = 10f;
    public float growthFactor = 1f;


    private CinemachineFramingTransposer transposer;

    void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {
        float newDistance = baseDistance + playerHole.localScale.x * growthFactor;
        transposer.m_CameraDistance = newDistance;
    }
}