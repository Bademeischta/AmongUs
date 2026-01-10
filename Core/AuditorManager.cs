using System.Collections.Generic;
using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class AuditorManager
    {
        private static AuditorManager _instance;
        public static AuditorManager Instance => _instance ??= new AuditorManager();

        private readonly object _stateLock = new object();
        private readonly Dictionary<Vector2, TaskInfo> _completedTasks = new Dictionary<Vector2, TaskInfo>();

        public void AddCompletedTask(PlayerControl player, Vector2 location)
        {
            lock (_stateLock)
            {
                _completedTasks[location] = new TaskInfo(player.PlayerId, Time.time, player.Data.IsImpostor);
            }
        }

        public bool IsTaskAuditable(Vector2 location, out byte completerId, out bool wasImpostorTask)
        {
            completerId = byte.MaxValue;
            wasImpostorTask = false;
            lock (_stateLock)
            {
                if (_completedTasks.TryGetValue(location, out var taskInfo))
                {
                    if (Time.time - taskInfo.CompletionTime < 30f)
                    {
                        completerId = taskInfo.CompleterId;
                        wasImpostorTask = taskInfo.WasImpostorTask;
                        return true;
                    }
                }
            }
            return false;
        }

        public void ClearTasks()
        {
            lock (_stateLock)
            {
                _completedTasks.Clear();
            }
        }

        private class TaskInfo
        {
            public byte CompleterId { get; }
            public float CompletionTime { get; }
            public bool WasImpostorTask { get; }

            public TaskInfo(byte completerId, float completionTime, bool wasImpostorTask)
            {
                CompleterId = completerId;
                CompletionTime = completionTime;
                WasImpostorTask = wasImpostorTask;
            }
        }
    }
}
