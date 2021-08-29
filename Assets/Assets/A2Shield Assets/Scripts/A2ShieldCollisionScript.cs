using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A2Shield
{
    public class A2ShieldCollisionScript : MonoBehaviour
    {
        [SerializeField] Collider collider;
        static public List<PlayerCharacterScript> PlayerCharacterScripts = new List<PlayerCharacterScript>();

        private void Update()
        {
            foreach (PlayerCharacterScript p in PlayerCharacterScripts)
            {
                CharacterController controller = p.controller;
                Vector3 shieldDir = transform.forward;
                Vector3 rPos = controller.gameObject.transform.position - transform.position;
                float velDotShield = Vector3.Dot(controller.velocity, shieldDir);
                float rposDotShield = Vector3.Dot(rPos, shieldDir);
                if (velDotShield > 0)
                {
                    Physics.IgnoreCollision(controller, collider, true);
                }
                else
                {
                    Physics.IgnoreCollision(controller, collider, false);
                }
            }
        }
    }
}
