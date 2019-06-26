using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamController2 : MonoBehaviour
{

    WebCamTexture webcamTexture1;
    WebCamTexture webcamTexture2;
    Color[] configcolor1 = new Color[100];
    Color[] configcolor2 = new Color[100];
    WebCamDevice[] devices;

    //以下カメラの設定
    int width = 3;  //解像度横
    int height = 1; //解像度縦
    int fps = 15;
    int count = 0;  //数える用

    public LineIns lineIns;

    public bool camera1Flag;
    public bool camera2Flag;
    public bool rotFlag;

    void Start()
    {
        devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices)
        {
            Debug.Log("WebCamDevice " + device.name);
        }
        Request();
        webcamTexture1 = new WebCamTexture(devices[0].name, width, height, fps);
        webcamTexture2 = new WebCamTexture(devices[2].name, width, height, fps);
        webcamTexture1.Play();
        webcamTexture2.Play();

        camera1Flag = camera2Flag = rotFlag = true;
    }

    void Update()
    {
        //実機ではこれを曲が始まる前に実行する
        //スペースキーが押されたフレーム画像を取得して、今後その画像と比べる
        if (Input.anyKeyDown && Gate.StartGate)
        {
            ImportOffsetColor();
            camera1Flag = camera2Flag = rotFlag = false;
        }

        //毎フレームカメラが元画像と比べて色が規定値以上変化してないか調べる
        //分けてる理由としては1フレームに2つ以上"初期画像と違う"ログを出したくない
        //実機でも分けるべきかとこのログの数判定が出るからあんまきれいじゃない
        //細かいとこは任せるまとめてくれても構わん
        for (int x = 0,count = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CheckColor(configcolor1[count], webcamTexture1.GetPixel(x, y)) && !camera1Flag && Gate.StartGate)
                {
                    Debug.Log("初期画像と違う_カメラ1");
                    camera1Flag = true;
                    goto bre1;
                }
                count++;
            }
        }
        bre1:;

        for (int x = 0,count = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CheckColor(configcolor2[count], webcamTexture2.GetPixel(x, y)) && !camera2Flag && Gate.StartGate)
                {
                    Debug.Log("初期画像と違う_カメラ2");
                    camera2Flag = true;
                    goto bre2;
                }
                count++;
            }
        }
        bre2:;
    }

    bool CheckColor(Color conf, Color color)
    {
        //こ↑こ↓を設定することで設定変更できるよ(規定値)
        //この値より変化の具合が大きければ変化したとみなす
        float r = 0.05f;
        float b = 0.05f;
        float g = 0.05f;

        //値を確認したいときは下のコードをOnにする
        //Debug.Log(color + "," + conf);

        if (Mathf.Abs(conf.r - color.r) >= r && Mathf.Abs(conf.b - color.b) >= b && Mathf.Abs(conf.g - color.g) >= g)
        {
            return true;
        }
        return false;
    }


    public void ImportOffsetColor()
    {
        for (int x = 0,count = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                configcolor1[count] = webcamTexture1.GetPixel(x, y);
                configcolor2[count] = webcamTexture2.GetPixel(x, y);
                count++;
            }
        }
        Debug.Log("初期色取得");
    }

    IEnumerator Request()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
    }
}
