using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRegistry : MonoBehaviour
{
    // Start is called before the first frame update
    private bool inputLocked;
    private float inputInterval = 0.5f;
    private float timer;
    void Start()
    {
        inputLocked = false;
        timer = inputInterval;
    }

    // Update is called once per frame
    void Update()
    {   
        timer += Time.deltaTime;
        if (timer >= inputInterval)
        {
            inputLocked = false;
        }
        
        if (!inputLocked)
        {
            checkForPressedKeys();
            //Lock input to prevent 60 inputs per second
            lockInput();
        }

        

        


    }

    private static void checkForPressedKeys()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //slow down
            RotateCylinder.decrementSpeed();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            //speed up
            RotateCylinder.incrementSpeed();
        }
    }

    void lockInput()
    {
        inputLocked = false;
        timer = 0;
    }
}
