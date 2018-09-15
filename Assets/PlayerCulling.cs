using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCulling : MonoBehaviour {
    [SerializeField] Transform mapParent;
    [SerializeField] float checkTime = 2;
    Culling[] cullMaps;
    Culling[] closestMaps;

    void OnEnable() {
        cullMaps = mapParent.GetComponentsInChildren<Culling>();

        InvokeRepeating(nameof(GetClosest), 0, checkTime);
    }

    void GetClosest() {
        closestMaps = cullMaps.OrderBy(t=>(t.transform.position - transform.position).sqrMagnitude)
                              .Take(9)   //or use .FirstOrDefault();  if you need just one
                              .ToArray();
        CheckClosest();
    }

    void CheckClosest() {
        foreach (Culling cullmap in closestMaps)
            if (cullmap.cullState != true)
                cullmap.Cull(true);
    }
}
