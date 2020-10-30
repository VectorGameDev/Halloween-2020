

// Using Statements
using UnityEngine;


public class Checkpoint : MonoBehaviour
{


    public void OnTriggerEnter2D ( Collider2D collision )
    {
        // when there is a collision
        // check the tag to see if it is actually the player
        if ( collision.CompareTag ( "Player" ) )
        {

            // set the checkpoint position
            collision.GetComponent<PlayerController> ( ).SetCheckPointPosition ( transform.position );

        }
        
    }

}
