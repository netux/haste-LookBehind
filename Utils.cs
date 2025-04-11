using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

namespace HasteLookBehindMod;

internal class Utils
{
    public static InputAction AddInputAction(string actionName, InputBinding keyboardBinding, InputBinding gamepadBinding, bool force = false)
    {
        if (GetInputBindingIdInternal(keyboardBinding) == null)
        {
            throw new ArgumentException("Keyboard Binding is missing an id. Id's are required for persisting binding changes!", "keyboardBinding");
        }
        if (GetInputBindingIdInternal(gamepadBinding) == null)
        {
            throw new ArgumentException("Gamepad Binding is missing an id. Id's are required for persisting binding changes!", "gamepadBinding");
        }

        if (keyboardBinding.groups != null)
        {
            throw new ArgumentException("Keyboard Binding cannot have a group, as it will be overwritten", "keyboardBinding");
        }

        if (gamepadBinding.groups != null)
        {
            throw new ArgumentException("Gamepad Binding cannot have a group, as it will be overwritten", "gamepadBinding");
        }

        var defaultInputActionMap = Zorro.ControllerSupport.InputHandler.Instance.Default;

        // "CaNnOt AdD, rEmOvE, oR cHaNgE eLeMeNtS oF iNpUtAcTiOnAsSeT hAsTeInPuTaCtIoNs (UnItYeNgInE.iNpUtSyStEm.InPuTaCtIoNaSsEt) WhIlE oNe Or MoRe Of ItS aCtIoNs ArE eNaBlEd"
        foreach (var actionMap in defaultInputActionMap.asset.actionMaps)
        {
            actionMap.Disable();
        }
        defaultInputActionMap.Disable();

        var oldInputAction = defaultInputActionMap.FindAction(actionName);
        if (oldInputAction != null)
        {
            if (!force)
            {
                return oldInputAction;
            }

            oldInputAction.RemoveAction();
        }

        var newInputAction = defaultInputActionMap.AddAction(actionName);

        // Probably not the best to override this without informing the user of the method, but whatever.
        keyboardBinding.groups = ";Keyboard&Mouse";
        gamepadBinding.groups = ";Gamepad";

        newInputAction.AddBinding(keyboardBinding);
        newInputAction.AddBinding(gamepadBinding);

        foreach (var actionMap in defaultInputActionMap.asset.actionMaps)
        {
            actionMap.Enable();
        }
        defaultInputActionMap.Enable();

        return newInputAction;
    }

    /*
     * Get internal InputBinding ID value because the public `id` field's getter generates an id one is not already present.
     */
    private static string? GetInputBindingIdInternal(InputBinding inputBinding)
    {
        var idField = typeof(InputBinding).GetField("m_Id", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return (string?) idField.GetValue(inputBinding);
    }

    public static void AddLocalizedString(string table, string key, string localized)
    {
        LocalizationSettings.StringDatabase
            .GetTable(table)
            .AddEntry(key, localized);
    }
}
