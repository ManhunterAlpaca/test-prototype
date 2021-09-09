using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager {

    public static void Save(GameState save) {
        string savePath = getSavePath();

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(savePath)) {
            binaryFormatter.Serialize(fileStream, save);
        }
    }

    public static GameState Load() {
        string savePath = getSavePath();
        if (File.Exists(savePath)) {
            GameState save;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (var fileStream = File.Open(savePath, FileMode.Open)) {
                save = (GameState)binaryFormatter.Deserialize(fileStream);
            }
            return save;
        } else {
            return new GameState();
        }
    }

    public static string getSavePath() {
        return Application.persistentDataPath + "/persistent.save";
    }


}