using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRegistry : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action On_AKey_Pressed;
    public static event Action On_DKey_Pressed;
    /*
     * Events:
     * 1.Definiert ein event wie folgt public static event Action myEvent;
     * 2.Fire das event im Code mit myEvent();
     * 3.Subscribed in einer anderen Klasse das event wie folgt: MyEventFireClass.myEvent += methodFromNewClass;
     *
     * jedes mal wenn das event gefired wird, werden alle subscriber informiert und der entsprechende Code ausgeführt
     * WICHTIG: vergesst nicht die events wieder zu unsubscriben wenn sie nicht mehr benötigt werden (MyEventFireClass.myEvent -= methodFromNewClass)
     */
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            On_AKey_Pressed();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            On_DKey_Pressed();
        }
    }

    
}
