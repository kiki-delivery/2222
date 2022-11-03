using System.Collections;
using UnityEngine;


/// <summary>
/// 키넥트 안될경우의 보험(자동 액션 모드)
/// </summary>
public class AutoMode : MonoBehaviour
{

    [SerializeField]
    Camera mainCamera;  // 

    [SerializeField]
    int rayDistance;    // 이 길이조절 자체가 최적화가 될 수 있으니 일단 놔두자

    RaycastHit hitInfo; // 레이 맞는 애 저장
    Ray ray;


    int InterActionLayerMask;


    /// <summary>
    /// 인터렉션 반응하는 스크린 포인트 저장
    /// </summary>
    [SerializeField]
    Vector3[] interationPoints;

    /// <summary>
    /// 랜덤 액션 발동에 걸리는 시간
    /// </summary>
    WaitForSeconds waitTime;
    private void Awake()
    {
        InterActionLayerMask = 1 << LayerMask.NameToLayer("InterAction");
        StartCoroutine(RandomAction());
    }

    /// <summary>
    /// 랜덤으로 액션 발동
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
    /// 트래킹 위치에서 레이쏘기
    /// </summary>
    /// <param name="trackingPos">트래킹된 위치(월드 포지션)</param>
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
