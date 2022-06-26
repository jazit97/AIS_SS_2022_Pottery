using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeform : MonoBehaviour
{

    [SerializeField] private float movementSpeed;
    [SerializeField] private float force;
    [SerializeField] private CylinderDeform cylinder;

    public Rigidbody deformerRB;

    private Vector3 movementVector;

    private bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        if(deformerRB == null)
        deformerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = Input.GetMouseButton(0);

        if (isMoving)
        {
            movementVector = new Vector3(Input.GetAxis("Mouse X"), 
                Input.GetAxis("Mouse Y"), 0f) * (movementSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            deformerRB.position += movementVector;
            
        }
    }

    private void OnCollisionStay(Collision other)
    {
        Coll coll = other.collider.GetComponent<Coll>();
        if (coll != null)
        {
            // hit Collider
            coll.HitCollider(force);
            cylinder.hit(coll.index,force);
        }
    }
}

