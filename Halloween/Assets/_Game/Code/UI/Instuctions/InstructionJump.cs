
// Using Statements
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class InstructionJump : MonoBehaviour
{

    private bool hasShown = false;

    public List<SpriteRenderer> allInstructionImages = new List<SpriteRenderer> ( );

    public float startDelay = 5.0f;
    public float fadeInSpeed = 2.0f;
    public float fadeOutSpeed = 2.0f;
    public float displayTime = 2.0f;
    public float threshold = 0.05f;



    private void Start ( )
    {
        
        allInstructionImages.ForEach ( o => o.color = new Color ( o.color.r, o.color.g, o.color.b, 0f ) );

    }

    public void OnTriggerEnter2D ( Collider2D collision )
    {
        
        if ( collision.CompareTag ( "Player" ) )
        {

            if ( !hasShown )
            {
                StartCoroutine ( ShowInstruction ( ) );
                hasShown = true;
            }

        }

    }


    private IEnumerator ShowInstruction ( )
    {

        yield return new WaitForSeconds ( startDelay );

        allInstructionImages.ForEach ( o => o.color = new Color ( o.color.r, o.color.g, o.color.b, 0f ) );

        do
        {

            allInstructionImages.ForEach ( o => o.color = new Color ( o.color.r, o.color.g, o.color.b, 
                Mathf.Lerp ( o.color.a, 1.0f, Time.deltaTime * fadeInSpeed ) ) );

            yield return null;

        } while ( allInstructionImages.First ( ).color.a < ( 1.0f - threshold ) );

        allInstructionImages.ForEach ( o => o.color = new Color ( o.color.r, o.color.g, o.color.b, 1f ) );

        yield return new WaitForSeconds ( displayTime );

        do
        {

            allInstructionImages.ForEach ( o => o.color = new Color ( o.color.r, o.color.g, o.color.b, 
                Mathf.Lerp ( o.color.a, 0.0f, Time.deltaTime * fadeInSpeed ) ) );

            yield return null;

        } while ( allInstructionImages.First ( ).color.a > ( 0f + threshold ) );

        allInstructionImages.ForEach ( o => o.color = new Color ( o.color.r, o.color.g, o.color.b, 0f ) );

    }

}
