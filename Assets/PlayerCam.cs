using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform Orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;                           // Mauszeiger ist in der Mitte des Screens gesperrt
        Cursor.visible = false;                                             // Mauszeiger ist unsichtbar
    }

    private void Update()
    {
        // -- mouse input bekommen --

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);                      // ---> Könnte für die Augen wichtig sein beim Spieler die den anderen Spieler anschauen soll.

        // -- Kamera und Orientation drehen --

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);     // Kamera in beide Richtungen drehen 
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);           // Spieler in y-Achse bewegen

    
    }
}
