using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera mainCam;
    
    void Start()
    {
        mainCam = Camera.main;
    }
    
    void Update()
    {
        transform.forward = mainCam.transform.forward;
    }
}
