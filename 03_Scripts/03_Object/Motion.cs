using UnityEngine;

/// <summary>
/// 상하, 좌우, 대각선 왕복 움직임 만들기(해파리 사용)
/// </summary>
public class Motion : MonoBehaviour
{
    /// <summary>
    /// 움직이는 경로 
    /// </summary>
    public enum EMoveWay
    {
        LeftRight,
        UpDown,
        DeaGak
    }

    [Header("움직일 방식 지정")]
    public EMoveWay moveWay;

    [Header("움직일 방향 지정(-1, 1)")]
    public int moveVector;

    [Header("움직이는 속도")]
    public float speed = 0.5f;

    [Header("움직이는 범위")]
    public float moveRange = 1f;

    // 왕복운동 방향
    Vector3 vector;

    // 목표지점
    Vector3 originalVector, targetVector;

    // 처음 방향
    int dir = 1;

    // 타겟을 향해 가고 있나 체크
    bool goTarget = true;

    private void Awake()
    {
        originalVector = transform.position;    // 처음 위치는 저장하고 시작
    }

    private void OnEnable()
    {
        // 초기화 영역
        goTarget = true;
        transform.position = originalVector;
        dir = 1;
        dir = dir * moveVector; // 처음에 움직일 방향 지정

        // 초기화 끝

        // 움직임 지정한 경로에 따라 방향 설정
        if (moveWay == EMoveWay.LeftRight)
        {
            vector = Vector3.right;
        }
        else if (moveWay == EMoveWay.UpDown)
        {
            vector = Vector3.up;
        }
        else
        {
            vector = new Vector3(1, 1, 0);
        }

        
        targetVector = originalVector + vector * moveRange * moveVector;
    }

    private void Update()
    {
        if (moveWay == EMoveWay.LeftRight)
        {
            vector = Vector3.right;
        }
        else if (moveWay == EMoveWay.UpDown)
        {
            vector = Vector3.up;
        }
        else
        {
            vector = new Vector3(1, 1, 0);
        }

        // 타겟을 향해 가고있고, 타겟과 가까워지면 방향 바꾸기
        if (goTarget)
        {
            if (Vector3.Distance(transform.position, targetVector) < 0.1f)
            {
                dir = -dir;
                goTarget = false;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, originalVector) < 0.1f)
            {
                dir = -dir;
                goTarget = true;
            }
        }


        transform.position = transform.position + vector * dir * Time.deltaTime * speed;

    }

}
