using System;
using TaskScheduler;

namespace SU.Classes.Autorun;

public class TaskInfo
{
	public string Name;

	public string Description;

	public string Path;

	public bool Enabled;

	public DateTime LastRunTime;

	public int LastRunResult;

	public DateTime NextRunTime;

	public string State;

	public TaskAction[] TaskAction;

	public Trigger[] Trigger;

	public TaskInfo(string name, string description, string path, bool enabled, DateTime lastruntime, int lastrunresult, DateTime nextruntime, _TASK_STATE state, TaskAction[] taskaction, Trigger[] trigger)
	{
		Name = name;
		Path = path;
		Enabled = enabled;
		LastRunTime = lastruntime;
		LastRunResult = lastrunresult;
		NextRunTime = nextruntime;
		TaskAction = taskaction;
		Description = description;
		Trigger = trigger;
		switch (state)
		{
		case _TASK_STATE.TASK_STATE_UNKNOWN:
			State = "Неизвестно";
			break;
		case _TASK_STATE.TASK_STATE_DISABLED:
			State = "Выключена";
			break;
		case _TASK_STATE.TASK_STATE_QUEUED:
			State = "В очереди";
			break;
		case _TASK_STATE.TASK_STATE_READY:
			State = "Готово";
			break;
		case _TASK_STATE.TASK_STATE_RUNNING:
			State = "Работает";
			break;
		}
	}
}
