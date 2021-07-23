using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLegendScript : MonoBehaviour
{

    GameObject[] Legends;

    int LegendIndex;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateLegend();
    }

    public void InstantiateLegend()
    {
        Instantiate(Legends[LegendIndex], transform.position, transform.rotation, transform);
    }
}
