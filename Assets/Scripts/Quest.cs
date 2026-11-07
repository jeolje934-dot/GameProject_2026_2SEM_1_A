using UnityEngine;

public enum QuestState
{
    NotStated,
    InProgress,
    ReadyToComplete,
    Completed
}

[System.Serializable]
public class Quest
{
    public string questName = "아이템 수집";
    public string targetItemName = "사과";
    public int targetAmount = 3;

    [HideInInspector] public int currentAmount;
    [HideInInspector] public QuestState state = QuestState.NotStated;
}

