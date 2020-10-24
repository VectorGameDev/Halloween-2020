// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Death Box
// Just a simple script to make the player move back to the ground 
// if the player has fallen.


// Using Statements
using UnityEngine;


public class DeathBox : MonoBehaviour
{

    public void OnTriggerEnter2D ( Collider2D collision )
    {
        // when there is a collision
        // check the tag to see if it is actually the player
        if ( collision.CompareTag ( "Player" ) )
        {

            // if it is send it back to the ground
            // the ground position is set on the PlayerController script
            collision.GetComponent<PlayerController> ( ).GetPlayerBackToGround ( );

        }
        
    }

}
