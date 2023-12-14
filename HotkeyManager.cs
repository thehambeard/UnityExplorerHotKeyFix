using HarmonyLib;
using Kingmaker;
using Kingmaker.GameModes;
using Kingmaker.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UnityExplorerHotKeyFix.Utility;

namespace UnityExplorerHotKeyFix
{
    internal class HotkeyManager : IModEventHandler
    {
        public int Priority => throw new NotImplementedException();

        public void HandleModDisable()
        {
            UnbindKey("UEHOTKEYFIXMOUSEINSPECTUI", () => InspectUnderMouse.Instance.StartInspect(MouseInspectMode.UI));
            UnbindKey("UEHOTKEYFIXMOUSEINSPECTWORLD", () => InspectUnderMouse.Instance.StartInspect(MouseInspectMode.World));
        }

        public void HandleModEnable()
        {
            BindKey(KeyCode.F3, "UEHOTKEYFIXMOUSEINSPECTUI", () => InspectUnderMouse.Instance.StartInspect(MouseInspectMode.UI));
            BindKey(KeyCode.F4, "UEHOTKEYFIXMOUSEINSPECTWORLD", () => InspectUnderMouse.Instance.StartInspect(MouseInspectMode.World));
        }

        private void BindKey(KeyCode key, string name, Action action)
        {
            Game.Instance.Keyboard.Bind(name, action);
            Game.Instance.Keyboard.RegisterBinding(name, key, GameModeType.All,
                false, false, false, KeyboardAccess.TriggerType.KeyDown, KeyboardAccess.ModificationSide.Any);
        }

        private void UnbindKey(string name, Action action)
        {
            Game.Instance.Keyboard.Unbind(name, action);
            Game.Instance.Keyboard.UnregisterBinding(name);
        }

        [HarmonyPatch(typeof(KeyboardAccess))]
        static class KeyboardAccessPatch
        {
            [HarmonyPatch(nameof(KeyboardAccess.IsInputFieldSelected)), HarmonyPrefix]
            static bool Prefix(ref bool __result)
            {
                try
                {
                    EventSystem current = EventSystem.current;
                    if (current == null)
                        return false;
                    GameObject selectedGameObject = current.currentSelectedGameObject;
                    __result = selectedGameObject != null && selectedGameObject.GetComponent<InputField>() != null;
                    return !__result;
                }
                finally
                {
                }
            }
        }
    }
}
