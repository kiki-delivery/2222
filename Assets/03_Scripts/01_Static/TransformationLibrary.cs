using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 좌표 변환 기능 모음
/// </summary>
public static class TransformationLibrary
{
    /*
    public delegate Vector3 Transformation(Vector3 jointPosition, Camera camera);

    public enum TransformationName { RectPosition }

    static Transformation[] transformations = { GetRectPosition };

    public static Transformation GetTransformation(TransformationName name)
    {
        return transformations[(int)name];
    }
    */

    /// <summary>
    /// 키넥트의 조인트 포지션을 유니티상 RectPosition으로 변환
    /// </summary>
    /// <param name="jointPosition"></param>
    /// <param name="kinectCamera"></param>
    /// <param name="changeWidth"></param>
    /// <returns></returns>
    public static Vector3 GetRectPosition(Vector3 jointPosition, Camera kinectCamera, int changeWidth)
    {
        Vector3 screenPo = kinectCamera.WorldToScreenPoint(jointPosition);  // 여기서 이미 내 스크린 포인트로 바꾸니 키넥트 해상도는 상관이 없는거야
        screenPo = new Vector3(screenPo.x - Screen.width/4, screenPo.y - Screen.height/2, 0);   // 스크린포인트는 좌하단이 00이고, rect는 중간이 00이니까 하는 계산
        return new Vector3(screenPo.x + changeWidth, -screenPo.y, 0);
    }

    /// <summary>
    /// RectPosition => 스크릔 포지션으로 변환용도
    /// </summary>
    /// <param name="RectPosition"></param>
    /// <returns></returns>
    public static Vector3 GetScreenPoint(Vector3 RectPosition)
    {
        Vector3 screenPoint = new Vector3(RectPosition.x + Screen.width/2 , RectPosition.y + Screen.height / 2, 0);
        return screenPoint;
    }
    
}
