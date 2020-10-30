// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Blink Object


// Using Statements
using System.Collections;
using UnityEngine;


public class BlinkObject : MonoBehaviour
{

    public SpriteRenderer[] renderers = null;

    public float timeRange = 15.0f;
    public float random = 5.0f;

    public float blinkingTime = 0.1f;

    private float counter = 0.0f;
    private float targetTimer = 0.0f;

    private bool isBlinking = false;

    private void Start ( )
    {
        
        foreach ( SpriteRenderer renderer in renderers )
            renderer.enabled = false;

        counter = 0.0f;
        CalculateTimer ( );

    }

    private void Update ( )
    {
        if ( isBlinking )
            return;

        counter += Time.deltaTime;
        if ( counter > targetTimer )
        {

            CalculateTimer ( );
            counter = 0f;

            isBlinking = true;
            StartCoroutine ( Blinking ( ) );

        }
        
    }

    private void CalculateTimer ( )
    {

        targetTimer = timeRange + Random.Range ( 0.0f, random );

    }

    private IEnumerator Blinking ( )
    {

        foreach ( SpriteRenderer renderer in renderers )
            renderer.enabled = true;

        yield return new WaitForSeconds ( blinkingTime * 3 );

        foreach ( SpriteRenderer renderer in renderers )
            renderer.enabled = false;

        //yield return new WaitForSeconds ( blinkingTime );

        //foreach ( SpriteRenderer renderer in renderers )
        //    renderer.enabled = true;

        //yield return new WaitForSeconds ( blinkingTime );

        //foreach ( SpriteRenderer renderer in renderers )
        //    renderer.enabled = false;

        isBlinking = false;

    }


}
