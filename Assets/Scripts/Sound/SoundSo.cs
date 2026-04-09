using UnityEngine;

[CreateAssetMenu(fileName = "SoundSo", menuName = "Scriptable Objects/SoundSo")]
public class SoundSo : ScriptableObject
{
    public enum SoundType
    {
        SFX,
        Music
    }

    public SoundType soundType;
    public AudioClip audioClip;
    public bool loop;
    [Range(0f, 2f)]
    public float volume = 1.0f;
    [Range(0.1f, 3f)]   
    public float pitch = 1.0f;
    public bool randomizePitch;
    [Range(0.1f, 1f)]
    public float pitchVariationRangeModifier = 0.1f;
}
