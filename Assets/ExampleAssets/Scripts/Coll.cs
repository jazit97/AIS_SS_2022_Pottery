using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll : MonoBehaviour
{
    public int index;

    private BoxCollider boxCollider;

    private const float force = 0.0002f;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        //index = transform.GetSiblingIndex();
    }

    public void HitCollider(float impactForce)
    {
        if (boxCollider.size.x - impactForce > 0.0f || boxCollider.size.y - impactForce > 0.0f)
        {
            boxCollider.size = new Vector3(boxCollider.size.x - impactForce, boxCollider.size.y - impactForce, boxCollider.size.z);
            Debug.Log("Collider wird verändert");
        }
        else
        {
            //Destroy(this);
            Debug.Log("Why the fuck would I Destroy");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //HitCollider(force);
    }

    private void OnCollisionStay(Collision other)
    {
        //HitCollider(force);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
