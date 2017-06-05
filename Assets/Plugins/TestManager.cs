//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class TestManager : MonoBehaviour {

//    [MenuItem("My Project/Create Simple Prefab")]
//    static void DoCreateSimplePrefab()
//    {
//        Transform[] transforms = Selection.transforms;
//        foreach (Transform t in transforms)
//        {
//            GameObject prefab = PrefabUtility.CreatePrefab("Assets/Temporary/" + t.gameObject.name + ".prefab", t.gameObject, ReplacePrefabOptions.ReplaceNameBased);
//        }
//    }
//}
