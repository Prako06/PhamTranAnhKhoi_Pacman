using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public SaveData activeSave;
    public SaveGhosts activeSaveGhost;

    public bool hasLoaded;

    private void Awake() {
        instance = this;
        Load();
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Save();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            DeleteSaveData();
        }
    }

    public void Save()
    {
        string dataPath = Application.persistentDataPath;

        var serializer = new XmlSerializer(typeof(SaveData));
        var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Create);
        serializer.Serialize(stream, activeSave);
        stream.Close();

        Debug.Log("Saved");
    }

    public void Load()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            var serializer = new XmlSerializer(typeof(SaveData));
            var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Open);
            activeSave = serializer.Deserialize(stream) as SaveData;
            stream.Close();

            Debug.Log("LOADED");

            hasLoaded = true;
        }
    }

    public void DeleteSaveData()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            File.Delete(dataPath + "/" + activeSave.saveName + ".save");
        }
    }

    public void SaveGhost()
    {
        string dataPath = Application.persistentDataPath;

        var serializer = new XmlSerializer(typeof(SaveData));
        var stream = new FileStream(dataPath + "/" + activeSaveGhost.saveName + ".save", FileMode.Create);
        serializer.Serialize(stream, activeSaveGhost);
        stream.Close();

        Debug.Log("Saved");
    }

    public void LoadGhost()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSaveGhost + ".save"))
        {
            var serializer = new XmlSerializer(typeof(SaveData));
            var stream = new FileStream(dataPath + "/" + activeSaveGhost.saveName + ".save", FileMode.Open);
            activeSaveGhost = serializer.Deserialize(stream) as SaveGhosts;
            stream.Close();

            Debug.Log("LOADED");

            hasLoaded = true;
        }
    }

    public void DeleteGhost()
    {
         string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSaveGhost.saveName + ".save"))
        {
            File.Delete(dataPath + "/" + activeSaveGhost.saveName + ".save");
        }
    }
}

[System.Serializable]
public class SaveData
{
    public string saveName;
    public int score;
    public int lives = 3;
    public Vector3 pacMan;
    public int highscore;
    public List<int> pellets;
}

[System.Serializable]
public class SaveGhost
{
    public Vector3 ghostsPosition;
    public Vector2 CurrenDirection;
    public Vector2 nextDirection;
    public GhostBehavior ghostsBehavior;
}

[System.Serializable]
public class SaveGhosts
{
    public string saveName;
    public List<SaveGhost> saveGhosts = new List<SaveGhost>();
}
