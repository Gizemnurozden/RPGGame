using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float lifeTime;

    [SerializeField]
    private Text text;

    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut()); //fadeout oyun başlar başlamaz çalışsın diye startta başladık.
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime); //yazının hareket etmesi için yukarı doğru hız verdik.
    }

    public IEnumerator FadeOut() //yazının silinerek gitmesi için
    {
        float startAlpha = text.color.a;

        float rate = 1.0f / lifeTime;

        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tmp = text.color;

            tmp.a = Mathf.Lerp(startAlpha, 0, progress);

            text.color = tmp;

            progress += rate * Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
