using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AutoBattle
{
    public class HPBar_View : MonoBehaviour
    {
        [SerializeField]
        private Slider hp_Slider;
        [SerializeField]
        private float decay_Speed = 0.25f;

        public void Init(float value)
        {
            SetValue(value, true);
        }

        float targetValue = 0;
        public void SetValue(float value, bool instant = false)
        {
            targetValue = Mathf.Clamp01(value);
            if (instant)
            {
                hp_Slider.value = targetValue;
                isProcessing = false;
            }
            else
            {
                isProcessing = true;
            }            
        }

        bool isProcessing;
        private void Update()
        {
            if(isProcessing)
            {
                hp_Slider.value = Mathf.Lerp(hp_Slider.value, targetValue, decay_Speed);
            }
        }
    }
}

