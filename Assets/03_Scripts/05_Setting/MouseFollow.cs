using UnityEngine;

/// <summary>
/// �ȷο� ������Ʈ ���콺 ����ٴϴ� �뵵(�׽�Ʈ)
/// </summary>
public class MouseFollow : MonoBehaviour
{

    [Header("üũ�ϸ� ���콺 ����ٴ�")]
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
