using UnityEngine;
using System.Collections;

public class CustomerEffects : MonoBehaviour
{
    public GameObject berserkEffectPrefab;
    public GameObject goldRainEffectPrefab;
    public GameObject timeSlowEffectPrefab;
    public GameObject aggressionEffectPrefab;
    public GameObject healingEffectPrefab;

    private CustomerBase customer;

    private void Awake()
    {
        customer = GetComponent<CustomerBase>();
    }

    public void ActivateEffect(EffectData effect)
    {
        if (effect == null) return;

        Debug.Log($"Activating effect: {effect.effectName} on {customer.race?.raceName}");

        switch (effect.effectType)
        {
            case EffectData.EffectType.Berserk:
                StartCoroutine(BerserkRoutine(effect));
                break;
            case EffectData.EffectType.GoldRain:
                StartCoroutine(GoldRainRoutine(effect));
                break;
            case EffectData.EffectType.TimeSlow:
                StartCoroutine(TimeSlowRoutine(effect));
                break;
            case EffectData.EffectType.Aggression:
                StartCoroutine(AggressionRoutine(effect));
                break;
            case EffectData.EffectType.Healing:
                StartCoroutine(HealingRoutine(effect));
                break;
        }

        // Визуальный эффект (частицы)
        SpawnEffectVFX(effect);
    }

    private IEnumerator BerserkRoutine(EffectData effect)
    {
        Debug.Log("🧌 ORC BERSERK! Вращается и атакует всех вокруг");

        // Временно увеличиваем скорость вращения/анимации
        float duration = effect.duration > 0 ? effect.duration : 3f;

        // TODO: найти всех посетителей в радиусе и нанести урон

        yield return new WaitForSeconds(duration);

        Debug.Log("Berserk ended");
    }

    private IEnumerator GoldRainRoutine(EffectData effect)
    {
        Debug.Log("💰 GOBlIN GOLD RAIN! Монеты летят во все стороны");

        // Создаём несколько монет вокруг
        int coinCount = effect.power > 0 ? effect.power : 5;

        for (int i = 0; i < coinCount; i++)
        {
            // TODO: создать монету в случайном месте
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator TimeSlowRoutine(EffectData effect)
    {
        Debug.Log("⏰ ELF TIME SLOW! Время замедляется");

        // Замедляем время
        Time.timeScale = 0.5f;

        float duration = effect.duration > 0 ? effect.duration : 5f;
        yield return new WaitForSeconds(duration);

        // Возвращаем нормальное время
        Time.timeScale = 1f;
    }

    private IEnumerator AggressionRoutine(EffectData effect)
    {
        Debug.Log("👨 HUMAN AGGRESSION! Атакует ближайшего");

        // TODO: найти ближайшего посетителя и атаковать

        yield return null;
    }

    private IEnumerator HealingRoutine(EffectData effect)
    {
        Debug.Log("🧚 FAIRY HEALING! Лечит всех вокруг");

        // TODO: восстановить терпение себе и соседям

        yield return null;
    }

    private void SpawnEffectVFX(EffectData effect)
    {
        GameObject prefab = GetEffectPrefab(effect.effectType);
        if (prefab == null) return;

        Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity, transform);
    }

    private GameObject GetEffectPrefab(EffectData.EffectType type)
    {
        switch (type)
        {
            case EffectData.EffectType.Berserk: return berserkEffectPrefab;
            case EffectData.EffectType.GoldRain: return goldRainEffectPrefab;
            case EffectData.EffectType.TimeSlow: return timeSlowEffectPrefab;
            case EffectData.EffectType.Aggression: return aggressionEffectPrefab;
            case EffectData.EffectType.Healing: return healingEffectPrefab;
            default: return null;
        }
    }
}