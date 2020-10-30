// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InstructionMovement : MonoBehaviour
{

    public List<SpriteRenderer> allInstructionImages = new List<SpriteRenderer> ( );

    public float startDelay = 5.0f;
    public float fadeInSpeed = 2.0f;
    public float fadeOutSpeed = 2.0f;
    public float displayTime = 2.0f;
    public float threshold = 0.05f;


    private void Start ( )
    {
        if ( allInstructionImages.Count > 0 ) 
            allInstructionImages.ForEach ( o => o.color = new Color ( o.color.r, o.color.g, o.color.b, 0f ) );
    }

    public void StartInstruction ( )
    {
        if ( allInstructionImages.Count > 0 ) 
            StartCoroutine ( FadeInOut ( ) );
    }


    private IEnumerator FadeInOut ( )
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
