<Query Kind="Program" />

void Main()
{
	string path = "C:\\Users\\yjli\\Documents\\GitHub\\BattleLevel\\Assets\\SpriteMap";
	string fileName = path + "\\BattleMap1.json";
	string fileOutName = path + "\\ReadBattleMap1.txt";
	string[] contents = File.ReadAllLines(fileName);
	List<string> outLines = new List<string>();
	foreach(string line in contents){
		if(line.Contains("data")){
			outLines.Add(line.Trim().Replace("\"data\":[", "").Replace("]",""));
		}
	}
	
	File.WriteAllLines(fileOutName, outLines.ToArray());
}

// Define other methods and classes here
