using UnityEngine;

/// <summary>
/// ����, �¿�, �밢�� �պ� ������ �����(���ĸ� ���)
/// </summary>
public class Motion : MonoBehaviour
{
    /// <summary>
    /// �����̴� ��� 
    /// </summary>
    public enum EMoveWay
    {
        LeftRight,
        UpDown,
        DeaGak
    }

    [Header("������ ��� ����")]
    public EMoveWay moveWay;

    [Header("������ ���� ����(-1, 1)")]
    public int moveVector;

    [Header("�����̴� �ӵ�")]
    public float speed = 0.5f;

    [Header("�����̴� ����")]
    public float moveRange = 1f;

    // �պ�� ����
    Vector3 vector;

    // ��ǥ����
    Vector3 originalVector, targetVector;

    // ó�� ����
    int dir = 1;

    // Ÿ���� ���� ���� �ֳ� üũ
    bool goTarget = true;

    private void Awake()
    {
        originalVector = transform.position;    // ó�� ��ġ�� �����ϰ� ����
    }

    private void OnEnable()
    {
        // �ʱ�ȭ ����
        goTarget = true;
        transform.position = originalVector;
        dir = 1;
        dir = dir * moveVector; // ó���� ������ ���� ����

        // �ʱ�ȭ ��

        // ������ ������ ��ο� ���� ���� ����
        if (moveWay == EMoveWay.LeftRight)
        {
            vector = Vector3.right;
        }
        else if (moveWay == EMoveWay.UpDown)
        {
            vector = Vector3.up;
        }
        else
        {
            vector = new Vector3(1, 1, 0);
        }

        
        targetVector = originalVector + vector * moveRange * moveVector;
    }

    private void Update()
    {
        if (moveWay == EMoveWay.LeftRight)
        {
            vector = Vector3.right;
        }
        else if (moveWay == EMoveWay.UpDown)
        {
            vector = Vector3.up;
        }
        else
        {
            vector = new Vector3(1, 1, 0);
        }

        // Ÿ���� ���� �����ְ�, Ÿ�ٰ� ��������� ���� �ٲٱ�
        if (goTarget)
        {
            if (Vector3.Distance(transform.position, targetVector) < 0.1f)
            {
                dir = -dir;
                goTarget = false;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, originalVector) < 0.1f)
            {
                dir = -dir;
                goTarget = true;
            }
        }


        transform.position = transform.position + vector * dir * Time.deltaTime * speed;

    }

}
