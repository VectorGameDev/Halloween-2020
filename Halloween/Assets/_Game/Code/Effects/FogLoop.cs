// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Fog Loop


// Using Statements
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class FogLoop : MonoBehaviour
{

    public Transform[] fog = null;
    public Direction direction = Direction.FORWARD;
    public float random = 3.0f;
    public float xStart = 0.0f;
    public float xEnd = 5.0f;
    public float speed = 5.0f;


    private void Update ( )
    {

        // move all fogs
        foreach ( Transform t in fog )
        {

            float r = Random.Range ( 1.0f, random );

            switch ( direction )
            {

                case Direction.FORWARD:

                    t.position += Vector3.right * speed * r * Time.deltaTime;

                    if ( t.position.x > xEnd )
                        t.position = new Vector3 ( xStart, t.position.y, t.position.z );

                    break;

                case Direction.BACKWARD:

                    t.position += Vector3.left * speed * r * Time.deltaTime;

                    if ( t.position.x < xEnd )
                        t.position = new Vector3 ( xStart, t.position.y, t.position.z );

                    break;

            }

            

        }
        
    }

}
