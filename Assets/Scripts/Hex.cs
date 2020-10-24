using System.Collections;
using UnityEditor;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public Transform piece;
    public int selected;

    void Start()
    {
        StartCoroutine(ttime());
    }
    IEnumerator ttime()
    {
        yield return new WaitForSeconds(0.36f);
        
        transform.Translate(new Vector3(0, 0, 90));
        InstantiatePiece();

        /*
        //Altıgenlerin üstüne kordinatlarını yazdırma.(Daha rahat test için.)
        GameObject gameObject = new GameObject("Text");
        gameObject.transform.SetParent(this.transform);

        gameObject.AddComponent<Text>().text = transform.name;
        gameObject.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        gameObject.transform.localScale /= 80;
        gameObject.transform.position = transform.position;*/
    }
    void InstantiatePiece() //Altıgenin her köşesine bir parça oluşturuyoruz. Üçül gruplar seçebilmek için
    {
        float angle = 360f / 6f;
        for (int i = 0; i < 6; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(i * angle, Vector3.forward);
            Vector3 direction = rotation * Vector3.up;

            Vector3 position = transform.position + (transform.forward * -2) + (direction * 0.2887f);
            Transform piecee = Instantiate(piece, position, rotation);
            piecee.parent = transform;
            piecee.name = (i*angle).ToString();
        }
    }
}
