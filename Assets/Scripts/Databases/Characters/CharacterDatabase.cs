using UnityEngine;
using System.Collections;

public class CharacterDatabase : BaseDatabase<CharacterAsset> {
    const string DatabasePath = @"Resources/Databases/";
    const string DatabaseName = @"CharacterDatabase.asset";

    static private CharacterDatabase _instance = null;
    static public CharacterDatabase Instance {
        get {
            if (_instance == null) {
                _instance = GetDatabase<CharacterDatabase>(DatabasePath, DatabaseName);
            }
            return _instance;
        }
    }

    static public CharacterAsset GetAsset(int id) {
        if (Instance != null) {
            return Instance.GetById(id);
        }
        return null;
    }
}
