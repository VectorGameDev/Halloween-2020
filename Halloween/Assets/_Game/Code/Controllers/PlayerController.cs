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

    public Vector2 minVelocity = new Vector2 ( 50f, 50f );

    public float offGroundJumpLimit = 0.2f;

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

    private Vector3 lastCheckpointPosition = Vector3.zero;



    [ Header ( "Light Settings" ) ]

    // the light - emission from the character's eyes
    public Transform eyeLight = null;


    [Header ( "Raycasting Settings" )]

    // raycast offset
    public float raycastOffset = 0.15f;

    // number of raycasts
    public float[] raycastXOffsets = null;


    [Header ( "Player Input" )]

    public PlayerInput playerInput = null;

    private GameControl currentGameControl = GameControl.KEYBOARD;


    private SpriteRenderer hatRenderer = null;


    private float offGroundCounter = 0.0f;
    private bool hasJumped = false;
    private bool isPressingJump = false;
    private float jumpPressCounter = 0.0f;



    public static PlayerControlSchemeUpdate OnPlayerControlSchemeUpdate = null;


    public GameControl GetGameControl() => currentGameControl;


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
        
        OnPlayerControlSchemeUpdate?.Invoke ( currentGameControl, currentGameControl );

        lastCheckpointPosition = transform.position;

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

        // apply the movement to the player
        rigidbody.velocity = new Vector2 (  movementDirection.x * movementSpeed * Time.deltaTime, 
                                            rigidbody.velocity.y );

        // clamping the velocity
        Vector2 velocity = rigidbody.velocity;
        velocity.x = Mathf.Clamp ( velocity.x, -minVelocity.x, maxVelocity.x );
        velocity.y = Mathf.Clamp ( velocity.y, -minVelocity.y, maxVelocity.y );


        if ( hasJumped && velocity.y > 0 )
        {

            velocity.y *= ( !isPressingJump ) ? jumpPressModifier : 1.0f;

        }

        rigidbody.velocity = velocity;

        if ( Mathf.Abs ( rigidbody.velocity.x ) > 0.05f )
        {

            // flipping the sprite when needed
            renderer.flipX = rigidbody.velocity.x < 0.0f;

            if ( hatRenderer != null )
                hatRenderer.flipX = renderer.flipX;

            // also make sure light is pointing to the correct position
            eyeLight.rotation = Quaternion.Euler ( 0f, 0f, ( renderer.flipX ) ? 90f : -90f );

        }

        // reset the input values
        //jump = false;
        interact = false;
        
    }

    private void Update ( )
    {
        
        if ( ( IsGrounded ( ) || offGroundCounter < offGroundJumpLimit ) && !hasJumped )
        {
            
            if ( jumpPressCounter < 0.25f )
            {
                hasJumped = true;
                jump = false;

                // apply the jump force to the player
                rigidbody.velocity = new Vector2 ( rigidbody.velocity.x, jumpForce );
            }

            //// check for input
            //if ( jump )
            //{

            //    hasJumped = true;
            //    jump = false;

            //    // apply the jump force to the player
            //    rigidbody.velocity = new Vector2 ( rigidbody.velocity.x, jumpForce );

            //}

        }


        jumpPressCounter += Time.deltaTime;


    }

    private bool IsGrounded ( )
    {

        Vector2 center = collider.bounds.center + ( Vector3 ) collider.offset;
        RaycastHit2D hit2D = Physics2D.Raycast ( center, Vector2.down, collider.size.y * 0.5f + raycastOffset, groundLayerMask );

        if ( hit2D.transform == null )
        {

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

            offGroundCounter = 0.0f;
            hasJumped = false;

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

            offGroundCounter += Time.deltaTime;

            return false;

        }


    }

    private void CheckForInteractions ( )
    {

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

        lastCheckpointPosition = position;

    }


    public void OnMovement ( InputValue value )
    {

        horizontal = value.Get<float> ( );
        movementDirection.x = horizontal;

        CheckScheme ( );

    }

    public void OnJump ( InputValue value )
    {

        jump = value.Get<float> ( ) > 0.05f;
        isPressingJump = value.Get<float> ( ) > 0.05f;


        if ( jump )
            jumpPressCounter = 0.0f;

        CheckScheme ( );

    }

    public void OnInteract ( InputValue value )
    {

        interact = true;

        CheckScheme ( );

    }


    private void CheckScheme ( )
    {

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

        if ( new_scheme == currentGameControl )
            return;

        OnPlayerControlSchemeUpdate?.Invoke ( currentGameControl, new_scheme );
        currentGameControl = new_scheme;

    }


    private void OnDrawGizmos ( )
    {
        
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
