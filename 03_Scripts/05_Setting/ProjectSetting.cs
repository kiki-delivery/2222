using UnityEngine;

/// <summary>
/// 프로젝트 환경 세팅
/// </summary>
public class ProjectSetting : MonoBehaviour
{
    [SerializeField]
    int targetFps = 30;

    [SerializeField]
    GameObject[] LightSwitchs;

    [SerializeField]
    int screenWidth,screenHeight;

    private void Awake()
    {
        Application.targetFrameRate = targetFps;
        Screen.SetResolution(screenWidth, screenHeight, true); 
    }

    /// <summary>
    /// 코루틴 사용으로 인해 에디터 모드에서 라이트 스위치가 안꺼지는 경우가 있어 추가
    /// </summary>
    private void OnApplicationQuit()
    {
        for(int i =0; i< LightSwitchs.Length; i++)
        {
            LightSwitchs[i].SetActive(false);
        }
    }
}
