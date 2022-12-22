using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaManager : MonoBehaviour
{
    public WaveSpawnPointCategories waveSpawnPointCategories;

    public AreaTilemap[] areaTilemaps;

    [Serializable]
    public struct AreaTilemap
    {
        public string name;
        public Tilemap tilemap;
    }

    public void OpenArea(string name)
    {
        waveSpawnPointCategories.ActivateArea(name);

        StartCoroutine(LerpAlphaDown(name));
    }

    private IEnumerator LerpAlphaDown(string name)
    {
        var areaTilemap = Array.Find(areaTilemaps, areaTilemap => areaTilemap.name == name);

        Color lerpColor = areaTilemap.tilemap.color;

        float alpha = areaTilemap.tilemap.color.a;

        while(alpha > 0)
        {
            alpha -= Time.deltaTime;

            lerpColor.a = alpha;

            areaTilemap.tilemap.color = lerpColor;

            yield return null;
        }
    }
}
