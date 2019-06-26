using System;
using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    bool Gate1;
    bool Gate2;
    bool OneTime;
    public static bool StartGate;

    // Use this for initialization
    void Start()
    {
        Gate1 = false;
        Gate2 = false;
        StartGate = false;
        OneTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        Gate1 = LoadData.Gate1;
        Gate2 = NotesJudge.Gate2;
        //すべての読み込みが完了したら
        if (Gate1 && Gate2)
        {
            StartCoroutine(GameStart(3f, () =>
            {
                if (!OneTime)
                {
                    Debug.Log("Start");
                    StartGate = true;
                    OneTime = true;
                }
            }));
        }
    }
    IEnumerator GameStart(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
