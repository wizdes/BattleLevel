using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public GameObject[] floorTiles;
    private int columns = 16;
    private int rows = 9;
    private Transform boardHolder;

    void BoardSetup(){
        boardHolder = new GameObject("Board").transform;

        // read the numbers from the XML (print?)
        string[] battleMap = File.ReadAllLines("../SpriteMap/BattleMap1Final.txt");
        string[] terrainMap = battleMap[0].Split(',');
        string[] objectMap = battleMap[1].Split(',');

        for (int y = 0; y < rows; y++){
            for (int x = 0; x < columns; x++){

                // read the number and put it here
                int terrainValue = int.Parse(terrainMap[x * rows + y]);

                //map the terrainValue to the actual value (should just be an addition/subtraction)
                int mappedTerrainValue = terrainValue;

                GameObject toInstantiate = floorTiles[mappedTerrainValue];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);

				// read the number and put it here
				int objectValue = int.Parse(objectMap[x * rows + y]);

				//map the terrainValue to the actual value (should just be an addition/subtraction)
				int mappedObjectValue = objectValue;

                toInstantiate = floorTiles[mappedObjectValue];
				instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent(boardHolder);
			}
        }
    }

    public void SetupScene(int level){
        BoardSetup();
    }
}
