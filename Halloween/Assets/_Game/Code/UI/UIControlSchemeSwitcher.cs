﻿// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ControlSchemeUIData
{

    public GameControl scheme;
    public List<GameObject> objects;

}


public class UIControlSchemeSwitcher : MonoBehaviour
{


    public List<ControlSchemeUIData> uiData = new List<ControlSchemeUIData> ( );


    private void OnEnable ( )
    {

        PlayerController.OnPlayerControlSchemeUpdate += OnControlSchemeUpdated;
        
    }

    private void OnDisable ( )
    {
        
        PlayerController.OnPlayerControlSchemeUpdate -= OnControlSchemeUpdated;

    }


    private void Start ( )
    {

        PlayerController pc = FindObjectOfType<PlayerController> ( );
        if ( pc != null )
        {

            GameControl gc = pc.GetGameControl ( );
            OnControlSchemeUpdated ( gc, gc );

        }

    }


    private void OnControlSchemeUpdated ( GameControl old_scheme, GameControl new_scheme )
    {

        foreach ( ControlSchemeUIData data in uiData )
        {

            data.objects.ForEach ( o => o.SetActive ( data.scheme == new_scheme ) );

        }

    }




}
