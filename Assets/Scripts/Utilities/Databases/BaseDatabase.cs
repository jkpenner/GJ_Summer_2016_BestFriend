using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BaseDatabase<T> : ScriptableObject where T : BaseDatabaseAsset {
    [SerializeField]
    private List<T> _objects;
    protected List<T> Objects {
        get {
            if (_objects == null) {
                _objects = new List<T>();
            }
            return _objects;
        }
    }

    public void Add(T t) {
        Objects.Add(t);
        OnAddObject(t);
    }

    public void Remove(T t) {
        Objects.Remove(t);
        OnRemoveObject(t);

    }

    public void RemoveAt(int index) {
        var obj = Objects[index];
        Objects.RemoveAt(index);
        OnRemoveObject(obj);
    }

    public void Contains(T t) {
        Objects.Contains(t);
    }

    public void Insert(int index, T t) {
        Objects.Insert(index, t);
        OnAddObject(t);
    }

    public int Count {
        get { return Objects.Count; }
    }

    public T Get(int index) {
        if (this.Count > 0) {
            if (index >= 0 && index < this.Count) {
                return Objects[index];
            }
            return null;
        }
        return null;
    }

    public T GetById(int id) {
        for (int i = 0; i < Count; i++) {
            var asset = Get(i);
            if (asset.Id == id) {
                return Get(i) as T;
            }
        }
        return null;
    }

    public void Replace(int index, T t) {
        var old = Objects[index];
        Objects[index] = t;
        OnRemoveObject(old);
        OnAddObject(t);
    }

    protected virtual void OnAddObject(T t) {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    protected virtual void OnRemoveObject(T t) {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    static public U GetDatabase<U>(string dbPath, string dbName) where U : ScriptableObject {
#if UNITY_EDITOR
        string dbFullPath = @"Assets/" + dbPath + dbName;

        U database = AssetDatabase.LoadAssetAtPath(dbFullPath, typeof(U)) as U;

        if (database == null) {
            string folderPath = "Assets";
            string[] folders = dbPath.Split(new char[] { '/' });
            foreach (string folder in folders) {
                if (string.IsNullOrEmpty(folder)) continue;

                if (!AssetDatabase.IsValidFolder(folderPath + '/' + folder)) {
                    AssetDatabase.CreateFolder(folderPath, folder);
                }
                folderPath += "/" + folder;
            }

            database = ScriptableObject.CreateInstance<U>();
            AssetDatabase.CreateAsset(database, dbFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return database;
#else
        Debug.LogFormat("Loading Database at path ({0}) with name {1}", dbPath, dbName);
        var db = Resources.Load<U>(dbPath.Replace("Resources/", "") + dbName.Replace(".asset", ""));
        if (db == null) {
            Debug.Log("No Database found");
            return null;
        } else {
            Debug.Log("All GOOD!");
            return db;
        }
#endif
    }

    public int GetNextId() {
        if (Count <= 0) {
            return 1;
        } else {
            int max = 1;
            for (int i = 0; i < Count; i++) {
                if (Get(i).Id >= max) {
                    max = Get(i).Id + 1;
                }
            }
            return max;
        }
    }
}