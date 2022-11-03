using UnityEngine;

/// <summary>
/// 바디트래킹 했을 때 화면에 레이 발사 용도
/// </summary>
public class UserAction : MonoBehaviour
{

    [SerializeField]
    Camera mainCamera;  


    [SerializeField]
    int rayDistance;    // 이 길이조절 자체가 최적화가 될 수 있으니 일단 놔두자

    RaycastHit hitInfo; // 레이 맞는 애 저장
    Ray ray;


    /// <summary>
    /// 바디 아이디가 몇 번까지 할당되면 다시 처음부터 시작할건지
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
    /// 트래킹 위치에서 레이쏘기
    /// </summary>
    /// <param name="trackingPos">트래킹된 위치(월드 포지션)</param>
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
    /// 스프라이트가 사람 따라다니도록
    /// </summary>
    /// <param name="num"></param>
    public void FollowUserSprite(uint bodyId, Vector3 pos)
    {
        sprites[CalcBodyId(bodyId)].transform.localPosition = pos;

    }

    /// <summary>
    /// 누구 하나 빠져나가면 일단 다끄기.. 어떻게 바꾸고싶음..
    /// </summary>
    public void SpriteAllOff()
    {
        for(int i =0; i<sprites.Length; i++)
        {
            sprites[i].SetActive(false);
        }
    }

    /// <summary>
    /// 매칭된 사람의 아이디에 맞는 스프라이트 오브젝트 준비
    /// </summary>
    /// <param name="bodyId"></param>
    public void SpriteReady(uint bodyId)
    {
        sprites[CalcBodyId(bodyId)].SetActive(true);
    }

    /// <summary>
    /// 1~100 사이의 바디아이디 사용하도록 수정(준비된 스프라이트 배열이 100이라)
    /// </summary>
    /// <param name="bodyId"></param>
    /// <returns></returns>
    int CalcBodyId(uint bodyId)
    {
        //Debug.Log("바디아이디    " + (int)bodyId);
        //Debug.Log("계산된 아이디    " + (int)bodyId % bodyIdInitCount);
        return (int)bodyId % bodyIdInitCount;
    }


    //파티클 따라다니는 용도로 구현해봤지만 잘 안됨
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
    /// 파티클 준비
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
