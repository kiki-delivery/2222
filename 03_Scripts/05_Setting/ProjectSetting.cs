using UnityEngine;

/// <summary>
/// ������Ʈ ȯ�� ����
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
    /// �ڷ�ƾ ������� ���� ������ ��忡�� ����Ʈ ����ġ�� �Ȳ����� ��찡 �־� �߰�
    /// </summary>
    private void OnApplicationQuit()
    {
        for(int i =0; i< LightSwitchs.Length; i++)
        {
            LightSwitchs[i].SetActive(false);
        }
    }
}
