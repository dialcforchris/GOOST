using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour, IPoolable<FloatingText>
{
    #region IPoolable
    public PoolData<FloatingText> poolData { get; set; }
    #endregion

    
    [SerializeField]
    Text score;
    private bool onOff = true;
	
    void Update()
    {
        if (onOff)
        {
            score.color = new Color(score.color.r, score.color.g, score.color.b, score.color.a - Time.deltaTime/2);
            score.rectTransform.position = new Vector2 (score.rectTransform.position.x,score.rectTransform.position.y + Time.deltaTime);
        }
        if (score.color.a<=0)
        {
            ReturnPool();
        }
    }

    public void OnPooled(int _score,Vector2 _position, Color _colour)
    {
        gameObject.SetActive(true);
        score.color = new Color( _colour.r,_colour.g,_colour.b,1);
        score.text = _score.ToString();
        score.rectTransform.position = _position;
    }

    public void ReturnPool()
    {
        score.color = new Color(1, 1, 1, 1);
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    } 
}
