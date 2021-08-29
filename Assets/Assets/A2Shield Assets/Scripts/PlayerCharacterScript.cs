using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace A2Shield
{
    public class PlayerCharacterScript : MonoBehaviour
    {
        [SerializeField] Transform followpoint;
        public CharacterController controller { get; private set; }

        Vector3 dir;
        float speed = 5.0f;
        int ShieldLayerMask;

        private void Start()
        {
            A2ShieldCollisionScript.PlayerCharacterScripts.Add(this);
            controller = GetComponent<CharacterController>();
            ShieldLayerMask = LayerMask.NameToLayer("A2Shield");
            EventManager.Instance.Listen("ForwardInput", onForwardInput);
            EventManager.Instance.Listen("BackInput", onBackInput);
            EventManager.Instance.Listen("LeftInput", onLeftInput);
            EventManager.Instance.Listen("RightInput", onRightInput);
            EventManager.Instance.Listen("MouseInput", onMouseInput);
        }

        private void OnDestroy()
        {
            A2ShieldCollisionScript.PlayerCharacterScripts.Remove(this);
            EventManager.Instance.Close("ForwardInput", onForwardInput);
            EventManager.Instance.Close("BackInput", onBackInput);
            EventManager.Instance.Close("LeftInput", onLeftInput);
            EventManager.Instance.Close("RightInput", onRightInput);
            EventManager.Instance.Close("MouseInput", onMouseInput);
        }

        void onLeftInput(IEventRequestInfo info)
        {
            dir -= transform.right;
        }

        void onRightInput(IEventRequestInfo info)
        {
            dir += transform.right;
        }

        void onForwardInput(IEventRequestInfo info)
        {
            dir += transform.forward;
        }

        void onBackInput(IEventRequestInfo info)
        {
            dir -= transform.forward;
        }

        void onMouseInput(IEventRequestInfo info)
        {
            if (info is EventRequestInfo<MouseData>)
            {
                EventRequestInfo<MouseData> mouseInfo = (EventRequestInfo<MouseData>)info;
                followpoint.transform.Rotate(-Vector3.right * mouseInfo.body.Y);
                transform.Rotate(Vector3.up * mouseInfo.body.X);
            }
        }

        private void Update()
        {
            if (dir.sqrMagnitude != 0)
                controller.Move(speed * dir.normalized * Time.deltaTime);

            dir = Vector3.zero;
        }
    }
}