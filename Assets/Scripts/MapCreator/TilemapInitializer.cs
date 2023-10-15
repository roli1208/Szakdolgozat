using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TilemapInitializer : Singleton<TilemapInitializer>
{
    [SerializeField] List<BuildingCategory> tilemapCategories;
    [SerializeField] Transform grid;

    private void Start()
    {
        CreateMaps();
    }
    private void CreateMaps()
    {
        foreach(BuildingCategory category in tilemapCategories)
        {
            GameObject obj = new GameObject(category.name);

            Tilemap map = obj.AddComponent<Tilemap>();
            TilemapRenderer tr = obj.AddComponent<TilemapRenderer>();
            TilemapCollider2D tc = obj.AddComponent<TilemapCollider2D>();

            obj.transform.SetParent(grid);

            tr.sortingOrder = category.SortingOrder;

            tc.isTrigger = true ;



            category.Tilemap = map;
        }
    }
}
