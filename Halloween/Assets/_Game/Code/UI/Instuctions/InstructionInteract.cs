// Vector Game Dev
// Halloween 2020
// Game Dev Session


// Using Statements
using System.Collections.Generic;
using UnityEngine;


// Update the UI Interaction display
public delegate void UpdateUIInteract ( bool active, Vector2 position );


public class InstructionInteract : MonoBehaviour
{

    private new Transform transform = null;

    public List<GameObject> objects = new List<GameObject> ( );

    public static UpdateUIInteract OnUpdateUIInteract = null;



    private void OnEnable ( )
    {

        OnUpdateUIInteract += SendInteractInstructionToPosition;

    }

    private void OnDisable ( )
    {

        OnUpdateUIInteract -= SendInteractInstructionToPosition;

    }

    private void Awake ( )
    {
        transform = GetComponent<Transform> ( );
    }

    private void Start ( )
    {
        SendInteractInstructionToPosition ( false, Vector2.zero );
    }


    public void SendInteractInstructionToPosition ( bool active, Vector2 position )
    {

        objects.ForEach ( o => o.SetActive ( active ) );
        transform.position = position;

    }

}
