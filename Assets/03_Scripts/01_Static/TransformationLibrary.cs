using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ǥ ��ȯ ��� ����
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
    /// Ű��Ʈ�� ����Ʈ �������� ����Ƽ�� RectPosition���� ��ȯ
    /// </summary>
    /// <param name="jointPosition"></param>
    /// <param name="kinectCamera"></param>
    /// <param name="changeWidth"></param>
    /// <returns></returns>
    public static Vector3 GetRectPosition(Vector3 jointPosition, Camera kinectCamera, int changeWidth)
    {
        Vector3 screenPo = kinectCamera.WorldToScreenPoint(jointPosition);  // ���⼭ �̹� �� ��ũ�� ����Ʈ�� �ٲٴ� Ű��Ʈ �ػ󵵴� ����� ���°ž�
        screenPo = new Vector3(screenPo.x - Screen.width/4, screenPo.y - Screen.height/2, 0);   // ��ũ������Ʈ�� ���ϴ��� 00�̰�, rect�� �߰��� 00�̴ϱ� �ϴ� ���
        return new Vector3(screenPo.x + changeWidth, -screenPo.y, 0);
    }

    /// <summary>
    /// RectPosition => ��ũ�p ���������� ��ȯ�뵵
    /// </summary>
    /// <param name="RectPosition"></param>
    /// <returns></returns>
    public static Vector3 GetScreenPoint(Vector3 RectPosition)
    {
        Vector3 screenPoint = new Vector3(RectPosition.x + Screen.width/2 , RectPosition.y + Screen.height / 2, 0);
        return screenPoint;
    }
    
}
