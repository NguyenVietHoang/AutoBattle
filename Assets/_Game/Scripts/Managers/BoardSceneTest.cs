using AutoBattle;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSceneTest : MonoBehaviour
{
    public BoardControl board;
    public CharacterControl playerPrefab;
    public CharacterModel playerModel;
    public CharacterModel enemyModel;

    List<CharacterControl> allyList;
    List<CharacterControl> enemyList;
    // Start is called before the first frame update
    void Start()
    {
        board.InitBoard(14, 14);

        //allyList = new List<CharacterControl>();
        //CharacterControl ally1 = Instantiate(playerPrefab);
        //ally1.Init(playerModel, CHARACTER_SIDE.ALLY);
        //allyList.Add(ally1);
        //board.SetCharacter(new Vector2(4,0), ally1, true );

        //CharacterControl ally2 = Instantiate(playerPrefab);
        //ally2.Init(playerModel, CHARACTER_SIDE.ALLY);
        //allyList.Add(ally2);
        //board.SetCharacter(new Vector2(4, 1), ally2, true);

        //CharacterControl ally3 = Instantiate(playerPrefab);
        //ally3.Init(playerModel, CHARACTER_SIDE.ALLY);
        //allyList.Add(ally3);
        //board.SetCharacter(new Vector2(5, 0), ally3, true);

        //enemyList = new List<CharacterControl>();
        //CharacterControl enemy1 = Instantiate(playerPrefab);
        //enemy1.Init(enemyModel, CHARACTER_SIDE.ENEMY);
        //enemyList.Add(enemy1);
        //board.SetCharacter(new Vector2(3, 0), enemy1, true);

        //CharacterControl enemy2 = Instantiate(playerPrefab);
        //enemy2.Init(enemyModel, CHARACTER_SIDE.ENEMY);
        //enemyList.Add(enemy2);
        //board.SetCharacter(new Vector2(2, 0), enemy2, true);

        //CharacterControl enemy3 = Instantiate(playerPrefab);
        //enemy3.Init(enemyModel, CHARACTER_SIDE.ENEMY);
        //enemyList.Add(enemy3);
        //board.SetCharacter(new Vector2(5, 3), enemy3, true);

        //var path = board.DetectClosestEnemy(ally1);
        //Debug.Log(path.ToString());
        //path = board.DetectClosestEnemy(ally2);
        //Debug.Log(path.ToString());
        //path = board.DetectClosestEnemy(ally3);
        //Debug.Log(path.ToString());
    }
}
