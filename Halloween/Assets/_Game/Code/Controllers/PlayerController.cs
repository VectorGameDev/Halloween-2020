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


// Control Scheme Enum
public enum GameControl
{

    KEYBOARD = 0,
    GAMEPAD

}


// delegae to be used when the player changes the control scheme
public delegate void PlayerControlSchemeUpdate ( GameControl old_scheme, GameControl new_scheme );


[RequireComponent ( typeof ( Transform ), typeof ( Rigidbody2D ), typeof ( CapsuleCollider2D ) )]
[RequireComponent ( typeof ( SpriteRenderer ) )]
public class PlayerController : MonoBehaviour
{

    // components that are required in order to use the 
    // PlayerController
    private new Transform transform = null;
    private new Rigidbody2D rigidbody = null;
    private new CapsuleCollider2D collider = null;
    private new SpriteRenderer renderer = null;

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

    // limit the velocity so character does not move through objects
    public Vector2 maxVelocity = new Vector2 ( 50f, 50f );

    // the minimum velocity for the character - "limit the negative value"
    public Vector2 minVelocity = new Vector2 ( 50f, 50f );

    // the amount of time the player is allowed to jump after leaving the ground
    public float offGroundJumpLimit = 0.2f;

    // how much impact releasing the jump button has on the jump height
    public float jumpPressModifier = 0.85f;


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

    // cache the rigidbody2D of the moving platform
    private Rigidbody2D movingPlatformRigidbody = null;

    // the last checkpoint position
    private Vector3 lastCheckpointPosition = Vector3.zero;


    [Header ( "Raycasting Settings" )]

    // raycast offset
    public float raycastOffset = 0.15f;

    // number of raycasts
    public float[] raycastXOffsets = null;


    [Header ( "Player Input" )]

    // player input reference
    public PlayerInput playerInput = null;

    // the current game scheme that the player is using
    private GameControl currentGameControl = GameControl.KEYBOARD;

    // the renderer of the hat
    private SpriteRenderer hatRenderer = null;


    // counter for when the character is off ground
    private float offGroundCounter = 0.0f;

    // check if the player has jumped
    private bool hasJumped = false;

    // check if the player is holding the jump key
    private bool isPressingJump = false;

    // how long ago has the player pressed the jump button
    private float jumpPressCounter = 0.0f;


    // event to update the player control scheme
    public static PlayerControlSchemeUpdate OnPlayerControlSchemeUpdate = null;




    private void Awake ( )
    {

        // getting the references
        transform = GetComponent<Transform> ( );
        rigidbody = GetComponent<Rigidbody2D> ( );
        collider = GetComponent<CapsuleCollider2D> ( );
        renderer = GetComponent<SpriteRenderer> ( );

        // Game feels too choppy when the framerate is not set.
        // setting it to 120 | can change this to whaterver... 30, 60 etc.
        Application.targetFrameRate = 120;

    }

    private void Start ( )
    {
        
        // update the control scheme at the start
        OnPlayerControlSchemeUpdate?.Invoke ( currentGameControl, currentGameControl );

        // reset values
        jumpPressCounter = 10.0f;
        lastCheckpointPosition = transform.position;

    }

    private void FixedUpdate ( )
    {

        // check if there are any interactions nearby
        CheckForInteractions ( );

        // if there are and the player has pressed the button then
        if ( selectedInteraction != null && interact )
        {

            interact = false;

            // interact with the selected object
            selectedInteraction.Interact ( this );

        }

        // apply the movement to the player
        rigidbody.velocity = new Vector2 (  movementDirection.x * movementSpeed * Time.deltaTime, 
                                            rigidbody.velocity.y );

        // grab the rigidbody velocity
        Vector2 velocity = rigidbody.velocity;

        // check if the player is on top of a moving platform
        if ( movingPlatformRigidbody != null )
        {

            // apply the platform velocity to the player velocity
            // I am restricting to only X axis, but feel free to change this to your needs
            velocity.x += movingPlatformRigidbody.velocity.x;

            //rigidbody.velocity += movingPlatformRigidbody.velocity; 

        }

        // clamping the velocity | max & min values
        velocity.x = Mathf.Clamp ( velocity.x, -minVelocity.x, maxVelocity.x );
        velocity.y = Mathf.Clamp ( velocity.y, -minVelocity.y, maxVelocity.y );

        // limit the jump height for how long the player holds the jump button
        if ( hasJumped && velocity.y > 0 )
        {

            velocity.y *= ( !isPressingJump ) ? jumpPressModifier : 1.0f;

        }

        // re-apply the rigidbody velocity
        rigidbody.velocity = velocity;


        if ( Mathf.Abs ( movementDirection.x ) > 0.05f )
        {

            // flipping the sprite when needed
            renderer.flipX = movementDirection.x < 0.0f;

            if ( hatRenderer != null )
                hatRenderer.flipX = renderer.flipX;

        }

        // reset some input values
        interact = false;
        
    }

    private void Update ( )
    {
        
        if ( ( IsGrounded ( ) || offGroundCounter < offGroundJumpLimit ) && !hasJumped )
        {
            
            if ( jumpPressCounter < 0.25f )
            {
                //jumpPressCounter = 10.0f;
                hasJumped = true;
                jump = false;

                // apply the jump force to the player
                rigidbody.velocity = new Vector2 ( rigidbody.velocity.x, jumpForce );
            }

        }

        if ( jumpPressCounter < 10.0f )
            jumpPressCounter += Time.deltaTime;


    }


    // get the current game control scheme
    public GameControl GetGameControl ( ) => currentGameControl;


    private bool IsGrounded ( )
    {

        // setup the variables for raycasting
        Vector2 center = collider.bounds.center + ( Vector3 ) collider.offset;
        RaycastHit2D hit2D = Physics2D.Raycast ( center, Vector2.down, collider.size.y * 0.5f + raycastOffset, groundLayerMask );

        if ( hit2D.transform == null )
        {

            // check ground with multiple raycasts
            int counter = 0;

            do
            {

                hit2D = Physics2D.Raycast ( center + ( Vector2.right * raycastXOffsets [ counter ] ), Vector2.down, collider.size.y * 0.5f + raycastOffset, groundLayerMask );
                counter++;

                if ( hit2D.transform != null )
                    break;

            } while ( counter < raycastXOffsets.Length );

        }


        if ( hit2D.transform != null )
        {

            // the character has found ground

            // reset off ground counter
            offGroundCounter = 0.0f;

            // make sure the character is not jumping or moving upwards, otherwise it will create
            // a weird double jump
            if ( Mathf.Abs ( rigidbody.velocity.y ) < Mathf.Abs ( rigidbody.velocity.x ) ||
                 rigidbody.velocity.y < 0.15f )
                hasJumped = false;


            // check and save the player's last grounded position
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

            // make sure the player register the moving platform correctly
            if ( hit2D.transform.CompareTag ( "MovingPlatform" ) )
            {

                movingPlatform = hit2D.transform;

                // make sure we dont get the same rigidbody every single frame to improve performance
                if ( ( movingPlatformRigidbody != null && movingPlatform.gameObject.GetInstanceID ( ) != movingPlatformRigidbody.gameObject.GetInstanceID ( ) ) ||
                       movingPlatformRigidbody == null )
                movingPlatformRigidbody = movingPlatform.GetComponent<Rigidbody2D> ( );

            }
            else
            {

                // reset the moving platform variables
                movingPlatform = null;
                movingPlatformRigidbody = null;

            }

            return true;

        }
        else
        {

            // if no ground has been found, reset values:

            movingPlatform = null;
            groundPositionCounter = 0f;

            offGroundCounter += Time.deltaTime;

            return false;

        }


    }

    private void CheckForInteractions ( )
    {

        // creates a circle to find all possible interactions
        Collider2D[] colliders = Physics2D.OverlapCircleAll (   rigidbody.position, 
                                                                interactionSearchRadius, 
                                                                interacitonLayerMask );

        // loops through all interactions and find the closest object
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

        // set-up the best interaction target based on the distance from the player
        if ( best_target != null )
        {

            if ( selectedInteraction != null )
                selectedInteraction.Deselect ( );

            best_target.Select ( );
            selectedInteraction = best_target;

        }
        else
        {

            if ( selectedInteraction != null )
                selectedInteraction.Deselect ( );

            selectedInteraction = null;

        }

    }


    public void SetNewHat ( Transform new_hat )
    {

        // set the hat on the player
        new_hat.position = hatPlacement.position;
        new_hat.SetParent ( hatPlacement );

        hatRenderer = new_hat.GetComponent<SpriteRenderer> ( );

        if ( hatRenderer != null )
            hatRenderer.flipX = renderer.flipX;

    }

    public void GetPlayerBackToGround ( )
    {

        // reset the velocity so that the character does not fall through the ground
        rigidbody.velocity = Vector2.zero;

        // setting the rigidbody to its last known ground position
        //rigidbody.position = lastGroundPosition;
        rigidbody.position = lastCheckpointPosition;

    }

    public void SetCheckPointPosition ( Vector3 position )
    {

        // set the checkpoint position
        lastCheckpointPosition = position;

    }


    public void OnMovement ( InputValue value )
    {

        // method called from the PlayerInput component
        // called via message
        // set the horizontal movement

        horizontal = value.Get<float> ( );
        movementDirection.x = horizontal;

        CheckScheme ( );

    }

    public void OnJump ( InputValue value )
    {

        // method called from the PlayerInput component
        // called via message
        // set the jump action - it is pass through
        // this means that we will get a float value

        jump = value.Get<float> ( ) > 0.05f;
        isPressingJump = value.Get<float> ( ) > 0.05f;


        if ( jump )
            jumpPressCounter = 0.0f;

        CheckScheme ( );

    }

    public void OnInteract ( InputValue value )
    {

        // method called from the PlayerInput component
        // called via message
        // set the interact - it is a simple button press

        interact = true;

        CheckScheme ( );

    }


    private void CheckScheme ( )
    {

        // check which control scheme the player is using...

        if ( playerInput.currentControlScheme == "Keyboard" )
        {

            CheckCurrentGameControlScheme ( GameControl.KEYBOARD );

        }
        else if ( playerInput.currentControlScheme == "Gamepad" )
        {

            CheckCurrentGameControlScheme ( GameControl.GAMEPAD );

        }

    }

    private void CheckCurrentGameControlScheme ( GameControl new_scheme )
    {

        // calls the event to switch the player control scheme if it is a different scheme

        if ( new_scheme == currentGameControl )
            return;

        OnPlayerControlSchemeUpdate?.Invoke ( currentGameControl, new_scheme );
        currentGameControl = new_scheme;

    }


    private void OnDrawGizmos ( )
    {
        
        // drawing some gizmos

        if ( rigidbody == null )
            rigidbody = GetComponent<Rigidbody2D> ( );

        if ( collider == null )
            collider = GetComponent<CapsuleCollider2D> ( );

        Gizmos.DrawWireSphere ( rigidbody.position, interactionSearchRadius );

        if ( selectedInteraction != null )
        {

            Gizmos.DrawWireCube ( selectedInteraction.GetPosition ( ), Vector3.one * 0.5f );
            Gizmos.DrawLine ( selectedInteraction.GetPosition ( ), rigidbody.position );

        }

        Gizmos.color = Color.green;
        Vector2 center = collider.bounds.center + ( Vector3 ) collider.offset;
        Gizmos.DrawWireSphere ( center, 0.25f );
        Gizmos.DrawLine ( center, center + ( Vector2.down * ( collider.size.y * 0.5f + raycastOffset ) ) );

        RaycastHit2D hit2D = Physics2D.Raycast ( center, Vector2.down, collider.size.y * 0.5f + raycastOffset, groundLayerMask );
        if ( hit2D.transform != null )
        {

            Gizmos.color = Color.red;
            Gizmos.DrawLine ( center, hit2D.point );
            Gizmos.DrawWireSphere ( hit2D.point, 0.25f );

        }


        if ( raycastXOffsets.Length > 0 )
        {

            int counter = 0;
            do
            {

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere ( center + ( Vector2.right * raycastXOffsets [ counter ] ), 0.25f );
                Gizmos.DrawLine ( center + ( Vector2.right * raycastXOffsets [ counter ] ), center + ( Vector2.right * raycastXOffsets [ counter ] ) + ( Vector2.down * ( collider.size.y * 0.5f + raycastOffset ) ) );
                hit2D = Physics2D.Raycast ( center + ( Vector2.right * raycastXOffsets [ counter ] ), Vector2.down, collider.size.y * 0.5f + raycastOffset, groundLayerMask );

                if ( hit2D.transform != null )
                {

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine ( center + ( Vector2.right * raycastXOffsets [ counter ] ), hit2D.point );
                    Gizmos.DrawWireSphere ( hit2D.point, 0.25f );

                }

                counter++;
            } while ( counter < raycastXOffsets.Length );

        }
        



    }

}
