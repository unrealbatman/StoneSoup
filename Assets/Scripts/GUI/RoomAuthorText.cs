using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomAuthorText : MonoBehaviour {

	protected Text _text;
	public bool triggered = false;

	// Use this for initialization
	void Start () {
		_text = GetComponent<Text>();
	}
		
	// Update is called once per frame
	void Update () {
        //_text.text = string.Format("Room by: {0}", GameManager.instance.currentRoom.roomAuthor);
        if (triggered)
        {
			_text.text = "HA HA HA you got tricked, find the exit";

        }
        else
        {
			_text.text = "Your are trapped in a haunted ailen base, find the key to open the door behind you to escape";
		}		
	}
}
