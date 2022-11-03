using System.Collections;
using UnityEngine;

/// <summary>
/// 오브젝트 조명 컨트롤러, 조명마다 가지고 있음
/// </summary>
public class LightController : MonoBehaviour
{
    [SerializeField]
    Light light;

    [SerializeField]
    TimeSetting TimeSetting;

    [Header("시작 밝기, 최종 밝기")]
    [SerializeField]
    float startIntencity, endIntencity;

    [Header("시작 범위, 끝 범위")]
    [SerializeField]
    float startRange, endRange;

    /// <summary>
    /// 불 켜지는 시간, 꺼지는 시간 담아둘 변수
    /// </summary>
    float lghtOnTime, lghtOffTime;


    float lightOnICSpeed, lightOffICSpeed;  // Intencity Change
    float lightOnRCSpeed, lightOffRCSpeed;  // Range Change

    /// <summary>
    /// 불 꺼지는 연출 다 한 후 오브젝트 끄는 용도
    /// </summary>
    WaitForSeconds disableWaitTime;

    private void Awake()
    {
        light.intensity = startIntencity;
        light.range = startRange;
        lghtOnTime = TimeSetting.lightOnTime;
        lghtOffTime = TimeSetting.lightOffTime;
        disableWaitTime = new WaitForSeconds(lghtOffTime);

        LightSpeedCalc();
    }

    /// <summary>
    /// 오브젝트 켜지면서 불 켜지는 연출
    /// </summary>
    private void OnEnable()
    {        
        StartCoroutine(ILightIntencityControl(endIntencity, lightOnICSpeed));
        StartCoroutine(ILightRangeControl(endRange, lightOnRCSpeed));
    }

    private void OnDisable()
    {
        Init();
    }

    /// <summary>
    /// 바뀐 거 초기화
    /// </summary>
    void Init()
    {
        light.intensity = startIntencity;
        light.range = startRange;
    }

    /// <summary>
    /// 프레임 기준으로 지정된 시간동안 온/오프 연출을 하기 위한 계산
    /// 나중에 생각해보니까 이미 Time.deltaTime으로 잘 만들어져 있음...
    /// </summary>
    void LightSpeedCalc()
    {        
        int currntFps = Application.targetFrameRate;    

        float targetIValue = endIntencity - startIntencity;
        float targetRValue = endRange - startRange;

        lightOnICSpeed = targetIValue / (currntFps * lghtOnTime);
        lightOffICSpeed = targetIValue / (currntFps * lghtOffTime);

        lightOnRCSpeed = targetRValue / (currntFps * lghtOnTime);
        lightOffRCSpeed = targetRValue / (currntFps * lghtOffTime);

    }

    /// <summary>
    /// 밝기 변하는 연출
    /// </summary>
    /// <param name="targetIntencity">목표 밝기</param>
    /// <param name="lightICSpeed">바뀌는 속도</param>
    /// <returns></returns>
    IEnumerator ILightIntencityControl(float targetIntencity, float lightICSpeed)
    {
        while (light.intensity != targetIntencity)
        {
            light.intensity = Mathf.MoveTowards(light.intensity, targetIntencity, lightICSpeed);    // 1 프레임당..
            yield return null;
        }
    }

    /// <summary>
    /// Range 변하는 연출
    /// </summary>
    /// <param name="targetRange"></param>
    /// <param name="lightRCSpeed"></param>
    /// <returns></returns>
    IEnumerator ILightRangeControl(float targetRange,  float lightRCSpeed)
    {

        while (light.range != targetRange)
        {
            light.range = Mathf.MoveTowards(light.range, targetRange, lightRCSpeed);    // 1 프레임당..
            yield return null;
        }
    }

    /// <summary>
    /// 오브젝트마다 애니메이션 시간이 달라서 애니메이션 끝난 후 호출할 수 있도록..
    /// </summary>
    public void LightOff()
    {
        StartCoroutine(ILightOff());
    }

    IEnumerator ILightOff()
    {
        StartCoroutine(ILightIntencityControl(startIntencity, lightOffICSpeed));
        StartCoroutine(ILightRangeControl(startRange, lightOffRCSpeed));
        yield return disableWaitTime;
        // 꺼지는 연출 다 했으면 오브젝트 끄기
        gameObject.SetActive(false);
    }

}
