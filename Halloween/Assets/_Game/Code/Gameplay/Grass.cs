// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using UnityEngine;


public class Grass : MonoBehaviour
{

    public Animator animator = null;
    

    public void OnTriggerEnter2D ( Collider2D collision )
    {
        
        if ( collision.CompareTag ( "Player" ) )
        {

            animator.SetTrigger ( "Collision" );

        }

    }

}
