// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using System.Collections;
using UnityEngine;


public class MessageMenu : MonoBehaviour
{

    public float delayToLoad = 2.0f;
    public string sceneName = "Start Menu";


    private void Start ( )
    {

        StartCoroutine ( LoadStartMenu ( ) );
        
    }

    private IEnumerator LoadStartMenu ( )
    {

        yield return new WaitForSeconds ( delayToLoad );
        SimpleSceneLoader._Instance.LoadScene ( sceneName );

    }

}
