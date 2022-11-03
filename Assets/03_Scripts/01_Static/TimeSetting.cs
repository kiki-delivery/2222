using UnityEngine;

/// <summary>
/// ������Ʈ���� �������� ���� ���ð�
/// </summary>
[CreateAssetMenu(fileName = "TimeSetting", menuName = "Scriptable Object Asset/TimeSetting")]
public class TimeSetting : ScriptableObject
{
    [Header("�� ������ �ð�")]
    public float lightOnTime = 0.3f;

    [Header("�� ������ �ð�")]
    public float lightOffTime = 2f;

    [Header("���ͷ��� �� �ٽ� �� ������ ��� �ð�")]
    public float waitActionTime = 2f;

    [Header("�������� �ϴ� �ð�")]
    public float readyTime = 1f;
    
}

