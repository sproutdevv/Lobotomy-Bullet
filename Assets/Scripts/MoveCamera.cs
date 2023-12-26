using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // ----- make camera move with player -----

    public Transform cameraPosition;

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
