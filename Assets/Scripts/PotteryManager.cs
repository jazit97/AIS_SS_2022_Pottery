using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity
{
    public class PotteryManager : MonoBehaviour
    {
        #region Attributes

        public int ClayResolution; // number of vertices per unit
        public float ClayHeight, ClayRadius, ClayVariance;
        public float effectStrength, affectedArea; 
        public Lathe latheController;
        private Spline spline; 
        public Controller leapController;
        public Vector3 positionOfTip; // position of fingertip
        public LeapServiceProvider handController;

        #endregion

        #region Enums

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

        #endregion

        #region Unity_Lifecycle

        void Start()
        {
            spline = new Spline(ClayRadius, ClayHeight, ClayResolution, ClayVariance);
                        latheController.init(spline.getSplineList());
                        latheController.updateMesh(spline.getSplineList());
        }
        void Update()
        {
            reactToGesture();
            
            List<Vector3> updatedSpline = spline.getSplineList();
            latheController.updateMesh(updatedSpline);
        }
        public  void Initialise()
        {
            spline = new Spline(ClayRadius, ClayHeight, ClayResolution, ClayVariance);
            latheController.init(spline.getSplineList());
            latheController.updateMesh(spline.getSplineList());
        }
        #endregion

        #region Gesture Recognition
        /// <summary>
        /// Recognizes the current gesture and chooses the corresponding deform function. Does nothing if no gesture is
        /// recognized.
        /// </summary>
        private void reactToGesture()
        {
            // get current gesture
            var currentGesture = getCurrentGesture(handController.CurrentFrame.Hands);
            Debug.Log(currentGesture);
            switch (currentGesture)
            {
                case GESTURE.PUSH1: // push with one finger
                    push1();
                    break;
                case GESTURE.PULL1: //pull with open hand - use height 
                    pull1();
                    break;
                case GESTURE.SMOOTH1: 
                    smooth1();
                    break;
                case GESTURE.NONE:
                    break;
                default: // Wenn keine Geste erkannt wurde
                    Debug.Log("no gesture defined for " + currentGesture);
                    break;
            }
        }

        /// <summary>
        /// Scans the given Hand for a recognizable gesture. 
        /// </summary>
        /// <param name="hand">List of visible hands. First element in the list will be scanned for gesture</param>
        /// <returns></returns>
        private GESTURE getCurrentGesture(List<Hand> hand)
        {
            // if no hands are visible ==> do nothing
            if (hand.Count == 0)
            {
                Debug.Log("No hands visible");
                return GESTURE.NONE;
            }
           
            // Closest finger to mesh
            Vector3 closestFinger = getClosestFinger(hand[0]);
            Vector3 thumbPosition = getScaledPosition(hand[0], LEAPHAND.THUMB);
            Vector3 palmPosition = getScaledPosition(hand[0], LEAPHAND.PALM);

            // is palm in obj & finger-palm-center angle ~90 degree?=>smooth
            Vector3 v1 = palmPosition - closestFinger;
            Vector3 v2 = palmPosition - new Vector3(0, palmPosition.y, 0);
            float dotValue = Vector3.Dot(v1.normalized, v2.normalized);
            if (dotValue > 1.0f)
                dotValue = 1.0f;
            else if (dotValue < -1.0f)
                dotValue = -1.0f;

            GESTURE recognizedGesture;

            bool angleInRangePlusMinus72 = dotValue < 0.3f && dotValue > -0.3f;
            bool palmCloseToMesh = spline.DistanceToMesh(palmPosition) <= 0.05f;

            bool thumbIsTouchingMesh = (spline.DistanceToMesh(thumbPosition) <= 0f);
            bool closestFingerIsTouchingMesh = spline.DistanceToMesh(getClosestFinger(hand[0], true)) <= 0f;
            
            if ( angleInRangePlusMinus72 && palmCloseToMesh)
            {
                positionOfTip = palmPosition;
                recognizedGesture = GESTURE.SMOOTH1;
            }
            else if (thumbIsTouchingMesh && closestFingerIsTouchingMesh)
            {
                // is thumb in obj? & closest in obj => pull
                positionOfTip = getClosestFinger(hand[0], true);
                recognizedGesture = GESTURE.PULL1;
            }
            else if (spline.DistanceToMesh(closestFinger) < 0f)
            {
                // if closest in obj=> push
                positionOfTip = getClosestFinger(hand[0]);
                recognizedGesture = GESTURE.PUSH1;
            }
            else
            {
                return GESTURE.NONE;
            }

            return recognizedGesture;
        }
        
        /// <summary>
        /// Scales the given hand with the position and scale of the leap handcontroller gameobject.
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="type">Hand part</param>
        /// <returns></returns>
        private Vector3 getScaledPosition(Hand hand, LEAPHAND type)
        {
            Vector3 retVal;
            //get Leap-Coords
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

            //scale with handControler
            retVal *= handController.transform.localScale.x;

            //absolute value 
            if (retVal.x < 0)
                retVal.x = retVal.x * -1;
            if (retVal.y < 0)
                retVal.y = retVal.y * -1;
            if (retVal.z < 0)
                retVal.z = retVal.z * -1;
            
            //offset with handcontroller
            retVal += handController.transform.position;

            return retVal;
        }
        
        /// <summary>
        /// Get the closest finger to the mesh.
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="ignoreThumb">If the thumb should be counted as closest finger</param>
        /// <returns>Finger closest to the mesh</returns>
        private Vector3 getClosestFinger(Hand hand, bool ignoreThumb = false)
        {
            Vector3 closestFinger = Vector3.zero;

            //iterate over all fingers
            for (int i = 0; i < hand.Fingers.Count; i++)
            {
                //scale finger
                Vector3 tmp = handController.transform.localScale.x * hand.Fingers[i].TipPosition.ToVector3();
                
                if (tmp.x < 0)
                    tmp.x = tmp.x * -1;
                if (tmp.y < 0)
                    tmp.y = tmp.y * -1;
                if (tmp.z < 0)
                    tmp.z = tmp.z * -1;
                
                //adjust in worldspace according to parent object
                tmp += handController.transform.position;
                if (ignoreThumb)
                {
                    // if thumb => jump to next finger
                    if (hand.Fingers[i].Type == Finger.FingerType.TYPE_THUMB)
                        continue; // next Finger
                }

                if (closestFinger == Vector3.zero) // first iteration? -> closest finger to mesh is whatever finger is currently selected
                {
                    closestFinger = tmp;
                }
                else if (spline.DistanceToMesh(closestFinger) > spline.DistanceToMesh(tmp))
                {
                    closestFinger = tmp;
                }
            }
            return closestFinger;
        }

        #endregion

        #region Deform Functions
        
        private void smooth1()
        {
            Debug.Log("Case smooth");
            Func<float, float> currentDeformFunction = delegate(float input) { return Mathf.Cos(input) * 1f; };
            spline.SmoothArea(positionOfTip, 0.2f, affectedArea * 0.3f, 8f, currentDeformFunction);
        }

        /// <summary>
        /// Pull the area between thumb and index finger out.
        /// </summary>
        private void pull1()
        {
            Func<float, float> currentDeformFunction = delegate(float input) { return Mathf.Cos(input) * 0.5f; };

            Vector3 indexTipPosition = getScaledPosition(handController.CurrentFrame.Hands[0], LEAPHAND.INDEX);
            Vector3 thumbTipPosition = getScaledPosition(handController.CurrentFrame.Hands[0], LEAPHAND.THUMB);

            float affectedHeight = Mathf.Abs(indexTipPosition.y - thumbTipPosition.y);
            Vector3 center = (indexTipPosition - thumbTipPosition) / 2f; // center between thumb and index
            center.y += thumbTipPosition.y; // adjust center to actual height
            
            spline.PullAtPosition(center,effectStrength, affectedHeight/2f , currentDeformFunction, Spline.UseAbsolutegeHeight);
        }

        /// <summary>
        /// Push mesh in with closest finger.
        /// </summary>
        private void push1()
        {
            Func<float, float> currentDeformFunction = delegate(float input)
            {
                return Mathf.Pow(Mathf.Cos(input), 2f);
            };
            spline.PushAtPosition(positionOfTip, spline.DistanceToMesh(positionOfTip), effectStrength,
                affectedArea, currentDeformFunction);
        }

        
        #endregion
    } // End class
} // End Namespace