using UnityEngine;
using UnityEngine.UI;

public class RatingNumItemController : MonoBehaviour
{
    [Header("Texture")]
    public Sprite[] NumSprites;

    private Image image;
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        
    }

    void Update()
    {
        
    }
    public void Load(int i)
    {
        if(i is -1)
        {
            gameObject.SetActive(false);
            return;
        }

        if(i is > 9 or < 0)
        {
            i = 0;
        }
        if(image != null)
        {
            image.sprite = NumSprites[i];
        }

        gameObject.SetActive(true);
    }
}
