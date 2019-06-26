using UnityEngine;

public class NotesPlay : MonoBehaviour
{
    bool StartGate, MusicGate;

    public GameObject Lane0MoveObjects;
    public GameObject Lane1MoveObjects;
    public GameObject Lane2MoveObjects;
    public GameObject EndPositon1;
    public GameObject EndPositon2;
    public GameObject EndPositon3;

    public LoadData LoadData;
    public LineIns LineIns;


    public WebCamController2 WebCamController2;
    void Update()
    {
        StartGate = Gate.StartGate;

        if (LoadData.Playing)
        {
            if (StartGate)
            {
                if (!MusicGate)
                {
                    //WebCamController2.ImportOffsetColor();
                    LoadData.PlayMusic();                   
                    MusicGate = true;
                }

                var campos = LoadData.bpm / 60f;
                campos = 1.5f * campos;

                var endposition0 = new Vector3(EndPositon1.transform.position.x, Lane0MoveObjects.transform.position.y, EndPositon1.transform.position.z);
                var endposition1 = new Vector3(EndPositon2.transform.position.x, Lane1MoveObjects.transform.position.y, EndPositon2.transform.position.z);
                var endposition2 = new Vector3(EndPositon3.transform.position.x, Lane2MoveObjects.transform.position.y, EndPositon3.transform.position.z);
                /*switch (LineIns.turnCount % 3)
                {                  
                    case 0:
                        endposition = new Vector3(EndPositon1.transform.position.x * 100000, 0, EndPositon1.transform.position.z * 100000);
                        break;
                    case 1:
                        endposition = new Vector3(EndPositon1.transform.position.x * 100000, 0, EndPositon1.transform.position.z * 100000);
                        break;
                    case 2:
                        endposition = new Vector3(EndPositon1.transform.position.x * 100000, 0, EndPositon1.transform.position.z * 100000);
                        break;                       
                }*/
                //endposition = new Vector3(EndPositon1.transform.position.x, 0, EndPositon1.transform.position.z);
                Lane0MoveObjects.transform.position = Vector3.MoveTowards(Lane0MoveObjects.transform.position, endposition0, campos * LoadData.speed * Time.deltaTime);

                Lane1MoveObjects.transform.position = Vector3.MoveTowards(Lane1MoveObjects.transform.position, endposition1, campos * LoadData.speed * Time.deltaTime);
                Lane1MoveObjects.transform.localEulerAngles = new Vector3(0, 120, 0);

                Lane2MoveObjects.transform.position = Vector3.MoveTowards(Lane2MoveObjects.transform.position, endposition2, campos * LoadData.speed * Time.deltaTime);
                Lane2MoveObjects.transform.localEulerAngles = new Vector3(0, -120, 0);

            }
        }
    }
}
