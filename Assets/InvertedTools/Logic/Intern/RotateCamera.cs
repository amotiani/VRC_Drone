using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    //Referencing the Camera
    public GameObject rotatingCamera;

    //Setting the rotation speed
    public float speed = 0.5f;
    void Update()
    {
        //Geting the horizontal input
        float horizontal = Input.GetAxis("Horizontal");

        //Rotating the camera
        rotatingCamera.transform.Rotate(0.0f, -horizontal * speed, 0.0f);
    }
}
