using TMPro;
using UnityEngine;

public class Combo : MonoBehaviour
{
    public static int perfect;
    public static int good;
    public static int ok;
    public static int miss;

    public int combo;

    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = (combo / 3).ToString();

        perfect = NotesJudge.perfect;
        good = NotesJudge.good;
        ok = NotesJudge.ok;
        miss = NotesJudge.miss;
    }

    public void ComboCount(int i)
    {
        if (i == 1)
        {
            combo += 1;
        }
        else if (i == 0)
        {
            combo = 0;
        }
    }
}
