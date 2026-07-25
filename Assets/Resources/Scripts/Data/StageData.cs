using UnityEngine;

public enum GimmickType { None, StrongWind, LightOff, SmallSafeZone, MovingSafeZone, }

[System.Serializable]
public class Gimmick
{
    public float timerDuration;
    public GimmickType gimmickType;
    public float gimmickValue;
}

[CreateAssetMenu(menuName = "ScripableObject/StageData")]
public class StageData : ScriptableObject
{
    public Gimmick[] gimmicks;

    public Gimmick GetRandomGimmick()
    {
        return gimmicks[Random.Range(0, gimmicks.Length)];
    }
}
