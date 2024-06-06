using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;

public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private Transform map;

    [SerializeField]
    private Texture2D[] mapData;

    [SerializeField]
    private MapElement[] mapElements;

    [SerializeField]
    private Sprite defaultTile;


    private Dictionary<Point, GameObject> waterTiles = new Dictionary<Point, GameObject>();

    [SerializeField]
    private SpriteAtlas waterAtlas;

    private Vector3 WorldStartPos
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(0, 0));  //oyunun kamerasını her zaman 0 0 noktasından başlatıcak
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateMap()
    {
        int height = mapData[0].height;
        int width = mapData[0].width;

        for (int i = 0; i < mapData.Length; i++)
        {
            for (int x = 0; x < mapData[i].width; x++)  //fotoğrafın x yönü
            {
                for (int y = 0; y < mapData[i].height; y++) //fotorğafın y yönü
                {
                    Color c = mapData[i].GetPixel(x, y); //pikseller halinde alıyorum x ve y

                    //Debug.Log(ColorUtility.ToHtmlStringRGBA(c));

                    //rengimizde layerımız aynıysa pixellerini kontrol et mapin
                    MapElement newElement = Array.Find(mapElements, e => e.MyColor == c);

                    if (newElement != null)
                    {
                        //tileın x ve y pozisyonlarını alır
                        float xPos = WorldStartPos.x + (defaultTile.bounds.size.x * x);
                        float yPos = WorldStartPos.y + (defaultTile.bounds.size.y * y);

                        GameObject go = Instantiate(newElement.MyElementPrefab); //tile yapar
                        go.transform.position = new Vector2(xPos, yPos); //pozisyonunu ayarlar

                        if (newElement.MyTileTag == "Water")
                        {
                            waterTiles.Add(new Point(x, y), go);
                        }

                        if (newElement.MyTileTag == "Tree1")
                        {
                            go.GetComponent<SpriteRenderer>().sortingOrder = height * 2 - y * 2;
                        }


                        go.transform.parent = map;
                    }

                }
            }
        }
        CheckWater();
    }

    private void CheckWater()
    {
        foreach (KeyValuePair<Point, GameObject> tile in waterTiles)
        {
            string composition = TileCheck(tile.Key);

            if (composition[1] == 'E' && composition[3] == 'W' && composition[4] == 'E' && composition[6] == 'W')
            {
                tile.Value.GetComponent<SpriteRenderer>().sprite = waterAtlas.GetSprite("0");
            }
            if (composition[1] == 'W' && composition[2] == 'E' && composition[4] == 'W')
            {
                GameObject go = Instantiate(tile.Value, tile.Value.transform.position, Quaternion.identity, map);
                go.GetComponent<SpriteRenderer>().sprite = waterAtlas.GetSprite("38");
                go.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            if (composition[1] == 'W' && composition[3] == 'W' && composition[4] == 'W' && composition[6] == 'W')
            {
                int randomChance = UnityEngine.Random.Range(0, 100);

                if (randomChance < 15)
                {
                    tile.Value.GetComponent<SpriteRenderer>().sprite = waterAtlas.GetSprite("46");
                }
            }
            if (composition[1] == 'W' && composition[2] == 'W' && composition[3] == 'W' && composition[4] == 'W' && composition[5] == 'W' && composition[6] == 'W')
            {
                int randomChance = UnityEngine.Random.Range(0, 100);

                if (randomChance < 15)
                {
                    tile.Value.GetComponent<SpriteRenderer>().sprite = waterAtlas.GetSprite("47");
                }
            }


        }

    }
    private string TileCheck(Point currentPoint)
    {
        string composition = string.Empty;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 || y != 0)
                {
                    if (waterTiles.ContainsKey(new Point(currentPoint.MyX + x, currentPoint.MyY + y)))
                    {
                        composition += "W";
                    }
                    else
                    {
                        composition += "E";
                    }
                }
            }
        }

        return composition;
    }


    [Serializable]
    public class MapElement
    {
        [SerializeField]
        private string tileTag;

        [SerializeField]
        private Color color;

        [SerializeField]
        private GameObject elementPrefab;

        public GameObject MyElementPrefab
        {
            get
            {
                return elementPrefab;
            }
        }


        public string MyTileTag
        {

            get
            {
                return tileTag;
            }
        }

        public Color MyColor { get => color; }
    }
}

public struct Point
{
    public int MyX { get; set; }
    public int MyY { get; set; }

    public Point (int x, int y)
    {
        this.MyX = x;
        this.MyY = y;

    }
}
