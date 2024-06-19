using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UHFPS.Tools;
using ThunderWire.Attributes;
using Newtonsoft.Json.Linq;

namespace UHFPS.Runtime
{
    [InspectorHeader("Interactable Light")]
    public class InteractableLight : MonoBehaviour, IInteractStart, ISaveable
    {
        [Serializable]
        public struct LightEventsStruct
        {
            public UnityEvent OnLightOn;
            public UnityEvent OnLightOff;
        }

        public sealed class LightComponent
        {
            public Light light;
            public float intensity;
            public float current;
        }

        public bool IsSwitchedOn;

        [Header("Light")]
        public List<Light> LightComponents = new();
        public bool SmoothLight;
        public float SmoothDuration;

        [Header("Emission")]
        public RendererMaterial LightMaterial;
        [Space]
        public string EmissionKeyword = "_EMISSION";
        public bool EnableEmission = true;

        [Header("Sounds")]
        public SoundClip LightSwitchOn;
        public SoundClip LightSwitchOff;

        [Space, Boxed("AnimationWindowEvent Icon", title = "Light Events")]
        public LightEventsStruct LightEvents = new();

        private LightComponent[] lightComponents;

        private void Awake()
        {
            lightComponents = new LightComponent[LightComponents.Count];
            for (int i = 0; i < LightComponents.Count; i++)
            {
                lightComponents[i] = new LightComponent()
                {
                    light = LightComponents[i],
                    intensity = LightComponents[i].intensity,
                    current = LightComponents[i].intensity
                };
            }

            SetLightState(IsSwitchedOn);
        }

        public void InteractStart()
        {
            if(IsSwitchedOn = !IsSwitchedOn)
            {
                SetLightState(true);
                GameTools.PlayOneShot3D(transform.position, LightSwitchOn, "Lamp On");
                LightEvents.OnLightOn?.Invoke();
            }
            else
            {
                SetLightState(false);
                GameTools.PlayOneShot3D(transform.position, LightSwitchOff, "Lamp Off");
                LightEvents.OnLightOff?.Invoke();
            }
        }

        public void SetLightState(bool state)
        {
            if(!SmoothLight) LightComponents.ForEach(x => x.enabled = state);
            else
            {
                StopAllCoroutines();
                StartCoroutine(SwitchLightSmoothly(state));
            }

            if (LightMaterial.IsAssigned && EnableEmission)
            {
                if (state) LightMaterial.ClonedMaterial.EnableKeyword(EmissionKeyword);
                else LightMaterial.ClonedMaterial.DisableKeyword(EmissionKeyword);
            }

            IsSwitchedOn = state;
        }

        IEnumerator SwitchLightSmoothly(bool state)
        {
            float elapsedTime = 0;

            // set current light intensity
            foreach (var light in lightComponents)
            {
                light.current = light.light.intensity;
                if(state)
                {
                    light.light.intensity = 0f;
                    light.light.enabled = true;
                }
            }

            // lerp light intensity smoothly
            while (elapsedTime < SmoothDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / SmoothDuration);

                foreach (var light in lightComponents)
                {
                    float target = state ? light.intensity : 0f;
                    light.light.intensity = Mathf.Lerp(light.current, target, t);
                }

                yield return null;
            }

            // disable light after lerping
            if (!state)
            {
                foreach (var light in lightComponents)
                {
                    light.current = 0f;
                    light.light.enabled = false;
                }
            }
        }

        public StorableCollection OnSave()
        {
            return new StorableCollection()
            {
                { "lightState", IsSwitchedOn }
            };
        }

        public void OnLoad(JToken data)
        {
            bool lightState = (bool)data["lightState"];
            SetLightState(lightState);
        }
    }
}