using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    List<string> lineCsv = new List<string>();
    List<string> rankCsv = new List<string>();

    List<TextMeshProUGUI> _Score = new List<TextMeshProUGUI>();
    List<TextMeshProUGUI> _Name = new List<TextMeshProUGUI>();

    public Text PlayerName;
    public TextMeshProUGUI Score;
    public Text No;
    public GameObject Thumbnail;
    public Text SongName;
    public GameObject Texts;

    /*
    public TextMeshProUGUI Score1;
    public TextMeshProUGUI PlayerName1;
    public TextMeshProUGUI Score2;
    public TextMeshProUGUI PlayerName2;
    public TextMeshProUGUI Score3;
    public TextMeshProUGUI PlayerName3;
    public TextMeshProUGUI Score4;
    public TextMeshProUGUI PlayerName4;
    public TextMeshProUGUI Score5;
    public TextMeshProUGUI PlayerName5;
    public TextMeshProUGUI Score6;
    public TextMeshProUGUI PlayerName6;
    */

    public TextMeshProUGUI Perfect;
    public TextMeshProUGUI Good;
    public TextMeshProUGUI OK;
    public TextMeshProUGUI Miss;

    /*
    public GameObject Frisk;
    public GameObject Mettaton;
    public GameObject Sans;
    public GameObject Flowey;
    public GameObject panel;
    public GameObject FullComboText;
    public GameObject ALLPerfectText;
    */

    int count,one,playerrank;
    string name,selectsongfilename;
    float sumscore;
    FileInfo fileInfo;
    StreamWriter streamWriter;


    // Use this for initialization
    void Start()
    {
        name = "";
        count = one = 0;
        //selectsongfilename = ReSongSelect.selectSongName;

        var pathname = Environment.CurrentDirectory;
        pathname = Path.Combine(pathname, "Songs");

        //var NotFoundPath = Path.Combine(pathname, "NotFound.jpg");
       // pathname = Path.Combine(pathname, selectsongfilename);

        //var rankpath = Path.Combine(pathname, "Ranking.csv");
       // var infopath = Path.Combine(pathname, selectsongfilename + "_info.csv");
       // var ThumPath = Path.Combine(pathname, "Thum.jpg");

        /*StreamReader sr = new StreamReader(rankpath);
        string csvTextLine = sr.ReadLine();
        while (csvTextLine != null)
        {
            rankCsv.Add(csvTextLine);
            csvTextLine = sr.ReadLine();
        }
        sr.Close();*/

        /*StreamReader sr = new StreamReader(infopath);
        string csvTextLine = sr.ReadLine();
        csvTextLine = sr.ReadLine();
        while (csvTextLine != null)
        {
            lineCsv.Add(csvTextLine);
            csvTextLine = sr.ReadLine();
        }
        sr.Close();*/

        //スコアと名前を書き込む場所の設定
        //fileInfo = new FileInfo(rankpath);
        //streamWriter = fileInfo.AppendText();

        //=====================================
        //            スコア表示
        //=====================================

        /*
        ALLPerfectText.SetActive(false);
        FullComboText.SetActive(false);
        */

        Score.text = NotesJudge._Score;
        Perfect.text = (Combo.perfect / 3).ToString();
        Good.text = (Combo.good / 3).ToString();
        OK.text = (Combo.ok / 3).ToString();
        Miss.text = (Combo.miss / 3).ToString();

        /*
        if (Miss.text == "0")
        {
            if (OK.text == "0" && Good.text == "0")
            {
                ALLPerfectText.SetActive(true);
            }
            else
            {
                FullComboText.SetActive(true);
            }
        }
        */

        //=====================================
        //            テクスチャ設定
        //=====================================

        /*
        Texture Thum = Thumbnail.GetComponent<Texture>();

        if (!File.Exists(ThumPath))
        {
            Debug.Log("!");
           // Thum = ReadTexture(NotFoundPath + "/NotFound.jpg", 200, 200);
        }
        else
        {
            Debug.Log("?");
            Thum = ReadTexture(ThumPath, 200, 200);
        }

        // テクスチャーを適用
        Thumbnail.GetComponent<Renderer>().material.mainTexture = Thum;

        // 下地の色は白にしておく (そうしないと下地の色と乗算みたいになる)
        Thumbnail.GetComponent<Renderer>().material.color = Color.white;
        */

        //=====================================
        //             曲名設定
        //=====================================

        //SongName.text = lineCsv[6];


        //=====================================
        //           ランキング表示
        //=====================================
        /*
        _Score.Clear();
        _Name.Clear();
        _Score.Add(Score1);
        _Score.Add(Score2);
        _Score.Add(Score3);
        _Score.Add(Score4);
        _Score.Add(Score5);
        _Score.Add(Score6);
        _Name.Add(PlayerName1);
        _Name.Add(PlayerName2);
        _Name.Add(PlayerName3);
        _Name.Add(PlayerName4);
        _Name.Add(PlayerName5);
        _Name.Add(PlayerName6);

        //人がいないときのこと
        for (int i = 0; i < 6; i++)
        {
            _Score[i].text = "0";
            _Name[i].text = "None";
        }

        //ソートしたい
        List<float> test = new List<float>();
        List<string> test3 = new List<string>();

        //スコア
        List<float> test2 = new List<float>();

        //名前
        List<string> test4 = new List<string>();
        for (int i = 0; i < rankCsv.Count; i++)
        {
            var divide = rankCsv[i];
            var _divide = divide.Split(',');
            test.Add(float.Parse(_divide[0]));
            test3.Add(_divide[1]);
        }
        test.Sort();
        test.Reverse();
        for(int i= 0; i < test.Count; i++)
        {
            var one = 0;
            for(int w = 0; w < test.Count; w++)
            {
                var divide = rankCsv[w];
                var _divide = divide.Split(',');
                Debug.Log(test[i]);
                Debug.Log(_divide[0]);
                if (test[i] == float.Parse(_divide[0]))
                {
                    if (one == 0)
                    {
                        Debug.Log(test[i]);
                        test2.Add(test[i]);
                        test4.Add(test3[w]);
                        one = 1;
                    }
                }
            }
        }

        //ソート完了
        //ここに今回のスコアを入れる
        sumscore = float.Parse(NotesJudge._Score);
        Score.text = sumscore.ToString();
        for (int i = 0; i < rankCsv.Count; i++)
        {
            if (i < 6)
            {
                _Score[i].text = test2[i].ToString();
                _Name[i].text = test4[i];
            }
            if(test2[i] <= sumscore)
            {
                if(one == 0)
                {
                    playerrank = i + 1;
                    one = 1;
                }
            }
            No.text = playerrank.ToString("F0");
        }
        if (one == 0)
        {
            playerrank = rankCsv.Count + 1;
            No.text = playerrank.ToString("F0");
        }
        */
    }

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("SongSelect");
        }

        /*
        var pos = Texts.transform.localPosition;


        //テキストを右に1つずらす
        if (Input.GetKeyDown(KeyCode.F) && pos.x > -5300)
        {
            Texts.transform.localPosition = new Vector3(pos.x - 200, pos.y, pos.z);
            count++;
        }

        //テキストを左に1つずらす
        if (Input.GetKeyDown(KeyCode.D) && pos.x < -100)
        {
            Texts.transform.localPosition = new Vector3(pos.x + 200, pos.y, pos.z);
            count--;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (name.Length != 8)
            {
                switch (count)
                {
                    case 0: name = name + "A"; break;
                    case 1: name = name + "B"; break;
                    case 2: name = name + "C"; break;
                    case 3: name = name + "D"; break;
                    case 4: name = name + "E"; break;
                    case 5: name = name + "F"; break;
                    case 6: name = name + "G"; break;
                    case 7: name = name + "H"; break;
                    case 8: name = name + "I"; break;
                    case 9: name = name + "J"; break;
                    case 10: name = name + "K"; break;
                    case 11: name = name + "L"; break;
                    case 12: name = name + "M"; break;
                    case 13: name = name + "N"; break;
                    case 14: name = name + "O"; break;
                    case 15: name = name + "P"; break;
                    case 16: name = name + "Q"; break;
                    case 17: name = name + "R"; break;
                    case 18: name = name + "S"; break;
                    case 19: name = name + "T"; break;
                    case 20: name = name + "U"; break;
                    case 21: name = name + "V"; break;
                    case 22: name = name + "W"; break;
                    case 23: name = name + "X"; break;
                    case 24: name = name + "Y"; break;
                    case 25: name = name + "Z"; break;
                    case 26:
                        if (name.Length >= 1)
                        {
                            var num = name.Length;
                            name = name.Remove(num - 1);
                        }
                        break;
                    case 27:
                        if (name != "")
                        {
                            //名前とスコア書き込み
                            streamWriter.WriteLine(Score.text+ ","+ PlayerName.text);streamWriter.Flush();
                            SceneManager.LoadScene("ReSongSelect");
                        }
                        break;

                }
            }
            else
            {
                switch (count)
                {
                    case 26:
                        var num = name.Length;
                        name = name.Remove(num - 1);
                        break;
                    case 27:
                        if (name != "")
                        {
                            //名前とスコア書き込み
                            streamWriter.WriteLine(Score.text + "," + PlayerName.text); streamWriter.Flush();
                            SceneManager.LoadScene("ReSongSelect");
                        }
                        break;
                }
            }
            //プレイヤーネームの更新
            PlayerName.text = name;
            //隠し要素
            if(name == "SANS")
            {
                Sans.SetActive(true);
            }
            else
            {
                Sans.SetActive(false);
            }
            if(name == "METTATON")
            {
                Mettaton.SetActive(true);
            }
            else
            {
                Mettaton.SetActive(false);
            }
            if(name == "FLOWEY")
            {
                Flowey.SetActive(true);
            }
            else
            {
                Flowey.SetActive(false);
            }
            if (name == "FRISK")
            {
                Frisk.SetActive(true);
                panel.GetComponent<Image>().color = Color.red;
            }
            else
            {
                Frisk.SetActive(false);
                panel.GetComponent<Image>().color = Color.white;
            }
        }
        */
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
}
