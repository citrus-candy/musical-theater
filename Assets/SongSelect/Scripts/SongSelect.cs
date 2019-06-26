using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour {

    List<GameObject> SongSelectButton = new List<GameObject>();
    List<string> infoCsv = new List<string>();

    public Book Book;
    public Texture2D BookCover;
    public Texture2D BookBackCover;

    public Texture2D[] Thumbnail;
    public string[] SongName;
    public string[] Composer;
    public string[] BPM;
    public string[] Lv;

    public GameObject LeftMusicInfo;
    public GameObject LeftThum;
    public GameObject LeftSongName;
    public GameObject LeftComposer;
    public GameObject LeftBPM;
    public GameObject LeftLv;

    public GameObject RightMusicInfo;
    public GameObject RightThum;
    public GameObject RightSongName;
    public GameObject RightComposer;
    public GameObject RightBPM;
    public GameObject RightLv;

    public GameObject[] ThumObject;
    public GameObject[] SongNameObject;
    public GameObject[] ComposerObject;
    public GameObject[] BPMObject;
    public GameObject[] LvObject;

    public string[] SongFolders;
    string[] _songfilename;
    public static float speed = 10;

    public PageSelect PageSelect;

    string pathname;
    string csvTextLine;

    void Start()
    {
        pathname = Path.Combine(Environment.CurrentDirectory, "Songs");  // \Songs\

        // このソフトの存在するフォルダの"Songs"の中にあるフォルダの名前すべてをSongFoldersに保存
        SongFolders = Directory.GetDirectories(pathname, "*");
        _songfilename = new string[SongFolders.Length];
        PageSelect.songLength = SongFolders.Length;

        // 本のページ数
        int bookPagesArraySize;
        if (SongFolders.Length % 2 == 1)
        {
            bookPagesArraySize = SongFolders.Length + 1;
        }
        else
        {
            bookPagesArraySize = SongFolders.Length;
        }
        Array.Resize(ref Book.bookPages, bookPagesArraySize + 2);  // ページ数の配列のリサイズ
        Book.bookPages[0] = Sprite.Create(BookCover, new Rect(0, 0, BookCover.width, BookCover.height), Vector2.zero);  // 表紙
        if (SongFolders.Length % 2 == 1)
        {
            Book.bookPages[SongFolders.Length + 2] = Sprite.Create(BookBackCover, new Rect(0, 0, BookCover.width, BookCover.height), Vector2.zero);  // 裏表紙
        }
        else
        {
            Book.bookPages[SongFolders.Length + 1] = Sprite.Create(BookBackCover, new Rect(0, 0, BookCover.width, BookCover.height), Vector2.zero);  // 裏表紙
        }

        // 配列の要素を決める
        Thumbnail = new Texture2D[PageSelect.songLength];
        SongName = new string[PageSelect.songLength];
        Composer = new string[PageSelect.songLength];
        BPM = new string[PageSelect.songLength];
        Lv = new string[PageSelect.songLength];


        ThumObject = new GameObject[PageSelect.songLength];
        SongNameObject = new GameObject[PageSelect.songLength];
        ComposerObject = new GameObject[PageSelect.songLength];
        BPMObject = new GameObject[PageSelect.songLength];
        LvObject = new GameObject[PageSelect.songLength];

        // Info.csvの情報を配列に格納
        for (int i = 0; i < PageSelect.songLength; i++) { 
            string songPath = Path.Combine(pathname, SongFolders[i]);  // \Songs\SongFolders[i]\
            string infoPath = Path.Combine(songPath, "Info.csv");  // \Songs\SongFolders[i]\Info.csv

            StreamReader streamReader = new StreamReader(infoPath);
            csvTextLine = streamReader.ReadLine();

            while (csvTextLine != null)
            {
                infoCsv.Add(csvTextLine);
                csvTextLine = streamReader.ReadLine();               
            }
            var ThumPath = Path.Combine(pathname, SongFolders[i]);  // \Songs\SongFolders[i]
            ThumPath = Path.Combine(ThumPath, "Thum.jpg");  // \Songs\SongFlolders[i]\Thum.jpg
            Thumbnail[i] = ReadTexture(ThumPath, 150, 150) as Texture2D;

            Debug.Log(PageSelect.songLength);
            
            // 曲名
            SongName[i] = infoCsv[6];
            
            // Composer
            Composer[i] = infoCsv[4];

            // BPM
            BPM[i] = "BPM:" + infoCsv[8];

            // LV
            Lv[i] = "Lv:" + infoCsv[15];
            
            infoCsv.Clear();

            // LeftMusicInfo
            if(i % 2 == 0)
            {
                GameObject LeftThumInstantiate = Instantiate(LeftThum, LeftMusicInfo.transform);
                ThumObject[i] = LeftThumInstantiate;
                LeftThumInstantiate.name = "LeftThumInstantiate" + i.ToString();
                LeftThumInstantiate.GetComponent<Image>().sprite = Sprite.Create(Thumbnail[i], new Rect(0, 0, Thumbnail[i].width, Thumbnail[i].height), Vector2.zero);

                GameObject LeftSongNameInstantiate = Instantiate(LeftSongName, LeftMusicInfo.transform);
                SongNameObject[i] = LeftSongNameInstantiate;
                LeftSongNameInstantiate.name = "LeftSongNameInstantiate" + i.ToString();
                LeftSongNameInstantiate.GetComponent<TextMeshProUGUI>().text = SongName[i];

                GameObject LeftComposerInstantiate = Instantiate(LeftComposer, LeftMusicInfo.transform);
                ComposerObject[i] = LeftComposerInstantiate;
                LeftComposerInstantiate.name = "LeftComposerInstantiate" + i.ToString();
                LeftComposerInstantiate.GetComponent<TextMeshProUGUI>().text = Composer[i];

                GameObject LeftBPMInstantiate = Instantiate(LeftBPM, LeftMusicInfo.transform);
                BPMObject[i] = LeftBPMInstantiate;
                LeftBPMInstantiate.name = "LeftBPMInstantiate" + i.ToString();
                LeftBPMInstantiate.GetComponent<TextMeshProUGUI>().text = BPM[i];

                GameObject LeftLvInstantiate = Instantiate(LeftLv, LeftMusicInfo.transform);
                LvObject[i] = LeftLvInstantiate;
                LeftLvInstantiate.name = "LeftLvInstantiate" + i.ToString();
                LeftLvInstantiate.GetComponent<TextMeshProUGUI>().text = Lv[i];
            }
            // RightMusicInfo
            else if (i % 2 == 1)
            {
                GameObject RightThumInstantiate = Instantiate(RightThum, RightMusicInfo.transform);
                ThumObject[i] = RightThumInstantiate;
                RightThumInstantiate.name = "RIghtThumInstantiate" + i.ToString();
                RightThumInstantiate.GetComponent<Image>().sprite = Sprite.Create(Thumbnail[i], new Rect(0, 0, Thumbnail[i].width, Thumbnail[i].height), Vector2.zero);

                GameObject RightSongNameInstantiate = Instantiate(RightSongName, RightMusicInfo.transform);
                SongNameObject[i] = RightSongNameInstantiate;
                RightSongNameInstantiate.name = "RightSongNameInstantiate" + i.ToString();
                RightSongNameInstantiate.GetComponent<TextMeshProUGUI>().text = SongName[i];

                GameObject RightComposerInstantiate = Instantiate(RightComposer, RightMusicInfo.transform);
                ComposerObject[i] = RightComposerInstantiate;
                RightComposerInstantiate.name = "RightComposerInstantiate" + i.ToString();
                RightComposerInstantiate.GetComponent<TextMeshProUGUI>().text = Composer[i];

                GameObject RightBPMInstantiate = Instantiate(RightBPM, RightMusicInfo.transform);
                BPMObject[i] = RightBPMInstantiate;
                RightBPMInstantiate.name = "RightBPMInstantiate" + i.ToString();
                RightBPMInstantiate.GetComponent<TextMeshProUGUI>().text = BPM[i];

                GameObject RightLvInstantiate = Instantiate(RightLv, RightMusicInfo.transform);
                LvObject[i] = RightLvInstantiate;
                RightLvInstantiate.name = "RightLvInstantiate" + i.ToString();
                RightLvInstantiate.GetComponent<TextMeshProUGUI>().text = Lv[i];
            }
        }


    }

    void Update()
    {
        if (PageSelect.cursor > 0)
        {
            /*
            if (PageSelect.cursor == 2 * PageSelect.cursor - 1 && PageSelect.cursor != PageSelect.songLength + 1)
            {
                for (int i = -PageSelect.songLength; i < PageSelect.songLength; i++)
                {
                    ObjectActive(PageSelect.songLength + i, true);
                }
                ObjectActive(PageSelect.songLength, true);
            }*/
            if (PageSelect.cursor == 1  || PageSelect.cursor == 2 && PageSelect.cursor != PageSelect.songLength + 1)
            {
                ObjectActive(0, true);
                ObjectActive(1, true);
                ObjectActive(2, false);
            }
            else if (PageSelect.cursor == 3 && PageSelect.cursor != PageSelect.songLength + 1)
            {
                ObjectActive(0, false);
                ObjectActive(1, false);
                ObjectActive(2, true);
            }
            else if (PageSelect.cursor != PageSelect.songLength + 1)
            {
                ObjectActive(0, false);
                ObjectActive(1, false);
                ObjectActive(2, false);
            }
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

    void ObjectActive(int cursor, bool Flag)
    {
        ThumObject[cursor].SetActive(Flag);
        SongNameObject[cursor].SetActive(Flag);
        ComposerObject[cursor].SetActive(Flag);
        BPMObject[cursor].SetActive(Flag);
        LvObject[cursor].SetActive(Flag);
    }
}
