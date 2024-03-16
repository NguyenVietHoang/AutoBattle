using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattle
{
    [CreateAssetMenu(fileName = "Data", menuName = "AutoBattle/SpawnCharacterModel", order = 1)]
    public class CharacterModel : ScriptableObject
    {
        public string axieId;
        public string genesString = "512";

        public int HP;
        public int ATK;
        public int MOVE;
        public int RANGE;
    }

    public class BaseCharacterData
    {
        public int HP;
        public int currentHP;
        public int ATK;

        public BaseCharacterData(CharacterModel rawData)
        {
            HP = rawData.HP;
            currentHP = HP;
            ATK = rawData.ATK;
        }
    }
}

