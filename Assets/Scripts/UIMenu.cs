using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;


public class UIMenu : MonoBehaviour
{
    public LeapProvider handController;
    public GameObject CanvasHolder;
    public PotteryManager Manager;
    private float palmDeg;

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
    }

    public void export()
        {
            GameObject cylinder = GameObject.Find("ClayCylinder");
            Mesh mesh = cylinder.GetComponent<MeshFilter>().mesh;
            Export.ExportMeshToSTL(mesh);
        }
    public void reset()
        {
            Manager.Initialise();
        }
    
    
}
