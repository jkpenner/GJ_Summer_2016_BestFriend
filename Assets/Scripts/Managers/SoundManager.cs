using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {

	static AudioSource audioSource;

    private void Awake() {
		audioSource = gameObject.GetComponentInChildren<AudioSource>();

        if (Instance != this) {
            Destroy(this.gameObject);
        } else {
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
    }

	static public void PlaySoundEffect(AudioClip audioClip){
		audioSource.PlayOneShot(audioClip);
	}
}
