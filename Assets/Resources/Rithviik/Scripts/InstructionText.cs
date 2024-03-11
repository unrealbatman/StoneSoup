using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InstructionText : MonoBehaviour
{
    protected Text _text;

    // Use this for initialization
    void Start()
    {
        _text = GetComponent<Text>();
        _text.text = "You are stuck in an alien chamber.Find the key to unlock the massive door!";

    }

    // Update is called once per frame
    void Update()
    {
        if ((GameManager.instructionChange==true))
        {
            _text.text = "HA HA HA!!!!. You were tricked. Find the real exit and escape the FIREEEEEE!!!!!!!!! ";

        }
       
    }
}
