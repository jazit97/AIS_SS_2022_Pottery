using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultraleap;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction.Examples;

namespace Leap.Unity
{
    

    public class PotteryManager : MonoBehaviour
    {
        public int ClayResolution;
        public float ClayHeight, ClayRadius, ClayVariance;
        public float effectStrength, affectedArea;
        public Lathe latheController;
        private Spline spline;
        public Controller leapController;
        public Vector3 positionOfTip;

        public LeapServiceProvider handController;

        enum GESTURE // Welche Gesten genutzt werden können
        {
            PUSH1,
            PULL1,
            PULL2,
            NONE,
            SMOOTH1,
        }
        enum LEAPHAND // Welche einzelnen Teile durch die Leap erkannt werden sollen ( Welche Finger ) 
        {
            INDEX,
            MIDDLE,
            PINKY,
            RING,
            THUMB,
            PALM
        }
        // Start is called before the first frame update
        private GESTURE getCurrentGesture(List<Hand> hand)
        {
            // Nearest Finger Detection
            Vector3 closestFinger = getClosestFinger(hand[0]);
            Vector3 thumbPosition = getScaledPosition(hand[0], LEAPHAND.THUMB);
            Vector3 palmPosition = getScaledPosition(hand[0], LEAPHAND.PALM);
            
            // is palm in obj & finger-palm-center angle ~90 degree?=>smooth
            Vector3 v1 = palmPosition - closestFinger;
            Vector3 v2 = palmPosition - new Vector3(0, palmPosition.y, 0);
            float dotValue = Vector3.Dot(v1.normalized, v2.normalized);  // Berechnet das Skalarprodukt
            if (dotValue > 1.0f)
                dotValue = 1.0f;
            else if (dotValue < -1.0f)
                dotValue = -1.0f;

            GESTURE recognizedGesture;

            if (dotValue < 0.3f && dotValue > -0.3f && spline.DistanceToMesh(palmPosition) <= 0.05f)
            {
                positionOfTip = palmPosition;
                Debug.Log("dotValue:\t" + dotValue + "\tsmoothing");
                recognizedGesture = GESTURE.SMOOTH1;
            }
            else if ((spline.DistanceToMesh(thumbPosition) <= 0f) && (spline.DistanceToMesh(getClosestFinger(hand[0], true)) <= 0f))
            {         // is thumb in obj? & closest in obj => pull
                positionOfTip = getClosestFinger(hand[0], true);
                Debug.Log(thumbPosition + " | " + getClosestFinger(hand[0], true));
                recognizedGesture = GESTURE.PULL1;
                
            }
            else if (spline.DistanceToMesh(closestFinger) < 0f)
            {        // if closest in obj=> push
                positionOfTip = getClosestFinger(hand[0]);
                Debug.Log("push: "+ spline.DistanceToMesh(closestFinger));
                recognizedGesture = GESTURE.PUSH1;
            }
            else
            {
                return GESTURE.NONE;
            }

            Debug.Log(recognizedGesture);
            return recognizedGesture;
        }

        private Vector3 getClosestFinger(Hand hand, bool ignoreThumb = false)
        {
            int index = 0;
            Vector3 closestFinger = Vector3.zero;

            for (int i = 0; i < hand.Fingers.Count; i++)
            {
                
                Vector3 tmp = handController.transform.localScale.x * hand.Fingers[i].TipPosition.ToVector3();
             /*   if (tmp.x < 0)
                    tmp.x = tmp.x * -1;
                if (tmp.y < 0)
                    tmp.y = tmp.y * -1;
                if (tmp.z < 0)
                    tmp.z = tmp.z * -1; */

                tmp += handController.transform.position;
                if (ignoreThumb)
                {
                    // if thumb => jump to next finger
                    if (hand.Fingers[i].Type == Finger.FingerType.TYPE_THUMB)
                        continue; // next Finger
                } 
                if (closestFinger == Vector3.zero) 
                {
                    index = i;
                    closestFinger = tmp;
                }
                else if (spline.DistanceToMesh(closestFinger) > spline.DistanceToMesh(tmp))
                {
                    index = i;
                    closestFinger = tmp;
                }
            }
            //Debug.Log("closest finger: " + hand.Fingers[index].Type);
            return closestFinger;
        }
        
        private Vector3 getScaledPosition(Hand hand, LEAPHAND type)
        {
            Vector3 retVal;
            // 3 steps:
            // 1 get Leap-Coords
            switch (type)
            {
                case LEAPHAND.INDEX:
                    retVal = hand.GetIndex().TipPosition.ToVector3();
                    break;
                case LEAPHAND.MIDDLE:
                    retVal = hand.GetMiddle().TipPosition.ToVector3();
                    break;
                case LEAPHAND.RING:
                    retVal = hand.GetRing().TipPosition.ToVector3();
                    break;
                case LEAPHAND.PINKY:
                    retVal = hand.GetPinky().TipPosition.ToVector3();
                    break;
                case LEAPHAND.THUMB:
                    retVal = hand.GetThumb().TipPosition.ToVector3();
                    break;
                case LEAPHAND.PALM:
                    retVal = hand.GetPalmPose().position;
                    break;
                default:
                    retVal = hand.GetIndex().TipPosition.ToVector3();
                    break;
            }

            // 2 scale with handControler
            retVal *= handController.transform.localScale.x;
        
            // 3 offset with handcontroller
            retVal += handController.transform.position;

            return retVal;
        }
        void Start()
        {

            spline = new Spline(ClayRadius, ClayHeight, ClayResolution, ClayVariance);
            latheController.init(spline.getSplineList());
            List<Vector3> currentSpline = spline.getSplineList();
            //leapController = handController.GetLeapController();
            //generate new Mesh
            latheController.updateMesh(currentSpline);
            InputRegistry.On_EKey_Pressed += deform;
            var currentGesture = getCurrentGesture(handController.CurrentFrame.Hands);

                    switch (currentGesture)
                    {
                        case GESTURE.PUSH1: // Mathematische Änderung der Verformung - hier Cosinus ^ 2 
                            {
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Pow(Mathf.Cos(input), 2f); };
                                // Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 0.1f; };
                                // v-- uses the percentage number of the vertices
                                spline.PushAtPosition(positionOfTip, spline.DistanceToMesh(positionOfTip), effectStrength, affectedArea, currentDeformFunction);
                                deform();
                            }
                            break;

                        case GESTURE.PULL1: //pull with open hand - use height // Hier Cosinus * 0,5
                            {
                                // Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Pow(Mathf.Cos(input), 2f); };
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 0.5f; };
                                
                                Vector3 indexTipPosition = getScaledPosition(handController.CurrentFrame.Hands[0], LEAPHAND.INDEX); 
                                Vector3 thumbTipPosition = getScaledPosition(handController.CurrentFrame.Hands[0], LEAPHAND.THUMB);

                                float affectedHeight = Mathf.Abs(indexTipPosition.y - thumbTipPosition.y); // Betragsrechnung

                                Vector3 center = (positionOfTip + thumbTipPosition) / 2f;
                                spline.PullAtPosition(center, effectStrength * 2f, affectedHeight, currentDeformFunction, Spline.UseAbsolutegeHeight);
                            }
                            break;
                        case GESTURE.PULL2: // pull with pinch // Hier Cosinus * 0,5
                            {
                                // Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Pow(Mathf.Cos(input), 2f); };
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 0.5f; };

                                spline.PullAtPosition(positionOfTip, effectStrength * 2f, affectedArea, currentDeformFunction);
                            }
                            break;
                        case GESTURE.SMOOTH1: // Hier Cosinus * 1
                            {
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 1f; };
                                // spline.SmoothAtPosition(tipPosition, effectStrength, affectedArea*0.5f, currentDeformFunction);
                                spline.SmoothArea(positionOfTip, 0.2f, affectedArea * 0.3f, 8f, currentDeformFunction);
                            }
                            break;
                        case GESTURE.NONE: // Macht nix
                            break;
                        default: // Wenn keine Geste erkannt wurde
                            {
                                Debug.Log("no gesture defined for " + currentGesture);
                            }
                            break;
                    }
                    //}
        

        //get List of deformed Spline
        currentSpline = spline.getSplineList();

        //generate new Mesh
        latheController.updateMesh(currentSpline);
        }

        // Update is called once per frame
        void Update()
        {
            //Frame frame = handController.CurrentFixedFrame;

        // Guess what the user wants to do
       

                    // get current gesture

                    var currentGesture = getCurrentGesture(handController.CurrentFrame.Hands);

                    switch (currentGesture)
                    {
                        case GESTURE.PUSH1: // Mathematische Änderung der Verformung - hier Cosinus ^ 2 
                            {
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Pow(Mathf.Cos(input), 2f); };
                                // Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 0.1f; };
                                // v-- uses the percentage number of the vertices
                                spline.PushAtPosition(positionOfTip, spline.DistanceToMesh(positionOfTip), effectStrength, affectedArea, currentDeformFunction);
                            }
                            break;

                        case GESTURE.PULL1: //pull with open hand - use height // Hier Cosinus * 0,5
                            {
                                // Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Pow(Mathf.Cos(input), 2f); };
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 0.5f; };
                                
                                Vector3 indexTipPosition = getScaledPosition(handController.CurrentFrame.Hands[0], LEAPHAND.INDEX); 
                                Vector3 thumbTipPosition = getScaledPosition(handController.CurrentFrame.Hands[0], LEAPHAND.THUMB);

                                float affectedHeight = Mathf.Abs(indexTipPosition.y - thumbTipPosition.y); // Betragsrechnung

                                Vector3 center = (positionOfTip + thumbTipPosition) / 2f;
                                spline.PullAtPosition(center, effectStrength * 2f, affectedHeight, currentDeformFunction, Spline.UseAbsolutegeHeight);
                            }
                            break;
                        case GESTURE.PULL2: // pull with pinch // Hier Cosinus * 0,5
                            {
                                // Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Pow(Mathf.Cos(input), 2f); };
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 0.5f; };

                                spline.PullAtPosition(positionOfTip, effectStrength * 2f, affectedArea, currentDeformFunction);
                            }
                            break;
                        case GESTURE.SMOOTH1: // Hier Cosinus * 1
                            {
                                Func<float, float> currentDeformFunction = delegate (float input) { return Mathf.Cos(input) * 1f; };
                                // spline.SmoothAtPosition(tipPosition, effectStrength, affectedArea*0.5f, currentDeformFunction);
                                spline.SmoothArea(positionOfTip, 0.2f, affectedArea * 0.3f, 8f, currentDeformFunction);
                            }
                            break;
                        case GESTURE.NONE: // Macht nix
                            break;
                        default: // Wenn keine Geste erkannt wurde
                            {
                                Debug.Log("no gesture defined for " + currentGesture);
                            }
                            break;
                    }
                    //}
        

        //get List of deformed Spline
        List<Vector3> currentSpline = spline.getSplineList();

        //generate new Mesh
        latheController.updateMesh(currentSpline);
        }

        void deform()
        {
            Debug.Log("Deform");
            spline.PushAtPosition(spline.getVertex(spline.getSize() / 2), effectStrength, affectedArea / 2,
                affectedArea, delegate(float f) { return Mathf.Pow(Mathf.Cos(f), 0.8f); }, false);
            latheController.updateMesh(spline.getSplineList());
        }

        private void OnDestroy()
        {
            InputRegistry.On_EKey_Pressed -= deform;
        }
    } // End class
}// End Namespace