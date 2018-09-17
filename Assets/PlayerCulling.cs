using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCulling : MonoBehaviour {
    [SerializeField] Transform mapParent;
    [SerializeField] float checkTime = 2;
    Culling[] cullMaps;
    public Culling[,] cullMapsGrid;
    Culling[] closestMaps;
    Culling currentMap;

    void OnEnable() {
        cullMaps = mapParent.GetComponentsInChildren<Culling>();
        GetNineClosest();
    }

    void GetNineClosest() {
        closestMaps = cullMaps.OrderBy(t=>(t.transform.position - transform.position).sqrMagnitude)
                              .Take(9)   //or use .FirstOrDefault();  if you need just one
                              .ToArray();
        ActivateTheseAndCullAllElse(closestMaps);
        currentMap = GetClosest();
        InvokeRepeating(nameof(CheckClosest), checkTime, checkTime);
    }

    Culling[] GetNineClosest(Vector2Int coords) {
        Culling[] res = new Culling[9];
        res[0] = cullMapsGrid[coords.x - 1, coords.y - 1];
        res[1] = cullMapsGrid[coords.x,     coords.y    ];
        res[2] = cullMapsGrid[coords.x + 1, coords.y + 1];
        res[3] = cullMapsGrid[coords.x - 1, coords.y - 1];
        res[4] = cullMapsGrid[coords.x,     coords.y    ];
        res[5] = cullMapsGrid[coords.x + 1, coords.y + 1];
        res[6] = cullMapsGrid[coords.x - 1, coords.y - 1];
        res[7] = cullMapsGrid[coords.x,     coords.y    ];
        res[8] = cullMapsGrid[coords.x + 1, coords.y + 1];
        return res;
    }

    void ActivateTheseAndCullAllElse(Culling[] mapsToKeep) {
        foreach (Culling cull in cullMaps) cull.Cull(mapsToKeep.Contains(cull));
    }

    void CheckClosest() {
        var closest = GetClosest();
        if (closest == currentMap) return;
        closestMaps = GetNineClosest(closest.coords);
        ActivateTheseAndCullAllElse(closestMaps);
    }

    Culling GetClosest() {
        var shortestDistance = 100f;
        var closestMap = closestMaps[4];
        foreach (Culling map in closestMaps) {
            var distance = Vector3.Distance(transform.position, map.transform.position);
            if (!(distance < shortestDistance)) continue;
            shortestDistance = distance;
            closestMap = map;
        }
        return closestMap;
    }

    public void SetupMapGrid(int x, int y) => cullMapsGrid = new Culling[x, y];

    public void NewMap(Culling newMap, int x, int y) => cullMapsGrid[x, y] = newMap;
}
