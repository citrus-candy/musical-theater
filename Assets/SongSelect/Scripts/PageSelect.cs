using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelect : MonoBehaviour
{
    public AutoFlip AutoFlip;

    public SongSelect SongSelect;
    public int songLength;

    public GameObject LeftMusicInfo;
    public GameObject RightMusicInfo;

    public GameObject LeftCursor;
    public GameObject RightCursor;

    public static string toPlayGameSongName;

    public int cursor = 0;
    bool cursorFlag = false;
    int page = 0;
    bool pageFlag = false;
    float timeCount = 1f;

    void Update()
    {
        //Debug.Log(cursor);

        timeCount -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.K) && timeCount <= 0)
        {
            
            if (!cursorFlag)
            {
                AutoFlip.FlipRightPage();
                cursor++;
                cursorFlag = true;
                timeCount = 1f;
            }
            else
            {                
                cursor++;
                cursorFlag = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.D) && timeCount <= 0)
        {
            
            if (!cursorFlag)
            {
                cursor--;
                cursorFlag = true;
            }
            else
            {
                AutoFlip.FlipLeftPage();
                cursor--;
                cursorFlag = false;
                timeCount = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            toPlayGameSongName = SongSelect.SongName[cursor - 1];
            SceneManager.LoadScene("PlayGame");
        }

        int pageLength = songLength + 1;

        if (cursor > pageLength)
        {
            cursor = pageLength + 1;
        }
        else if (cursor <= 0)
        {
            cursor = 0;
        }

        if (cursor == 0 && cursor != pageLength || cursor == 4 || cursor == 5)
        {
            LeftMusicInfo.SetActive(false);
            RightMusicInfo.SetActive(false);
            LeftCursor.SetActive(false);
            RightCursor.SetActive(false);         
        }
        else if(cursor % 2 == 1 && cursor != pageLength)
        {
            LeftMusicInfo.SetActive(true);
            RightMusicInfo.SetActive(true);
            LeftCursor.SetActive(true);          
            RightCursor.SetActive(false);          
        }
        else if (cursor % 2 == 0 && cursor != pageLength)
        {
            LeftMusicInfo.SetActive(true);
            RightMusicInfo.SetActive(true);
            LeftCursor.SetActive(false);
            RightCursor.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Escape)) Quit();
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }

}
