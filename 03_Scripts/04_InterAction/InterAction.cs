using System.Collections;
using UnityEngine;

public class InterAction : MonoBehaviour
{
    [Header("재생할 애니메이터 다 여기 넣기")]
    [SerializeField]
    Animator[] InteractionAni;

    [Header("재생할 사운드 다 여기 넣기")]
    [SerializeField]
    AudioClip[] actionSounds;

    [Header("사운드 수만큼 필요")]
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
    /// 인터렉션 작동 전 세팅
    /// </summary>
    void Init()
    {
        collider.enabled = true;
        objectState = State.Idle;
        startEffect.SetActive(false);
        actionWaitTime = timeSetting.waitActionTime;    // 인터렉션 후 다시 작동할 때까지 대기시간
        //effectWaitTime = new WaitForSeconds(timeSetting.startEffectTime); 
        lightOffWaitTime = new WaitForSeconds(timeSetting.lightOnTime + actionWaitTime); // 불 꺼지고 바로 작동안하도록
        waitAniCheckTime = new WaitForSeconds(0.5f);    // 애니메이션 중인지 바로 체크하면 제대로 기능을 못함
        readyTime = new WaitForSeconds(timeSetting.readyTime);
    }

    public enum State
    {
        Idle,
        Action
    }

    public State objectState = State.Idle;

    // 오브젝트 상태에 따른 동작
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
    /// 애니메이션 시작전에 일어나는 일들
    /// </summary>
    public void ActionStartEvent()
    {
        lightSwitch.LightOn();
        startEffect.SetActive(true);
    }

    /// <summary>
    /// 애니메이션 시작 신호 주기
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
    /// 애니메이션 끝난 후 할 동작
    /// </summary>
    /// <returns></returns>
    IEnumerator IAniEndCheck()
    {
        if (actionSounds != null)    // 나중에 소리세팅 다 되면 빼기
        {
            SoundPlay(actionSounds);
        }
        yield return waitAniCheckTime;   // 대기 안하면 normalizedTime 판단을 잘못함

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
    /// 불 다 끄기
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
    /// 버튼 눌리면 동작(외부에서 동작시킴)
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
