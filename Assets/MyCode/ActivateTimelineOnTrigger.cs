using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ActivateTimelineOnTrigger : MonoBehaviour
{
    // ������ �� Timeline Asset
    public TimelineAsset timelineAsset;

    // ������ �� Playback Director
    public PlayableDirector playableDirector;

    // ������ �� �������
    public GameObject triggerObject;

    // ����, ������������, ���� �� ��� �������� �������� Timeline
    private bool timelineStarted = false;

    void Start()
    {
        // �������� �� ������� ���� ����������� �����������
        if (timelineAsset == null || playableDirector == null || triggerObject == null)
        {
            Debug.LogError("�� ��� ���������� ���������! ��������� ���������.");
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ��������, ��� �������� ��������� � ���������
        if (other.gameObject == triggerObject)
        {
            // ��������, ���� �� ��� �������� �������� Timeline
            if (!timelineStarted)
            {
                // ������ �������� Timeline
                playableDirector.playableAsset = timelineAsset;
                playableDirector.Play();
                timelineStarted = true;
            }
        }
    }
}