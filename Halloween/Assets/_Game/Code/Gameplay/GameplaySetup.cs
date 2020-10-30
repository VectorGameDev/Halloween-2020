


// Using Statements
using System.Collections;
using UnityEngine;
using Cinemachine;


public class GameplaySetup : MonoBehaviour
{

    public CanvasGroup canvasGroupIntroObject = null;
    public PlayerController playerController = null;
    public SpriteRenderer playerRenderer = null;
    public SpriteRenderer glowRenderer = null;
    public InstructionMovement instructionMovement = null;
    public CinemachineVirtualCamera cinemachineCam = null;

    public Color playerStartColor = Color.black;
    public Color playerTargetColor = Color.white;
    public Color playerGlowStartColor = Color.black;
    public Color playerGlowTargetColor = Color.white;

    public float startDelay = 0.25f;
    public float threshold = 0.015f;
    public float fadeOutSpeed = 3.0f;
    public float colorLerpSpeed = 3.0f;
    public float orthoSizeStart = 3.75f;
    public float orthoSizeEnd = 10.0f;
    public float orthoLerpSpeed = 2.0f;
    public float activateControllerDelay = 0.5f;




    private void Start ( )
    {

        Setup ( );
        StartCoroutine ( Intro ( ) );

    }

    private void Setup ( )
    {

        // setup
        playerController.enabled = false;
        playerRenderer.color = playerStartColor;
        glowRenderer.color = playerGlowStartColor;
        canvasGroupIntroObject.alpha = 1.0f;
        cinemachineCam.m_Lens.OrthographicSize = orthoSizeStart;

    }


    public IEnumerator Intro ( )
    {

        yield return new WaitForSeconds ( startDelay );

        // intro alpha - fade
        do
        {

            canvasGroupIntroObject.alpha = Mathf.Lerp ( canvasGroupIntroObject.alpha, 0.0f, fadeOutSpeed * Time.deltaTime );
            yield return null;

        } while ( canvasGroupIntroObject.alpha > 0.0 + threshold );

        canvasGroupIntroObject.alpha = 0.0f;


        // player glow
        do
        {

            playerRenderer.color = Color.Lerp ( playerRenderer.color, playerTargetColor, colorLerpSpeed* Time.deltaTime );
            glowRenderer.color = Color.Lerp ( glowRenderer.color, playerGlowTargetColor, colorLerpSpeed* Time.deltaTime );
            yield return null;

        } while ( 
            ! (
            Mathf.Abs ( playerRenderer.color.r - playerTargetColor.r ) < threshold &&
            Mathf.Abs ( playerRenderer.color.g - playerTargetColor.g ) < threshold &&
            Mathf.Abs ( playerRenderer.color.b - playerTargetColor.b ) < threshold &&
            Mathf.Abs ( glowRenderer.color.r - playerGlowTargetColor.r ) < threshold &&
            Mathf.Abs ( glowRenderer.color.g - playerGlowTargetColor.g ) < threshold &&
            Mathf.Abs ( glowRenderer.color.b - playerGlowTargetColor.b ) < threshold 
            ) );

        playerRenderer.color = playerTargetColor;
        glowRenderer.color = playerGlowTargetColor;

        // ortho size anim
        do
        {

            cinemachineCam.m_Lens.OrthographicSize = Mathf.Lerp ( cinemachineCam.m_Lens.OrthographicSize, orthoSizeEnd, orthoLerpSpeed * Time.deltaTime );
            yield return null;

        } while ( Mathf.Abs ( cinemachineCam.m_Lens.OrthographicSize - orthoSizeEnd ) > threshold  );

        cinemachineCam.m_Lens.OrthographicSize = orthoSizeEnd;

        yield return new WaitForSeconds ( activateControllerDelay );

        instructionMovement.StartInstruction ( );
        playerController.enabled = true;


    }


}
