using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class EggJail : MonoBehaviour
{
    private static EggJail eggJail = null;
    public static EggJail instance { get { return eggJail; } }

    public int cost = 100;
    public int inflation = 50;
    int numberOfEggs = 3;
    public GameObject Egg;
    [SerializeField]
    private Text price;
    [SerializeField]
    private Text remaining;

    [SerializeField] private Transform eggDropoff = null;
    public Transform dropoff { get { return eggDropoff; } }

    private void Awake()
    {
        eggJail = this;
    }

    void Update()
    {
        ShowEggs();
    }
   void ShowEggs()
    {
        bool eggsThen = false;
        eggsThen = numberOfEggs>0?  true : false;
        Egg.SetActive(eggsThen);
        price.supportRichText = true;

        price.text = "EGG RETURN SERVICE" + "\n"+ "<color=#c0c0c0ff>" + cost + " SILVER</color>";// +"\n" + "TO GET YOUR EGGS UNHARMED";
        if (numberOfEggs > 0)
        {
            remaining.text = "ONLY " + numberOfEggs + " REMAINING!";
        }
        else
        {
            remaining.text = "SOLD OUT!";
        }
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
