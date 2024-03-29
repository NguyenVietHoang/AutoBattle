﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LoadingScene : MonoBehaviour
    {
        public void OnButtonClicked(int idx)
        {
            if (idx == 0)
            {
                SceneManager.LoadScene("DemoMixer");
            }
            else if (idx == 0)
            {
                SceneManager.LoadScene("FlappyAxie");
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }
}
