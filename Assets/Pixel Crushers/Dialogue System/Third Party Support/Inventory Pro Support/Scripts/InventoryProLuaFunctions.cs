// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using Devdog.General;

namespace PixelCrushers.DialogueSystem.InventoryProSupport
{

    /// <summary>
    /// Adds additional Inventory Pro Lua functions.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Inventory Pro/Inventory Pro Lua Functions (on Dialogue Manager)")]
    public class InventoryProLuaFunctions : MonoBehaviour
    {

        private void OnEnable()
        {
            Lua.RegisterFunction("GetStat", this, SymbolExtensions.GetMethodInfo(() => GetStat(string.Empty, string.Empty)));
            Lua.RegisterFunction("SetStat", this, SymbolExtensions.GetMethodInfo(() => SetStat(string.Empty, string.Empty, (double)0)));
        }

        private void OnDisable()
        {
            Lua.UnregisterFunction("GetStat");
            Lua.UnregisterFunction("SetStat");
        }

        private double GetStat(string statCategory, string statName)
        {
            if (PlayerManager.instance.currentPlayer == null || PlayerManager.instance.currentPlayer.inventoryPlayer == null ||
                PlayerManager.instance.currentPlayer.inventoryPlayer.stats == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: GetStat(" + statCategory + ", " + statName + "): Can't find current player's stats.");
                return 0;
            }
            else
            {
                var stat = PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get(statCategory, statName);
                if (stat == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: GetStat(" + statCategory + ", " + statName + "): Current player doesn't have a stat with this name.");
                    return 0;
                }
                else
                {
                    if (DialogueDebug.logInfo) Debug.Log("Dialogue System: GetStat(" + statCategory + ", " + statName + ")");
                    return stat.currentValue;
                }
            }
        }

        private void SetStat(string statCategory, string statName, double value)
        {
            if (PlayerManager.instance.currentPlayer == null || PlayerManager.instance.currentPlayer.inventoryPlayer == null ||
                PlayerManager.instance.currentPlayer.inventoryPlayer.stats == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: SetStat(" + statCategory + ", " + statName + ", " + value + "): Can't find current player's stats.");
            }
            else
            {
                var stat = PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get(statCategory, statName);
                if (stat == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: SetStat(" + statCategory + ", " + statName + ", " + value + "): Current player doesn't have a stat with this name.");
                }
                else
                {
                    if (DialogueDebug.logInfo) Debug.Log("Dialogue System: SetStat(" + statCategory + ", " + statName + ", " + value + ")");
                    stat.SetCurrentValueRaw((float)value);
                }
            }
        }

    }
}