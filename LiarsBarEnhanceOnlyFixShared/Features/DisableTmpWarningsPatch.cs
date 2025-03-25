using HarmonyLib;

using TMPro;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DisableTmpWarningsPatch
{
    [HarmonyPatch(typeof(TMP_Settings), nameof(TMP_Settings.warningsDisabled), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool TMP_SettingsPrefix(ref bool __result)
    {
        __result = true;
        return false;
    }

    //[HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), [typeof(object), typeof(Object)])]
    //[HarmonyPrefix]
    //public static bool LogWarningPrefix()
    //{
    //    return false;
    //}
}