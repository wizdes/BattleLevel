using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public GameObject[] floorTiles;
    public GameObject[] objectTiles;

    public GameObject playerUnit;
	public GameObject archerUnit;
	public GameObject soldierUnit;
    public GameObject squareUnit;

	public static List<GameObject> units;

	public static List<string> collisionCoordinateMap;

	public int[] collisionMarkers = new int[]{
		49,
		50,
		102,
		51,
		52,
		146,
		99,
		200,
		1518,
	};

    private int columns = 16;
    private int rows = 9;
    private Transform boardHolder;

    // Notes:
    // when instantiating objects
    // z value is "INTO" the page, so higher means it's in the background
    // set constants for the different layers
    // turn off anti-aliasing for pixel games

    void BoardSetup(){
		units = new List<GameObject> ();
        boardHolder = new GameObject("Board").transform;

        // read the numbers from the XML (print?)
        // create a config file that has all these details
        // update the linq file to read from the config file as well
        string[] battleMap = File.ReadAllLines("Assets/SpriteMap/ReadBattleMap1.txt");
        string[] terrainMap = battleMap[0].Split(',');
        string[] objectMap = battleMap[1].Split(',');
        string[] unitMap = battleMap[2].Split(',');
		collisionCoordinateMap = new List<string> ();

        for (int y = 0; y < rows; y++){
            for (int x = 0; x < columns; x++){

                // read the number and put it here
                int terrainValue = int.Parse(terrainMap[y * columns + x]);

				for (int i = 0; i < collisionMarkers.Length; i++) {
					if (collisionMarkers [i] == terrainValue) {
						collisionCoordinateMap.Add (x + "," + y);
						break;
					}
				}

                terrainValue -= 1;

                //map the terrainValue to the actual value (should just be an addition/subtraction)
                int mappedTerrainValue = terrainValue;

                GameObject toInstantiate = floorTiles[mappedTerrainValue];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, rows - y - 1, 3f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);

                // read the number and put it here
                int objectValue = int.Parse(objectMap[y * columns + x]);

                if(objectValue != 0)
                {
                    GameObject ObjectToInstantiate = null;

                    if (objectValue > 1440)
                    {
                        objectValue = objectValue - 1441;
                        ObjectToInstantiate = objectTiles[objectValue];
                    }
                    else
                    {
                        objectValue -= 1;
                        ObjectToInstantiate = floorTiles[objectValue];
                    }

                    GameObject ObjectInstance = Instantiate(ObjectToInstantiate, new Vector3(x, rows - y - 1, 2f), Quaternion.identity) as GameObject;

                    ObjectInstance.transform.SetParent(boardHolder);
                }

                // read the number and put it here
                int unitValue = int.Parse(unitMap[y * columns + x]);

                if (unitValue != 0)
                {
                    if (unitValue > 1609)
                    {
                        unitValue = unitValue - 1609;
                    }
                    else
                    {
                        unitValue -= 1;
                    }

					GameObject gameUnitToUse = null;

					if (unitValue % 3 == 0) {
						gameUnitToUse = playerUnit;
					} else if (unitValue % 3 == 1) {
						gameUnitToUse = soldierUnit;
					} else {
						gameUnitToUse = archerUnit;
					}
						
                    //use the unit values to determine which unit to push
                    // create 3 player units
					GameObject ObjectInstance = Instantiate(gameUnitToUse, new Vector3(x, rows - y - 1, 0f), Quaternion.identity) as GameObject;
					units.Add (ObjectInstance);

                    ObjectInstance.transform.SetParent(boardHolder);
                }
            }
        }

        // place the main hero at 1,1
        // GameObject PlayerInstance = Instantiate(playerUnit, new Vector3(1, 1, 0f), Quaternion.identity) as GameObject;
        // PlayerInstance.transform.SetParent(boardHolder);
    }

    public void SetupScene(int level){
        BoardSetup();
    }
}
