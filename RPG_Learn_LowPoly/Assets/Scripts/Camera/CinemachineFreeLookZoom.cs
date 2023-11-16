using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CinemachineCamera
{
    public class CinemachineFreeLookZoom : MonoBehaviour
    {
        private CinemachineFreeLook freelook;  // Referência à câmera livre do Cinemachine.
        private CinemachineFreeLook.Orbit[] originalOrbits;  // Armazena as órbitas originais da câmera.

        [Tooltip("The minimum scale for the orbits")]
        [Range(0.01f, 1f)]
        public float minScale = 0.5f;  // Escala mínima para o zoom.

        [Tooltip("The maximum scale for the orbits")]
        [Range(1F, 5f)]
        public float maxScale = 1;  // Escala máxima para o zoom.

        [Tooltip("The zoom axis. Value is 0..1. How much to scale the orbits")]
        [AxisStateProperty]
        public AxisState zAxis = new AxisState(0, 1, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);

        void OnValidate()
        {
            minScale = Mathf.Max(0.01f, minScale);  // Garante que minScale seja pelo menos 0.01.
            maxScale = Mathf.Max(minScale, maxScale);  // Garante que maxScale seja pelo menos igual a minScale.
        }

        void Awake()
        {
            freelook = GetComponent<CinemachineFreeLook>();  // Obtém a componente CinemachineFreeLook anexada a este GameObject.

            if (freelook != null)
            {
                originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];  // Inicializa o array de órbitas originais.

                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    // Salva as alturas e raios originais das órbitas da câmera.
                    originalOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
                    originalOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
                }

#if UNITY_EDITOR
                // Registra o método RestoreOriginalOrbits para ser chamado ao salvar a cena no Editor.
                SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
                SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
#endif
            }
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            // Remove o método RestoreOriginalOrbits do evento de salvamento ao destruir o objeto.
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
        }

        private void RestoreOriginalOrbits()
        {
            if (originalOrbits != null)
            {
                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    // Restaura as alturas e raios originais das órbitas da câmera.
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius;
                }
            }
        }
#endif

        void Update()
        {
            if (originalOrbits != null)
            {
                zAxis.Update(Time.deltaTime);  // Atualiza o eixo de zoom com base no tempo.
                float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);  // Calcula a escala de zoom interpolando entre minScale e maxScale.

                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    // Aplica a escala de zoom às órbitas da câmera, alterando as alturas e raios.
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * scale;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * scale;
                }
            }
        }
    }
}