using Landfall.Modding;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HasteLookBehindMod;

[LandfallPlugin]
public class LookBehindMod
{
    public static InputAction? LookBehindInputAction;

    static LookBehindMod()
    {
        On.HasteSettingsHandler.ctor += (original, self) =>
        {
            original(self);

            LookBehindInputAction = Utils.AddInputAction(
                actionName: $"{nameof(LookBehindMod)}--LookBehind",
                keyboardBinding: new InputBinding
                {
                    id = new Guid("1006be41-9d00-6b00-0000-000000000000"),
                    path = "<Keyboard>/q",
                    interactions = "hold(duration=0)"
                },
                gamepadBinding: new InputBinding
                {
                    id = new Guid("1006be41-9d00-6900-0000-000000000000"),
                    path = "<GamePad>/rightStickPress",
                    interactions = "hold(duration=0)"
                }
            );

            Utils.AddLocalizedString("Settings", LookBehindInputAction.name, "Look Behind");

            self.AddSetting(new InputRebindSetting(LookBehindInputAction));
        };

        On.CameraMovement.ApplyRotation += (original, self, lookRotationEulerAngles) =>
        {
            original(self, lookRotationEulerAngles);

            if (LookBehindInputAction == null)
            {
                return;
            }

            if (LookBehindInputAction.IsInProgress())
            {
                self.transform.localEulerAngles = new Vector3(
                    self.transform.localEulerAngles.x,
                    self.transform.localEulerAngles.y + 180f,
                    self.transform.localEulerAngles.z
                );
            }
        };
    }
}
