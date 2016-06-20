using UnityEngine;
using System.Collections;

public class DontDestroyObjectOnLoad : MonoBehaviour {
    private void Awake() {
        transform.SetParent(null);
        DontDestroyOnLoad(this.gameObject);
    }
}
