using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;    // 애져 키넥트 사용용도
using Microsoft.Azure.Kinect.BodyTracking;
using System.Threading.Tasks; // 비동기처리용....

/// <summary>
/// 키넥트 트래킹 용도
/// </summary>
public class KinectTracking : MonoBehaviour
{
    Device kinect;
    Tracker tracker;

    // 키넥트가 촬영하는 이미지 담을 텍스쳐
    Texture2D kinectTexture;

    [Header("초기 세팅")]
    [SerializeField]
    Camera kinectCamera;    // 키넥트가 (0,0,0)이라 그 위치 대신할 카메라 오브젝트

    [SerializeField]
    UnityEngine.UI.RawImage rawImage;

    // 카메라 설치 위치 설정
    [SerializeField]
    CameraSideSelect cameraSide;

    int changeWidth;    // 오른쪽에서 사용할경우 트래킹 위치 별도 계산 필요
    int kinectCameraIndex;  // 키넥트 카메라 구분용도

    /// <summary>
    /// 사용자 인터렉션 기능 연결
    /// </summary>
    [SerializeField]
    UserAction userAction;

    [Header("크로마키 관련(현재 사용 안함)")]
    // 크로마키 인식 뎁스범위 설정임
    [SerializeField]
    private int _depthDistanceMin = 200;
    [SerializeField]
    private int _depthDistanceMax = 3000;
    // 특정 값까지 자르는 설정이 필요(안하면 잔잔바리로 남는게 있음)
    [SerializeField]
    int cutLine = 0;

    /// <summary>
    /// 직전에 트래킹 했을 때의 인원수 저장
    /// </summary>
    uint prePeopleCount = 0;

    /// <summary>
    /// 카메라 위치 지정
    /// </summary>
    public enum CameraSideSelect
    {
        Left, Right
    }


    private void Start()
    {
        CameraSideCheck();
        InitKinect();

        transformation = kinect.GetCalibration().CreateTransformation();
        Task t = KinectLoop();
    }

    /// <summary>
    /// 카메라 설치 위치에 따른 트래킹 위치 계산 및 키넥트 카메라 번호 부여
    /// 왼쪽에 있는 애를 첫 번째 키넥트로 한다 같은 방식이 아니라
    /// 첫 번째 키넥트를 왼쪽에 있다고 하겠다.. 방식의 코딩이라 문제가 있었음
    /// </summary>
    void CameraSideCheck()
    {
        // 팔로우 오브젝트 렉트 트랜스폼에 따라서 적는 수치가 달라짐
        if (cameraSide == CameraSideSelect.Left)
        {
            changeWidth = -Screen.width / 2;
            //changeWidth = 0;
            kinectCameraIndex = 0;
        }
        else
        {
            //changeWidth = -Screen.width / 2;
            changeWidth = 0;
            kinectCameraIndex = 1;
        }
    }

    Transformation transformation; // 좌표 변환 용도




    // VFX용
    /*
    [SerializeField]
    GetPointCloud pointCloud;
    */


    /// <summary>
    /// 키넥트 설정 초기화
    /// </summary>
    void InitKinect()
    {
        kinect = Device.Open(kinectCameraIndex); // kinectCameraIndex 키넥트와 연결

        // 초기 설정
        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,  // 이거라도 줄이고 싶음.. => 이거 줄인다고 최적화에 도움이 되는게 아닌듯
            ColorResolution = ColorResolution.R1080p,
            DepthMode = DepthMode.NFOV_2x2Binned, // 뎁스용  WFOV_2x2Binned
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS15   // 바디트래킹만 하는 지금 상황에서는 15로 충분

        });

        /*
        // VFX용
        pointCloud.CloudSetting(kinect);
        */

        // 키넥트 해상도 크기의 텍스쳐 사이즈 설정
        int width = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
        int height = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
        kinectTexture = new Texture2D(width, height);

        // 트래커 초기화
        tracker = Tracker.Create(kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default });
    }

    
    // 키넥트 트래킹
    private async Task KinectLoop()
    {
        while(true)
        {
            using (Capture capture = await Task.Run(() => kinect.GetCapture()).ConfigureAwait(true))    // 캡쳐가 준비되면
            {
                //rawImage.texture = GetDepthImg(capture);    // 여기서 하면 느린 이유 모르겠음..
                Frame frame = GetBodyTrackingFrame(capture);
                BodyTracking(frame);
            }
        }
    }

    // 촬영 이미지 필요할 때 사용
    Texture2D GetColorImg(Capture capture)
    {
        Image colorImg = capture.Color;
        int width = colorImg.WidthPixels;
        int height = colorImg.HeightPixels;
        kinectTexture = new Texture2D(width, height);

        Color32[] pixels = colorImg.GetPixels<Color32>().ToArray();

        int pixelsLength = pixels.Length;

        Color32[] rePixels = new Color32[pixelsLength];


        for (int i = 0; i < pixelsLength; i++)
        {
            rePixels[i] = pixels[pixelsLength - i - 1];
        }

        kinectTexture.SetPixels32(rePixels);
        kinectTexture.Apply();

        return kinectTexture;
    }

    Texture2D GetDepthImg(Capture capture)
    {

        Image depthImage = transformation.DepthImageToColorCamera(capture);

        ushort[] depthByteArr = depthImage.GetPixels<ushort>().ToArray(); // 뎁스이미지는 color32로 바로 오는게 안됨
        Color32[] colorArr = new Color32[depthByteArr.Length];

        // ushort => Color32 로 바꾸기
        for (int i = 0; i < colorArr.Length; i++)
        {
            int index = colorArr.Length - 1 - i; // 역순 정렬용도, 뒤집어서 나오니까
                                                 // 255 = RGB의 최대값
                                                 // 픽셀의 depth 값이 깊을수록 RGB값이 작아질거임

            int depthVal = 255 - (255 * (depthByteArr[index] - _depthDistanceMin) / _depthDistanceMax);


            if (depthVal < cutLine)
            {
                depthVal = 255;
            }
            else if (depthVal > 255)
            {
                depthVal = 255;
            }

            colorArr[i] = new Color32(0, 0, (byte)depthVal, 255);


        }
        kinectTexture.SetPixels32(colorArr);
        kinectTexture.Apply();

        return kinectTexture;
    }


    /// <summary>
    /// 바디트래킹 정보가 담긴 frame가져오기
    /// </summary>
    /// <param name="capture"></param>
    /// <returns></returns>
    Frame GetBodyTrackingFrame(Capture capture)
    {
        tracker.EnqueueCapture(capture);
        //rawImage.texture = GetDepthImg(capture);    // 여기서 하는거랑 위 에서 돌리는거랑 반응속도 차이 심함, 이거 뎁스이미지 쓰는 용도
        
        /*
        // VFX용
        pointCloud.GetCloud(capture);
        */
        return tracker.PopResult();
    }

    /// <summary>
    /// 바디 트래킹
    /// </summary>
    /// <param name="frame"></param>

    void BodyTracking(Frame frame)
    {
        
        uint peopleCount = frame.NumberOfBodies;

        if (prePeopleCount != peopleCount)  // 사람 수가 변했다 = 누군가가 들어오거나 나갔다.
        {
            userAction.SpriteAllOff();  // 일단 전부 끄고

            for(uint i =0; i<peopleCount; i++)  // 감지된 사람 수 만큼
            {
                userAction.SpriteReady(frame.GetBodyId(i)); // 바디 아이디 찾아서 오브젝트 매칭
            }

            prePeopleCount = peopleCount;
        }
        
        // 트래킹된 인원수 만큼
        for (int tc = 0; tc < frame.NumberOfBodies; tc++)
        {
            // 트래킹 요소만큼(머리~발끝)
            /*
            for (int j = 0; j < 32; j++)
            {
                this.SetMarkPos(tc, (JointId)j, frame);
            }
            */

            // 실전은 하나면 돼서 바꿈
            this.SetMarkPos(tc, JointId.Pelvis, frame);
        }
    }


    /// <summary>
    /// 트래킹 되는 위치 감지
    /// </summary>
    /// <param name="trackingNumber">트래킹된 사람의 고유번호</param>
    /// <param name="jointId">조인트 위치</param>
    /// <param name="frame"></param>
    void SetMarkPos(int trackingNumber, JointId jointId, Frame frame)
    {
        uint bodyId = frame.GetBodyId((uint)trackingNumber);    // 사람을 특정할 수 있는 ID

        var joint = frame.GetBodySkeleton((uint)trackingNumber).GetJoint(jointId);  // 조인트 정보
        


        Vector3 jointPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);

        // pos를 스크린포인트로 바로 보내는걸로 실전에서는 코드 간소화가 될 줄 알았는데
        // 디스플레이의 절반만 쓰는 상황이라 바로 바꾸는게 소용이 없음, 현재 방법 그대로 쓰면서 오브젝트만 보이냐 마냐 차이로 하는게 맞을듯...
        Vector3 pos = TransformationLibrary.GetRectPosition(jointPosition, kinectCamera, changeWidth);

        userAction.FollowUserSprite(bodyId, pos);
        //userAction.followUserParticle(bodyId, pos);   // 파티클 쓸 경우 이거보기
        userAction.Raycast(pos);
    }



    private void OnApplicationQuit()
    {
        kinect.StopCameras();   // 키넥트 정리
    }
}
