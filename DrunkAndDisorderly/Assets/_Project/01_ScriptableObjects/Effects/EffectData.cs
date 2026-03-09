using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "DrunkAndDisorderly/EffectData")]
public class EffectData : ScriptableObject
{
    [Header("Basic Info")]
    public string effectName;
    public string description;

    [Header("Target")]
    public RaceData targetRace; // на какую расу действует
    public DrinkData triggerDrink; // какой напиток активирует

    [Header("Effect Parameters")]
    public EffectType effectType;
    public float duration = 3f;
    public float radius = 2f;
    public int power = 1; // сила эффекта (урон, лечение и т.д.)

    [Header("Visuals")]
    public Color effectColor = Color.red;
    public GameObject vfxPrefab;
    public Material effectMaterial;

    [Header("Audio")]
    public AudioClip startSound;
    public AudioClip loopSound;
    public AudioClip endSound;

    public enum EffectType
    {
        Berserk,      // орк: вращение, урон
        GoldRain,     // гоблин: монеты
        TimeSlow,     // эльф: замедление времени
        Aggression,   // человек: атака
        Healing,      // фея: лечение
        None
    }
}