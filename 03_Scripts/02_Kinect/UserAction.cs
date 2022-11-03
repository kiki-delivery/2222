using UnityEngine;

/// <summary>
/// �ٵ�Ʈ��ŷ ���� �� ȭ�鿡 ���� �߻� �뵵
/// </summary>
public class UserAction : MonoBehaviour
{

    [SerializeField]
    Camera mainCamera;  


    [SerializeField]
    int rayDistance;    // �� �������� ��ü�� ����ȭ�� �� �� ������ �ϴ� ������

    RaycastHit hitInfo; // ���� �´� �� ����
    Ray ray;


    /// <summary>
    /// �ٵ� ���̵� �� ������ �Ҵ�Ǹ� �ٽ� ó������ �����Ұ���
    /// </summary>
    [SerializeField]
    int bodyIdInitCount = 3;

    int InterActionLayerMask;

    private void Awake()
    {
        InterActionLayerMask = 1 << LayerMask.NameToLayer("InterAction");
        //followUserLayerMask = 1 << LayerMask.NameToLayer("FollowUser");
    }


    /// <summary>
    /// Ʈ��ŷ ��ġ���� ���̽��
    /// </summary>
    /// <param name="trackingPos">Ʈ��ŷ�� ��ġ(���� ������)</param>
    /// 

    public void Raycast(Vector3 trackingPos)
    {
        Vector3 trackingScreenPoint = TransformationLibrary.GetScreenPoint(trackingPos);
        ray = mainCamera.ScreenPointToRay(trackingScreenPoint);

        if (Physics.Raycast(ray, out hitInfo, rayDistance, InterActionLayerMask))
        {
            hitInfo.transform.GetComponent<InterAction>().Action();
        }

    }


    [SerializeField]
    GameObject[] sprites;


    /// <summary>
    /// ��������Ʈ�� ��� ����ٴϵ���
    /// </summary>
    /// <param name="num"></param>
    public void FollowUserSprite(uint bodyId, Vector3 pos)
    {
        sprites[CalcBodyId(bodyId)].transform.localPosition = pos;

    }

    /// <summary>
    /// ���� �ϳ� ���������� �ϴ� �ٲ���.. ��� �ٲٰ����..
    /// </summary>
    public void SpriteAllOff()
    {
        for(int i =0; i<sprites.Length; i++)
        {
            sprites[i].SetActive(false);
        }
    }

    /// <summary>
    /// ��Ī�� ����� ���̵� �´� ��������Ʈ ������Ʈ �غ�
    /// </summary>
    /// <param name="bodyId"></param>
    public void SpriteReady(uint bodyId)
    {
        sprites[CalcBodyId(bodyId)].SetActive(true);
    }

    /// <summary>
    /// 1~100 ������ �ٵ���̵� ����ϵ��� ����(�غ�� ��������Ʈ �迭�� 100�̶�)
    /// </summary>
    /// <param name="bodyId"></param>
    /// <returns></returns>
    int CalcBodyId(uint bodyId)
    {
        //Debug.Log("�ٵ���̵�    " + (int)bodyId);
        //Debug.Log("���� ���̵�    " + (int)bodyId % bodyIdInitCount);
        return (int)bodyId % bodyIdInitCount;
    }


    //��ƼŬ ����ٴϴ� �뵵�� �����غ����� �� �ȵ�
    /*
    [SerializeField]
    GameObject[] particles;

    public void followUserParticle(uint bodyId, Vector3 trackingPos)
    {
        Vector3 trackingScreenPoint = TransformationLibrary.GetScreenPoint(trackingPos);
        ray = mainCamera.ScreenPointToRay(trackingScreenPoint);

        if (Physics.Raycast(ray, out hitInfo, rayDistance, followUserLayerMask))
        {
            particles[CalcBodyId(bodyId)].transform.position = hitInfo.point;
        }
    }

    

    /// <summary>
    /// ��ƼŬ �غ�
    /// </summary>
    /// <param name="num"></param>
    public void ParticleReady(uint num)
    {
        for(uint i =0; i<num; i++)
        {
            particles[i].SetActive(true);
        }
        for(uint i = num; i<particles.Length; i++)
        {
            particles[i].SetActive(false);
        }
    }
    */
}
