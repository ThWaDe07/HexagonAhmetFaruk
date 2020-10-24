using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using cakeslice;

public class Grid : MonoBehaviour
{
    private Vector2 startTouchPosition, endTouchPosition;
    public GameObject[,] cntObj;
    
    public GameObject scoreObj;
    public GameObject goPanel;
    public Transform hexPrefab;
    public Transform bombPrefab;

    public int gridWidth = 8;
    public int gridHeight = 9;
    public float gap = 0f;
    public int bombScore = 1000;

    bool bomb;
    int left, right, p, s, score, score2, turn;
    float angle, angleMax;
    float hexWidth = 0.665f;
    float hexHeight = 0.577f;

    private GameObject go1, go2, go3, gameObj;

    public Color[] colors;

    Vector2 startPos;
    Vector3 centerOfRotate;

    void Start()
    {
        Screen.SetResolution(720, 1280, true);

        cntObj = new GameObject[gridWidth, gridHeight];

        AddGap(); //Altıgenlerin arasındaki boşluğu ayarlıyoruz.
        CalcStartPos(); //Grid in başlangıç pozisyonunu ayarlıyoruz.
        CreateGrid(); //Grid i oluşuturuyoruz.
        ControlColors(); //Hali hazırda üçlü grup varmı onu kontorl etmek için gönderiyoruz.

        p = 0;
    }

    void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    } //Altıgenlerin arasına boşluk ekledik.

    void CalcStartPos() //Grid in başlangıç pozisyonunu ayarladık.
    {
        float offSet = 0;
        if (gridHeight % 2 == 0) offSet = hexHeight / 2;

        float x = -hexWidth * 0.657f * (gridWidth / 2);
        float y = hexHeight * (gridHeight / 2) -  offSet;

        startPos = new Vector2(x, y);
    }

    Vector2 CalcWorldPos(Vector2 gridPos) //Oluşturulacak altıgenin grid üzerindeki kordinat sistemindeki yerini buldurmak için kullanıyoruz.
    {
        float offSet = 0;
        if (gridPos.x % 2 != 0) offSet = hexHeight / 2;

        float x = startPos.x + gridPos.x * hexWidth * 0.75f;
        float y = startPos.y - gridPos.y * hexHeight - offSet;

        return new Vector2(x, y);
    }

    void CreateGrid()  //Grid oluşturduk.
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Transform hex = Instantiate(hexPrefab); //altıgen oluşturduk.
                Vector2 gridPos = new Vector2(x, y); //altıgenin pozisyonunu bulduk.
                hex.position = CalcWorldPos(gridPos);  //altıgenin pozisyonunu ayarladık.
                hex.GetComponent<SpriteRenderer>().material.color = colors[UnityEngine.Random.Range(0, colors.Length)];  //altıgenin rengini ayarladık.
                hex.parent = transform;
                hex.name = x + "." + y;  //altıgenin kordinalarını isim olarak ayarladık (0.0) (2.5) (0.3)...
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        } //1. noktayı ayarladık

        if (Input.GetMouseButtonUp(0))
        {
            if (left == 0 && right == 0 && s == 0)
            {
                endTouchPosition = Input.mousePosition; //2. noktaı ayarladık

                if ((endTouchPosition.x < startTouchPosition.x) && (startTouchPosition.x - endTouchPosition.x >= 50f)) //sola kaydırdık (1. ve 2. noktalara göre)
                {
                    TurnHexagons("left"); //altıgenleri döndürmek için.
                } else if ((endTouchPosition.x > startTouchPosition.x) && (endTouchPosition.x - startTouchPosition.x >= 50f)) //sağa kaydırdık (1. ve 2. noktalara göre)
                {
                    TurnHexagons("right"); //altıgenleri döndürmek için.
                } else  //altıgen grubu seçtik
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        if (hit.transform != null)
                        {
                            SelectHexagons(hit.transform.gameObject); //seçilen altıgen grubunu belli etmek için.
                            angle = 0;  //seçilen altıgen grubunun dönmesi için gerekli açısal değerleri ayarladık
                            angleMax = 120;
                        }
                    }
                }
            }
        }

        if (go1 != null && go2 != null && go3 != null) //eğer altıgen grubu seçiliyse
        {
            if (left == 1) //eğer sola kaydırdıysak
            {
                if (angle < angleMax) //açısal değerleri kontrol et ona göre altıgen grubunu döndür.
                {
                    go1.transform.RotateAround(centerOfRotate, Vector3.forward, -5f);
                    go1.transform.eulerAngles += new Vector3(0, 0, 5f);
                    go2.transform.RotateAround(centerOfRotate, Vector3.forward, -5f);
                    go2.transform.eulerAngles += new Vector3(0, 0, 5f);
                    go3.transform.RotateAround(centerOfRotate, Vector3.forward, -5f);
                    go3.transform.eulerAngles += new Vector3(0, 0, 5f);
                    angle += 5;
                } else { ControlColors(); if (angle == 360) { angle = 0; angleMax = 120; left = 0; } } //eğer 120 derece döndüyse renkleri kontrol et. ve eğer grup tam tur döndüyse daha fazla döndürme.
            }

            if (right == 1) //sola kaydırma için olanların aynısı
            {
                if (angle < angleMax)
                {
                    go1.transform.RotateAround(centerOfRotate, Vector3.forward, 5f);
                    go1.transform.eulerAngles += new Vector3(0, 0, -5f);
                    go2.transform.RotateAround(centerOfRotate, Vector3.forward, 5f);
                    go2.transform.eulerAngles += new Vector3(0, 0, -5f);
                    go3.transform.RotateAround(centerOfRotate, Vector3.forward, 5f);
                    go3.transform.eulerAngles += new Vector3(0, 0, -5f);
                    angle += 5;
                }
                else { ControlColors(); if (angle == 360) { angle = 0; angleMax = 120; right = 0; } }
            }
        }

        if (bomb == true && GameObject.Find("bomb(Clone)") == null) //eğer bomba oluşturulduysa ve şuan yoksa sayacı sıfırla
        {
            bomb = false;
            turn = 99;
        }
    }

    void SelectHexagons(GameObject go) //altıgen grubu seçme
    {
        string str = go.transform.parent.name; //üstüne bastığımız altgenin ismini al

        string[] chars = str.Split('.'); //chars[0] = X değeri, chars[1] = Y değeri

        int c0 = Convert.ToInt32(chars[0]);
        int c1 = Convert.ToInt32(chars[1]);
        int c2 = Convert.ToInt32(go.name);

        ClearHex(); //bütün objelerin selected değerlerini sıfırlamaya gönderiyoruz.

        //Tıklanılan objenin açı değerine göre üçlü grubu seçme.
        if (c0 % 2 == 0)
        {
            if (c2 == 0)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find(c0 + "." + (c1 - 1));
                go3 = GameObject.Find((c0 - 1) + "." + (c1 - 1));
            }
            if (c2 == 60)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 - 1) + "." + (c1 - 1));
                go3 = GameObject.Find((c0 - 1) + "." + c1);
            }
            if (c2 == 120)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 - 1) + "." + c1);
                go3 = GameObject.Find(c0 + "." + (c1 + 1));
            }
            if (c2 == 180)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find(c0 + "." + (c1 + 1));
                go3 = GameObject.Find((c0 + 1) + "." + c1);
            }
            if (c2 == 240)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 + 1) + "." + c1);
                go3 = GameObject.Find((c0 + 1) + "." + (c1 - 1));
            }
            if (c2 == 300)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 + 1) + "." + (c1 - 1));
                go3 = GameObject.Find(c0 + "." + (c1 - 1));
            }
        } else
        {
            if (c2 == 0)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find(c0 + "." + (c1 - 1));
                go3 = GameObject.Find((c0 - 1) + "." + c1);
            }
            if (c2 == 60)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 - 1) + "." + c1);
                go3 = GameObject.Find((c0 - 1) + "." + (c1 + 1));
            }
            if (c2 == 120)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 - 1) + "." + (c1 + 1));
                go3 = GameObject.Find(c0 + "." + (c1 + 1));
            }
            if (c2 == 180)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find(c0 + "." + (c1 + 1));
                go3 = GameObject.Find((c0 + 1) + "." + (c1 + 1));
            }
            if (c2 == 240)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 + 1) + "." + (c1 + 1));
                go3 = GameObject.Find((c0 + 1) + "." + c1);
            }
            if (c2 == 300)
            {
                go1 = GameObject.Find(c0 + "." + c1);
                go2 = GameObject.Find((c0 + 1) + "." + c1);
                go3 = GameObject.Find(c0 + "." + (c1 - 1));
            }
        } 

        if (go1 != null && go2 != null && go3 != null) //eğer üçlü grubu seçildiyse bunların seçili olduklarını belli et.
        {
            go1.GetComponent<Hex>().selected = 1;
            go2.GetComponent<Hex>().selected = 1;
            go3.GetComponent<Hex>().selected = 1;

            go1.GetComponent<Outline>().enabled = true;
            go2.GetComponent<Outline>().enabled = true;
            go3.GetComponent<Outline>().enabled = true;
        }
    }

    void TurnHexagons(String side) //seçilen altıgen grubunu döndürme
    {
        if (go1 != null && go2 != null && go3 != null)
        {
            centerOfRotate = (go1.transform.position + go2.transform.position + go3.transform.position) / 3; //grubun dönme merkezini aylardık.

            //update fonksiyonundan alınan veriye göre sola mı sağa mı döndürdüğümüzü belirleme.
            if (side == "left")
            {
                left = 1;
                right = 0;
            }
            if (side == "right")
            {
                right = 1;
                left = 0;
            }
        }
    }

    void ControlColors() //grid deki patlamaya hazır üçlü grupları kontrol ediyoruz.
    {
        s = 1; //kontrol bitmeden başka altıgen seçmesini engelledik
        //dönen altıgenlerin kordinat sistemindeki yeri değiştiği için isimlerini de değiştiriyoruz.
        if (left == 1)
        {
            String name = go3.name;
            go3.name = go2.name;
            go2.name = go1.name;
            go1.name = name;
            //go1.GetComponentInChildren<UnityEngine.UI.Text>().text = go1.name;
            //go2.GetComponentInChildren<UnityEngine.UI.Text>().text = go2.name;
            //go3.GetComponentInChildren<UnityEngine.UI.Text>().text = go3.name;
        } 
        if (right == 1)
        {
            String name = go1.name;
            go1.name = go2.name;
            go2.name = go3.name;
            go3.name = name;
            //go1.GetComponentInChildren<UnityEngine.UI.Text>().text = go1.name;
            //go2.GetComponentInChildren<UnityEngine.UI.Text>().text = go2.name;
            //go3.GetComponentInChildren<UnityEngine.UI.Text>().text = go3.name;
        }

        for (int x = 0; x < gridWidth; x++) //grid sistemindeki bütün objeleri array list e alıyoruz.
        {
            for (int y = 0; y < gridHeight; y++)
            {
                cntObj[x, y] = GameObject.Find(x + "." + y);
            }
        }

        if (angleMax < 360) { angleMax += 120; } //açıyı 120 derece arttırdık.

        //Grid üzerindeki bütün altıgenleri bir algoritma ile renklerini kontorl ediyoruz.
        for (int x = 0; x < gridWidth - 1; x += 2)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                //eğer renkler aynıysa yani üçlü grup olarak patlamaya hazırsa.
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color && cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if ((left == 1 || right == 1) && bomb == true) //eğer bomba varsa, bombanın countdownunu 1 azaltıyoruz 
                    {
                        turn -= 1;
                        GameObject bombb = GameObject.Find("bombTurn");
                        bombb.GetComponent<UnityEngine.UI.Text>().text = turn.ToString();
                    }
                    left = 0; //daha fazla dönmemesi için kaydırmayı iptal ettik.
                    right = 0; //daha fazla dönmemesi için kaydırmayı iptal ettik.
                    angle = 0; //daha fazla dönmemesi için açıyı ayarladık.
                    angleMax = 120; //daha fazla dönmemesi için açıyı ayarladık.
                    CreateNewHex(x, y); //patlayan objenin korditanında bir obje daha oluşturmaya gönderdik.
                    CreateNewHex(x, y + 1); //patlayan objenin korditanında bir obje daha oluşturmaya gönderdik.
                    CreateNewHex(x + 1, y); //patlayan objenin korditanında bir obje daha oluşturmaya gönderdik.
                    ClearHex(); //Objelerin seçili değerlerini sıfırlamaya gönderdik
                    p = 1; //bir patlama olduğunu belirttik.
                    goto end; //for döngüsünden çıkmasını istedik.
                }
            }
        }
        for (int x = 1; x < gridWidth - 1; x += 2)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color && cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if ((left == 1 || right == 1) && bomb == true)
                    {
                        turn -= 1;
                        GameObject bombb = GameObject.Find("bombTurn");
                        bombb.GetComponent<UnityEngine.UI.Text>().text = turn.ToString();
                        if (turn == 0) { goPanel.SetActive(true); }
                    }
                    left = 0;
                    right = 0;
                    angle = 0;
                    angleMax = 120;
                    CreateNewHex(x, y);
                    CreateNewHex(x + 1, y);
                    CreateNewHex(x + 1, y + 1);
                    ClearHex();
                    p = 1;
                    goto end;
                }
            }
        }
        for (int x = 0; x < gridWidth - 1; x += 2)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                if (cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color && cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if ((left == 1 || right == 1) && bomb == true)
                    {
                        turn -= 1;
                        GameObject bombb = GameObject.Find("bombTurn");
                        bombb.GetComponent<UnityEngine.UI.Text>().text = turn.ToString();
                        if (turn == 0) { goPanel.SetActive(true); }
                    }
                    left = 0;
                    right = 0;
                    angle = 0;
                    angleMax = 120;
                    CreateNewHex(x, y + 1);
                    CreateNewHex(x + 1, y);
                    CreateNewHex(x + 1, y + 1);
                    ClearHex();
                    p = 1;
                    goto end;
                }
            }
        }
        for (int x = 1; x < gridWidth - 1; x += 2)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color && cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if ((left == 1 || right == 1) && bomb == true)
                    {
                        turn -= 1;
                        GameObject bombb = GameObject.Find("bombTurn");
                        bombb.GetComponent<UnityEngine.UI.Text>().text = turn.ToString();
                        if (turn == 0) { goPanel.SetActive(true); }
                    }
                    left = 0;
                    right = 0;
                    angle = 0;
                    angleMax = 120;
                    CreateNewHex(x, y);
                    CreateNewHex(x, y + 1);
                    CreateNewHex(x + 1, y + 1);
                    ClearHex();
                    p = 1;
                    goto end;
                }
            }
        }
    end: if (p == 1) { p = 0; StartCoroutine(ttime()); } else { GameEnd(); } //eğer patlama olduysa, zamanlayıcı çalıştırdık. eğer patlama olmadıysa yapılacak başka hamle varmı onu kontrol etmeye gönderdik.
    }

    IEnumerator ttime() 
    {
        yield return new WaitForSeconds(0.5f);
        if (turn == 0 && bomb == true) { goPanel.SetActive(true); } //eğer bomba countdown u 0 sa ve bomba varsa oyun bitsin.
        ControlColors(); //patlamadan sonra tekrar controle gönderdik. (Bütün patlamalar bitene kadar döngü olacak.)
    }
    void CreateNewHex(int x, int y) //yeni altıgen oluşturma.
    {
        Destroy(cntObj[x, y]); //patlayacak olan altıgeni yok et.

        //başta da yaptığımız gibi tekrar oluştur altıgeni.
        Transform hex = Instantiate(hexPrefab);
        Vector2 gridPos = new Vector2(x, y);
        hex.GetComponent<SpriteRenderer>().material.color = colors[UnityEngine.Random.Range(0, colors.Length)];
        hex.position = CalcWorldPos(gridPos);
        hex.parent = transform;
        hex.name = x + "." + y;
        score += 5; //görünen skoru 5 arttırdık.
        score2 += 5; //bomba oluşması için gerek skoru 5 arttırdık.
        scoreObj.GetComponent<UnityEngine.UI.Text>().text = score.ToString(); //skoru yazdırdık.
    }

    void ClearHex() //altıgenlerin seçili değerlerini sıfırlama
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gameObj = GameObject.Find(x + "." + y);
                gameObj.GetComponent<Hex>().selected = 0;
                gameObj.GetComponent<Outline>().enabled = false;
            }
        }
        //değerler sıfırlandıktan sonra skoru kontol ediyoruz. eğer yeterliyse bomba oluşturmaya gönderiyoruz.
        if (score2 >= bombScore)
        {
            score2 -= bombScore;
            CreateBomb(); //bomba oluşturmaya gönderdik.
        }
    }

    void CreateBomb() //bomba oluşturma
    {
        bomb = true; //bomba oluşuğunu belirt.

        //grid üzerinden herhangi bir altıgeni bul
        int x = UnityEngine.Random.Range(0, gridWidth);
        int y = UnityEngine.Random.Range(0, gridHeight);
        cntObj[x, y] = GameObject.Find(x + "." + y);

        Transform bombPre = Instantiate(bombPrefab, cntObj[x, y].transform.position + cntObj[x, y].transform.forward * -1, Quaternion.identity); //bulunan altıgen üzerinde bomba oluştur.
        bombPre.parent = cntObj[x, y].transform;

        //Oluşturulan bombanın üzerine Countdown u görebileceğimiz bir text oluştur.
        GameObject gameObject = new GameObject("bombTurn");
        gameObject.transform.SetParent(bombPre.transform);
        turn = UnityEngine.Random.Range(6, 11);
        gameObject.AddComponent<UnityEngine.UI.Text>().text = turn.ToString();
        gameObject.GetComponent<UnityEngine.UI.Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        gameObject.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
        gameObject.transform.localScale /= 80;
        gameObject.transform.position = bombPre.position;
        gameObject.transform.Translate(new Vector3(0, 0, -2));
        
    }

    void GameEnd() //oyunun sonunu kontrol etme.
    {
        for (int x = 0; x < gridWidth; x++)  //grid üzerindeki bütün altıgenlerin kordinatlarını aldık.
        {
            for (int y = 0; y < gridHeight; y++)
            {
                cntObj[x, y] = GameObject.Find(x + "." + y);
            }
        }

        //Yine bir algoritma ile olabilecek olasılıkları kontrol ettik. Eğer olasılık varsa bu fonksiyondan çıkıyoruz.
        for (int y = 1; y < gridHeight - 1; y++) //sol sütun
        {
            if (cntObj[0, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[1, y - 1].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[0, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
                else if (cntObj[0, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[0, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[0, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[1, y].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[0, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[0, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[1, y].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[0, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[0, y].GetComponent<SpriteRenderer>().material.color == cntObj[0, y - 1].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[0, y].GetComponent<SpriteRenderer>().material.color == cntObj[1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[0, y].GetComponent<SpriteRenderer>().material.color == cntObj[1, y - 1].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[0, y].GetComponent<SpriteRenderer>().material.color == cntObj[0, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
        }
        for (int y = 1; y < gridHeight - 1; y++) //sağ sütun
        {
            if (cntObj[gridWidth - 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 2, y].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[gridWidth - 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 2, y + 1].GetComponent<SpriteRenderer>().material.color)
                {   
                    s = 0;
                    return;
                }
                else if (cntObj[gridWidth - 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[gridWidth - 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 2, y + 1].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[gridWidth - 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[gridWidth - 2, y].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 2, y + 1].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[gridWidth - 2, y].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[gridWidth - 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[gridWidth - 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 2, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
            if (cntObj[gridWidth - 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 2, y].GetComponent<SpriteRenderer>().material.color)
            {
                if (cntObj[gridWidth - 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[gridWidth - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    s = 0;
                    return;
                }
            }
        }
        for (int x = 2; x < gridWidth - 1; x += 2) //x değeri çift olanlar
        {
            for (int y = 1; y < gridHeight - 1; y++)
            {
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
            }
        }
        for (int x = 1; x < gridWidth - 1; x += 2) //x değeri tek olanlar
        {
            for (int y = 1; y < gridHeight - 1; y++)
            {
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                    else if (cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
                if (cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x + 1, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }

                if (cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y + 1].GetComponent<SpriteRenderer>().material.color)
                {
                    if (cntObj[x, y + 1].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                    {
                        s = 0;
                        return;
                    }
                }
            }
        }
        //eğer olasılık yoksa oyun bitmiş oluyor ve gameover paneli çıkıyor.
        goPanel.SetActive(true);
    }
    public void Restart() //oyun sonu veya istediğimizde restart butonu için fonksiyon.
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

/*
 for (int  x= 0; x < gridWidth; x++)
        {
            if (p == 1) { break; }
            for (int y = 1; y < gridHeight; y++)
            {
                if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x, y - 1].GetComponent<SpriteRenderer>().material.color)//eğer patlak olursa
                {
                    if (x % 2 == 0)
                    {
                        if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                        {
                            p = 1;
                            left = 0;
                            right = 0;
                            angle = 0;
                            angleMax = 120;
                            CreateNewHex(x, y);
                            CreateNewHex(x, y - 1);
                            CreateNewHex(x + 1, y - 1);
                            ClearHex();
                            break;
                        }
                        else if (x != 0 && cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y - 1].GetComponent<SpriteRenderer>().material.color)
                        {
                            p = 1;
                            left = 0;
                            right = 0;
                            angle = 0;
                            angleMax = 120;
                            CreateNewHex(x, y);
                            CreateNewHex(x, y - 1);
                            CreateNewHex(x - 1, y - 1);
                            ClearHex();
                            break;
                        } else { p = 0; }
                    }
                    else
                    {
                        if (cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x - 1, y].GetComponent<SpriteRenderer>().material.color)
                        {
                            p = 1;
                            left = 0;
                            right = 0;
                            angle = 0;
                            angleMax = 120;
                            CreateNewHex(x, y);
                            CreateNewHex(x, y - 1);
                            CreateNewHex(x - 1, y);
                            ClearHex();
                            break;
                        }
                        else if (x != gridWidth - 1 && cntObj[x, y].GetComponent<SpriteRenderer>().material.color == cntObj[x + 1, y].GetComponent<SpriteRenderer>().material.color)
                        {
                            p = 1;
                            left = 0;
                            right = 0;
                            angle = 0;
                            angleMax = 120;
                            CreateNewHex(x, y);
                            CreateNewHex(x, y - 1);
                            CreateNewHex(x + 1, y);
                            ClearHex();
                            break;
                        } else { p = 0; }
                    }
                } else { p = 0; }
            }
        }
        if (angleMax < 360) { angleMax += 120; }
 
 */