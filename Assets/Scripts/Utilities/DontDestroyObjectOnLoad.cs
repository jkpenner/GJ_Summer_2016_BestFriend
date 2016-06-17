using UnityEngine;
using System.Collections;

public class DontDestroyObjectOnLoad : MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }
}
