using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotteryManager : MonoBehaviour
{
    public int ClayResolution;
    public float ClayHeight, ClayRadius, ClayVariance;
    public float effectStrength, affectedArea;
    public Lathe latheController;
    private Spline spline;
    // Start is called before the first frame update
    void Start()
    {
        spline = new Spline(ClayRadius, ClayHeight, ClayResolution, ClayVariance);
        latheController.init(spline.getSplineList());
        List<Vector3> currentSpline = spline.getSplineList();

        //generate new Mesh
        latheController.updateMesh(currentSpline);
        InputRegistry.On_EKey_Pressed += deform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void deform()
    {
        Debug.Log("Deform");
        spline.PushAtPosition(spline.getVertex(spline.getSize()/2),effectStrength,affectedArea/2,affectedArea,delegate(float f) {return Mathf.Pow(Mathf.Cos(f), 0.8f);  },false );
        latheController.updateMesh(spline.getSplineList());
    }

    private void OnDestroy()
    {
        InputRegistry.On_EKey_Pressed -= deform;
    }
}
