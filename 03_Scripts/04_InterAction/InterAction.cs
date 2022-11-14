using System.Collections;
using UnityEngine;

public class InterAction : MonoBehaviour
{
    [Header("����� �ִϸ����� �� ���� �ֱ�")]
    [SerializeField]
    Animator[] InteractionAni;

    [Header("����� ���� �� ���� �ֱ�")]
    [SerializeField]
    AudioClip[] actionSounds;

    [Header("���� ����ŭ �ʿ�")]
    [SerializeField]
    AudioSource[] audios;

    [SerializeField]
    LightSwitch lightSwitch;

    [SerializeField]
    Collider collider;

    [SerializeField]
    GameObject startEffect;

    [SerializeField]
    TimeSetting timeSetting;

    [SerializeField]
    LightSoundSetting lightSoundSetting;

    float actionWaitTime;


    WaitForSeconds readyTime, lightOffWaitTime, waitAniCheckTime;
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// ���ͷ��� �۵� �� ����
    /// </summary>
    void Init()
    {
        collider.enabled = true;
        objectState = State.Idle;
        startEffect.SetActive(false);
        actionWaitTime = timeSetting.waitActionTime;    // ���ͷ��� �� �ٽ� �۵��� ������ ���ð�
        //effectWaitTime = new WaitForSeconds(timeSetting.startEffectTime); 
        lightOffWaitTime = new WaitForSeconds(timeSetting.lightOnTime + actionWaitTime); // �� ������ �ٷ� �۵����ϵ���
        waitAniCheckTime = new WaitForSeconds(0.5f);    // �ִϸ��̼� ������ �ٷ� üũ�ϸ� ����� ����� ����
        readyTime = new WaitForSeconds(timeSetting.readyTime);
    }

    public enum State
    {
        Idle,
        Action
    }

    public State objectState = State.Idle;

    // ������Ʈ ���¿� ���� ����
    void ObjectState(State state)
    {
        if (state == objectState)
        {
            return;
        }

        objectState = state;

        switch (objectState)
        {
            case State.Action:
                ActionStartEvent();
                StartCoroutine(aniStart());
                break;
            case State.Idle:
                StartCoroutine(ILightOff());
                break;
        }
    }

    /// <summary>
    /// �ִϸ��̼� �������� �Ͼ�� �ϵ�
    /// </summary>
    public void ActionStartEvent()
    {
        lightSwitch.LightOn();
        startEffect.SetActive(true);
    }

    /// <summary>
    /// �ִϸ��̼� ���� ��ȣ �ֱ�
    /// </summary>
    IEnumerator aniStart()
    {
        yield return readyTime;
        for (int i = 0; i < InteractionAni.Length; i++)
        {
            InteractionAni[i].SetTrigger("Action");
        }
        StartCoroutine(IAniEndCheck());
    }

    /// <summary>
    /// �ִϸ��̼� ���� �� �� ����
    /// </summary>
    /// <returns></returns>
    IEnumerator IAniEndCheck()
    {
        if (actionSounds != null)    // ���߿� �Ҹ����� �� �Ǹ� ����
        {
            SoundPlay(actionSounds);
        }
        yield return waitAniCheckTime;   // ��� ���ϸ� normalizedTime �Ǵ��� �߸���

        if (InteractionAni.Length != 0)
        {
            while (InteractionAni[0].GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
            {
                yield return null;
            }
        }
        ObjectState(State.Idle);
    }

    /// <summary>
    /// �� �� ����
    /// </summary>
    /// <returns></returns>
    IEnumerator ILightOff()
    {
        lightSwitch.LightOff();
        startEffect.SetActive(false);
        yield return lightOffWaitTime;
        collider.enabled = true;
    }



    int rayCount;

    /// <summary>
    /// ��ư ������ ����(�ܺο��� ���۽�Ŵ)
    /// </summary>
    public void Action()
    {

        collider.enabled = false;
        ObjectState(State.Action);
    }


    public void SoundPlay(AudioClip[] clip)
    {
        for (int i = 0; i < clip.Length; i++)
        {
            audios[i].clip = clip[i];

            if (!audios[i].isPlaying)
            {
                audios[i].Play();
            }
        }
    }
}
