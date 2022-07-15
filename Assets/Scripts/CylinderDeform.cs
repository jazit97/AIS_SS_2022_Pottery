using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderDeform : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private float maxKeyWidth = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hit(int keyIndex, float force)
    {
        float colliderHeight = 0.00091f;
        float newWeight = skinnedMeshRenderer.GetBlendShapeWeight(keyIndex) + force * (10f/colliderHeight); //100f/colliderheight
        if (newWeight > maxKeyWidth) newWeight = maxKeyWidth;
        skinnedMeshRenderer.SetBlendShapeWeight(keyIndex, newWeight);
    }
}
