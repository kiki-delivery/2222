using UnityEngine;

/// <summary>
/// �� ��¦�̴� �ִϸ��̼�
/// �������� ������ �ִ� ���׸��� �����ϴ� ����� ������ �������� 
/// ����ȭ���� ���ذ� �ȴٰ� �Ǵ�, ���׸��� ��ü�� ������Ƽ�� �ٲٴ� ������� ����
/// </summary>
public class GemLightAnimation : MonoBehaviour
{
    [Header("�ִϸ��̼� �� ���׸���")]
    [SerializeField]
    Material myGemLight;

    [Header("���� �� ���")]
    [SerializeField][Range(1, 100)]
    float startIntencity;    

    [Header("��� �ٲ�� ����(��ο���(-1)-�Ⱥ���(0)-�����(1))")]
    [SerializeField] [Range(-1, 1)]
    int change;

    [Header("��� �ٲ�� �ӵ�")]
    [SerializeField][Range(40, 60)] 
    int changeSpeed;

    [Header("�ִ� ���")]
    [SerializeField]
    int maxValue = 20;

    /// <summary>
    /// ���׸��� ������ intencity ��
    /// </summary>
    float intencity;

    private void Awake()
    {
        if(change == 0)      // ��¦��¦ ���Ұ��
        {
            changeSpeed = 0;
        }

        intencity = startIntencity; // ó�� ��� ����ֱ�

        myGemLight.SetFloat("_Intensity", startIntencity);      // ������ ���� ����
    }

    private void Update()
    {
        // �� ������� ���� ��ο�����
        // ��¦ ���ϴ� �ֵ��� �Ʒ� if�� ��𿡵� �ش�ȵ�
        if(intencity >= maxValue)
        {
            change = -1;
            intencity = maxValue;
        }
        else if(intencity<=10)   // �� ��ο����� ���� �������
        {
            change = 1;
            intencity = 11;
        }

        if (changeSpeed != 0)   // ��¦ ���ص� �Ǵ� ���..
        {
            intencity = intencity + Time.deltaTime * change * changeSpeed;
        }

        // ���׸����� ������Ƽ�� ����
        myGemLight.SetFloat("_Intensity", intencity);
    }
}
