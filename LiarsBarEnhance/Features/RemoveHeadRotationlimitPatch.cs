using HarmonyLib;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class RemoveHeadRotationlimitPatch
{
    [HarmonyPatch(typeof(CharController), "ClampAngle")]
    [HarmonyPrefix]
    public static void ClampAnglePrefix(float lfAngle, ref float __result, ref bool __runOriginal)
    {
        if (Plugin.BooleanViewRemoveRotationLimit.Value)
        {
            __runOriginal = false;
            var newAngle = lfAngle;

            if (newAngle < -180.0f)
                newAngle += 360f;

            if (newAngle > 180.0f)
                newAngle -= 360f;

            __result = newAngle;
        } else{
            __runOriginal = true;
        }
    }
}