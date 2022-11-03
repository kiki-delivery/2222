using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;    // ���� Ű��Ʈ ���뵵
using Microsoft.Azure.Kinect.BodyTracking;
using System.Threading.Tasks; // �񵿱�ó����....

/// <summary>
/// Ű��Ʈ Ʈ��ŷ �뵵
/// </summary>
public class KinectTracking : MonoBehaviour
{
    Device kinect;
    Tracker tracker;

    // Ű��Ʈ�� �Կ��ϴ� �̹��� ���� �ؽ���
    Texture2D kinectTexture;

    [Header("�ʱ� ����")]
    [SerializeField]
    Camera kinectCamera;    // Ű��Ʈ�� (0,0,0)�̶� �� ��ġ ����� ī�޶� ������Ʈ

    [SerializeField]
    UnityEngine.UI.RawImage rawImage;

    // ī�޶� ��ġ ��ġ ����
    [SerializeField]
    CameraSideSelect cameraSide;

    int changeWidth;    // �����ʿ��� ����Ұ�� Ʈ��ŷ ��ġ ���� ��� �ʿ�
    int kinectCameraIndex;  // Ű��Ʈ ī�޶� ���п뵵

    /// <summary>
    /// ����� ���ͷ��� ��� ����
    /// </summary>
    [SerializeField]
    UserAction userAction;

    [Header("ũ�θ�Ű ����(���� ��� ����)")]
    // ũ�θ�Ű �ν� �������� ������
    [SerializeField]
    private int _depthDistanceMin = 200;
    [SerializeField]
    private int _depthDistanceMax = 3000;
    // Ư�� ������ �ڸ��� ������ �ʿ�(���ϸ� ���ܹٸ��� ���°� ����)
    [SerializeField]
    int cutLine = 0;

    /// <summary>
    /// ������ Ʈ��ŷ ���� ���� �ο��� ����
    /// </summary>
    uint prePeopleCount = 0;

    /// <summary>
    /// ī�޶� ��ġ ����
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
    /// ī�޶� ��ġ ��ġ�� ���� Ʈ��ŷ ��ġ ��� �� Ű��Ʈ ī�޶� ��ȣ �ο�
    /// ���ʿ� �ִ� �ָ� ù ��° Ű��Ʈ�� �Ѵ� ���� ����� �ƴ϶�
    /// ù ��° Ű��Ʈ�� ���ʿ� �ִٰ� �ϰڴ�.. ����� �ڵ��̶� ������ �־���
    /// </summary>
    void CameraSideCheck()
    {
        // �ȷο� ������Ʈ ��Ʈ Ʈ�������� ���� ���� ��ġ�� �޶���
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

    Transformation transformation; // ��ǥ ��ȯ �뵵




    // VFX��
    /*
    [SerializeField]
    GetPointCloud pointCloud;
    */


    /// <summary>
    /// Ű��Ʈ ���� �ʱ�ȭ
    /// </summary>
    void InitKinect()
    {
        kinect = Device.Open(kinectCameraIndex); // kinectCameraIndex Ű��Ʈ�� ����

        // �ʱ� ����
        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,  // �̰Ŷ� ���̰� ����.. => �̰� ���δٰ� ����ȭ�� ������ �Ǵ°� �ƴѵ�
            ColorResolution = ColorResolution.R1080p,
            DepthMode = DepthMode.NFOV_2x2Binned, // ������  WFOV_2x2Binned
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS15   // �ٵ�Ʈ��ŷ�� �ϴ� ���� ��Ȳ������ 15�� ���

        });

        /*
        // VFX��
        pointCloud.CloudSetting(kinect);
        */

        // Ű��Ʈ �ػ� ũ���� �ؽ��� ������ ����
        int width = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
        int height = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
        kinectTexture = new Texture2D(width, height);

        // Ʈ��Ŀ �ʱ�ȭ
        tracker = Tracker.Create(kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default });
    }

    
    // Ű��Ʈ Ʈ��ŷ
    private async Task KinectLoop()
    {
        while(true)
        {
            using (Capture capture = await Task.Run(() => kinect.GetCapture()).ConfigureAwait(true))    // ĸ�İ� �غ�Ǹ�
            {
                //rawImage.texture = GetDepthImg(capture);    // ���⼭ �ϸ� ���� ���� �𸣰���..
                Frame frame = GetBodyTrackingFrame(capture);
                BodyTracking(frame);
            }
        }
    }

    // �Կ� �̹��� �ʿ��� �� ���
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

        ushort[] depthByteArr = depthImage.GetPixels<ushort>().ToArray(); // �����̹����� color32�� �ٷ� ���°� �ȵ�
        Color32[] colorArr = new Color32[depthByteArr.Length];

        // ushort => Color32 �� �ٲٱ�
        for (int i = 0; i < colorArr.Length; i++)
        {
            int index = colorArr.Length - 1 - i; // ���� ���Ŀ뵵, ����� �����ϱ�
                                                 // 255 = RGB�� �ִ밪
                                                 // �ȼ��� depth ���� �������� RGB���� �۾�������

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
    /// �ٵ�Ʈ��ŷ ������ ��� frame��������
    /// </summary>
    /// <param name="capture"></param>
    /// <returns></returns>
    Frame GetBodyTrackingFrame(Capture capture)
    {
        tracker.EnqueueCapture(capture);
        //rawImage.texture = GetDepthImg(capture);    // ���⼭ �ϴ°Ŷ� �� ���� �����°Ŷ� �����ӵ� ���� ����, �̰� �����̹��� ���� �뵵
        
        /*
        // VFX��
        pointCloud.GetCloud(capture);
        */
        return tracker.PopResult();
    }

    /// <summary>
    /// �ٵ� Ʈ��ŷ
    /// </summary>
    /// <param name="frame"></param>

    void BodyTracking(Frame frame)
    {
        
        uint peopleCount = frame.NumberOfBodies;

        if (prePeopleCount != peopleCount)  // ��� ���� ���ߴ� = �������� �����ų� ������.
        {
            userAction.SpriteAllOff();  // �ϴ� ���� ����

            for(uint i =0; i<peopleCount; i++)  // ������ ��� �� ��ŭ
            {
                userAction.SpriteReady(frame.GetBodyId(i)); // �ٵ� ���̵� ã�Ƽ� ������Ʈ ��Ī
            }

            prePeopleCount = peopleCount;
        }
        
        // Ʈ��ŷ�� �ο��� ��ŭ
        for (int tc = 0; tc < frame.NumberOfBodies; tc++)
        {
            // Ʈ��ŷ ��Ҹ�ŭ(�Ӹ�~�߳�)
            /*
            for (int j = 0; j < 32; j++)
            {
                this.SetMarkPos(tc, (JointId)j, frame);
            }
            */

            // ������ �ϳ��� �ż� �ٲ�
            this.SetMarkPos(tc, JointId.Pelvis, frame);
        }
    }


    /// <summary>
    /// Ʈ��ŷ �Ǵ� ��ġ ����
    /// </summary>
    /// <param name="trackingNumber">Ʈ��ŷ�� ����� ������ȣ</param>
    /// <param name="jointId">����Ʈ ��ġ</param>
    /// <param name="frame"></param>
    void SetMarkPos(int trackingNumber, JointId jointId, Frame frame)
    {
        uint bodyId = frame.GetBodyId((uint)trackingNumber);    // ����� Ư���� �� �ִ� ID

        var joint = frame.GetBodySkeleton((uint)trackingNumber).GetJoint(jointId);  // ����Ʈ ����
        


        Vector3 jointPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);

        // pos�� ��ũ������Ʈ�� �ٷ� �����°ɷ� ���������� �ڵ� ����ȭ�� �� �� �˾Ҵµ�
        // ���÷����� ���ݸ� ���� ��Ȳ�̶� �ٷ� �ٲٴ°� �ҿ��� ����, ���� ��� �״�� ���鼭 ������Ʈ�� ���̳� ���� ���̷� �ϴ°� ������...
        Vector3 pos = TransformationLibrary.GetRectPosition(jointPosition, kinectCamera, changeWidth);

        userAction.FollowUserSprite(bodyId, pos);
        //userAction.followUserParticle(bodyId, pos);   // ��ƼŬ �� ��� �̰ź���
        userAction.Raycast(pos);
    }



    private void OnApplicationQuit()
    {
        kinect.StopCameras();   // Ű��Ʈ ����
    }
}
