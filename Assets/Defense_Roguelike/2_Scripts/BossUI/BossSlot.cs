using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossSlot : MonoBehaviour
{
    public Image portrait;
    public TMP_Text nameText;

    void OnEnable()
    {
        GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 1000), 0.5f)
       .SetDelay(2.2f)
       .SetEase(Ease.InBack)
       .SetLoops(1)
       .SetAutoKill(true)
       .SetRelative(true);
    }
}
