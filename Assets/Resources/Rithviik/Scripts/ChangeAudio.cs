using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class ChangeAudio : MonoBehaviour
{

    public AudioSource AudioSource1;
    public AudioSource AudioSource2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isBossOpen)
        {
            AudioSource1.Stop();
            AudioSource2.Play();
           GameManager.isBossOpen = false;
        }   
    }
}
