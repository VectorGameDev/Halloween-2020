// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Cloud Loop

// Using Statements
using UnityEngine;


public class CloudLoop : MonoBehaviour
{

    public Transform center = null;

    public Transform[] clouds = null;
    public Direction direction = Direction.FORWARD;
    public float random = 3.0f;
    public float xStart = 0.0f;
    public float xEnd = 5.0f;
    public float speed = 5.0f;
    public float maxDistance = 500.0f;


    private void Update ( )
    {

        // move all clouds
        foreach ( Transform t in clouds )
        {

            float r = Random.Range ( 1.0f, random );

            switch ( direction )
            {

                case Direction.FORWARD:

                    t.position += Vector3.right * speed * r * Time.deltaTime;

                    if ( Vector3.Distance ( center.position, t.position ) > maxDistance )
                        t.position = center.position + Vector3.left * xStart;

                    break;

                case Direction.BACKWARD:

                    t.position += Vector3.left * speed * r * Time.deltaTime;

                    if ( Vector3.Distance ( center.position, t.position ) > maxDistance )
                        t.position = center.position + Vector3.right * xStart;

                    break;

            }

            

        }
        
    }

}
