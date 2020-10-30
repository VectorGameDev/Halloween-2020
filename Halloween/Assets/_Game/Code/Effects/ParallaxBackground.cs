// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Parallax Background


// Using Statements
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class ParallaxBackground : MonoBehaviour
{

    public Transform[] backgrounds = null;

    public float speed = 5.0f;

    private Vector3[] targetPositions = null;

    private Transform cameraTransform = null;
    private Vector3 previousCameraPos = Vector3.zero;


    private void Awake ( )
    {
        
        cameraTransform = Camera.main.GetComponent<Transform> ( );

    }

    private void Start ( )
    {

        previousCameraPos = cameraTransform.position;
        targetPositions = new Vector3 [ backgrounds.Length ];

        for ( int i = 0; i < backgrounds.Length; i++ )
            targetPositions [ i ] = backgrounds [ i ].position;
        
    }

    private void Update ( )
    {


        for ( int i = 0; i < backgrounds.Length; i++ )
        {

            if ( Mathf.Abs ( cameraTransform.position.x - previousCameraPos.x ) > 0.05f )
            {

                Vector3 target_position = new Vector3 ( 
                    ( previousCameraPos.x - cameraTransform.position.x ) * ( -backgrounds [ i ].position.z ) + backgrounds [ i ].position.x,
                    backgrounds [ i ].position.y,
                    backgrounds [ i ].position.z );

                targetPositions [ i ] = target_position;

            }

            backgrounds [ i ].position = Vector3.LerpUnclamped ( backgrounds [ i ].position, targetPositions[ i ], speed * Time.deltaTime );

        }
        
        //foreach ( Transform background in backgrounds )
        //{

        //    Vector3 target_position = new Vector3 ( 
        //        ( previousCameraPos.x - cameraTransform.position.x ) * ( -background.position.z ) + background.position.x,
        //        background.position.y,
        //        background.position.z );

        //    background.position = new Vector3 (
        //        background.position.x + ( ( previousCameraPos.x - cameraTransform.position.x ) * ( -background.position.z ) / speed ) * Time.deltaTime,
        //        background.position.y,
        //        background.position.z
        //        );

        //}


        previousCameraPos = cameraTransform.position;

    }

}
