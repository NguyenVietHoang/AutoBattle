using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace AutoBattle
{
    public class MainGameManager : MonoBehaviour
    {
        [Header("Player Model")]
        [SerializeField]
        private CharacterModel playerModel;
        [SerializeField]
        private CharacterModel enemyModel;
        [SerializeField]
        private List<CharacterControl> allyCharacterList;
        [SerializeField]
        private List<CharacterControl> enemyCharacterList;

        [Header("Manager")]
        [SerializeField]
        private BoardControl board;
        [SerializeField]
        private ForceBar_View forceBar;
        [SerializeField]
        private float phaseTime = 1.0f;

        GAME_PHASE CurrentPhase;
        public int AllyForce {  get; private set; }
        public int EnemyForce {  get; private set; }
        // Start is called before the first frame update
        void Start()
        {
            board.InitBoard(14, 14);
            InitPlayer();
            CurrentPhase = GAME_PHASE.START;
            Debug.Log("Start Game Process...");

            deathAllies = new List<CharacterControl>();
            deathEnemies = new List<CharacterControl>();
            StartCoroutine(Move_Phase());
        }

        private void InitPlayer()
        {
            if(allyCharacterList == null 
                || enemyCharacterList == null
                || playerModel == null
                || enemyModel == null)
            {
                Debug.LogError("Init Player ERROR: Invalid Input...");
            }

            AllyForce = allyCharacterList.Count;
            EnemyForce = enemyCharacterList.Count;
            forceBar.SetBar(AllyForce, EnemyForce);

            for (int i = 0; i < allyCharacterList.Count; i++)
            {
                allyCharacterList[i].Init(playerModel, CHARACTER_SIDE.ALLY, i);
                board.SetCharacter(allyCharacterList[i].transform.localPosition, allyCharacterList[i], true);
            }
            for (int i = 0; i < enemyCharacterList.Count; i++)
            {
                enemyCharacterList[i].Init(enemyModel, CHARACTER_SIDE.ENEMY, i);
                board.SetCharacter(enemyCharacterList[i].transform.localPosition, enemyCharacterList[i], true);
            }
        }
        
        IEnumerator Move_Phase()
        {
            Debug.Log("--------MOVE PHASE----------");
            CurrentPhase = GAME_PHASE.MOVE;
            forceBar.SetMsg("MOVE");

            for (int i = 0; i < allyCharacterList.Count; i++)
            {
                if (allyCharacterList[i] != null)
                {
                    int charMove = allyCharacterList[i].CharacterModel.MOVE;
                    List<CharacterControl> enemies = board.GetCharacterAround(allyCharacterList[i], false);
                    if (charMove > 0 && enemies.Count <= 0)
                    {
                        var pathToEnemy = board.DetectClosestEnemy(allyCharacterList[i]);
                        if (pathToEnemy != null && pathToEnemy.path.Count > 0)
                        {
                            //Debug.Log("Calculated Path: " + pathToEnemy.ToString());
                            board.MoveCharacter(pathToEnemy.path, allyCharacterList[i]);
                        }
                    }
                    yield return null;
                }                
            }
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(Attack_Phase());
        }

        IEnumerator Attack_Phase()
        {
            Debug.Log("--------ATTACK PHASE----------");
            CurrentPhase = GAME_PHASE.ATTACK;
            forceBar.SetMsg("ATTACK");

            for (int i = 0; i < allyCharacterList.Count; i++)
            {
                if (allyCharacterList[i] != null)
                {
                    List<CharacterControl> enemies = board.GetCharacterAround(allyCharacterList[i], false);
                    List<CharacterControl> allies = board.GetCharacterAround(allyCharacterList[i], true);
                    int enemiesNb = enemies.Count;
                    int alliesNb = allies.Count;
                    if (enemies.Count > 0)
                    {
                        int damage = GetDamage(alliesNb + 1, enemiesNb) + allyCharacterList[i].currentData.ATK;
                        allyCharacterList[i].SetAnimation(ANIM_STATE.ATTACK);
                        allyCharacterList[i].Flip(enemies[0].transform.position);
                        enemies[0].TakeDamage(damage);
                        Debug.Log(allyCharacterList[i].name + " - " + allyCharacterList[i].CurrentSide.ToString() 
                            + " attack " + enemies[0].name + " - " + enemies[0].CurrentSide.ToString() + ": " + damage + " damage.");

                        if (enemies[0].currentData.currentHP <= 0)
                        {
                            deathEnemies.Add(enemies[0]);
                            Debug.Log("Death: " + enemies[0].name);
                        }
                    }
                    yield return null;
                }                
            }

            for (int i = 0; i < enemyCharacterList.Count; i++)
            {
                if (enemyCharacterList[i] != null)
                {
                    List<CharacterControl> enemies = board.GetCharacterAround(enemyCharacterList[i], false);
                    List<CharacterControl> allies = board.GetCharacterAround(enemyCharacterList[i], true);
                    int enemiesNb = enemies.Count;
                    int alliesNb = allies.Count;
                    if (enemies.Count > 0)
                    {
                        int damage = GetDamage(alliesNb + 1, enemiesNb) + enemyCharacterList[i].currentData.ATK;
                        enemyCharacterList[i].SetAnimation(ANIM_STATE.ATTACK);
                        enemyCharacterList[i].Flip(enemies[0].transform.position);
                        enemies[0].TakeDamage(damage);

                        Debug.Log(enemyCharacterList[i].name + " - " + enemyCharacterList[i].CurrentSide.ToString()
                            + " attack " + enemies[0].name + " - " + enemies[0].CurrentSide.ToString() + ": " + damage + " damage.");
                        if (enemies[0].currentData.currentHP <= 0)
                        {
                            deathAllies.Add(enemies[0]);
                            Debug.Log("Death: " + enemies[0].name);
                        }
                    }
                    yield return null;
                }                
            }

            yield return new WaitForSeconds(0.3f);
            StartCoroutine(Death_Phase());
        }

        int GetDamage(int atteackerNb, int targetNb)
        {
            int calc = (3 + atteackerNb - targetNb) % 3;
            int damage = 0;
            if (calc == 0)
                damage = 4;
            if (calc == 1)
                damage = 5;
            if (calc == 2)
                damage = 3;

            //Debug.Log("[GetDamage] Attacker: " + atteackerNb 
            //    + " | Target: " + targetNb 
            //    + " | Damage: " + damage);

            return damage;
        }

        List<CharacterControl> deathAllies;
        List<CharacterControl> deathEnemies;
        IEnumerator Death_Phase()
        {
            Debug.Log("--------DEATH PHASE----------");
            CurrentPhase = GAME_PHASE.DEATH;
            forceBar.SetMsg("DEATH");

            if (deathAllies != null && deathAllies.Count > 0)
            {
                for(int i = 0; i < deathAllies.Count; i++)
                {
                    CharacterControl death = deathAllies[i];                    
                    if (death != null)
                    {
                        //Debug.Log("Ally Death id: " + death.CharacterId + " | Side: " + death.CurrentSide.ToString());
                        allyCharacterList[death.CharacterId] = null;
                        death.CurrentTile.SetCharacter(null);                        
                        Destroy(death.gameObject);
                        AllyForce--;
                    }
                    yield return null;
                }
            }

            if (deathEnemies != null && deathEnemies.Count > 0)
            {
                for (int i = 0; i < deathEnemies.Count; i++)
                {
                    CharacterControl death = deathEnemies[i];
                    if(death != null)
                    {
                        //Debug.Log("Enemy Death id: " + death.CharacterId + " | Side: " + death.CurrentSide.ToString());
                        enemyCharacterList[death.CharacterId] = null;
                        death.CurrentTile.SetCharacter(null);                        
                        Destroy(death.gameObject);
                        EnemyForce--;                        
                    }
                    yield return null;
                }
            }

            //Debug.Log("[Death Phase] Ally: " + AllyForce + " | Enemy: " + EnemyForce);
            forceBar.SetBar(AllyForce, EnemyForce);

            if (AllyForce <= 0)
            {
                Debug.Log("You Lose");
                forceBar.SetMsg("You Lose");
                CurrentPhase = GAME_PHASE.END;
            }
            else if(EnemyForce <= 0)
            {
                Debug.Log("You Win");
                forceBar.SetMsg("You Win");
                CurrentPhase = GAME_PHASE.END;
            }
            else
            {
                deathAllies = new List<CharacterControl>();
                deathEnemies = new List<CharacterControl>();
                yield return null;
                CurrentPhase = GAME_PHASE.START;
                StartCoroutine(Move_Phase());
            }
            
        }

        float t = 0;
        // Update is called once per frame
        void Update()
        {
            if (t >= phaseTime && CurrentPhase == GAME_PHASE.START)
            {
                StartCoroutine(Move_Phase());
                t = 0;
            }
            else
            {
                t += Time.deltaTime;
            }
        }

        public void ToLoadingScene()
        {
            SceneManager.LoadScene("Loading");
        }
    }

    public enum GAME_PHASE
    {
        START= 0,
        MOVE = 1,
        ATTACK = 2,
        DEATH = 3,
        END = 4,
    }
}

