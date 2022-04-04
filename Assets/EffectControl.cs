using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            Image image = GetComponent<Image>();
            image.color = new Color(1, 1, 1, image.color.a - 2 * Time.deltaTime);

            if (image.color.a <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void ActivateEffect(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;
        gameObject.SetActive(true);
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
