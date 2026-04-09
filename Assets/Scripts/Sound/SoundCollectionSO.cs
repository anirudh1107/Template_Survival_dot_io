using UnityEngine;

[CreateAssetMenu(fileName = "SoundCollectionSO", menuName = "Scriptable Objects/SoundCollectionSO")]
public class SoundCollectionSO : ScriptableObject
{
    public SoundSo[] GunShots;
    public SoundSo[] PlayerHurt;
    public SoundSo[] EnemyHurt;
    public SoundSo[] PlayerDeath;
}
