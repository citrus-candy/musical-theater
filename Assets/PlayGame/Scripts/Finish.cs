using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    float finish,timer;
    bool StartGate;

    void Start()
    {
        timer = 0;
        StartGate = false;
    }

    void Update()
    {
        StartGate = Gate.StartGate;
        if (StartGate)
        {
            finish = LoadData.finishnotes;
            timer += Time.deltaTime;
            //最後のノーツが来てからｘ秒後
            if(timer - finish >  5)
            {
                SceneManager.LoadScene("Result");
            }
        }
    }
}
