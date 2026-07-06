namespace FewClicksDev.TaskList
{
    using FewClicksDev.Core;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    using Preferences = TaskListUserPreferences;

    public enum TaskListType
    {
        Current = 0,
        Backlog = 1
    }

    public enum TaskPriority
    {
        Immediate = 0,
        High = 1,
        Normal = 2,
        Low = 3
    }

    [System.Flags]
    public enum TasksVisibilityMode
    {
        None = 0,
        Open = 1 << 0,
        Closed = 1 << 1,
        Immediate = 1 << 2,
        High = 1 << 3,
        Normal = 1 << 4,
        Low = 1 << 5,

        AllOpen = Open | Immediate | High | Normal | Low,
        All = ~0
    }

    public enum TasksSortMode
    {
        CreationDate = 0,
        Priority = 1,
        Label = 2,
        Custom = 3
    }

    public enum TaskContextType
    {
        Object = 0,
        Script = 1,
        Scenes = 2
    }

    public static class TaskList
    {
        public const string TASK_LIST_NAME = "TASK LIST";
        public const string VERSION = "1.1.0";

        public static readonly Color MAIN_COLOR = new Color(0.186405f, 0.217852f, 0.408805f, 1f);
        public static readonly Color LOGS_COLOR = new Color(0.238816f, 0.299039f, 0.660377f, 1f);

        public static void Log(string _message)
        {
            if (Preferences.PrintLogs == false)
            {
                return;
            }

            BaseLogger.Log(TASK_LIST_NAME, _message, LOGS_COLOR);
        }

        public static void LogWarning(string _message)
        {
            if (Preferences.PrintLogs == false)
            {
                return;
            }

            BaseLogger.Warning(TASK_LIST_NAME, _message, LOGS_COLOR);
        }

        public static void LogError(string _message)
        {
            if (Preferences.PrintLogs == false)
            {
                return;
            }

            BaseLogger.Error(TASK_LIST_NAME, _message, LOGS_COLOR);
        }

        public static string FindScenePath(string _sceneName)
        {
            string[] _guids = AssetDatabase.FindAssets($"{_sceneName} t:Scene");

            foreach (string _guid in _guids)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_guid);

                if (Path.GetFileNameWithoutExtension(_path) == _sceneName)
                {
                    return _path;
                }
            }

            LogError($"Scene '{_sceneName}' was not found in the project!");
            return string.Empty;
        }

        public static int HighestCustomOrder(this List<Task> _tasks)
        {
            if (_tasks.IsNullOrEmpty())
            {
                return Task.INVALID_CUSTOM_ORDER;
            }

            int _highest = Task.INVALID_CUSTOM_ORDER;

            foreach (var _task in _tasks)
            {
                if (_task.CustomOrder > _highest)
                {
                    _highest = _task.CustomOrder;
                }
            }

            return _highest;
        }

        public static int LowestCustomOrder(this List<Task> _tasks)
        {
            if (_tasks.IsNullOrEmpty())
            {
                return Task.INVALID_CUSTOM_ORDER;
            }

            int _lowest = int.MaxValue;

            foreach (var _task in _tasks)
            {
                if (_task.CustomOrder < _lowest)
                {
                    _lowest = _task.CustomOrder;
                }
            }

            return _lowest == int.MaxValue ? Task.INVALID_CUSTOM_ORDER : _lowest;
        }

        public static int GetNextHighestOrder(this List<Task> _selected, List<Task> _all)
        {
            if (_selected.IsNullOrEmpty() || _all.IsNullOrEmpty())
            {
                return Task.INVALID_CUSTOM_ORDER;
            }

            int _highestFromSelected = _selected.HighestCustomOrder();
            int _nextHighest = _highestFromSelected;

            foreach (var _task in _all)
            {
                if (_task.CustomOrder > _highestFromSelected)
                {
                    _nextHighest = _task.CustomOrder;
                }
            }

            return _nextHighest;
        }

        public static int GetNextLowestOrder(this List<Task> _selected, List<Task> _all)
        {
            if (_selected.IsNullOrEmpty() || _all.IsNullOrEmpty())
            {
                return Task.INVALID_CUSTOM_ORDER;
            }

            int _lowestFromSelected = _selected.LowestCustomOrder();
            int _nextLowest = _lowestFromSelected;

            foreach (var _task in _all)
            {
                if (_task.CustomOrder < _lowestFromSelected)
                {
                    _nextLowest = _task.CustomOrder;
                }
            }

            return _nextLowest;
        }
    }
}