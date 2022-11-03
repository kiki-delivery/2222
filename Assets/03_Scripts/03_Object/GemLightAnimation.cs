using UnityEngine;

/// <summary>
/// 젬 반짝이는 애니메이션
/// 렌더러가 가지고 있는 메테리얼에 접근하는 방식이 연출은 예쁘지만 
/// 최적화에는 방해가 된다고 판단, 메테리얼 자체의 프로퍼티를 바꾸는 방식으로 변경
/// </summary>
public class GemLightAnimation : MonoBehaviour
{
    [Header("애니메이션 할 메테리얼")]
    [SerializeField]
    Material myGemLight;

    [Header("시작 시 밝기")]
    [SerializeField][Range(1, 100)]
    float startIntencity;    

    [Header("밝기 바뀌는 방향(어두워짐(-1)-안변함(0)-밝아짐(1))")]
    [SerializeField] [Range(-1, 1)]
    int change;

    [Header("밝기 바뀌는 속도")]
    [SerializeField][Range(40, 60)] 
    int changeSpeed;

    [Header("최대 밝기")]
    [SerializeField]
    int maxValue = 20;

    /// <summary>
    /// 메테리얼에 전해줄 intencity 값
    /// </summary>
    float intencity;

    private void Awake()
    {
        if(change == 0)      // 반짝반짝 안할경우
        {
            changeSpeed = 0;
        }

        intencity = startIntencity; // 처음 밝기 담아주기

        myGemLight.SetFloat("_Intensity", startIntencity);      // 설정한 밝기로 시작
    }

    private void Update()
    {
        // 다 밝아지고 나면 어두워지기
        // 반짝 안하는 애들은 아래 if문 어디에도 해당안됨
        if(intencity >= maxValue)
        {
            change = -1;
            intencity = maxValue;
        }
        else if(intencity<=10)   // 다 어두워지고 나면 밝아지기
        {
            change = 1;
            intencity = 11;
        }

        if (changeSpeed != 0)   // 반짝 안해도 되는 경우..
        {
            intencity = intencity + Time.deltaTime * change * changeSpeed;
        }

        // 메테리얼의 프로퍼티값 변경
        myGemLight.SetFloat("_Intensity", intencity);
    }
}
