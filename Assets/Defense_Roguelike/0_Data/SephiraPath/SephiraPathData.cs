using UnityEngine;

[CreateAssetMenu(fileName = "SephiraPathData", menuName = "SephiraPathData", order = 1)]
public class SephiraPathData : ScriptableObject
{
    public enum SephirahPath { None, Kether, Chokhmah, Binah, Chesed, Gevurah, Tiferet, Netzach, Hod, Yesod, Malkuth, }
    public Sprite mainIcon;
    public Sprite pictogramIcon;

    public string sephiraName;
    [TextArea] public string sephiraPathDesc;
    [TextArea] public string prefix;
    public Color sephiraColor;
    public SephirahPath sephirahPath;
    // public enum DistortionSephirahPath { Thaumiel, Ghagiel, Sathariel, Gamchicoth, Golachab, Thagirion, Aarab_Zaraq, Samael, Gamaliel, Nehemoth }
}
