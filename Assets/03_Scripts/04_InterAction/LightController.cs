using System.Collections;
using UnityEngine;

/// <summary>
/// ������Ʈ ���� ��Ʈ�ѷ�, ������ ������ ����
/// </summary>
public class LightController : MonoBehaviour
{
    [SerializeField]
    Light light;

    [SerializeField]
    TimeSetting TimeSetting;

    [Header("���� ���, ���� ���")]
    [SerializeField]
    float startIntencity, endIntencity;

    [Header("���� ����, �� ����")]
    [SerializeField]
    float startRange, endRange;

    /// <summary>
    /// �� ������ �ð�, ������ �ð� ��Ƶ� ����
    /// </summary>
    float lghtOnTime, lghtOffTime;


    float lightOnICSpeed, lightOffICSpeed;  // Intencity Change
    float lightOnRCSpeed, lightOffRCSpeed;  // Range Change

    /// <summary>
    /// �� ������ ���� �� �� �� ������Ʈ ���� �뵵
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
    /// ������Ʈ �����鼭 �� ������ ����
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
    /// �ٲ� �� �ʱ�ȭ
    /// </summary>
    void Init()
    {
        light.intensity = startIntencity;
        light.range = startRange;
    }

    /// <summary>
    /// ������ �������� ������ �ð����� ��/���� ������ �ϱ� ���� ���
    /// ���߿� �����غ��ϱ� �̹� Time.deltaTime���� �� ������� ����...
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
    /// ��� ���ϴ� ����
    /// </summary>
    /// <param name="targetIntencity">��ǥ ���</param>
    /// <param name="lightICSpeed">�ٲ�� �ӵ�</param>
    /// <returns></returns>
    IEnumerator ILightIntencityControl(float targetIntencity, float lightICSpeed)
    {
        while (light.intensity != targetIntencity)
        {
            light.intensity = Mathf.MoveTowards(light.intensity, targetIntencity, lightICSpeed);    // 1 �����Ӵ�..
            yield return null;
        }
    }

    /// <summary>
    /// Range ���ϴ� ����
    /// </summary>
    /// <param name="targetRange"></param>
    /// <param name="lightRCSpeed"></param>
    /// <returns></returns>
    IEnumerator ILightRangeControl(float targetRange,  float lightRCSpeed)
    {

        while (light.range != targetRange)
        {
            light.range = Mathf.MoveTowards(light.range, targetRange, lightRCSpeed);    // 1 �����Ӵ�..
            yield return null;
        }
    }

    /// <summary>
    /// ������Ʈ���� �ִϸ��̼� �ð��� �޶� �ִϸ��̼� ���� �� ȣ���� �� �ֵ���..
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
        // ������ ���� �� ������ ������Ʈ ����
        gameObject.SetActive(false);
    }

}
