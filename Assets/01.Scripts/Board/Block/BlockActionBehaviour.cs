using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BlockActionBehaviour : MonoBehaviour
{
    [SerializeField]
    private BlockConfig _blockConfig;
    public bool IsMoving { get; set; }

    private Queue<Vector3> _movementQueue = new Queue<Vector3>(); // x= col , y = row, z = acceleration

    // 주어진 수 만큼 이동
    public void MoveDrop(Vector2 inDropDistance)
    {
        _movementQueue.Enqueue(new Vector3(inDropDistance.x, inDropDistance.y,1));

        if(!IsMoving)
        {
            StartCoroutine(DoActionMoveDrop());
        }    
    }

    private IEnumerator DoActionMoveDrop(float acc = 1.0f)
    {
        IsMoving = true;

        while (_movementQueue.Count > 0)
        {
            Vector3 destination = _movementQueue.Dequeue();

            int dropIndex = Mathf.Min(_blockConfig.dropSpeed.Length, Mathf.Max(1, (int)Mathf.Abs(destination.y)));
            float duration = _blockConfig.dropSpeed[dropIndex -1];
            yield return CoStartDropSmooth(destination, duration * acc);
        }

        IsMoving = false;
        yield break;
    }

    private IEnumerator CoStartDropSmooth(Vector3 dropDistance,float duration)
    {
        Vector3 to = new Vector3(transform.position.x + dropDistance.x, transform.position.y - dropDistance.y, transform.position.z);
        //yield return null/*Action2D.MoveTo(transform, to, duration)*/;
        yield return transform.DOMove(to, duration).WaitForCompletion();
    }
}
