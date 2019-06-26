using UnityEngine;
using UnityEngine.UI;

delegate void RT();
delegate void LT();

public class LineIns : MonoBehaviour
{
    public GameObject GroundLines;
    public GameObject Lines;
    public GameObject Lane0MoveObject;
    public GameObject Lane1MoveObject;
    public GameObject Lane2MoveObject;
    public GameObject EndPositon1;
    public GameObject EndPositon2;
    public GameObject EndPositon3;

    public Text Linevalue;
    private float around = 360;
    public int turnCount = 0;
    int oldTurnCount;
    float turnSpeed = 6f;
    public float step;

    bool leftFlag = false;
    bool rightFlag = false;

    public float rot;

    public WebCamController2 WebCamController2;

    void Start()
    {
       // CreatLines(3);
    }

    void Update()
    {
        {
            step = turnSpeed * Time.deltaTime;
            rot = around / 3 * turnCount;
            GroundLines.transform.rotation = Quaternion.Slerp(GroundLines.transform.rotation, Quaternion.Euler(0, rot, 0), step);
            //Debug.Log(rot);
        }

        if (turnCount < 0)
        {
            turnCount = 2;
        }

        if (Input.GetKeyDown(KeyCode.N) || WebCamController2.camera1Flag && !WebCamController2.rotFlag && Gate.StartGate)
        {
            LeftTurn();
            WebCamController2.rotFlag = true;
        }

        if (Input.GetKeyDown(KeyCode.V) || WebCamController2.camera2Flag && !WebCamController2.rotFlag && Gate.StartGate)
        {
            RightTurn();
            WebCamController2.rotFlag = true;
        }
    }

    void CreatLines(int value)
    {
        for (int i = 1; i < value + 1; i++)
        {
            var rot = around / value * i;
            var Obj = (GameObject)Instantiate(Lines, this.transform.position, Quaternion.Euler(0, rot, 0));

            Obj.transform.parent = GroundLines.transform;
            
            switch (i)
            {
                case 1:
                    Obj.name = "Lane2";
                    Lane2MoveObject.transform.parent = Obj.transform;
                    EndPositon3.transform.parent = Obj.transform;
                    break;
                case 2:
                    Obj.name = "Lane1";
                    Lane1MoveObject.transform.parent = Obj.transform;
                    EndPositon2.transform.parent = Obj.transform;
                    break;
                case 3:
                    Obj.name = "Lane0";
                    Lane0MoveObject.transform.parent = Obj.transform;
                    EndPositon1.transform.parent = Obj.transform;
                    break;
            }
        }
    }

    public void RightTurn()
    {
        turnCount++;
        Debug.Log(turnCount);
    }

    public void LeftTurn()
    {
        turnCount--;
        Debug.Log(turnCount);
    }
}