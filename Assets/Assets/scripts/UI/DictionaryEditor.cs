using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class DictionaryEditor : MonoBehaviour
{
    [SerializeField]
    private TaskSprite[] TaskSpriteLink;
    public Dictionary<System.Type, Sprite> SpritByTask;
    public Dictionary<Sprite, System.Type> TaskBySprite;

    void Start()
    {
        UpdateDictionary();
    }
#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateDictionary();
        }
    }
#endif

    private void UpdateDictionary()
    {
        Task[] availableTasks = GetComponents<Task>();
        for (int i = 0; i < availableTasks.Length; i++)
        {
            if (availableTasks[i].enabled)
            {
                availableTasks[i].enabled = false;
            }
        }
        if (TaskSpriteLink != null && TaskSpriteLink.Length != availableTasks.Length)
        {
            TaskSprite[] newTaskSprite = CreateTaskSprite(availableTasks);
            TaskSpriteLink = newTaskSprite;
        }
        SpritByTask = CreateDictionary(TaskSpriteLink);
        TaskSpriteLink = CreateTaskSprite(availableTasks);
    }

    protected TaskSprite[] CreateTaskSprite(Task[] availableTasks)
    {
        TaskSprite[] newTaskSprite = new TaskSprite[availableTasks.Length];
        for (int i = 0; i < newTaskSprite.Length; i++)
        {
            Type taskType = availableTasks[i].GetType();
            if (SpritByTask == null || !SpritByTask.ContainsKey(taskType))
            {
                newTaskSprite[i] = new TaskSprite(taskType, null);
            }
            else
            {
                newTaskSprite[i] = new TaskSprite(taskType, SpritByTask[taskType]);
            }
        }
        return newTaskSprite;
    }

    protected Dictionary<Type, Sprite> CreateDictionary(TaskSprite[] taskSprite)
    {
        Dictionary<Type, Sprite> taskSpriteDictionary = new Dictionary<Type, Sprite>();
        TaskBySprite = new Dictionary<Sprite, Type>();
        for (int i = 0; i < taskSprite.Length; i++)
        {
            taskSpriteDictionary.Add(Type.GetType(taskSprite[i].TaskName), taskSprite[i].Image);
            if (taskSprite[i].Image != null)
            {
                TaskBySprite.Add(taskSprite[i].Image, Type.GetType(taskSprite[i].TaskName));
            }
        }
        return taskSpriteDictionary;
    }
}

[System.Serializable]
public class TaskSprite
{
    public string TaskName;
    public Sprite Image;

    public TaskSprite(Type commandClass, Sprite image)
    {
        TaskName = commandClass.ToString();
        this.Image = image;
    }
}

