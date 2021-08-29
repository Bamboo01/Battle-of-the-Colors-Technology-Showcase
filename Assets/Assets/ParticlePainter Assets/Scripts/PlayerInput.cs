using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePainter
{
    public class MouseData
    {
        public float X;
        public float Y;
        public MouseData(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public class PlayerInput : MonoBehaviour
    {
        EventManager em;
        private void Start()
        {
            em = EventManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                em.Publish("ForwardInput", this);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                em.Publish("BackInput", this);
            }

            if (Input.GetKey(KeyCode.A))
            {
                em.Publish("LeftInput", this);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                em.Publish("RightInput", this);
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            em.Publish("MouseInput", this, new MouseData(mouseX, mouseY));
        }
    }

}