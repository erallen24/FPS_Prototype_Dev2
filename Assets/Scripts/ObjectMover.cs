using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] float moveDist;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 moveDir;
 

    private Vector3 startPostion;
    private Vector3 endPostion;

    private bool isOpening = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPostion = transform.position;
        endPostion = startPostion + moveDir * moveDist;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPostion, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPostion, moveSpeed * Time.deltaTime);
        }
    }

    public void ToEndPos()
    {
        isOpening = true;
    }

    public void ToStartPos()
    {
        isOpening = false;
    }

}
