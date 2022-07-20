using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction.Examples;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public LeapProvider handController;
    public GameObject CanvasHolder;
    private float palmDeg;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        PalmRadius(handController.CurrentFrame.Hands);
    }

    private void PalmRadius(List<Hand> hand)
    {
        if (hand.Count != 0)
        {
            var currentLeftHand = hand[0];
            float palmRad = currentLeftHand.PalmNormal.Roll;
            palmDeg = (180 / Mathf.PI) * palmRad;
            //Debug.Log("PalmNormal Radius:" + palmDeg); 
        }
        if (palmDeg > 120)
            CanvasHolder.SetActive(true);
        else
            CanvasHolder.SetActive(false);
        return ;
    }

    public void ButtonPress()
    {
        Debug.Log("Buttton is pressed!");
    }
}
