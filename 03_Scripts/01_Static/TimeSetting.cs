using UnityEngine;

/// <summary>
/// 오브젝트들이 공통으로 쓰는 세팅값
/// </summary>
[CreateAssetMenu(fileName = "TimeSetting", menuName = "Scriptable Object Asset/TimeSetting")]
public class TimeSetting : ScriptableObject
{
    [Header("빛 켜지는 시간")]
    public float lightOnTime = 0.3f;

    [Header("빛 꺼지는 시간")]
    public float lightOffTime = 2f;

    [Header("인터렉션 후 다시 할 때까지 대기 시간")]
    public float waitActionTime = 2f;

    [Header("깜빡깜빡 하는 시간")]
    public float readyTime = 1f;
    
}

