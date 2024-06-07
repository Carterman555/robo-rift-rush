using RoboRiftRush.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers {

    public static Transform[] GetDirectChildren(this Transform t) {
        Transform[] children = new Transform[t.childCount];
        for (int i = 0; i < t.childCount; i++) {
            children[i] = t.transform.GetChild(i);
        }
        return children;
    }

    public static void Fade(this SpriteRenderer spriteRenderer, float value) {
        Color color = spriteRenderer.color;
        color.a = value;
        spriteRenderer.color = color;
    }
    public static void Fade(this Image image, float value) {
        Color color = image.color;
        color.a = value;
        image.color = color;
    }

    public static Vector3 PerpendicularDirection(this Vector3 originalDirection) {
        return new Vector3(originalDirection.y, -originalDirection.x);
    }
    public static Vector2 PerpendicularDirection(this Vector2 originalDirection) {
        return new Vector2(originalDirection.y, -originalDirection.x);
    }

    public static Vector2 GetVelocityFromRotation(Vector3 center, Vector3 objectPos, float angularVelocityDegrees) {

        // Calculate the angular velocity of the square in radians per second
        float angularVelocity = angularVelocityDegrees * Mathf.Deg2Rad;

        // Calculate the position of the circle relative to the square
        Vector3 relativePosition = objectPos - center;

        // Calculate the velocity of the circle
        Vector2 velocity = new Vector2(
            -relativePosition.y * angularVelocity,
            relativePosition.x * angularVelocity
        );

        Debug.DrawRay(objectPos, velocity);

        return velocity;
    }

    public static string ToTimerFormat(this float time) {
        int centisecondsInSecond = 100;
        int secondsInMinute = 60;

        int centiseconds = (int)((time * centisecondsInSecond) % centisecondsInSecond);
        int seconds = (int)((time % secondsInMinute));
        int minutes = (int)(time / secondsInMinute);

        if (minutes == 0) {
            return FormatToTwoDigits(seconds) + ":" + FormatToTwoDigits(centiseconds);
        }
        else {
            return FormatToTwoDigits(minutes) + ":" + FormatToTwoDigits(seconds) + ":" + FormatToTwoDigits(centiseconds);
        }

        string FormatToTwoDigits(int num) {
            if (num >= 10) {
                return num.ToString();
            }
            else {
                return "0" + num.ToString();
            }
        }
    }

    public static AudioClip RandomClip(this AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

    # region Game Specific

    public static bool TryGetEndingNumber(this string objectName, char charBeforeNumber, out int endingNumber) {
        int lastUnderscoreIndex = objectName.LastIndexOf(charBeforeNumber);
        if (lastUnderscoreIndex != -1 && lastUnderscoreIndex < objectName.Length - 1) {
            string endingNumberString = objectName.Substring(lastUnderscoreIndex + 1);
            if (int.TryParse(endingNumberString, out int _endingNumber)) {
                endingNumber = _endingNumber;
                return true;
            }
        }

        endingNumber = -1;
        return false;
    }

    #endregion
}
