using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Type : MonoBehaviour
{
    protected abstract void Effect();

    protected abstract void OnTriggerEnter2D(Collider2D collision);
}
