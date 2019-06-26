using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    class SongFile // セーブファイルのパスを示すクラス
    {
        public string Info = "";
        public string Notes = "";
        public string CNotes = "";
    }

    class InfoFile // Infoファイルに存在する値を保存するクラス
    {
        public float bpm = 0;
        public float offset = 0;
        public string songname = "";
    }

    public class NotesFile  // Notesファイルのデータクラス
    {
        public List<GameObject> NN = new List<GameObject>();  // 通常ノーツ
        public List<GameObject> SH = new List<GameObject>();  // ホールド（始点）
        public List<GameObject> FH = new List<GameObject>();  // ホールド（終点）
        public List<GameObject> H  = new List<GameObject>();   // ホールド（押してる間）
        public List<GameObject> CN = new List<GameObject>();  // カメラノーツ
    }

    class NNotesLineTime // NNotesの各ラインのノーツが降ってくる時間
    {
        public List<float> L1 = new List<float>();  // レーン1
        public List<float> L2 = new List<float>();  // レーン2
        public List<float> L3 = new List<float>();  // レーン3
        public List<float> L4 = new List<float>();  // レーン4
    }

    public static List<float> NNL1 = new List<float>();
    public static List<float> NNL2 = new List<float>();
    public static List<float> NNL3 = new List<float>();
    public static List<float> NNL4 = new List<float>();

    class HNotesLineTime // HNotesの各ラインのノーツが降ってくる時間
    {
        public List<float> L1 = new List<float>();  // レーン1
        public List<float> L2 = new List<float>();  // レーン2
        public List<float> L3 = new List<float>();  // レーン3
        public List<float> L4 = new List<float>();  // レーン4
    }

    public static List<float> HNL1 = new List<float>();
    public static List<float> HNL2 = new List<float>();
    public static List<float> HNL3 = new List<float>();
    public static List<float> HNL4 = new List<float>();

    class CNotesLineTime // CNotesの各ラインのノーツが降ってくる時間
    {
        public List<float> L5 = new List<float>();  // レーン5
        public List<float> L6 = new List<float>();  // レーン6
    }

    public static List<float> CNL1 = new List<float>();
    public static List<float> CNL2 = new List<float>();


    class FrontLanePosX  // レーンのｘ座標の設定
    {
        public float L1 = -1.5f; //一番左のレーン
        public float L2 = -0.5f; //左から2番目のレーン
        public float L3 = 0.5f; //左から3番目のレーン
        public float L4 = 1.5f; //左から4番目のレーン
        public float L5 = 0f; //左から4番目のレーン
        public float L6 = 0f; //左から4番目のレーン
    }

    List<string> lineCsv = new List<string>();  // csvファイルの内容を格納するリスト

    public GameObject Lane0Notes;
    public GameObject Lane1Notes;
    public GameObject Lane2Notes;
    public GameObject FrontNNotes;
    public GameObject FrontHNotes;
    public GameObject FrontHold;
    public GameObject LeftCameraNotes;
    public GameObject RightCameraNotes;

    public AudioSource melody;
    public AudioSource lane0;
    public AudioSource lane1;
    public AudioSource lane2;

   // public GameObject Thumbnail;
    public GameObject LoadPanel;

    public TextMeshProUGUI SongName;
    public TextMeshProUGUI Composer;
    public Text BPM_;
    public Text Lv;

    public new Camera camera;

    public string selectSongFileName = "";
    public float speed = 0;
    public float bpm = 0;
    float songstartpos, timer, songstarttimer, songoffse;
    bool Rote = false, StartGate, ParLine;

    public float notespos = 35f;

    Vector3 startrote = Vector3.zero;

    // 判定部分に渡される変数
    public static bool SongPalyCount, Playing;
    public static float offset, finishnotes;
    public static string musicname;
    public static int notescount, shadowsum;
    public static bool Gate1;

    SongFile SF = new SongFile();
    InfoFile IF = new InfoFile();
    NotesFile NF = new NotesFile();
    NNotesLineTime NLT = new NNotesLineTime();
    HNotesLineTime HLT = new HNotesLineTime();
    CNotesLineTime CNLT = new CNotesLineTime();
    FrontLanePosX FLPX = new FrontLanePosX();

    public Texture2D lane0BackGround;
    public Texture2D lane1BackGround;
    public Texture2D lane2BackGround;

    void Start()
    {
        // 初期化
        SF = new SongFile();
        IF = new InfoFile();
        NF = new NotesFile();
        NLT = new NNotesLineTime();
        HLT = new HNotesLineTime();
        CNLT = new CNotesLineTime();
        FLPX = new FrontLanePosX();

        selectSongFileName = PageSelect.toPlayGameSongName;
        timer = 0;  // 指定時間分マイナス(songstartpos)
        songstartpos = 0;
        SongPalyCount = false;
        Playing = false;
        finishnotes = 0;
        notescount = 0;
        Gate1 = false;
        StartGate = false;
        ParLine = true;
        LoadPanel.SetActive(true);

        if (selectSongFileName == null)
        {
            Debug.Log("ファイルが読み込めないため処理を中止しました。");
            return;
        }

        //speed = ReSongSelect.speed;
        speed = 2;

        // 背景画像の読み込み
        var pathname = Path.Combine(Environment.CurrentDirectory, "Songs");
        pathname = Path.Combine(pathname, selectSongFileName);
        var lane0BackGroundPath = Path.Combine(pathname, "Lane0BackGround.jpg");
        var lane1BackGroundPath = Path.Combine(pathname, "Lane1BackGround.jpg");
        var lane2BackGroundPath = Path.Combine(pathname, "Lane2BackGround.jpg");
        lane0BackGround = ReadTexture(lane0BackGroundPath, 1920, 1080) as Texture2D;
        lane1BackGround = ReadTexture(lane1BackGroundPath, 1920, 1080) as Texture2D;
        lane2BackGround = ReadTexture(lane2BackGroundPath, 1920, 1080) as Texture2D;

        /* コルーチンを使うと、「処理１」→「一定時間待つ」→「処理２」→「一定時間待つ」→「処理３」のような一連の処理を１つの関数で直感的に書ける。
         * Load(1f → 1秒待つ , 実行する処理)       
         */
        StartCoroutine(Load(1f, () =>
        {
            Debug.Log(selectSongFileName);

            /* パスの指定
             * 
             * Path.Combine(abc, def);
             * →「abc」と「def」のファイルパスを結合する
             * →abc\def
             * 
             * Environment.CurrentDirectory
             * →作業フォルダのパスが文字列で取得できる
             */
         //   var pathname = Path.Combine(Environment.CurrentDirectory, "Songs");
          //  pathname = Path.Combine(pathname, selectSongFileName);
            //SF.Info = Path.Combine(pathname, selectSongFileName + "_Info.csv");
            //SF.Notes = Path.Combine(pathname, selectSongFileName + "_Notes.csv");
            SF.Info = Path.Combine(pathname, "Info.csv");
            SF.Notes = Path.Combine(pathname, "Notes.csv");
            SF.CNotes = Path.Combine(pathname, "CNotes.csv");

            //=====================================
            //        Infoファイルの読み込み
            //=====================================

            // StreamReaderクラス：テキストファイルの内容を文字列として読み込む
            StreamReader sr = new StreamReader(SF.Info);

            // ReadLineメソッド：一行ずつ読み込む
            string csvTextLine = sr.ReadLine();

            // リスト：lineCsvにテキストの内容を一行ずつすべて格納する
            while (csvTextLine != null)
            {
                lineCsv.Add(csvTextLine);
                csvTextLine = sr.ReadLine();
            }
            sr.Close();

            // テキストの8行目：BPM
            IF.bpm = float.Parse(lineCsv[8].ToString());
            bpm = IF.bpm;

            // テキストの2行目：曲名
            IF.songname = lineCsv[2];

            // テキストの10行目：オフセット
            IF.offset = float.Parse(lineCsv[10].ToString());
            var todayoffset = float.Parse(lineCsv[16].ToString()); // todayoffset: ?
            songstartpos = IF.bpm / 60 * 1.5f * 3 * speed;  // 初めに何秒間空白の時間を入れるかの設定（ここでは3秒）
            songstartpos = 0; // ?
            offset = IF.offset + todayoffset;

            // サムネイルの指定・生成
            var ThumPath = Path.Combine(pathname, "Thum.jpg"); // サムネイルのパス
            var NotFoundPath = Path.Combine(Environment.CurrentDirectory, "Songs");
            //Texture Thum = Thumbnail.GetComponent<Texture>();  // Textureクラス：テクスチャを扱う基盤となるクラス
            if (!File.Exists(ThumPath))
            {
                // サムネイルがない場合、デフォルトのサムネイルを指定 (150*150)
              //  Thum = ReadTexture(NotFoundPath + "/NotFound.jpg", 150, 150);
            }
            else
            {
                // サムネイルがある場合、サムネイルを指定 (150*150)
              //  Thum = ReadTexture(ThumPath, 150, 150);
            }
            // テクスチャーを適用
            //Thumbnail.GetComponent<Renderer>().material.mainTexture = Thum;
            // 下地の色は白にしておく (そうしないと下地の色と乗算みたいになる)
           // Thumbnail.GetComponent<Renderer>().material.color = Color.white;

            // SongName
            SongName.text = lineCsv[6];

            // Composer
            Composer.text = lineCsv[4];

            // BPM
            //BPM_.text = lineCsv[8];

            // Lv
            //Lv.text = lineCsv[15];

            // 値の整理
            BPM(IF.bpm);
            lineCsv.Clear();


            //=====================================
            //            曲の読み込み
            //=====================================

            // 音声ファイルのパスの指定
            var path = Path.Combine(Environment.CurrentDirectory, "Songs");
            path = Path.Combine(path, IF.songname);
            var melodyPath = Path.Combine(path, "melody.ogg");
            var lane0Path = Path.Combine(path, "Lane0.ogg");
            var lane1Path = Path.Combine(path, "Lane1.ogg");
            var lane2Path = Path.Combine(path, "Lane2.ogg");
            // path = path + ".ogg";

            // ファイルが無かったら終わる
            if (File.Exists(melodyPath))
            {
                //Debug.Log("File does NOT exist!! file path = " + path);
                WWW melodyRequest = new WWW("file://" + melodyPath);
                while (!melodyRequest.isDone)
                {
                    new WaitForEndOfFrame();
                }
                AudioClip melodyAudioTrack = melodyRequest.GetAudioClip(false, true);

                while (melodyAudioTrack.loadState == AudioDataLoadState.Loading)
                {
                    // ロードが終わるまで待つ
                    new WaitForEndOfFrame();
                }

                if (melodyAudioTrack.loadState != AudioDataLoadState.Loaded)
                {
                    // 読み込み失敗
                    Debug.Log("Failed to Load!");
                }

                // 生成したsourceに読み込んだAudioClipを設定する
                melody.clip = melodyAudioTrack;
                melody.time = offset;
            }

            // 指定したファイルをロードする
            WWW mainRequest = new WWW("file://" + lane0Path);
            WWW pianoRequest = new WWW("file://" + lane1Path);
            WWW picopicoRequest = new WWW("file://" + lane2Path);

            // ロードが終わるまで待つ
            while (!mainRequest.isDone && !pianoRequest.isDone && !picopicoRequest.isDone)
            {
                new WaitForEndOfFrame();
                Debug.Log("MusicLoading...");
            }

            // 読み込んだファイルからAudioClipを取り出す
            AudioClip lane0AudioTrack = mainRequest.GetAudioClip(false, true);
            AudioClip lane1AudioTrack = pianoRequest.GetAudioClip(false, true);
            AudioClip lane2AudioTrack = picopicoRequest.GetAudioClip(false, true);
            while (lane0AudioTrack.loadState == AudioDataLoadState.Loading && lane1AudioTrack.loadState == AudioDataLoadState.Loading && lane2AudioTrack.loadState == AudioDataLoadState.Loading)
            {
                // ロードが終わるまで待つ
                new WaitForEndOfFrame();
            }

            if (lane0AudioTrack.loadState != AudioDataLoadState.Loaded || lane1AudioTrack.loadState != AudioDataLoadState.Loaded || lane2AudioTrack.loadState != AudioDataLoadState.Loaded)
            {
                // 読み込み失敗
                Debug.Log("Failed to Load!");
            }

            // 生成したsourceに読み込んだAudioClipを設定する
            lane0.clip = lane0AudioTrack;
            lane1.clip = lane1AudioTrack;
            lane2.clip = lane2AudioTrack;
            lane0.time = offset;
            lane1.time = offset;
            lane2.time = offset;


            //=====================================
            //      Notesファイルの読み込み
            //=====================================

            sr = new StreamReader(SF.Notes);
            csvTextLine = sr.ReadLine();
            while (csvTextLine != null)
            {
                lineCsv.Add(csvTextLine);
                csvTextLine = sr.ReadLine();
            }

            sr.Close();

            // 使う変数の宣言
            string notesInfo;
            string[] notesinfo = new string[7];
            char[] remove = new char[] { '(', ')' };

            for (int i = 0; i < lineCsv.Count; i++)
            {
                // 括弧を外す 
                foreach (char c in remove)
                {
                    lineCsv[i] = lineCsv[i].Replace(c.ToString(), "");
                }

                notesInfo = lineCsv[i];
                notesinfo = notesInfo.Split(','); // notesinfoにNotesの情報を入れる

                var front = 0.6f; // 表のY座標
                var pos = new Vector3(0, 0, 0); // ノーツの生成位置の変数
                var bpos = 0f;  // ホールドの座標設定用変数
                var hold = new Vector3(0, 0, 0); // ホールドの座標設定用変数
                var zscale = 0f;// ホールドのZ座標のスケールの大きさ設定用変数
                var scale = new Vector3(0.75f, 0.01f, 0); // ノーツのスケールの初期値

                // 一番最後のノーツを調べる
                if (finishnotes < float.Parse(notesinfo[3]))
                {
                    finishnotes = float.Parse(notesinfo[3]);
                }

                // 通常ノーツの生成
                if (notesinfo[0] == "1")
                {
                    // ノーツ数カウント
                    notescount++;
                    switch (notesinfo[1])
                    {
                        case "2": // 一番左のレーン
                            {
                                pos = new Vector3(FLPX.L1, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            NotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            NotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            NotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }                             
                                NLT.L1.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "3": //左から2つ目のレーン
                            {
                                pos = new Vector3(FLPX.L2, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            NotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            NotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            NotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                NLT.L2.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "4"://左から3番目のレーン
                            {

                                pos = new Vector3(FLPX.L3, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            NotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            NotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            NotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                NLT.L3.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "5"://左から4番目のレーン
                            {
                                pos = new Vector3(FLPX.L4, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            NotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            NotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            NotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                NLT.L4.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                    }
                }
                
                //始点のホールドノーツの生成             
                else if (notesinfo[0] == "2")  //ファイルがいじられてなければ”2”の次は”3”
                {
                    //ノーツ数カウント
                    notescount++;
                    switch (notesinfo[1])
                    {
                        case "2": //一番左のレーン
                            {
                                pos = new Vector3(FLPX.L1, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            StartHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            StartHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            StartHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L1.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "3"://左から2つ目のレーン
                            {
                                pos = new Vector3(FLPX.L2, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            StartHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            StartHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            StartHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L2.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "4"://左から3番目のレーン
                            {
                                pos = new Vector3(FLPX.L3, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            StartHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            StartHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            StartHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L3.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "5"://左から4番目のレーン
                            {
                                pos = new Vector3(FLPX.L4, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            StartHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            StartHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            StartHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L4.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                    }
                }

                //終点のホールドノーツ生成&押してる間のホールドの始点と終点を結ぶラインの生成
                else if (notesinfo[0] == "3")
                {
                    //ノーツ数カウント
                    notescount++;
                    switch (notesinfo[1])
                    {
                        case "2": //一番左のレーン
                            {
                                pos = new Vector3(FLPX.L1, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L1.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存

                                //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - (float.Parse(notesinfo[5]) + songstartpos) * speed);  //ラインの長さを調べる
                                bpos = bpos / 2 - notespos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L1, front, bpos);
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            HoldNotesGenerate(hold, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            HoldNotesGenerate(hold, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            HoldNotesGenerate(hold, Lane2Notes);
                                            break;
                                        }
                                }

                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - notespos - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }                         
                        case "3"://左から2つ目のレーン
                            {
                                pos = new Vector3(FLPX.L2, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L2.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                                                       
                                //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - float.Parse(notesinfo[5]) * speed + songstartpos);  //ラインの長さを調べる
                                bpos = bpos / 2  - notespos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L2, front, bpos);
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            HoldNotesGenerate(hold, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            HoldNotesGenerate(hold, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            HoldNotesGenerate(hold, Lane2Notes);
                                            break;
                                        }
                                }

                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - notespos - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }                          
                        case "4"://左から3番目のレーン
                            {
                                pos = new Vector3(FLPX.L3, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L3.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                                                       //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - float.Parse(notesinfo[5]) * speed + songstartpos);  //ラインの長さを調べる
                                bpos = bpos / 2 - notespos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L3, front, bpos);
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            HoldNotesGenerate(hold, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            HoldNotesGenerate(hold, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            HoldNotesGenerate(hold, Lane2Notes);
                                            break;
                                        }
                                }

                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - notespos - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }                           
                        case "5"://左から4番目のレーン
                            {
                                pos = new Vector3(FLPX.L4, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            FinishHoldNotesGenerate(pos, Lane2Notes);
                                            break;
                                        }
                                }
                                HLT.L4.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                                                       //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - float.Parse(notesinfo[5]) * speed + songstartpos);  //ラインの長さを調べる
                                bpos = bpos / 2 - notespos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L4, front, bpos);
                                switch (notesinfo[2])
                                {
                                    case "0":
                                        {
                                            HoldNotesGenerate(hold, Lane0Notes);
                                            break;
                                        }
                                    case "1":
                                        {
                                            HoldNotesGenerate(hold, Lane1Notes);
                                            break;
                                        }
                                    case "2":
                                        {
                                            HoldNotesGenerate(hold, Lane2Notes);
                                            break;
                                        }
                                }

                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - notespos - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }                                        
                    }
                }
            }

            lineCsv.Clear(); //また使うので中身をすべて消す

            //=====================================
            //      CNotesファイルの読み込み
            //=====================================
            
            
            sr = new StreamReader(SF.CNotes);
            csvTextLine = sr.ReadLine();
            while (csvTextLine != null)
            {
                lineCsv.Add(csvTextLine);
                csvTextLine = sr.ReadLine();
            }
            sr.Close();

            for (int i = 0; i < lineCsv.Count; i++)
            {
                // 括弧を外す 
                foreach (char c in remove)
                {
                    lineCsv[i] = lineCsv[i].Replace(c.ToString(), "");
                }

                notesInfo = lineCsv[i];
                notesinfo = notesInfo.Split(','); // notesinfoにCNotesの情報を入れる

                var front = 1.0f; // 表のY座標
                var pos = new Vector3(0, 0, 0); // ノーツの生成位置の変数
                var bpos = 0f;  // ホールドの座標設定用変数
                var hold = new Vector3(0, 0, 0); // ホールドの座標設定用変数
                var zscale = 0f;// ホールドのZ座標のスケールの大きさ設定用変数
                var scale = new Vector3(0.75f, 0.01f, 0); // ノーツのスケールの初期値

                // 一番最後のノーツを調べる
                if (finishnotes < float.Parse(notesinfo[3]))
                {
                    finishnotes = float.Parse(notesinfo[3]);
                }

                // カメラノーツの生成
                if (notesinfo[0] == "3")
                {
                    // ノーツ数カウント
                    notescount++;

                    pos = new Vector3(FLPX.L5, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                    switch (notesinfo[2])
                    {
                        case "0":
                        {
                            LeftCameraNotesGenerate(pos, Lane0Notes);
                            break;
                        }
                        case "1":
                        {
                            LeftCameraNotesGenerate(pos, Lane1Notes);
                            break;
                        }
                        case "2":
                        {
                          　LeftCameraNotesGenerate(pos, Lane2Notes);
                            break;
                        }
                  　}
                    CNLT.L5.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存                                            
                }

                // カメラノーツの生成
                else if (notesinfo[0] == "4")
                {
                    // ノーツ数カウント
                    notescount++;

                    pos = new Vector3(FLPX.L6, front, (float.Parse(notesinfo[5]) + songstartpos) * speed - notespos); //生成位置の設定
                    switch (notesinfo[2])
                    {
                        case "0":
                            {
                                RightCameraNotesGenerate(pos, Lane0Notes);
                                break;
                            }
                        case "1":
                            {
                                RightCameraNotesGenerate(pos, Lane1Notes);
                                break;
                            }
                        case "2":
                            {
                                RightCameraNotesGenerate(pos, Lane2Notes);
                                break;
                            }
                    }
                    CNLT.L6.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存                                            
                }

                /*
                // 始点のホールドノーツの生成
                else if (notesinfo[0] == "2")  //ファイルがいじられてなければ”2”の次は”3”
                {
                    //ノーツ数カウント
                    notescount++;
                    switch (notesinfo[1])
                    {
                        case "2": //一番左のレーン
                            {
                                pos = new Vector3(FLPX.L1, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.SH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.SH[NF.SH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L1.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "3"://左から2つ目のレーン
                            {
                                pos = new Vector3(FLPX.L2, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.SH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.SH[NF.SH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L2.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "4"://左から3番目のレーン
                            {
                                pos = new Vector3(FLPX.L3, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.SH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.SH[NF.SH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L3.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "5"://左から4番目のレーン
                            {
                                pos = new Vector3(FLPX.L4, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.SH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.SH[NF.SH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L4.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                    }
                }

                // 終点のホールドノーツ生成&押してる間のホールドの始点と終点を結ぶラインの生成
                else if (notesinfo[0] == "3")
                {
                    //ノーツ数カウント
                    notescount++;
                    switch (notesinfo[1])
                    {
                        case "2": //一番左のレーン
                            {
                                pos = new Vector3(FLPX.L1, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.FH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.FH[NF.FH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L1.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                                                       //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - (float.Parse(notesinfo[5]) + songstartpos) * speed);  //ラインの長さを調べる
                                bpos = bpos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L1, front, bpos);
                                NF.H.Add(Instantiate(FrontHold, hold, Quaternion.identity));  //生成
                                NF.H[NF.H.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }

                        case "3"://左から2つ目のレーン
                            {
                                pos = new Vector3(FLPX.L2, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.FH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.FH[NF.FH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L2.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                                                       //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - float.Parse(notesinfo[5]) * speed + songstartpos);  //ラインの長さを調べる
                                bpos = bpos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L2, front, bpos);
                                NF.H.Add(Instantiate(FrontHold, hold, Quaternion.identity));  //生成
                                NF.H[NF.H.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }
                        case "4"://左から3番目のレーン
                            {
                                pos = new Vector3(FLPX.L3, front, ((float.Parse(notesinfo[5]) + songstartpos) * speed)); //生成位置の設定
                                NF.FH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.FH[NF.FH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L3.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                                                       //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - float.Parse(notesinfo[5]) * speed + songstartpos);  //ラインの長さを調べる
                                bpos = bpos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L3, front, bpos);
                                NF.H.Add(Instantiate(FrontHold, hold, Quaternion.identity));  //生成
                                NF.H[NF.H.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }
                        case "5"://左から4番目のレーン
                            {
                                pos = new Vector3(FLPX.L4, front, ((float.Parse(notesinfo[5]) + songstartpos) * speed)); //生成位置の設定
                                NF.FH.Add(Instantiate(FrontHNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.FH[NF.FH.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                HLT.L4.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                                                       //ホールドの始点と終点を結ぶラインの生成
                                bpos = NF.SH[NF.SH.Count - 1].transform.position.z; //始点の数だけ終点は存在するから[ ]内はあれでいい
                                bpos = Mathf.Abs(bpos - float.Parse(notesinfo[5]) * speed + songstartpos);  //ラインの長さを調べる
                                bpos = bpos / 2;
                                bpos = Mathf.Min(float.Parse(notesinfo[5]) * speed, NF.SH[NF.SH.Count - 1].transform.position.z) + bpos;
                                hold = new Vector3(FLPX.L4, front, bpos);
                                NF.H.Add(Instantiate(FrontHold, hold, Quaternion.identity));  //生成
                                NF.H[NF.H.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                zscale = Mathf.Abs((float.Parse(notesinfo[5]) + songstartpos) * speed - NF.SH[NF.SH.Count - 1].transform.position.z);  //スケールの設定
                                scale = new Vector3(scale.x, scale.y, zscale);
                                NF.H[NF.H.Count - 1].transform.localScale = scale; //設定した数値を代入
                                break;
                            }
                    }
                }

                // 横レーンの右回転ノーツの生成
                else if (notesinfo[0] == "4")
                {
                    // ノーツ数カウント
                    notescount++;
                    switch (notesinfo[1])
                    {
                        case "2": // 一番左のレーン
                            {
                                pos = new Vector3(FLPX.L1, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L1.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "3": //左から2つ目のレーン
                            {
                                pos = new Vector3(FLPX.L2, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L2.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "4"://左から3番目のレーン
                            {

                                pos = new Vector3(FLPX.L3, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L3.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "5"://左から4番目のレーン
                            {
                                pos = new Vector3(FLPX.L4, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L4.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                    }
                }

                // 横レーンの左回転ノーツの生成
                else if (notesinfo[0] == "5")
                {
                    // ノーツ数カウント
                    notescount++;
                    switch (notesinfo[1])
                    {
                        case "2": // 一番左のレーン
                            {
                                pos = new Vector3(FLPX.L1, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L1.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "3": //左から2つ目のレーン
                            {
                                pos = new Vector3(FLPX.L2, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L2.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "4"://左から3番目のレーン
                            {

                                pos = new Vector3(FLPX.L3, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L3.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                        case "5"://左から4番目のレーン
                            {
                                pos = new Vector3(FLPX.L4, front, (float.Parse(notesinfo[5]) + songstartpos) * speed); //生成位置の設定
                                NF.NN.Add(Instantiate(FrontNNotes, pos, Quaternion.identity)); //ノーツの生成
                                NF.NN[NF.NN.Count - 1].transform.parent = Notes.transform; //"Notes"の子にする
                                NLT.L4.Add(float.Parse(notesinfo[3])); //ノーツの押すタイミングを保存
                                break;
                            }
                    }
                }*/
            }

            lineCsv.Clear(); //また使うので中身をすべて消す
            

            //=====================================
            //            　その他
            //=====================================

            //ローディング画面の非表示
            LoadPanel.SetActive(false);


            //====================================================
            //       読み込んだ譜面データを各スクリプトへ渡す
            //====================================================

            //ノーマルノーツ
            NLT.L1.Sort(); NNL1 = NLT.L1;
            NLT.L2.Sort(); NNL2 = NLT.L2;
            NLT.L3.Sort(); NNL3 = NLT.L3;
            NLT.L4.Sort(); NNL4 = NLT.L4;

            //ホールド
            HLT.L1.Sort(); HNL1 = HLT.L1;
            HLT.L2.Sort(); HNL2 = HLT.L2;
            HLT.L3.Sort(); HNL3 = HLT.L3;
            HLT.L4.Sort(); HNL4 = HLT.L4;

            //カメラノーツ
            CNLT.L5.Sort(); CNL1 = CNLT.L5;
            CNLT.L6.Sort(); CNL2 = CNLT.L6;


            //事前にすることはすべて終わったのでノーツを流す
            Playing = true;
            Gate1 = true;
            Debug.Log("Gate1:" + Gate1);
        }));
    }

    //ファイルの読み込み、ノーツの設置、ゲームの開始まで行うまで停止
    IEnumerator Load(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
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

    public static float BPM(float bpm)
    {
        return bpm;
    }

    public void PlayMusic()
    {
        melody.Play();
        lane0.Play();
        lane1.Play();
        lane2.Play();
    }

    void NotesGenerate(Vector3 position, GameObject NotesObject)
    {
        NF.NN.Add(Instantiate(FrontNNotes, position, Quaternion.identity)); //ノーツの生成
        NF.NN[NF.NN.Count - 1].transform.parent = NotesObject.transform; //"Notes"の子にする
    }

    void StartHoldNotesGenerate(Vector3 position, GameObject NotesObject)
    {
        NF.SH.Add(Instantiate(FrontHNotes, position, Quaternion.identity)); //ノーツの生成
        NF.SH[NF.SH.Count - 1].transform.parent = NotesObject.transform; //"Notes"の子にする
    }

    void FinishHoldNotesGenerate(Vector3 position, GameObject NotesObject)
    {
        NF.FH.Add(Instantiate(FrontHNotes, position, Quaternion.identity)); //ノーツの生成
        NF.FH[NF.FH.Count - 1].transform.parent = NotesObject.transform; //"Notes"の子にする
    }

    void HoldNotesGenerate(Vector3 position, GameObject NotesObject)
    {
        NF.H.Add(Instantiate(FrontHold, position, Quaternion.identity)); //ノーツの生成
        NF.H[NF.H.Count - 1].transform.parent = NotesObject.transform; //"Notes"の子にする
    }

    void LeftCameraNotesGenerate(Vector3 position, GameObject NotesObject)
    {
        NF.CN.Add(Instantiate(LeftCameraNotes, position, Quaternion.identity)); //ノーツの生成
        NF.CN[NF.CN.Count - 1].transform.parent = NotesObject.transform; //"Notes"の子にする
    }

    void RightCameraNotesGenerate(Vector3 position, GameObject NotesObject)
    {
        NF.CN.Add(Instantiate(RightCameraNotes, position, Quaternion.identity)); //ノーツの生成
        NF.CN[NF.CN.Count - 1].transform.parent = NotesObject.transform; //"Notes"の子にする
        NF.CN[NF.CN.Count - 1].transform.rotation = new Quaternion(0, 180, 0, 0);
    }
}
