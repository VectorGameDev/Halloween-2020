// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Which Hat
// This is the hat that will give abilities to the
// player.
// Also it attaches itself to the player.


// Using Statements
using UnityEngine;


public class WitchHat : Interaction
{

    // the particle FX to be disabled
    public GameObject particleFX = null;


    // -- this was done in a rush -- this code should be optimized
    // this is a temp way to make sure the player has gotten the hat!
    // this needs to be changed on the next update
    private static bool _hasGrabbed = false;
    private void Start ( )
    {
        
        if ( _hasGrabbed )
        {
            GetComponent<SpriteRenderer> ( ).enabled = true;
            FindObjectOfType<PlayerController> ( ).SetNewHat ( transform );
        }

    }
    // --


    public override void Interact ( PlayerController controller )
    {

        base.Interact ( controller );

        // disable the Interact component
        // so that the player cannot active this again
        isActive = false;

        // attach itself to the player
        controller.SetNewHat ( transform );

        // hide the particle FX
        particleFX.SetActive ( false );


        _hasGrabbed = true;

    }

}
