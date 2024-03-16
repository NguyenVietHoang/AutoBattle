using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForceBar_View : MonoBehaviour
{
    [SerializeField]
    private Slider forceBar;
    [SerializeField]
    private TextMeshProUGUI result;

    public void SetBar(int ally, int enemy)
    {
        if(ally == 0)
        {
            forceBar.value = 0;
        }
        else if(enemy == 0)
        {
            forceBar.value = 1;
        }
        else
        {
            forceBar.value = Mathf.Clamp01((float)ally/enemy);
        }
    }

    public void SetMsg(string msg)
    {
        result.text = msg;
    }
}
