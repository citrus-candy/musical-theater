using System.Collections.Generic;
using UnityEngine;

public class NotesJudge : MonoBehaviour
{
    public List<float> NLine;
    public List<float> HLine;
    public List<float> CNLine;

    //判定結果
    public static int perfect;
    public static int good;
    public static int ok;
    public static int miss;
    float score_perfect = 1f;
    float score_good = 0.7f;
    float score_ok = 0.5f;

    //カウント用変数
    private int count_H;
    private int count_N;
    private int count_CN;

    //コンボ用変数
    //public Combo _Combo;
    public GameObject Perfect;
    public GameObject Good;
    public GameObject OK;
    public GameObject Miss;

    public Combo Combo;
    public LineIns LineIns;

    float combotime,time,score,allscore;
    static float lifetime = 1.5f;

    public int combo = 0;

    //スコアをリザルトに持ってく
    public static string _Score;

    class Judge
    {
        public float P = 0.1f;
        public float G = 0.125f;
        public float O = 0.15f;
        public float M = 0.15f;
    }
    Judge J = new Judge();

    class Count
    {
        public int H = 0;
        public int HEX = 0;
        public int S = 0;
    }
    Count C = new Count();

    float timer, offset,onenotesscore,notescount;
    bool IsPlaying;
    bool StartGate;

    //読み込み完了を確認するための変数
    public static bool Gate2;

    public WebCamController2 WebCamController2;

    //初期化
    void Start()
    {
        J = new Judge();
        C = new Count();

        timer = C.S;

        IsPlaying = false;
        Gate2 = false;
        StartGate = false;

        _Score = "0";
        onenotesscore = allscore = score = time = combotime = perfect = good = ok = miss = count_N = count_H = 0;

        NLine.Clear();
        HLine.Clear();
        CNLine.Clear();

        //GameObject ComboText = GameObject.Find("Combo");
       // Combo = ComboText.AddComponent(typeof(Combo)) as Combo;
    }

    void Update()
    {
        StartGate = Gate.StartGate;

        //=============================
        //      ロードが完了するまで
        //=============================
        if (!IsPlaying)
        {
            if (LoadData.Playing)
            {
                //アタッチしたオブジェクトによって読み込む値を変更
                switch (gameObject.name)
                {
                    case "Lane1":
                        NLine = LoadData.NNL1;
                        HLine = LoadData.HNL1;
                        CNLine = LoadData.CNL1;
                        break;
                    case "Lane2":
                        NLine = LoadData.NNL2;
                        HLine = LoadData.HNL2;
                        CNLine = LoadData.CNL2;
                        break;
                    case "Lane3":
                        NLine = LoadData.NNL3;
                        HLine = LoadData.HNL3;
                        break;
                    case "Lane4":
                        NLine = LoadData.NNL4;
                        HLine = LoadData.HNL4;
                        break;
                }
                notescount = LoadData.notescount;
                onenotesscore = 1000000 / notescount;
                IsPlaying = true;
                Gate2 = true;
                Debug.Log("Gate2:" + Gate2);
            }
        }


        //=======================================
        //      ロードが完了した後(判定に入る)
        //=======================================
        else
        {
            if (StartGate == true)
            {
                //  Debug.Log(gameObject.name +","+  count_N);
                //タイマーの更新
                timer += Time.deltaTime;
                //ノーツの判定
                switch (gameObject.name)
                {
                    case "Lane1":
                        //ノーマルノーツの判定
                        if (count_N < NLine.Count)
                        {
                            if (timer - NLine[count_N] - offset >= -J.P && timer - NLine[count_N] - offset <= J.P)
                            {
                                if (Input.GetKeyDown(KeyCode.D))
                                {
                                    count_N++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.G && timer - NLine[count_N] - offset <= J.G)
                            {
                                if (Input.GetKeyDown(KeyCode.D))
                                {
                                    count_N++;
                                    good++;
                                    Combo.ComboCount(1);
                                    _Good();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.O && timer - NLine[count_N] - offset <= J.O)
                            {
                                if (Input.GetKeyDown(KeyCode.D))
                                {
                                    count_N++;
                                    ok++;
                                    Combo.ComboCount(1);
                                    _OK();
                                }
                            }
                            else if (timer - NLine[count_N] - offset > J.M)
                            {
                                count_N++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }

                        //ホールドノーツの判定
                        if (count_H < HLine.Count)
                        {
                            if (count_H % 2 == 0)  //ホールドの始点
                            {
                                if (timer - HLine[count_H] - offset >= -J.P && timer - HLine[count_H] - offset <= J.P)
                                {
                                    if (Input.GetKeyDown(KeyCode.D))
                                    {
                                        count_H++;
                                        perfect++;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.G && timer - HLine[count_H] - offset <= J.G)
                                {
                                    if (Input.GetKeyDown(KeyCode.D))
                                    {
                                        count_H++;
                                        good++;
                                        Combo.ComboCount(1);
                                        _Good();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.O && timer - HLine[count_H] - offset <= J.O)
                                {
                                    if (Input.GetKeyDown(KeyCode.D))
                                    {
                                        count_H++;
                                        ok++;
                                        Combo.ComboCount(1);
                                        _OK();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset > J.M)
                                {
                                    count_H++;
                                    miss++;
                                    Combo.ComboCount(0);
                                    _Miss();
                                    C.H = 1;
                                }
                            }
                            else  //ホールドの終点
                            {
                                if (HLine[count_H] + offset - timer >= 0.1f) //終点に来るまで
                                {
                                    if (C.H == 0)
                                    {
                                        if (Input.GetKeyUp(KeyCode.D))
                                        {
                                            miss++;
                                            C.H = 1;
                                            Combo.ComboCount(0);
                                            _Miss();
                                        }
                                    }
                                }
                                else  //終点に来たとき
                                {
                                    if (C.H == 0)  //ボタンを押していた
                                    {
                                        count_H++;
                                        perfect++;
                                        C.H = 0;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                    else  //ボタンを離していた
                                    {
                                        count_H++;
                                        C.H = 0;
                                    }
                                }
                            }
                        }

                        //カメラノーツの判定
                        if (count_CN < CNLine.Count)
                        {
                            if (timer - CNLine[count_CN] - offset >= -J.P - J.G - J.O && timer - CNLine[count_CN] - offset <= J.P + J.G + J.O)
                            {
                                if (Input.GetKeyDown(KeyCode.N) || WebCamController2.camera1Flag)
                                {
                                    count_CN++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }

                                /*if (timer - CNLine[count_CN] - offset >= -J.P + 0.09f && timer - CNLine[count_CN] - offset <= J.P - 0.09f)
                                {
                                    count_CN++;
                                    LineIns.LeftTurn();
                                }*/
                            }                          
                            else if (timer - CNLine[count_CN] - offset > J.M)
                            {
                                count_CN++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }
                        break;

                    case "Lane2":
                        //ノーマルノーツの判定
                        if (count_N < NLine.Count)
                        {
                            if (timer - NLine[count_N] - offset >= -J.P && timer - NLine[count_N] - offset <= J.P)
                            {
                                if (Input.GetKeyDown(KeyCode.F))
                                {
                                    count_N++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.G && timer - NLine[count_N] - offset <= J.G)
                            {
                                if (Input.GetKeyDown(KeyCode.F))
                                {
                                    count_N++;
                                    good++;
                                    Combo.ComboCount(1);
                                    _Good();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.O && timer - NLine[count_N] - offset <= J.O)
                            {
                                if (Input.GetKeyDown(KeyCode.F))
                                {
                                    count_N++;
                                    ok++;
                                    Combo.ComboCount(1);
                                    _OK();
                                }
                            }
                            else if (timer - NLine[count_N] - offset > J.M)
                            {
                                count_N++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }

                        //ホールドノーツの判定
                        if (count_H < HLine.Count)
                        {
                            if (count_H % 2 == 0)  //ホールドの始点
                            {
                                if (timer - HLine[count_H] - offset >= -J.P && timer - HLine[count_H] - offset <= J.P)
                                {
                                    if (Input.GetKeyDown(KeyCode.F))
                                    {
                                        count_H++;
                                        perfect++;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.G && timer - HLine[count_H] - offset <= J.G)
                                {
                                    if (Input.GetKeyDown(KeyCode.F))
                                    {
                                        count_H++;
                                        good++;
                                        Combo.ComboCount(1);
                                        _Good();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.O && timer - HLine[count_H] - offset <= J.O)
                                {
                                    if (Input.GetKeyDown(KeyCode.F))
                                    {
                                        count_H++;
                                        ok++;
                                        Combo.ComboCount(1);
                                        _OK();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset > J.M)
                                {
                                    count_H++;
                                    miss++;
                                    Combo.ComboCount(0);
                                    _Miss();
                                    C.H = 1;
                                }
                            }
                            else  //ホールドの終点
                            {
                                if (HLine[count_H] + offset - timer >= 0.1f) //終点に来るまで
                                {
                                    if (C.H == 0)
                                    {
                                        if (Input.GetKeyUp(KeyCode.F))
                                        {
                                            miss++;
                                            C.H = 1;
                                            Combo.ComboCount(0);
                                            _Miss();
                                        }
                                    }
                                }
                                else  //終点に来たとき
                                {
                                    if (C.H == 0)  //ボタンを押していた
                                    {
                                        count_H++;
                                        perfect++;
                                        C.H = 0;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                    else  //ボタンを離していた
                                    {
                                        count_H++;
                                        C.H = 0;
                                    }
                                }
                            }
                        }

                        //カメラノーツの判定
                        if (count_CN < CNLine.Count)
                        {
                            if (timer - CNLine[count_CN] - offset >= -J.P - J.G - J.O && timer - CNLine[count_CN] - offset <= J.P + J.G + J.O)
                            {
                                if (Input.GetKeyDown(KeyCode.V) || WebCamController2.camera2Flag)
                                {
                                    count_CN++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }
                                /*if (timer - CNLine[count_CN] - offset >= -J.P + 0.09f && timer - CNLine[count_CN] - offset <= J.P - 0.09f)
                                {
                                    LineIns.RightTurn();
                                }*/
                            }
                            else if (timer - CNLine[count_CN] - offset > J.M)
                            {
                                count_CN++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }
                        break;

                    case "Lane3":
                        //ノーマルノーツの判定
                        if (count_N < NLine.Count)
                        {
                            if (timer - NLine[count_N] - offset >= -J.P && timer - NLine[count_N] - offset <= J.P)
                            {
                                if (Input.GetKeyDown(KeyCode.J))
                                {
                                    count_N++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.G && timer - NLine[count_N] - offset <= J.G)
                            {
                                if (Input.GetKeyDown(KeyCode.J))
                                {
                                    count_N++;
                                    good++;
                                    Combo.ComboCount(1);
                                    _Good();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.O && timer - NLine[count_N] - offset <= J.O)
                            {
                                if (Input.GetKeyDown(KeyCode.J))
                                {
                                    count_N++;
                                    ok++;
                                    Combo.ComboCount(1);
                                    _OK();
                                }
                            }
                            else if (timer - NLine[count_N] - offset > J.M)
                            {
                                count_N++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }

                        //ホールドノーツの判定
                        if (count_H < HLine.Count)
                        {
                            if (count_H % 2 == 0)  //ホールドの始点
                            {
                                if (timer - HLine[count_H] - offset >= -J.P && timer - HLine[count_H] - offset <= J.P)
                                {
                                    if (Input.GetKeyDown(KeyCode.J))
                                    {
                                        count_H++;
                                        perfect++;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.G && timer - HLine[count_H] - offset <= J.G)
                                {
                                    if (Input.GetKeyDown(KeyCode.J))
                                    {
                                        count_H++;
                                        good++;
                                        Combo.ComboCount(1);
                                        _Good();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.O && timer - HLine[count_H] - offset <= J.O)
                                {
                                    if (Input.GetKeyDown(KeyCode.J))
                                    {
                                        count_H++;
                                        ok++;
                                        Combo.ComboCount(1);
                                        _OK();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset > J.M)
                                {
                                    count_H++;
                                    miss++;
                                    Combo.ComboCount(0);
                                    _Miss();
                                    C.H = 1;
                                }
                            }
                            else  //ホールドの終点
                            {
                                if (HLine[count_H] + offset - timer >= 0.1f) //終点に来るまで
                                {
                                    if (C.H == 0)
                                    {
                                        if (Input.GetKeyUp(KeyCode.J))
                                        {
                                            miss++;
                                            C.H = 1;
                                            Combo.ComboCount(0);
                                            _Miss();
                                        }
                                    }
                                }
                                else  //終点に来たとき
                                {
                                    if (C.H == 0)  //ボタンを押していた
                                    {
                                        count_H++;
                                        perfect++;
                                        C.H = 0;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                    else  //ボタンを離していた
                                    {
                                        count_H++;
                                        C.H = 0;
                                    }
                                }
                            }
                        }
                        break;

                    case "Lane4":
                        //ノーマルノーツの判定
                        if (count_N < NLine.Count)
                        {
                            if (timer - NLine[count_N] - offset >= -J.P && timer - NLine[count_N] - offset <= J.P)
                            {
                                if (Input.GetKeyDown(KeyCode.K))
                                {
                                    count_N++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.G && timer - NLine[count_N] - offset <= J.G)
                            {
                                if (Input.GetKeyDown(KeyCode.K))
                                {
                                    count_N++;
                                    good++;
                                    Combo.ComboCount(1);
                                    _Good();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.O && timer - NLine[count_N] - offset <= J.O)
                            {
                                if (Input.GetKeyDown(KeyCode.K))
                                {
                                    count_N++;
                                    ok++;
                                    Combo.ComboCount(1);
                                    _OK();
                                }
                            }
                            else if (timer - NLine[count_N] - offset > J.M)
                            {
                                count_N++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }

                        //ホールドノーツの判定
                        if (count_H < HLine.Count)
                        {
                            if (count_H % 2 == 0)  //ホールドの始点
                            {
                                if (timer - HLine[count_H] - offset >= -J.P && timer - HLine[count_H] - offset <= J.P)
                                {
                                    if (Input.GetKeyDown(KeyCode.K))
                                    {
                                        count_H++;
                                        perfect++;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.G && timer - HLine[count_H] - offset <= J.G)
                                {
                                    if (Input.GetKeyDown(KeyCode.K))
                                    {
                                        count_H++;
                                        good++;
                                        Combo.ComboCount(1);
                                        _Good();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.O && timer - HLine[count_H] - offset <= J.O)
                                {
                                    if (Input.GetKeyDown(KeyCode.K))
                                    {
                                        count_H++;
                                        ok++;
                                        Combo.ComboCount(1);
                                        _OK();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset > J.M)
                                {
                                    count_H++;
                                    miss++;
                                    C.H = 1;
                                    Combo.ComboCount(0);
                                    _Miss();
                                }
                            }
                            else  //ホールドの終点
                            {
                                if (HLine[count_H] + offset - timer >= 0.1f) //終点に来るまで
                                {
                                    if (C.H == 0)
                                    {
                                        if (Input.GetKeyUp(KeyCode.K))
                                        {
                                            miss++;
                                            C.H = 1;
                                            Combo.ComboCount(0);
                                            _Miss();
                                        }
                                    }
                                }
                                else  //終点に来たとき
                                {
                                    if (C.H == 0)  //ボタンを押していた
                                    {
                                        count_H++;
                                        perfect++;
                                        C.H = 0;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                    else  //ボタンを離していた
                                    {
                                        count_H++;
                                        C.H = 0;
                                    }
                                }
                            }
                        }
                        break;
                        /*
                    case "Lane5":
                        //ノーマルノーツの判定
                        if (count_N < NLine.Count)
                        {
                            if (timer - NLine[count_N] - offset >= -J.P && timer - NLine[count_N] - offset <= J.P)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    count_N++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.G && timer - NLine[count_N] - offset <= J.G)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    count_N++;
                                    good++;
                                    Combo.ComboCount(1);
                                    _Good();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.O && timer - NLine[count_N] - offset <= J.O)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    count_N++;
                                    ok++;
                                    Combo.ComboCount(1);
                                    _OK();
                                }
                            }
                            else if (timer - NLine[count_N] - offset > J.M)
                            {
                                count_N++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }

                        //ホールドノーツの判定
                        if (count_H < HLine.Count)
                        {
                            if (count_H % 2 == 0)  //ホールドの始点
                            {
                                if (timer - HLine[count_H] - offset >= -J.P && timer - HLine[count_H] - offset <= J.P)
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        count_H++;
                                        perfect++;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.G && timer - HLine[count_H] - offset <= J.G)
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        count_H++;
                                        good++;
                                        Combo.ComboCount(1);
                                        _Good();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.O && timer - HLine[count_H] - offset <= J.O)
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        count_H++;
                                        ok++;
                                        Combo.ComboCount(1);
                                        _OK();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset > J.M)
                                {
                                    count_H++;
                                    miss++;
                                    Combo.ComboCount(0);
                                    _Miss();
                                    C.H = 1;
                                }
                            }
                            else  //ホールドの終点
                            {
                                if (HLine[count_H] + offset - timer >= 0.1f) //終点に来るまで
                                {
                                    if (C.H == 0)
                                    {
                                        if (Input.GetMouseButtonUp(0))
                                        {
                                            miss++;
                                            C.H = 1;
                                            Combo.ComboCount(0);
                                            _Miss();
                                        }
                                    }
                                }
                                else  //終点に来たとき
                                {
                                    if (C.H == 0)  //ボタンを押していた
                                    {
                                        count_H++;
                                        perfect++;
                                        C.H = 0;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                    else  //ボタンを離していた
                                    {
                                        count_H++;
                                        C.H = 0;
                                    }
                                }
                            }
                        }
                        break;

                    case "Lane6":
                        //ノーマルノーツの判定
                        if (count_N < NLine.Count)
                        {
                            if (timer - NLine[count_N] - offset >= -J.P && timer - NLine[count_N] - offset <= J.P)
                            {
                                if (Input.GetMouseButtonDown(1))
                                {
                                    count_N++;
                                    perfect++;
                                    Combo.ComboCount(1);
                                    _Perfect();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.G && timer - NLine[count_N] - offset <= J.G)
                            {
                                if (Input.GetMouseButtonDown(1))
                                {
                                    count_N++;
                                    good++;
                                    Combo.ComboCount(1);
                                    _Good();
                                }
                            }
                            else if (timer - NLine[count_N] - offset >= -J.O && timer - NLine[count_N] - offset <= J.O)
                            {
                                if (Input.GetMouseButtonDown(1))
                                {
                                    count_N++;
                                    ok++;
                                    Combo.ComboCount(1);
                                    _OK();
                                }
                            }
                            else if (timer - NLine[count_N] - offset > J.M)
                            {
                                count_N++;
                                miss++;
                                Combo.ComboCount(0);
                                _Miss();
                            }
                        }

                        //ホールドノーツの判定
                        if (count_H < HLine.Count)
                        {
                            if (count_H % 2 == 0)  //ホールドの始点
                            {
                                if (timer - HLine[count_H] - offset >= -J.P && timer - HLine[count_H] - offset <= J.P)
                                {
                                    if (Input.GetMouseButtonDown(1))
                                    {
                                        count_H++;
                                        perfect++;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.G && timer - HLine[count_H] - offset <= J.G)
                                {
                                    if (Input.GetMouseButtonDown(1))
                                    {
                                        count_H++;
                                        good++;
                                        Combo.ComboCount(1);
                                        _Good();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset >= -J.O && timer - HLine[count_H] - offset <= J.O)
                                {
                                    if (Input.GetMouseButtonDown(1))
                                    {
                                        count_H++;
                                        ok++;
                                        Combo.ComboCount(1);
                                        _OK();
                                    }
                                }
                                else if (timer - HLine[count_H] - offset > J.M)
                                {
                                    count_H++;
                                    miss++;
                                    Combo.ComboCount(0);
                                    _Miss();
                                    C.H = 1;
                                }
                            }
                            else  //ホールドの終点
                            {
                                if (HLine[count_H] + offset - timer >= 0.1f) //終点に来るまで
                                {
                                    if (C.H == 0)
                                    {
                                        if (Input.GetMouseButtonUp(1))
                                        {
                                            miss++;
                                            C.H = 1;
                                            Combo.ComboCount(0);
                                            _Miss();
                                        }
                                    }
                                }
                                else  //終点に来たとき
                                {
                                    if (C.H == 0)  //ボタンを押していた
                                    {
                                        count_H++;
                                        perfect++;
                                        C.H = 0;
                                        Combo.ComboCount(1);
                                        _Perfect();
                                    }
                                    else  //ボタンを離していた
                                    {
                                        count_H++;
                                        C.H = 0;
                                    }
                                }
                            }
                        }
                        break;*/
                }
            }
        }
       // _Score = ComboCount.text;
    }
    
    void _Perfect()
    {
        Good.SetActive(false);
        OK.SetActive(false);
        Miss.SetActive(false);
        Perfect.SetActive(true);
        //score = score_perfect;
        allscore = score * onenotesscore;
        /* ComboCount.text = (float.Parse(ComboCount.text) + allscore).ToString("F0");
         if (float.Parse(ComboCount.text) < 1000009 && float.Parse(ComboCount.text) > 999991f)
         {
             ComboCount.text = "1000000";
         }*/

        Debug.Log("Perfect");
    }

    void _Good()
    {
        Perfect.SetActive(false);
        OK.SetActive(false);
        Miss.SetActive(false);
        Good.SetActive(true);
     //   score =  score_good;
        allscore = score * onenotesscore;
        /*  ComboCount.text = (float.Parse(ComboCount.text) + allscore).ToString("F0");
          if (float.Parse(ComboCount.text) < 1000009 && float.Parse(ComboCount.text) > 999991f)
          {
              ComboCount.text = "1000000";
          }*/
        Debug.Log("Good");
    }

    void _OK()
    {
        Good.SetActive(false);
        Perfect.SetActive(false);
        Miss.SetActive(false);
        OK.SetActive(true);
      //  score = score_ok;
        allscore = score * onenotesscore;
        /* ComboCount.text = (float.Parse(ComboCount.text) + allscore).ToString("F0");
         if (float.Parse(ComboCount.text) < 1000009 && float.Parse(ComboCount.text) > 999991f)
         {
             ComboCount.text = "1000000";
         }*/
        Debug.Log("OK");
    }

    void _Miss()
    {
         Good.SetActive(false);
         Perfect.SetActive(false);
         OK.SetActive(false);
         Miss.SetActive(true);
        Debug.Log("Miss");
    }
}