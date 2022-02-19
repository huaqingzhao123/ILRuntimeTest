using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBridge
{
    public void AddMonoMoveScript(GameObject gameObject)
    {
        var move = gameObject.AddComponent<HotFixMono_Move>();
        move.IEnumeratorMove();
    }
}
