using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private int[] tileType;
    // Start is called before the first frame update
    void Start()
    {
        int count = transform.childCount;
        GameObject[] tiles = new GameObject[count];
        tileType = new int[count];

        for (int bogus = 0; bogus < count; bogus++)
        {
            tiles[bogus] = transform.GetChild(bogus).gameObject;
        }

        for (int bogus = 0; bogus < count; bogus++)
        {
            if (tiles[bogus].CompareTag("Passable"))
            {
                tileType[bogus] = 0;
            }
            else if (tiles[bogus].CompareTag("Impassable"))
            {
                tileType[bogus] = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
