using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsLastSeibiling : MonoBehaviour
{
    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }
}
