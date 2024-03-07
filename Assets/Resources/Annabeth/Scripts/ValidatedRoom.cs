using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidatedRoom : Room
{
    public bool hasTopExit;
    public bool hasDownExit;
    public bool hasLeftExit;
    public bool hasRightExit;

    public bool hasLeftToDownPath;
    public bool hasLeftToRightPath;
    public bool hasLeftToUpPath;

}
