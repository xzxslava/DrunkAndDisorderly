using UnityEngine;

public class CustomerEffects : MonoBehaviour
{
    private CustomerBase customer;
    private Renderer[] renderers;
    private Color originalColor;

    private void Awake()
    {
        customer = GetComponent<CustomerBase>();
        renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
        }
    }

    public void ActivateEffect(EffectData effect)
    {
        Debug.Log($"Activating effect: {effect.effectName} on {customer.race.raceName}");

        switch (effect.effectType)
        {
            case EffectData.EffectType.Berserk:
                StartBerserk(effect);
                break;
            case EffectData.EffectType.GoldRain:
                StartGoldRain(effect);
                break;
            case EffectData.EffectType.TimeSlow:
                StartTimeSlow(effect);
                break;
            case EffectData.EffectType.Aggression:
                StartAggression(effect);
                break;
            case EffectData.EffectType.Healing:
                StartHealing(effect);
                break;
        }

        // Визуальный эффект (свечение)
        StartCoroutine(FlashEffect(effect.effectColor, effect.duration));
    }

    private void StartBerserk(EffectData effect)
    {
        // TODO: реализация берсерка (вращение, урон)
        // Пока просто заглушка
        Debug.Log("Orc goes BERSERK!");
    }

    private void StartGoldRain(EffectData effect)
    {
        // TODO: гоблин раскидывает монеты
        Debug.Log("Goblin drops coins!");
    }

    private void StartTimeSlow(EffectData effect)
    {
        // TODO: замедление времени
        Debug.Log("Time slows down!");
    }

    private void StartAggression(EffectData effect)
    {
        // TODO: атака ближайшего
        Debug.Log("Human attacks!");
    }

    private void StartHealing(EffectData effect)
    {
        // TODO: лечение соседей
        Debug.Log("Fairy heals!");
    }

    private System.Collections.IEnumerator FlashEffect(Color color, float duration)
    {
        float elapsed = 0f;

        // Меняем цвет всех материалов
        foreach (var rend in renderers)
        {
            rend.material.color = color;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаем исходный цвет
        foreach (var rend in renderers)
        {
            rend.material.color = originalColor;
        }
    }
}