using AxieMixer.Unity;
using Newtonsoft.Json.Linq;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace AutoBattle
{
    public class CharacterControl : MonoBehaviour
    {
        public delegate void OnEventTrigger();
        public OnEventTrigger onMoveFinish;
        public OnEventTrigger onAttackFinish;
        public OnEventTrigger onDeathFinish;

        [Header("Views")]
        [SerializeField]
        private CharacterMain_View charView;

        [Header("Params")]
        [SerializeField]
        SkeletonRenderer skeletonRenderer;
        [SerializeField]
        Animator animator;
        [SerializeField]
        private float moveSpeed = 2.0f;

        string anim_Idle_Tr = "Idle";
        string anim_Attack_Tr = "Attack";
        string anim_Death_Tr = "Death";
        string anim_Speed_F = "Speed";

        public BaseCharacterData currentData { get; private set; }
        public CharacterModel CharacterModel { get; private set; }
        public CHARACTER_SIDE CurrentSide { get; private set; }
        public TileModel CurrentTile { get; private set; }
        public int CharacterId { get; private set; }
        public void Init(CharacterModel model, CHARACTER_SIDE characterSide= CHARACTER_SIDE.ALLY, int id = 0)
        {
            Mixer.Init();

            currentData = new BaseCharacterData(model);
            SpawnCharacter(model);
            CharacterModel = model;

            charView.Init(currentData);

            CurrentSide = characterSide;
            targetList = new Queue<Vector2>();

            CharacterId = id;
        }

       
        
        private void SpawnCharacter(CharacterModel model)
        {
            //if (isFetchingGenes) return;
            //StartCoroutine(GetAxiesGenes(model.axieId));

            var result = Mixer.Builder.BuildSpineFromGene(model.axieId, model.genesString, 0.0012f);
            skeletonRenderer.skeletonDataAsset = result.skeletonDataAsset;
            skeletonRenderer.Initialize(true);
            //Mixer.SpawnSkeletonAnimation(skeletonAnimation, model.axieId, model.genesString);
            //SetAnimation(AnimationState.IDLE);
        }

        //bool isFetchingGenes = false;
        //public IEnumerator GetAxiesGenes(string axieId)
        //{
        //    isFetchingGenes = true;
        //    string searchString = "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}";
        //    JObject jPayload = new JObject();
        //    jPayload.Add(new JProperty("query", searchString));

        //    var wr = new UnityWebRequest("https://graphql-gateway.axieinfinity.com/graphql", "POST");
        //    //var wr = new UnityWebRequest("https://testnet-graphql.skymavis.one/graphql", "POST");
        //    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
        //    wr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        //    wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //    wr.SetRequestHeader("Content-Type", "application/json");
        //    wr.timeout = 10;
        //    yield return wr.SendWebRequest();
        //    if (wr.error == null)
        //    {
        //        var result = wr.downloadHandler != null ? wr.downloadHandler.text : null;
        //        if (!string.IsNullOrEmpty(result))
        //        {
        //            JObject jResult = JObject.Parse(result);
        //            string genesStr = (string)jResult["data"]["axie"]["newGenes"];
        //            Debug.Log(genesStr);
        //            Mixer.SpawnSkeletonAnimation(skeletonAnimation, axieId, genesStr);
        //        }
        //    }
        //    isFetchingGenes = false;
        //}

        public void SetAnimation(ANIM_STATE STATE, bool loop = true)
        {
            switch (STATE)
            {
                case ANIM_STATE.IDLE:
                    if(!isIdle)
                    {
                        animator.SetTrigger(anim_Idle_Tr);
                        animator.SetFloat(anim_Speed_F, 0f);
                        isIdle = true;
                    }
                    break;
                case ANIM_STATE.ATTACK:
                    animator.SetTrigger(anim_Attack_Tr);
                    break;
                case ANIM_STATE.MOVE:
                    //animator.SetTrigger(anim_Idle_Tr);
                    animator.SetFloat(anim_Speed_F, 1f);
                    isIdle = false;
                    break;
                case ANIM_STATE.DEATH:
                    animator.SetTrigger(anim_Death_Tr);
                    break;
                default:
                    animator.SetTrigger(anim_Idle_Tr);
                    animator.SetFloat(anim_Speed_F, 0f);
                    break;
            }

            //skeletonAnimation.state.SetAnimation(0, animName, loop);
        }

        public void EnableHightlight(bool isHighlight)
        {
            charView.SetHightlight(isHighlight);
        }

        public void Flip(Vector2 targetPos)
        {
            float dir = (targetPos.x > transform.position.x) ? -1 : 1;
            skeletonRenderer.skeleton.ScaleX = dir;
        }

        public void TakeDamage(int damage)
        {
            currentData.currentHP = Mathf.Clamp(currentData.currentHP-damage, 0, currentData.HP);

            charView.ModifyHP(currentData);
        }

        Queue<Vector2> targetList;
        public void Move(List<Vector2> path)
        {
            if(path != null && path.Count> 0.01f)
            {
                targetList = new Queue<Vector2>();
                string debug = "Real Path: ";
                
                for (int i = 0; i < path.Count; i++)
                {
                    debug += path[i].ToString() + "->";
                    targetList.Enqueue(path[i]);
                }
                //Debug.Log(debug);
                curDes = targetList.Dequeue();
            }
        }

        public void SetPosition(TileModel tile, bool updatePos = false)
        {
            if (tile == null)
            {
                return;
            }
            //Debug.Log("[Set Position]" + tile.currentPos);
            
            if (CurrentTile != null)
                CurrentTile.SetCharacter(null);
            CurrentTile = tile;
            transform.SetParent(tile.transform, true);

            if(updatePos)
            {
                transform.position = tile.currentPos;
                curDes = tile.currentPos;
            }
        }

        Vector2 curDes;
        bool isIdle = false;
        private void Update()
        {
            if(Vector2.Distance(curDes, transform.position) > 0.05f)
            {
                Vector2 dir = Vector3.Normalize(curDes - (Vector2)transform.position) * moveSpeed;
               
                Flip(curDes);
                
                transform.Translate(dir);
                SetAnimation(ANIM_STATE.MOVE);
                //transform.position = Vector2.Lerp(curDes, transform.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                if(targetList.Count > 0)
                {
                    transform.position = curDes;
                    curDes = targetList.Dequeue();
                }
                else
                {
                    SetAnimation(ANIM_STATE.IDLE);
                }
            }
        }
    }

    public enum ANIM_STATE
    {
        IDLE = 0,
        ATTACK = 1,
        MOVE = 2,
        DEATH = 3,
    }

    public enum CHARACTER_SIDE
    {
        ENEMY = 0,
        ALLY = 1,
    }
}

