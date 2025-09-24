using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TimeOfDayController : MonoBehaviour
    {
        private float _timeOfDay; // 0 to 24
        private GameObject _lightGameObject;
        private Light _light;

        private void Start()
        {
            _lightGameObject = GameObject.Find("sun");
            if (_lightGameObject == null)
            {
                Debug.LogError("No Directional Light found in scene. Please add one and name it 'Directional Light'.");
            }

            _light = _lightGameObject.GetComponent<Light>();
            _timeOfDay = 12f; // Start at noon
            setSunAndLighting(_timeOfDay);
        }

        private void Update()
        {
            _timeOfDay += Time.deltaTime * 0.1f; // Speed up time for testing
            if (_timeOfDay >= 24f) _timeOfDay = 0f;

            setSunAndLighting(_timeOfDay);
        }
        
        public float getTimeOfDay()
        {
            return _timeOfDay;
        }

        private void setSunAndLighting(float timeOfDay)
        {
            //set sun x rotation where noon is 90 degrees and midnight is -90 degrees
            var sunAngle = (timeOfDay / 24f) * 360f - 90f;
            _lightGameObject.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
            //set intensity based on angle so that at noon it's 1 and at midnight it's 0
            _light.intensity = Mathf.Clamp01(Mathf.Cos((timeOfDay - 12f) / 24f * 2f * Mathf.PI));
        }
    }
}