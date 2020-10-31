// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Interaction
// This is the base class for the interaction object
// it is like framework for all the objects that the
// player can interact with.
// To create a new interaction just create a new script
// and inherit the class from this Class -- then code
// the behaviour; make sure to use the Interact method
// this is when the player has actually interacted with
// the object.


// Using Statements
using UnityEngine;


public class Interaction : MonoBehaviour
{

    // the transform reference
    protected new Transform transform = null;

    // check if the interaction is active
    // this is used by the PlayerController to detect
    // if this interaction should be used
    public bool isActive = true;

    // the offset from which the UI will be displayed
    public float uiDisplayYOffset = 1.0f;


    protected virtual void Awake ( )
    {

        // retrieving the transform reference
        transform = GetComponent<Transform> ( );

    }

    /// <summary>
    /// Interact is called when the player presses the interact button.
    /// </summary>
    /// <param name="controller">The PlayerController reference -- the player that interacted with.</param>
    public virtual void Interact ( PlayerController controller )
    {

        //Debug.Log ( "The " + controller.name + " has used the " + gameObject.name + " interaction!" );
        Deselect ( );

    }

    /// <summary>
    /// Selects an interaction, invoking an event
    /// </summary>
    public virtual void Select ( )
    {

        InstructionInteract.OnUpdateUIInteract?.Invoke ( true, ( Vector2 ) transform.position + ( Vector2.up * uiDisplayYOffset ) );

    }

    /// <summary>
    /// Deselects an interaction, invoking an event
    /// </summary>
    public virtual void Deselect ( )
    {
        
        InstructionInteract.OnUpdateUIInteract?.Invoke ( false, ( Vector2 ) transform.position + ( Vector2.up * uiDisplayYOffset ) );

    }


    /// <summary>
    /// Get the current position of the interaction.
    /// </summary>
    /// <returns>A Vector2 representing the position in the 2D world that the interaction is in.</returns>
    public virtual Vector2 GetPosition ( )
    {

        return transform.position;

    }


    protected void OnDrawGizmos ( )
    {

        if ( transform == null )
            transform = GetComponent<Transform> ( );

        Gizmos.color = Color.blue;
        Gizmos.DrawLine ( transform.position, transform.position + ( Vector3 ) ( Vector2.up * uiDisplayYOffset ) );
        Gizmos.DrawWireSphere ( transform.position + ( Vector3 ) ( Vector2.up * uiDisplayYOffset ), 0.25f );
        
    }

}
