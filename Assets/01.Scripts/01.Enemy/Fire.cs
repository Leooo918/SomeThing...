using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private void EndFire()
    {
        PoolManager.instance.Enqueue("Fire", gameObject);
    }
}
