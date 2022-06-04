using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll : MonoBehaviour
{
    public int index;

    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        //index = transform.GetSiblingIndex();
    }

    public void HitCollider(float force)
    {
        if (boxCollider.size.x - force > 0.0f || boxCollider.size.y - force > 0.0f)
        {
            boxCollider.size = new Vector3(boxCollider.size.x - force,
            boxCollider.size.y - force,
            boxCollider.size.z);
            Debug.Log("Collider wird verändert");
        }
        else
        {
            Destroy(this);
            Debug.Log("Destroy");
        }
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
