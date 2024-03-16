using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattle
{
    public class CharacterMain_View : MonoBehaviour
    {
        [SerializeField]
        private CharacterStatus_View statusView;
        [SerializeField]
        private HPBar_View hPBar_View;
        [SerializeField]
        private Canvas mainCanvas;
        [SerializeField]
        private GameObject hightlightObj;

        // Start is called before the first frame update
        public void Init(BaseCharacterData data)
        {
            hPBar_View.Init(GetHP_InPercent(data));
            statusView.Init(data);
            mainCanvas.worldCamera = Camera.main;
            SetHightlight(false);
        }

        public void SetHightlight(bool isHighlighted)
        {
            hightlightObj.SetActive(isHighlighted);
            statusView.SetActive(isHighlighted);
        }
        private float GetHP_InPercent(BaseCharacterData currentData)
        {
            if (currentData == null || currentData.HP <= 0)
                return 0;
            else
                return Mathf.Clamp01((float)currentData.currentHP / currentData.HP);
        }

        public void ModifyHP(BaseCharacterData data)
        {
            statusView.SetBaseData?.Invoke(data);
            hPBar_View.SetValue(GetHP_InPercent(data));
        }
    }
}

