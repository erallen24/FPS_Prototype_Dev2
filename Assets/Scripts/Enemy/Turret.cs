using Unity.VisualScripting;
using UnityEngine;

public class Turret : EnemyAI
{
    Vector3 playerDir;

    public override void Movement()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }
}
