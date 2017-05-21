using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public GameObject[] floorTiles;
    private int columns = 9;
    private int rows = 16;
    private Transform boardHolder;

    void BoardSetup(){
        boardHolder = new GameObject("Board").transform;

        for (int x = 0; x < columns; x++){
            for (int y = 0; y < rows; y++){
                GameObject toInstantiate = floorTiles[0];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    public void SetupScene(int level){
        BoardSetup();
    }
}
