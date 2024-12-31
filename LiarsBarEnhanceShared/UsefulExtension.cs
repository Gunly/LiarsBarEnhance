using UnityEngine;

namespace LiarsBarEnhance;

public static class UsefulExtension
{
    public static T AddComponentIfNotExist<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.GetComponent<T>() == null)
            return gameObject.AddComponent<T>();
        return gameObject.GetComponent<T>();
    }

    public static Quaternion ToQuaternion(this Vector3 vector) => Quaternion.Euler(vector.x, vector.y, vector.z);


    public static bool PluginControl(this Manager manager) => !manager.GamePaused && !manager.Chatting;
}