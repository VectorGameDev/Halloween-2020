// Vector Game Dev
// Halloween 2020
// Game Dev Session

// Using Statements
using UnityEngine;



public class LoadLevelTrigger : MonoBehaviour
{


    public string sceneName = "";


    public void OnTriggerEnter2D ( Collider2D collision )
    {

        if ( collision.CompareTag ( "Player" ) )
        {

            SimpleSceneLoader._Instance.LoadScene ( sceneName );

        }
        
    }

}
