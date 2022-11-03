using System.Collections;
using UnityEngine;


/// <summary>
/// Ű��Ʈ �ȵɰ���� ����(�ڵ� �׼� ���)
/// </summary>
public class AutoMode : MonoBehaviour
{

    [SerializeField]
    Camera mainCamera;  // 

    [SerializeField]
    int rayDistance;    // �� �������� ��ü�� ����ȭ�� �� �� ������ �ϴ� ������

    RaycastHit hitInfo; // ���� �´� �� ����
    Ray ray;


    int InterActionLayerMask;


    /// <summary>
    /// ���ͷ��� �����ϴ� ��ũ�� ����Ʈ ����
    /// </summary>
    [SerializeField]
    Vector3[] interationPoints;

    /// <summary>
    /// ���� �׼� �ߵ��� �ɸ��� �ð�
    /// </summary>
    WaitForSeconds waitTime;
    private void Awake()
    {
        InterActionLayerMask = 1 << LayerMask.NameToLayer("InterAction");
        StartCoroutine(RandomAction());
    }

    /// <summary>
    /// �������� �׼� �ߵ�
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomAction()
    {
        waitTime = new WaitForSeconds(Random.Range(0.2f, 3));

        while (true)
        {
            Raycast(interationPoints[Random.Range(0, 6)]);



            yield return waitTime;
        }
    }





    /// <summary>
    /// Ʈ��ŷ ��ġ���� ���̽��
    /// </summary>
    /// <param name="trackingPos">Ʈ��ŷ�� ��ġ(���� ������)</param>
    /// 

    public void Raycast(Vector3 ScreenPoint)
    {
        ray = mainCamera.ScreenPointToRay(ScreenPoint);

        if (Physics.Raycast(ray, out hitInfo, rayDistance, InterActionLayerMask))
        {
            hitInfo.transform.GetComponent<InterAction>().Action();
        }

    }
}
