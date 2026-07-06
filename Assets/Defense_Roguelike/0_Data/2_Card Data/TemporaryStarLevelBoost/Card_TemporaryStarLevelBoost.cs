using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_TemporaryStarLevelBoost", menuName = "Card Effect/Card_TemporaryStarLevelBoost")]
public class Card_TemporaryStarLevelBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Unit> units = FieldManager.Inst.GetFieldBatchUnits();
        if (units == null || units.Count == 0)
            return;

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == null)
                continue;

            units[i].unitEvent.ApplyLevelUpUnit((int)Amount);
        }

        CoroutineRunner.Inst.StartCoroutine(TemporaryStarLevelRoutine((int)Amount));
    }

    IEnumerator TemporaryStarLevelRoutine(int Amount)
    {
        List<Unit> startUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (startUnits == null || startUnits.Count == 0)
            yield break;

        int curWave = GameManager.Inst.wave;
        int endWave = curWave + 3;

        yield return new WaitUntil(() => GameManager.Inst.wave >= endWave);

        List<Unit> endUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (endUnits == null || endUnits.Count == 0)
            yield break;

        for (int i = 0; i < endUnits.Count; i++)
        {
            if (endUnits[i] == null)
                continue;
            endUnits[i].unitEvent.ApplyLevelDownUnit(Amount);
        }
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Fail);
    }
}
