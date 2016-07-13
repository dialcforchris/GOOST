using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour 
{

    public bool getLaid = false;
    private bool inNest = false;
    [SerializeField]
    private GameObject brokenEgg;
    [SerializeField]
    private GameObject magpie;
    [SerializeField]
    Animator ani;
    float hatchTime = 0;
    float maxHatchTime = 6;

	// Use this for initialization
	void Start () 
    {
	    if (getLaid)
        {
            ani.Play("Laid");
            //do a thing
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (getLaid)
        {
          
            getLaid = false;
            
        }
       
        Hatch();
	}
    
    void OnColliderEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            transform.SetParent(col.gameObject.GetComponent<Player>().eggTrans.transform);
           
            transform.position = col.gameObject.GetComponent<Player>().eggTrans.transform.position;
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Nest")
        {
            inNest = true;
        }
    }
    void Hatch()
    {
        if (!inNest||transform.parent ==null)
        {
            if (hatchTime<maxHatchTime)
            {
                hatchTime += Time.deltaTime;
            }
            else
            {
                Instantiate(brokenEgg,new Vector2(transform.position.x,transform.position.y +1),transform.rotation);
                Destroy(gameObject);
                //spawn a magpie
               //lets have an eggshell particle system
              //Destroy this
            }
        }
    }
}
