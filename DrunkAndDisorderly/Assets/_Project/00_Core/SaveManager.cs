using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public int currentDay;
    public int playerLives;
    public int gold;
    public int debtPaid;
    public int[] resources;
    public bool[] upgrades;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/save.dat";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public void Save(SaveData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Game saved");
    }

    public SaveData Load()
    {
        if (!HasSave()) return null;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath, FileMode.Open);

        SaveData data = formatter.Deserialize(stream) as SaveData;
        stream.Close();

        Debug.Log("Game loaded");
        return data;
    }

    public void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(savePath);
            Debug.Log("Save deleted");
        }
    }
}