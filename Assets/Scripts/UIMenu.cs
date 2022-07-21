using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Playables;


public class UIMenu : MonoBehaviour
{
    public LeapProvider handController;
    public GameObject CanvasHolder;
    public PotteryManager Manager;
    private float palmDeg;
    public GameObject NotificationCanvas;

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
            StartCoroutine(FadeCo());
        }
    public void reset()
        {
            Manager.Initialise();
        }
    
    public IEnumerator FadeCo()
    {
        NotificationCanvas.SetActive(true);
        yield return new WaitForSeconds(1);
        NotificationCanvas.SetActive(false);
    }
    
}
