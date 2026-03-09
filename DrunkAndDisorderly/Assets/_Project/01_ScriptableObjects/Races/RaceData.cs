using UnityEngine;

[CreateAssetMenu(fileName = "NewRace", menuName = "DrunkAndDisorderly/RaceData")]
public class RaceData : ScriptableObject
{
    [Header("Basic Info")]
    public string raceName;
    public string description;

    [Header("Behavior")]
    public float basePatience = 10f;
    public float patienceRate = 1f; // скорость падения терпения
    public float moveSpeed = 2f;

    [Header("Visuals")]
    public Color skinColor = Color.white;
    public Material clothesMaterial;
    public RuntimeAnimatorController animator;
    public GameObject modelPrefab;

    [Header("Audio")]
    public AudioClip happySound;
    public AudioClip angrySound;
    public AudioClip specialSound;

    [Header("Preferences")]
    public DrinkData[] preferredDrinks; // напитки, вызывающие спецэффект
    public DrinkData[] regularDrinks; // обычные напитки (без эффекта)
}