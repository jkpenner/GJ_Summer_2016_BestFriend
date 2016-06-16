using UnityEngine;
using System.Collections;

public interface IPoolableObject {
    int Id { get; set; }
    string Name { get; set; }
    GameObject Prefab { get; set; }

	bool PoolRestricted { get; set; }
	int PoolCount { get; set; }
}
