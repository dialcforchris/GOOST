using UnityEngine;
using System.Collections;

public class EggJail : MonoBehaviour
{
    public int cost = 100;
    public int inflation = 50;
    int numberOfEggs = 16;
    public GameObject Egg;

    void Update()
    {
        ShowEggs();
    }
   void ShowEggs()
    {
        bool eggsThen = false;
        eggsThen = numberOfEggs>0?  true : false;
        Egg.SetActive(eggsThen);
    }

    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.gameObject.tag == "Player"&&_col.gameObject.GetComponent<Player>())
        {
            Player p = _col.gameObject.GetComponent<Player>();
            if (numberOfEggs>0)
            {
                if (p.GetScore() > cost)
                {
                    NumberOfEggs(-1);
                    p.ChangeScore(-cost);
                }
                else
                {
                    p.ChangeScore(-p.GetScore());
                }
                cost += inflation;
            }
        }
    }
    public void NumberOfEggs(int _difference)
    {
        numberOfEggs += _difference;
    }
}
