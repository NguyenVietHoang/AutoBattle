using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedControl : MonoBehaviour
{
    [SerializeField]
    private float speedStep, speedMin, speedMax;
    public void SpeedUp()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale + speedStep, speedMin, speedMax);
    }

    public void SpeedDown()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale - speedStep, speedMin, speedMax);
    }
}
