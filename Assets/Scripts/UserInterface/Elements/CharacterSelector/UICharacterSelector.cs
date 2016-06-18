using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class UICharacterSelector : Singleton<UICharacterSelector> {
    public GameObject characterPanelPrefab;
    private List<UICharacterPanel> characterPanels;

    public delegate void CharacterSelectEvent(int characterId);
    private CharacterSelectEvent characterSelectEvent;

    [System.Serializable]
    public class SlotInfo {
        public Sprite buttonIcon;
        public string buttonStr;
        public KeyCode keyCode;
    }

    public string buttonPrefix;
    public SlotInfo[] slotInfos;

    private void Awake() {
        if(characterPanels == null) characterPanels = new List<UICharacterPanel>();
        else characterPanels.Clear();

        for (int i = 0; i < CharacterDatabase.Instance.Count && i < slotInfos.Length; i++) {
            var go = GameObject.Instantiate(characterPanelPrefab);
            go.transform.SetParent(this.transform, false);

            var panel = go.GetComponent<UICharacterPanel>();
            if (panel != null) {
                characterPanels.Add(panel);
                var asset = CharacterDatabase.Instance.Get(i);
                panel.CharacterId = asset.Id;
                panel.CharacterName = asset.Name;
                panel.CharacterIcon = asset.Icon;
                panel.ButtonIcon = slotInfos[i].buttonIcon;
                panel.SelectButtonStr = buttonPrefix + slotInfos[i].buttonStr;
                panel.SelectKeyCode = slotInfos[i].keyCode;
                panel.Controller = this;
            } else {
                Debug.LogFormat("[{0}]: Character Panel Prefab doesn't have component UICharacterPanel",
                    this.name);
            }
        }
        this.gameObject.SetActive(false);


        Display((id) => {
            Debug.LogFormat("[{0}]: Character Select Trigger with id of {1}", this.name, id);
        });
    }

    public void Display(CharacterSelectEvent selectEvent) {
        gameObject.SetActive(true);
        characterSelectEvent = selectEvent;
    }

    public void TriggerCharacterSelect(int id) {
        if (characterSelectEvent != null) {
            characterSelectEvent.Invoke(id);
        }
        gameObject.SetActive(false);
    }
}
