using ILRuntime.CLR.TypeSystem;
using System.Collections;
using UnityEngine;

public class HotFixMono_Move:MonoBehaviour
{
    public void CubeMove()
    {
        var cube = GameObject.Find("Cube");
        cube.GetComponent<Move>().MoveFunc();
    }
    bool beginMove;
    private void Update()
    {
        if (beginMove)
        {
            transform.Translate(transform.right * 3 * Time.deltaTime, Space.World);
        }
    }
    public void BeginMoveFunc()
    {
        beginMove = true;

    }
    public void StopMove()
    {
        beginMove = false;
    }
    public void IEnumeratorMove()
    {
        StartCoroutine(ExcuteMove());
    }
    IEnumerator ExcuteMove()
    {
        while (true)
        {
            yield return null;
            transform.Translate(transform.right * 3 * Time.deltaTime, Space.World);
        }
    }
    public MonoBehaviourAdapter.Adaptor GetComponent(ILType type)
    {
        var arr = GetComponents<MonoBehaviourAdapter.Adaptor>();
        for (int i = 0; i < arr.Length; i++)
        {
            var instance = arr[i];
            if (instance.ILInstance != null && instance.ILInstance.Type == type)
            {
                return instance;
            }
        }
        return null;
    }
}
