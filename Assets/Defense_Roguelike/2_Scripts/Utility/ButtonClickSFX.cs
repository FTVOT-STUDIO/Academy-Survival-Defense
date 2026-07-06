using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClickSFX : MonoBehaviour
{
    public void OnBtnClickSFX()
    {
        if (AudioManager.Inst == null)
            return;
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ButtonClick2);
    }

    public void OnToggleClickSFX()
    {
        if (AudioManager.Inst == null)
            return;
        Toggle toggle = GetComponentInChildren<Toggle>();
        if (!toggle.isOn)
            return;
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ButtonClick2);
    }
}
