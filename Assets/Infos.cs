using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Infos : MonoBehaviour
{
    public string designation = "Name";
    public int team = 1;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    public void Start()
    {
        transform.parent.name = string.Format("Character : {0} ({1})", designation, team);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
