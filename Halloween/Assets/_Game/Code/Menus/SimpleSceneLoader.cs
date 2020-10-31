// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SimpleSceneLoader : MonoBehaviour
{


    // singleton
    private static SimpleSceneLoader _instance = null;
    public static SimpleSceneLoader _Instance
    {

        get
        {

            if ( _instance == null )
            {

                _instance = FindObjectOfType<SimpleSceneLoader> ( );

            }

            return _instance;

        }

    }


    public CanvasGroup target = null;
    public float fadeSpeed = 2.0f;


    private void OnEnable ( )
    {

        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable ( )
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }

    private void Start ( )
    {
        DontDestroyOnLoad ( gameObject );
    }




    public void LoadScene ( string name )
    {

        FadeIn ( ( ) =>
        {

            SceneManager.LoadScene ( name );

        } );

    }

    private void OnSceneLoaded ( Scene scene, LoadSceneMode mode )
    {

        FadeOut ( );

    }

    public void FadeIn ( Action callback = null )
    {

        StartCoroutine ( FadeInCoroutine ( callback ) );

    }

    public void FadeOut ( Action callback = null )
    {

        StartCoroutine ( FadeOutCoroutine ( callback ) );

    }


    private IEnumerator FadeInCoroutine ( Action callback )
    {

        do
        {

            target.alpha = Mathf.Lerp ( target.alpha, 1.0f, fadeSpeed * Time.deltaTime );
            yield return null;

        } while ( target.alpha < 0.98f );
        target.alpha = 1.0f;

        callback?.Invoke ( );

    }

    private IEnumerator FadeOutCoroutine ( Action callback )
    {

        do
        {

            target.alpha = Mathf.Lerp ( target.alpha, 0.0f, fadeSpeed * Time.deltaTime );
            yield return null;

        } while ( target.alpha > 0.02f );
        target.alpha = 0.0f;

        callback?.Invoke ( );

    }

}
