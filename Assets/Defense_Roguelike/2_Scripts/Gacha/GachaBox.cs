using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaBox : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    [SerializeField] GameObject shadow;
    public Button boxTabBtn;
    public TMP_Text rewardIndexText;
    public GameObject specialPortrait;
    public GameObject boxParticle;
    public GameObject specialUnitEffect;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // StartGachaSFX();
        // boxTabBtn.interactable = false;
        // boxParticle.SetActive(false);
    }

    void OnDisable()
    {
        boxParticle.SetActive(false);
    }

    void LateUpdate()
    {
        shadow.transform.position = new Vector3(transform.position.x, shadow.transform.position.y, shadow.transform.position.z);
    }

    public void StartGachaSFX()
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.spin0);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.spin1);
    }

    public void ShowFirstReward()
    {
        if (GachaManager.Inst.gachaRweardPanel.activeSelf)
            return;
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.boxOpen);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.BuyItem);
        GachaManager.Inst.TabBox();
        boxTabBtn.interactable = true;
        boxParticle.SetActive(true);
    }
}
