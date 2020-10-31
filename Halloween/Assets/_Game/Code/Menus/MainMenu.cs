// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{


    public TextMeshProUGUI tmpResolution = null;
    public PlayerInput playerInput = null;


    // resolution management
    private List<Resolution> allResolutions = new List<Resolution> ( );
    private int resolutionIndex = 0;

    private bool canChangeResolution = true;



    private void Start ( )
    {

        // get all available resolutions
        Resolution[] resolutions = Screen.resolutions;

        // manage all resolutions
        foreach ( Resolution resolution in resolutions )
        {

            allResolutions.Add ( resolution );
            Debug.Log ( resolution.width + "x" + resolution.height + " : " + resolution.refreshRate );

        }

        // current resolution
        resolutionIndex = allResolutions.FindIndex ( o => 
            o.width == Screen.currentResolution.width &&
            o.height == Screen.currentResolution.height && 
            o.refreshRate == Screen.currentResolution.refreshRate );

        UpdateResolutionText ( );

        PlayerController.OnPlayerControlSchemeUpdate?.Invoke ( GameControl.KEYBOARD, GameControl.KEYBOARD );

    }

    private void UpdateResolutionText ( )
    {

        if ( !canChangeResolution )
            return;

        resolutionIndex = Mathf.Clamp ( resolutionIndex, 0, allResolutions.Count - 1 );
        tmpResolution.text = allResolutions [ resolutionIndex ].ToString ( );

    }

    private void UpdateResolution ( )
    {

        if ( !canChangeResolution )
            return;

        Screen.SetResolution (  allResolutions [ resolutionIndex ].width,
                                allResolutions [ resolutionIndex ].height,
                                true,
                                allResolutions [ resolutionIndex ].refreshRate );

        // vsync
        //QualitySettings.vSyncCount = 0;

    }


    public void OnResolutionNext ( )
    {

        resolutionIndex++;
        UpdateResolutionText ( );
        CheckScheme ( );

    }

    public void OnResolutionPrevious ( )
    {

        resolutionIndex--;
        UpdateResolutionText ( );
        CheckScheme ( );

    }

    public void OnResolutionConfirm ( )
    {

        UpdateResolution ( );
        CheckScheme ( );

    }

    public void OnPlay ( )
    {

        canChangeResolution = false;
        SimpleSceneLoader._Instance.LoadScene ( "Demo" );
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

        PlayerController.OnPlayerControlSchemeUpdate?.Invoke ( new_scheme, new_scheme );

    }

}
