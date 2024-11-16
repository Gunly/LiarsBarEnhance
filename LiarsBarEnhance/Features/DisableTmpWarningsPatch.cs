using HarmonyLib;
using TMPro;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DisableTmpWarningsPatch
{
    [HarmonyPatch(typeof(TMP_Settings), "warningsDisabled", MethodType.Getter)]
    [HarmonyPrefix]
    public static bool TMP_SettingsPrefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}