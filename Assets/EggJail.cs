using UnityEngine;
using System.Collections;

public class EggJail : MonoBehaviour
{
    public int cost;
    public int inflation;
    int numberOfEggs = 0;


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (numberOfEggs>0)
            {
                col.gameObject.GetComponent<Player>().ChangeScore(-cost);
                cost += inflation;
                numberOfEggs--;
            }
        }
    }
}
