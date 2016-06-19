using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UICharacterPanel : MonoBehaviour {
    public Text txtName;
    public Image imgCharacter;
    public Image imgButton;
    public Button btnSelect;

    public int CharacterId { get; set; }
    public string SelectButtonStr { get; set; }
    public KeyCode SelectKeyCode { get; set; }

    public void Awake() {
        if (btnSelect != null) {
            btnSelect.onClick.AddListener(OnSelectButtonClick);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(SelectKeyCode)) {// || Input.GetButtonDown(SelectButtonStr)) {
            OnSelectButtonClick();
        }
    }

    private void OnSelectButtonClick() {
        if (Controller != null) {
            Controller.TriggerCharacterSelect(CharacterId);
        }
    }

    public string CharacterName {
        get { return txtName.text; }
        set { txtName.text = value; }
    }

    public Sprite CharacterIcon {
        get { return imgCharacter.sprite; }
        set {
            imgCharacter.sprite = value;
            imgCharacter.enabled = imgCharacter.sprite != null;
        }
    }

    public Sprite ButtonIcon {
        get { return imgButton.sprite; }
        set {
            imgButton.sprite = value;
            imgButton.enabled = imgButton.sprite != null;
        }
    }

    public UICharacterSelector Controller { get; set; }
}
