using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManagerProject;

namespace EventManagerProject
{
    public class FireworkScript : MonoBehaviour
    {
        float xNoise;
        float zNoise;
        float timeAlive;
        float lifetime = 10f;

        // Start is called before the first frame update
        void Start()
        {
            xNoise = Random.Range(1.0f, 1.8f);
            zNoise = Random.Range(1.0f, 1.8f);
            timeAlive = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            timeAlive += Time.deltaTime;
            transform.Translate(0, Time.deltaTime * 5f * timeAlive, 0);
            Vector3 position = transform.position;
            position.y += Time.deltaTime * 2f;
            position.x = Mathf.Sin(timeAlive) * xNoise;
            position.z = Mathf.Sin(timeAlive) * zNoise;
            if (timeAlive >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
