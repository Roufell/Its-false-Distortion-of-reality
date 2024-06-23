using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ActivateTimelineOnTrigger : MonoBehaviour
{
    // Ссылка на Timeline Asset
    public TimelineAsset timelineAsset;

    // Ссылка на Playback Director
    public PlayableDirector playableDirector;

    // Ссылка на триггер
    public GameObject triggerObject;

    // Флаг, определяющий, была ли уже запущена анимация Timeline
    private bool timelineStarted = false;

    void Start()
    {
        // Проверка на наличие всех необходимых компонентов
        if (timelineAsset == null || playableDirector == null || triggerObject == null)
        {
            Debug.LogError("Не все компоненты назначены! Проверьте настройки.");
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверка, что коллизия произошла с триггером
        if (other.gameObject == triggerObject)
        {
            // Проверка, была ли уже запущена анимация Timeline
            if (!timelineStarted)
            {
                // Запуск анимации Timeline
                playableDirector.playableAsset = timelineAsset;
                playableDirector.Play();
                timelineStarted = true;
            }
        }
    }
}