using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour
{
    public LineIns LineIns;

    private bool lane0Flag = false;
    private bool lane1Flag= false;
    private bool lane2Flag = false;

    public LoadData LoadData;

    void Update()
    {
        int turnCount = LineIns.turnCount;


            if (turnCount % 3 == 0 && !lane0Flag)
            {
                GetComponent<Image>().sprite = Sprite.Create(LoadData.lane0BackGround, new Rect(0, 0, LoadData.lane0BackGround.width, LoadData.lane0BackGround.height), Vector2.zero);

                lane0Flag = true;
                lane1Flag = false;
                lane2Flag = false;
            }
            else if (turnCount % 3 == 1 && !lane1Flag)
            {
                GetComponent<Image>().sprite = Sprite.Create(LoadData.lane1BackGround, new Rect(0, 0, LoadData.lane1BackGround.width, LoadData.lane1BackGround.height), Vector2.zero);

                lane0Flag = false;
                lane1Flag = true;
                lane2Flag = false;
            }
            else if (turnCount % 3 == 2 && !lane2Flag)
            {
                GetComponent<Image>().sprite = Sprite.Create(LoadData.lane2BackGround, new Rect(0, 0, LoadData.lane2BackGround.width, LoadData.lane2BackGround.height), Vector2.zero);

                lane0Flag = false;
                lane1Flag = false;
                lane2Flag = true;
            }
    }

    Texture ReadTexture(string path, int width, int height)
    {
        byte[] readBinary = ReadJpgFile(path);

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(readBinary);

        return texture;
    }

    byte[] ReadJpgFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    //ファイルの読み込み、ノーツの設置、ゲームの開始まで行うまで停止
    IEnumerator Load(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
