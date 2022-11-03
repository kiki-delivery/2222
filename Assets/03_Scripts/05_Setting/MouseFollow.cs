using UnityEngine;

/// <summary>
/// 팔로우 오브젝트 마우스 따라다니는 용도(테스트)
/// </summary>
public class MouseFollow : MonoBehaviour
{

    [Header("체크하면 마우스 따라다님")]
    public bool bMouseFollow;

    public RectTransform me;

    public int x, y;
    
    // Update is called once per frame
    void Update()
    {
        if(bMouseFollow)
        {
            me.position = Input.mousePosition;
        }
        else
        {
            me.position = new Vector3(x, y, 0);
        }
    }
}
