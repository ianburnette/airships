using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class DataLoader : MonoBehaviour {
     [SerializeField] TextAsset jsonData;

     public bool parse;

     void Update() {
          if (parse) Parse();
     }

     void Parse() {

          parse = false;
     }
}
