using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.CinemachineCamera
{
    public class ShakeCamera : MonoBehaviour
    {
        [SerializeField] private float frequencyDefault;
        [SerializeField] private float decreaseTime;

        private CinemachineFreeLook freeLookCamera;

        float magntude;
        bool increase;

        public void Start()
        {
            freeLookCamera = GameObject.Find("CM FreeLook1").GetComponent<CinemachineFreeLook>();

            increase = false;
        }

        public void Update()
        {
            if (increase)
            {
                freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = magntude;
                freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = magntude;
                magntude = Mathf.Clamp(magntude - decreaseTime * Time.deltaTime, 0, Mathf.Infinity);

                if (magntude <= 0.0f)
                {
                    freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                    freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                    increase = false;
                }
            }
        }

        public void startShaking(float maxMagntude)
        {
            magntude = maxMagntude;
            increase = true;
        }

    }
}

