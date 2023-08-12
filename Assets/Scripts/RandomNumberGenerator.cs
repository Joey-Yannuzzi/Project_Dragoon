using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomNumberGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int randomNumberGenerator(int[] sequence)
    {
        System.Random rand = new System.Random();
        float rnRaw;
        int rn;
        int ave = 0;

        for (int bogus = 0; bogus < sequence.Length; bogus++)
        {
            rnRaw = (float)rand.NextDouble();
            rn = (int)(rnRaw * 100);
            sequence[bogus] = rn;
        }

        for (int bogus = 0; bogus < sequence.Length; bogus++)
        {
            ave += sequence[bogus];
        }

        ave = ave / sequence.Length;
        return (ave);
    }
}
