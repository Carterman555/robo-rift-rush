using Blobber;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers
{
    /// <code>
    /// transform.DestroyChildren();
    /// </code>
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) UnityEngine.Object.Destroy(child.gameObject);
    }

    public static void SetActiveChildren(this Transform t, bool active)
    {
        foreach (Transform child in t) child.gameObject.SetActive(active);
    }

    public static Transform[] GetDirectChildren(this Transform t) {
        Transform[] children = new Transform[t.childCount];
        for (int i = 0; i < t.childCount; i++) {
            children[i] = t.transform.GetChild(i);
        }
        return children;
    }

    public static void Fade(this SpriteRenderer spriteRenderer, float value)
    {
        Color color = spriteRenderer.color;
        color.a = value;
        spriteRenderer.color = color;
    }
    public static void Fade(this Image image, float value)
    {
        Color color = image.color;
        color.a = value;
        image.color = color;
    }

    public static bool TryFindByName(out GameObject gameObject, string name) {
        gameObject = GameObject.Find(name);
        if (gameObject == null) {
            Debug.LogError(gameObject + " object not found in scene!");
            return false;
        }
        return true;
    }

    public static Vector3 PerpendicularDirection(this Vector3 originalDirection) {
        return new Vector3(originalDirection.y, -originalDirection.x);
    }
    public static Vector2 PerpendicularDirection(this Vector2 originalDirection) {
        return new Vector2(originalDirection.y, -originalDirection.x);
    }

    public static Vector2 AngleToDirection(this float angle) {
        return new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
    }

    public static bool IsMouseOverUI()
    {
        if (EventSystem.current == null) EventSystem.current = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).GetComponent<EventSystem>();

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.layer == 5) return true;
        }
        return false;
    }

    # region Game Specific

    public static int TryGetEndingNumber(this string objectName, char charBeforeNumber) {
        int lastUnderscoreIndex = objectName.LastIndexOf(charBeforeNumber);
        if (lastUnderscoreIndex != -1 && lastUnderscoreIndex < objectName.Length - 1) {
            string endingNumberString = objectName.Substring(lastUnderscoreIndex + 1);
            if (int.TryParse(endingNumberString, out int endingNumber)) {
                return endingNumber;
            }
        }

        Debug.LogError("Could Not Get Ending Number: " + objectName);
        return -1;
    }

    #endregion
}
