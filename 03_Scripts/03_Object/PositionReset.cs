using UnityEngine;

/// <summary>
/// �ڵ��� ���ͷ��ǿ� �ִ� ���ּ� ������ ���¿뵵
/// �ִϸ��̼� ���� �� ������ ��ġ�� �ٲ�� ������ �ľ� ���ؼ� �ϴ� ��ũ��Ʈ�� ��ġ
/// </summary>
public class PositionReset : MonoBehaviour
{
    [SerializeField]
    Transform me;


    Vector3 myPosition;

    private void Awake()
    {
        myPosition = transform.position;    
    }

    private void OnEnable()
    {
        transform.position = myPosition;
    }

}
