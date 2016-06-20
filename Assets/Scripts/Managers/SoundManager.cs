using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {
    private void Awake() {
        if (Instance != this) {
            Destroy(this.gameObject);
        } else {
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
