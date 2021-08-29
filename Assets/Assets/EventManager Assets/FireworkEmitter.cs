using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManagerProject;

namespace EventManagerProject
{
    public class FireworkEmitter : MonoBehaviour
    {
        public ParticleSystem fireworkParticleEmitter;
        public GameObject fireworkPrefab;
        public Transform emissionPoint;
        float resetTime = 0f;

        private void Start()
        {
            EventManager.Instance.Listen("ShootFireworks", shootFirework);
            fireworkParticleEmitter.Stop();
        }

        private void Update()
        {
            resetTime += Time.deltaTime;
        }

        [ContextMenu("Test Fire Function")]
        public void shootFirework()
        {
            fireworkParticleEmitter.Emit(1);
            fireworkParticleEmitter.Play();

            Instantiate(fireworkPrefab, emissionPoint.position, Quaternion.identity);
        }

        public void shootFirework (IEventRequestInfo requestInfo)
        {
            if (resetTime < 2.0f)
            {
                return;
            }
            if (requestInfo is EventRequestInfo<int>)
            {
                // 消息包含自订资料，我们在这里做类型转换
                //Typecast request info
                EventRequestInfo<int> eventRequestInfo;
                eventRequestInfo = requestInfo as EventRequestInfo<int>;

                // Body of info consists of smoke density
                // 资料决定我们的粒子发射器会发多少粒子
                fireworkParticleEmitter.Emit(eventRequestInfo.body);
                fireworkParticleEmitter.Play();

                // Fire the firework!
                // 射它！
                Instantiate(fireworkPrefab, emissionPoint.position, Quaternion.identity);

                // Reset the reset time
                resetTime = 0f;
            }
        }

    }
}
