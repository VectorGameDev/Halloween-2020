// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using UnityEngine;


public class Grass : MonoBehaviour
{

    // the animator used by the grass
    public Animator animator = null;
    

    public void OnTriggerEnter2D ( Collider2D collision )
    {
     
        // check to see if is the player
        if ( collision.CompareTag ( "Player" ) )
        {

            // set the animator trigger so it plays a different animation
            animator.SetTrigger ( "Collision" );

        }

    }

}
