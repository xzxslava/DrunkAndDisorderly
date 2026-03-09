using UnityEngine;

[CreateAssetMenu(fileName = "NewDrink", menuName = "DrunkAndDisorderly/DrinkData")]
public class DrinkData : ScriptableObject
{
    [Header("Basic Info")]
    public string drinkName;
    [TextArea] public string description;
    public int price = 10;
    public float preparationTime = 2f;

    [Header("Unlock")]
    public int unlockDay = 1; // с какого дня доступен

    [Header("Visuals")]
    public Sprite icon;
    public GameObject drinkPrefab; // 3D-модель напитка
    public Color drinkColor = Color.white;

    [Header("Effects")]
    public EffectData[] specialEffects; // для рас, на которые действует

    [Header("Audio")]
    public AudioClip pourSound;
    public AudioClip readySound;
}