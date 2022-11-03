using UnityEngine;

/// <summary>
/// 자동차 인터렉션시 나오는 바닥 그리드 애니메이션
/// </summary>
public class GridAniController : MonoBehaviour
{    
    [SerializeField]
    Material gridMat;
	
    [Header("속도조절(Y로)")]
    [SerializeField]
    Vector2 offset;

    /// <summary>
    /// offset 초기화 용도
    /// </summary>
    Vector2 initOffset = new Vector2(0, 0);

    private void Update()
    {
        // 그리드 메테리얼 오프셋 움직이기
        gridMat.mainTextureOffset = gridMat.mainTextureOffset + Time.deltaTime * offset;
;   }

    private void OnDisable()
    {
        // 그리드 초기화
        // 초기화 안하면 다시 애니메이션 할 때 그리드가 애매한 상태일거임
        gridMat.mainTextureOffset = initOffset;
    }
}

