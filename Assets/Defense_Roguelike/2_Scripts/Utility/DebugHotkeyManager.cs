using System.Collections.Generic;
using UnityEngine;

public class DebugHotkeyManager : MonoBehaviour
{
    void Update()
    {
        if (InputKey(KeyCode.Keypad7)) GameManager.Inst.ModifyCoin(1000);
        if (InputKey(KeyCode.Keypad8)) GameManager.Inst.ModifyDarkMatteCoin(100);
        if (InputKey(KeyCode.Keypad9)) FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddAbllityStats(10);
        if (InputKey(KeyCode.Keypad4) || InputKey(KeyCode.Alpha4)) EnemySpawnManager.Inst.gameObject.SetActive(!EnemySpawnManager.Inst.gameObject.activeSelf);

        if (InputKey(KeyCode.Return))
        {
            GameManager.Inst.skillCardManager.GetSkillCard(1);
        }

        if (InputKey(KeyCode.KeypadEnter) || InputKey(KeyCode.Q))
        {
            for (int i = 0; i < EquipmentManager.Inst.lockUnitTabItems.Count; i++)
            {
                UnitTabUIInitializer unitTab = EquipmentManager.Inst.lockUnitTabItems[i].GetComponent<UnitTabUIInitializer>();
                PlayerManager.Inst.ModifyUnit(unitTab.data, 1, false);
            }

            for (int i = 0; i < EquipmentManager.Inst.lockSephiraTabItems.Count; i++)
            {
                UnitTabUIInitializer sephiraTab = EquipmentManager.Inst.lockSephiraTabItems[i].GetComponent<UnitTabUIInitializer>();
                PlayerManager.Inst.ModifyUnit(sephiraTab.data, 1, false);
            }

            for (int i = 0; i < EquipmentManager.Inst.lockFieldTabItems.Count; i++)
            {
                FieldTabUIInitializer fieldTab = EquipmentManager.Inst.lockFieldTabItems[i].GetComponent<FieldTabUIInitializer>();
                PlayerManager.Inst.UnlockField(fieldTab.data);
            }
        }

        if (InputKey(KeyCode.KeypadPlus) || InputKey(KeyCode.W))
        {
            for (int i = 0; i < EquipmentManager.Inst.unlockUnitTabItems.Count; i++)
            {
                UnitTabUIInitializer unitTab = EquipmentManager.Inst.unlockUnitTabItems[i].GetComponent<UnitTabUIInitializer>();

                unitTab.ForcingLevelUpgradeUnit(unitTab.data);
            }

            for (int i = 0; i < EquipmentManager.Inst.unlockSephiraTabItems.Count; i++)
            {
                UnitTabUIInitializer sephiraTab = EquipmentManager.Inst.unlockSephiraTabItems[i].GetComponent<UnitTabUIInitializer>();

                sephiraTab.ForcingLevelUpgradeUnit(sephiraTab.data);
            }

            for (int i = 0; i < EquipmentManager.Inst.unlockFieldTabItems.Count; i++)
            {
                FieldTabUIInitializer fieldTab = EquipmentManager.Inst.unlockFieldTabItems[i].GetComponent<FieldTabUIInitializer>();

                fieldTab.ForcingLevelUpgradeField(fieldTab.data);
            }
        }

        if (InputKey(KeyCode.KeypadMultiply))
        {
            GameStatManager.Inst.AddGameSlowValue(0.1f);
        }

        if (InputKey(KeyCode.R))
        {
            FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddAbllityStats(FieldManager.Inst.fieldSephira.maxAbllityCount);
        }
        if (InputKey(KeyCode.T))
        {
            GameStatManager.Inst.AddGameAttackSpeed(+1f);
        }

        if (InputKey(KeyCode.I))
        {
            List<Unit> batchUnits = FieldManager.Inst.GetFieldBatchUnits();
            for (int i = 0; i < batchUnits.Count; i++)
            {
                batchUnits[i].unitEvent.ApplyLevelUpUnit(1);
            }
        }

        if (InputKey(KeyCode.O))
        {
            List<Unit> batchUnits = FieldManager.Inst.GetFieldBatchUnits();
            for (int i = 0; i < batchUnits.Count; i++)
            {
                batchUnits[i].unitEvent.ApplyLevelDownUnit(1);
            }
        }
    }

    bool InputKey(KeyCode keyCode)
    {
        return Input.GetKeyDown(keyCode);
    }
}
