using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManagerProject;

namespace EventManagerProject
{
    public class CubeController : MonoBehaviour
    {
        public float speed = 5.0f;

        void Update()
        {
            float zinput = Input.GetAxis("Vertical");
            float xinput = Input.GetAxis("Horizontal");

            transform.Translate(xinput * Time.deltaTime * speed, 0, zinput * Time.deltaTime * speed);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Requesting an event to all Listeners of channel ShootFireworks");
            // Send an event to all listeners of "ShootFireworks"
            // 把消息发给正在听"ShootFireworks"名词的频道
            EventManager.Instance.Publish("ShootFireworks", this, 3);
        }
    }
}


