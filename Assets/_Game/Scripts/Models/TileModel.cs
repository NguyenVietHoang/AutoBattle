using AutoBattle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AutoBattle
{
    public class TileModel : MonoBehaviour
    {
        public delegate void OnEventTrigger<T>(T data);
        public event OnEventTrigger<CharacterControl> onClicked;

        [SerializeField]
        private EventTrigger eventTrigger;
        public CharacterControl currentCharacter { get; private set; }
        public Vector2 currentPos { get; private set; }

        public void Init(Vector2 pos)
        {
            currentPos = pos;
            currentCharacter = null;

            eventTrigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener(OnTileClicked);
            eventTrigger.triggers.Add(entry);

        }

        public void SetCharacter(CharacterControl character)
        {
            currentCharacter = character;
        }

        private void OnTileClicked(BaseEventData data)
        {
            //Debug.Log("Tile Clicked...");
            onClicked?.Invoke(currentCharacter);
        }
    }
}

