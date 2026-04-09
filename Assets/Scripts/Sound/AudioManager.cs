using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private SoundCollectionSO soundCollectionSO; // Assign in Inspector

    private void OnEnable() {
        if (playerHealth != null) {
            playerHealth.OnPlayerHurt += HandlePlayerHurt;
            playerHealth.OnPlayerDeath += HandlePlayerDeath;
        }
        // EnemyHealth.OnEnemyHurt += HandleEnemyHurt;
        WeaponManager.OnGunFire += HandleGunShot;
        
    }

    private void OnDisable() {
        if (playerHealth != null) {
            playerHealth.OnPlayerHurt -= HandlePlayerHurt;
            playerHealth.OnPlayerDeath -= HandlePlayerDeath;
        }
        // EnemyHealth.OnEnemyHurt -= HandleEnemyHurt;
        WeaponManager.OnGunFire -= HandleGunShot;
        
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void HandlePlayerHurt()
    {
        PlayRandomSound(soundCollectionSO.PlayerHurt);
    }

    private void HandleEnemyHurt()
    {
        PlayRandomSound(soundCollectionSO.EnemyHurt);
    }

    private void HandlePlayerDeath()
    {
        PlayRandomSound(soundCollectionSO.PlayerDeath);
    }

    private void HandleGunShot()
    {
        PlayRandomSound(soundCollectionSO.GunShots);
    }


    private void PlayRandomSound(SoundSo[] soundSoArray)
    {
        if (soundSoArray == null || soundSoArray.Length == 0) return;

        int randomIndex = Random.Range(0, soundSoArray.Length);
        SoundSo selectedSound = soundSoArray[randomIndex];
        SoundToPlay(selectedSound);
    }

    private void SoundToPlay(SoundSo soundSo)
    {
        AudioClip clip = soundSo.audioClip;
        float volume = soundSo.volume;
        float pitch = soundSo.randomizePitch ?
         soundSo.pitch + Random.Range(-soundSo.pitchVariationRangeModifier,
          soundSo.pitchVariationRangeModifier) : soundSo.pitch;
        bool loop = soundSo.loop;
         
        PaySound(clip, volume, pitch, loop);
    }

    private void PaySound(AudioClip clip, float volume, float pitch, bool loop)
    {
        if (clip == null) return;

        GameObject soundObject = new GameObject("Sound_" + clip.name);
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

        // Destroy the AudioSource component after the clip finishes playing
        if(!loop)
            Destroy(audioSource, clip.length / audioSource.pitch);
    }
   
}
