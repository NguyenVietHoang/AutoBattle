using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static AutoBattle.CharacterStatus_View;

namespace AutoBattle
{
    public class CharacterStatus_View : MonoBehaviour
    {
        public delegate void SetData<T>(T data);
        public SetData<BaseCharacterData> SetBaseData;

        [SerializeField]
        private GameObject root;
        [SerializeField]
        private TextMeshProUGUI atkTxt;
        [SerializeField]
        private TextMeshProUGUI hpTxt;

        public void Init(BaseCharacterData data)
        {
            SetBaseData = null;
            SetBaseData += UpdateCharacterData;

            UpdateCharacterData(data);
            SetActive(false);

        }
        
        public void SetActive(bool active)
        {
            root.SetActive(active);
        }

        private void UpdateCharacterData(BaseCharacterData data)
        {
            atkTxt.text = data.ATK.ToString();
            hpTxt.text = data.currentHP.ToString();
        }
    }
}

