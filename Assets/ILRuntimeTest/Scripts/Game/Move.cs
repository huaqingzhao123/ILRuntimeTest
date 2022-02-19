using System.Collections;
using UnityEngine;

public class Move : MonoBehaviour
{
    public void MoveFunc()
    {
        StartCoroutine(ExcuteMove());
    }

    private void Update()
    {
        transform.Translate(transform.right * 3 * Time.deltaTime, Space.World);
    }
    IEnumerator ExcuteMove()
    {
        while (true)
        {
            yield return null;
            transform.Translate(transform.right * 3 * Time.deltaTime, Space.World);
        }
    }
}
