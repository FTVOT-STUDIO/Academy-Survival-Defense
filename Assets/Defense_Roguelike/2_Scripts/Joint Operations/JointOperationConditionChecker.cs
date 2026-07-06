using UnityEngine;

public class JointOperationConditionChecker : MonoBehaviour
{
    public void TryUnlockJointOperation()
    {
        if (EquipmentManager.Inst.unlockUnitTabItems.Count >= 8 && EquipmentManager.Inst.unlockSephiraTabItems.Count >= 3 && EquipmentManager.Inst.unlockFieldTabItems.Count >= 2)
        {
            InGameUIManager.Inst.SwitchToMainOperationPanel();
            DeckManager.Inst.jointOperationsDeckManager.InitializeJointOperationUIAndDeck();
            DeckManager.Inst.InitializeJointOperationsDeck();
            PlayerManager.Inst.isJointOperationUnlocked = true;
            GameModeManager.Inst.jointOperationsManager.curJointOperationData = GameModeManager.Inst.jointOperationsManager.GetRandomJointOperationData();
            GameModeManager.Inst.jointOperationsManager.InitJointOperationLobby();
        }
    }
}
