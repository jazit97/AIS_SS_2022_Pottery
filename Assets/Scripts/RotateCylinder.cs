using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCylinder : MonoBehaviour
{
    private GameObject cylinder;
    
        private static float degreesPerSecond = 90f;
    
        // Start is called before the first frame update
        void Start()
        {
            cylinder = GameObject.Find("Cylinder");
            InputRegistry.On_AKey_Pressed += decrementSpeed;
            InputRegistry.On_DKey_Pressed += incrementSpeed;
        }
    
        // Update is called once per frame
        void Update()
        {
            //1.Param: Punkt um den rotiert werden soll, bei der Rotation um sich selbst ist das die position des objekts selbst
            //2.Param: Eine der drei Achsen um die rotiert werden soll
            //3.Param: Gradanzahl um die rotiert werden soll, durch Einberechnung von dtime 
            //cylinder.transform.RotateAround(transform.position, transform.forward, Time.deltaTime * degreesPerSecond);
            cylinder.transform.Rotate(Vector3.up * Time.deltaTime*degreesPerSecond);
    }

        public static void incrementSpeed()
        {
            degreesPerSecond++;
            Debug.Log("Incremented to: " + degreesPerSecond);
        }

        public static void decrementSpeed()
        {
            degreesPerSecond--;
            Debug.Log("Decremented to: " + degreesPerSecond);
        }

        private void OnDestroy()
        {
            InputRegistry.On_AKey_Pressed -= decrementSpeed;
            InputRegistry.On_DKey_Pressed -= incrementSpeed;
        }
}
