using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Frog : apt283Rock
{
    private static AB_Frog _instance;
    public static AB_Frog Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AB_Frog>();
                if (_instance == null)
                {
                    _instance = new GameObject("AB_Frog").AddComponent<AB_Frog>();
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.C))
        {
            print("haha");
            Vector3 originLocation = transform.position;
            transform.position = FindObjectOfType<Player>().transform.position;
            FindObjectOfType<Player>().transform.position = originLocation;
        }
    }
}
