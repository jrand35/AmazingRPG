using UnityEngine;
using System.Collections;

public static class Move {
    private static int moveDuration = 50;
    public static IEnumerator MoveInFrontOfBattler(Battler user, Battler target, Vector3 startPos, Vector3 offset, int duration)
    {
        Quaternion startRotation = user.transform.rotation;
        Vector3 targetPos = target.gameObject.transform.position + offset;
        targetPos.y = startPos.y;
        var heading = targetPos - startPos;
        var distance = heading.magnitude;
        var direction = heading / distance;
        user.transform.rotation = Quaternion.LookRotation(direction);
        for (int i = 0; i < duration; i++)
        {
            Vector3 newPos = startPos + (targetPos - startPos) * (float)i / duration;
            user.gameObject.transform.position = newPos;
            yield return 0;
        }
        user.transform.rotation = startRotation;
        user.transform.position = targetPos;
    }
    public static IEnumerator MoveBackFromBattler(Battler user, Battler target, Vector3 startPos, Vector3 offset, int duration)
    {
        Quaternion startRotation = user.transform.rotation;
        Vector3 targetPos = target.gameObject.transform.position + offset;
        targetPos.y = startPos.y;
        var heading = startPos - targetPos;
        var distance = heading.magnitude;
        var direction = heading / distance;
        user.transform.rotation = Quaternion.LookRotation(direction);
        for (int i = 0; i < duration; i++)
        {
            Vector3 newPos = targetPos + (startPos - targetPos) * (float)i / duration;
            user.gameObject.transform.position = newPos;
            yield return 0;
        }
        user.transform.rotation = startRotation;
        user.transform.position = startPos;
    }

}
