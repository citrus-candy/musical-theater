using UnityEngine;

public class MusicController : MonoBehaviour
{
    public GameObject melody;
    public GameObject lane0;
    public GameObject lane1;
    public GameObject lane2;

    public LineIns LineIns;

    private bool mainFlag = false;
    private bool pianoFlag = false;
    private bool picopicoFlag = false;


    public LoadData LoadData;

    void Update()
    {
        int turnCount = LineIns.turnCount;

        if(turnCount % 3 == 0 && !mainFlag)
        {
            lane0.GetComponent<AudioSource>().mute = false;
            lane1.GetComponent<AudioSource>().mute = true;
            lane2.GetComponent<AudioSource>().mute = true;

            mainFlag = true;
            pianoFlag = false;
            picopicoFlag = false;
        }
        else if (turnCount % 3 == 1 && !pianoFlag)
        {
            lane0.GetComponent<AudioSource>().mute = true;
            lane1.GetComponent<AudioSource>().mute = false;
            lane2.GetComponent<AudioSource>().mute = true;

            mainFlag = false;
            pianoFlag = true;
            picopicoFlag = false;
        }
        else if (turnCount % 3 == 2 && !picopicoFlag)
        {
            lane0.GetComponent<AudioSource>().mute = true;
            lane1.GetComponent<AudioSource>().mute = true;
            lane2.GetComponent<AudioSource>().mute = false;

            mainFlag = false;
            pianoFlag = false;
            picopicoFlag = true;
        }
    }

    public void AudioPlay()
    {
        melody.GetComponent<AudioSource>().Play();
        lane0.GetComponent<AudioSource>().Play();
        lane1.GetComponent<AudioSource>().Play();
        lane2.GetComponent<AudioSource>().Play(); ;
    }
}
