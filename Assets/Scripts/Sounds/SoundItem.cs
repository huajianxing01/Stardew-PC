using UnityEngine;

[System.Serializable]
public class SoundItem
{
    public SoundName soundName;
    public AudioClip soundClip;
    public string soundDescription;
    [Range(0.1f, 1.5f)] public float soundPitchRandomVariationMin = 0.8f;//最小音高变化
    [Range(0.1f, 1.5f)] public float soundPitchRandomVariationMax = 1.2f;//最大音高变化
    [Range(0f, 1f)] public float soundVolume = 1f;//音量
}
