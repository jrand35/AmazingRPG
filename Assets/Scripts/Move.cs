using UnityEngine;
using System.Collections;

public static class Move {
    private static int moveDuration = 50;
    public static IEnumerator MoveInFrontOfBattler(Battler user, Battler target, Vector3 startPos)
    {
        Vector3 targetPos = target.gameObject.transform.position + new Vector3(-2f, 0f, 0f);
        targetPos.y = startPos.y;
        for (int i = 0; i < moveDuration; i++)
        {
            Vector3 newPos = startPos + (targetPos - startPos) * (float)i / moveDuration;
            user.gameObject.transform.position = newPos;
            yield return 0;
        }
        user.transform.position = targetPos;
    }
    public static IEnumerator MoveBackFromBattler(Battler user, Battler target, Vector3 startPos)
    {
        Vector3 targetPos = target.gameObject.transform.position + new Vector3(-2f, 0f, 0f);
        targetPos.y = startPos.y;
        for (int i = 0; i < moveDuration; i++)
        {
            Vector3 newPos = targetPos + (startPos - targetPos) * (float)i / moveDuration;
            user.gameObject.transform.position = newPos;
            yield return 0;
        }
        user.transform.position = startPos;
    }

}
