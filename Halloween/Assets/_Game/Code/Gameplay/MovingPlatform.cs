// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Moving Platform
// This is a script to move an object along a defined path in the inspector.
// It will move the object in a "Ping-Pong" fashion.


// Using Statements
using System.Collections.Generic;
using UnityEngine;


// Direction enumeration
public enum Direction
{

    FORWARD = 1,
    BACKWARD = -1

}


public class MovingPlatform : MonoBehaviour
{
    
    // the transform reference
    protected new Transform transform = null;
    protected new Rigidbody2D rigidbody = null;

    // the path that the platform will follow
    public List<Transform> path = new List<Transform> ( );

    // the speed of the platform (movement)
    public float speed = 5.0f;

    // how close the platform has to get before going to the next position
    public float threshold = 0.15f;


    // the current movement direction
    private Direction movingDirection = Direction.FORWARD;

    // the current point in the path that this platform is at (index)
    private int currentPathIndex = 0;



    private void Awake ( )
    {

        // retrieving the transform reference
        transform = GetComponent<Transform> ( );

        rigidbody = GetComponent<Rigidbody2D> ( );

    }

    private void FixedUpdate ( )
    {

        // check if the platform is close enough to the target position
        if ( Vector2.Distance ( rigidbody.position, path [ currentPathIndex ].position ) < threshold )
        {

            // if so change the next point on the path
            currentPathIndex += ( int ) movingDirection;

            // if it has reached the beginning
            if ( currentPathIndex < 0 )
            {
                movingDirection = ( Direction ) ( ( int ) movingDirection * -1 );

                switch ( movingDirection )
                {
                    case Direction.FORWARD: currentPathIndex = 1; break;
                    case Direction.BACKWARD: currentPathIndex = path.Count - 1; break;
                }
            }

            // if it has reached the end
            if ( currentPathIndex >= path.Count )
            {
                movingDirection = ( Direction ) ( ( int ) movingDirection * -1 );

                switch ( movingDirection )
                {
                    case Direction.FORWARD: currentPathIndex = 0; break;
                    case Direction.BACKWARD: currentPathIndex = path.Count - 2; break;
                }
            }

        }


        // move the platform
        Vector2 direction = ( ( Vector2 ) path [ currentPathIndex ].position - rigidbody.position ).normalized;
        rigidbody.velocity = direction * speed * Time.deltaTime;
        
    }


    private void OnDrawGizmos ( )
    {

        Gizmos.color = Color.green;

        for ( int i = 0; i < path.Count; i++ )
        {

            if ( path [ i ] != null )
                Gizmos.DrawWireSphere ( path [ i ].position, 0.25f );

            if ( i == path.Count - 1 )
            continue;

            if ( path [ i + 1 ] != null )
                Gizmos.DrawLine ( path [ i ].position, path [ i + 1 ].position );

        }

    }


}
