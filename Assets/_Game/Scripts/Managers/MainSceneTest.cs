using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoBattle;
using AxieMixer.Unity;

public class MainSceneTest : MonoBehaviour
{
    public CharacterModel playerModel;
    public CharacterModel enemyModel;

    public CharacterControl playerControl;
    public CharacterControl enemyControl;

    private void Start()
    {
        playerControl.Init(playerModel);
        enemyControl.Init(enemyModel);
        playerControl.Flip();
        playerControl.EnableHightlight(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerControl.SetAnimation(AutoBattle.ANIM_STATE.ATTACK, false);
            enemyControl.SetAnimation(AutoBattle.ANIM_STATE.MOVE, false);
        }
    }
}
