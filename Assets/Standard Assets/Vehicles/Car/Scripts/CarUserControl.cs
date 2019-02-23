using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        private float v;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void Update()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            if (CrossPlatformInputManager.GetButtonDown("Vertical")) {
                v = 1;
            }
            else if (CrossPlatformInputManager.GetButtonUp("Vertical")) {
                v = 0;
            }
            float v2 = CrossPlatformInputManager.GetAxis("Vertical2");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v2, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
