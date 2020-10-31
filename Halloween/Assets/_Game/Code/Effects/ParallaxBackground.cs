// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Parallax Background


// Using Statements
using UnityEngine;


public class ParallaxBackground : MonoBehaviour
{

    // all the backgrounds that will be moved
    public Transform[] backgrounds = null;

    // the speed of the movement
    public float speed = 5.0f;

    // camera transform
    private Transform cameraTransform = null;

    // previous camera position
    private Vector3 previousCameraPos = Vector3.zero;


    private void Awake ( )
    {
        
        // get camera transform reference
        cameraTransform = Camera.main.GetComponent<Transform> ( );

    }

    private void Start ( )
    {

        // get the previous camera position - initial position
        previousCameraPos = cameraTransform.position;
        
    }

    private void Update ( )
    {
        
        foreach ( Transform background in backgrounds )
        {

            // move the background elements
            background.position = new Vector3 (
                background.position.x + ( ( previousCameraPos.x - cameraTransform.position.x ) * ( -background.position.z ) / speed ) * Time.deltaTime,
                background.position.y,
                background.position.z
                );

        }


        // update the camera previous position
        previousCameraPos = cameraTransform.position;

    }

}
