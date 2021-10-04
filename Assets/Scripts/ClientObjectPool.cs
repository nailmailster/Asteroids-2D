using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientObjectPool : MonoBehaviour
{
    private BulletObjectPool _pool;

    public GameObject player;

    void Start()
    {
        _pool = gameObject.AddComponent<BulletObjectPool>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Spawn Bullets"))
            _pool.Spawn(player.transform.localPosition, player.transform.localRotation, transform);
    }
}
