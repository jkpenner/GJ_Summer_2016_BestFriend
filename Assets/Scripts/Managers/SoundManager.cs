using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {

	static AudioSource audioSource;

    private void Awake() {

        if (Instance != this) {
            Destroy(this.gameObject);
        } else {
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
    }

	private void Start(){
		audioSource = gameObject.GetComponentInChildren<AudioSource>();
	}

	static public void PlaySoundEffect(AudioClip audioClip){
		audioSource.PlayOneShot(audioClip);
	}
}
