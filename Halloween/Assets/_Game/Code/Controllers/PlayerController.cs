// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Player Controller
// The controller for the character -- intergrated with
// the new Input system.
// Takes care of the following
// Basic physics
// Horizontal Movement
// Jump
// Ground Position
// Interactions


// Using Statements
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent ( typeof ( Transform ), typeof ( Rigidbody2D ), typeof ( CapsuleCollider2D ) )]
public class PlayerController : MonoBehaviour
{

    // components that are required in order to use the 
    // PlayerController
    private new Transform transform = null;
    private new Rigidbody2D rigidbody = null;
    private new CapsuleCollider2D collider = null;

    [Header ( "Movement Settings" )]
    
    // movement speed -- how fast the character moves
    public float movementSpeed = 300.0f;

    // how much force is applied to the character when jumping
    public float jumpForce = 1500f;

    // the layer in which the ground is located
    public LayerMask groundLayerMask = 0;

    // has the player pressed the jump button
    private bool jump = false;

    // the horizontal value of the movement -- A and D in the keyboard
    private float horizontal = 0.0f;

    // the direction in which the character velocity will change
    private Vector2 movementDirection = Vector2.zero;


    [Header ( "Interaction Settings" )]

    // the search radius -- check the Gizmos to visualize this better
    public float interactionSearchRadius = 3.0f;

    // the layer in which the interaction should be located
    public LayerMask interacitonLayerMask = 0;

    // has the player pressed the interact button
    private bool interact = false;

    // the selected interaction which the player is using
    private Interaction selectedInteraction = null;


    [Header ( "Character Attachments" )]

    // a position which will be used to add a hat to the player
    public Transform hatPlacement = null;


    [Header ( "Ground Placement" )]    

    // register the last ground position
    // the minimum time required to record the last ground position
    public float timerToRegisterGroundPosition = 0.0f;

    // the maximum distance before resetting the timer
    public float maxDistanceBetweenPositions = 0.5f;

    // the last ground position
    private Vector2 lastGroundPosition = Vector2.zero;

    // a placeholder checker for the last ground position
    private Vector2 checkGroundPosition = Vector2.zero;

    // the counter so that it can register the last ground position
    private float groundPositionCounter = 0.0f;

    // the current moving platform
    private Transform movingPlatform = null;




    private void Awake ( )
    {

        // getting the references
        transform = GetComponent<Transform> ( );
        rigidbody = GetComponent<Rigidbody2D> ( );
        collider = GetComponent<CapsuleCollider2D> ( );

    }

    private void FixedUpdate ( )
    {

        // attaching the player to the moving platform
        if ( movingPlatform != null ) transform.SetParent ( movingPlatform );
        else transform.SetParent ( null );

        // check if there are any interactions nearby
        CheckForInteractions ( );

        // if there are and the player has pressed the button then
        if ( selectedInteraction != null && interact )
        {

            interact = false;

            // interact with the selected object
            selectedInteraction.Interact ( this );

        }

        // check for grounded
        if ( IsGrounded ( ) )
        {

            // check for input
            if ( jump )
            {

                jump = false;

                // apply the jump force to the player
                rigidbody.AddForce ( new Vector2 ( 0, jumpForce ) );

            }

        }

        // apply the movement to the player
        rigidbody.velocity = new Vector2 (  movementDirection.x * movementSpeed * Time.deltaTime, 
                                            rigidbody.velocity.y );


        // reset the input values
        jump = false;
        interact = false;
        
    }

    private bool IsGrounded ( )
    {

        RaycastHit2D hit2D = Physics2D.Raycast ( rigidbody.position, Vector2.down, collider.size.y * 0.5f + 0.1f, groundLayerMask );
        if ( hit2D.transform != null )
        {

            if ( Vector2.Distance ( checkGroundPosition, rigidbody.position ) < maxDistanceBetweenPositions )
            {

                groundPositionCounter += Time.deltaTime;
                if ( groundPositionCounter > timerToRegisterGroundPosition )
                {

                    groundPositionCounter = 0f;
                    lastGroundPosition = rigidbody.position;

                }

            }
            else
            {

                groundPositionCounter = 0;
                checkGroundPosition = rigidbody.position;

            }

            if ( hit2D.transform.CompareTag ( "MovingPlatform" ) )
            {

                movingPlatform = hit2D.transform;

            }
            else
            {

                movingPlatform = null;

            }

            return true;

        }
        else
        {

            movingPlatform = null;
            groundPositionCounter = 0f;
            return false;

        }


    }

    private void CheckForInteractions ( )
    {

        selectedInteraction = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll (   rigidbody.position, 
                                                                interactionSearchRadius, 
                                                                interacitonLayerMask );

        float current_distance = float.MaxValue;
        Interaction best_target = null;
        foreach ( Collider2D collider in colliders )
        {

            Interaction interaction = collider.GetComponent<Interaction> ( );
            if ( interaction != null && interaction.isActive )
            {

                if ( Vector2.Distance ( interaction.GetPosition ( ), rigidbody.position ) < current_distance )
                {

                    best_target = interaction;
                    current_distance = Vector2.Distance ( interaction.GetPosition ( ), rigidbody.position );

                }

            }

        }

        if ( best_target != null )
        {

            selectedInteraction = best_target;

        }

    }


    public void SetNewHat ( Transform new_hat )
    {

        new_hat.position = hatPlacement.position;
        new_hat.SetParent ( hatPlacement );

    }

    public void GetPlayerBackToGround ( )
    {

        rigidbody.position = lastGroundPosition;

    }


    public void OnMovement ( InputValue value )
    {

        horizontal = value.Get<float> ( );
        movementDirection.x = horizontal;

    }

    public void OnJump ( InputValue value )
    {

        jump = true;

    }

    public void OnInteract ( InputValue value )
    {

        interact = true;

    }


    private void OnDrawGizmos ( )
    {
        
        if ( rigidbody == null )
            rigidbody = GetComponent<Rigidbody2D> ( );

        Gizmos.DrawWireSphere ( rigidbody.position, interactionSearchRadius );

        if ( selectedInteraction != null )
        {

            Gizmos.DrawWireCube ( selectedInteraction.GetPosition ( ), Vector3.one * 0.5f );
            Gizmos.DrawLine ( selectedInteraction.GetPosition ( ), rigidbody.position );

        }

    }

}
