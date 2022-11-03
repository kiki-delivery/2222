using UnityEngine;

/// <summary>
/// 자동차 인터렉션에 있는 우주선 포지션 리셋용도
/// 애니메이션 시작 할 때마다 위치가 바뀌는 원인을 파악 못해서 일단 스크립트로 조치
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
