using System;
using System.Runtime.InteropServices;
using System.Xml;
using TaskScheduler;

namespace SU.Classes.Autorun;

internal class TaskCharp
{
	public delegate void TaskSchelderTaskHandler(IRegisteredTask task);

	public delegate void TaskSchelderFolderHandler(ITaskFolder task);

	public delegate void TaskSchelderHandlerStart();

	public delegate void TaskSchelderHandler();

	private const int TASK_FLAG_HIDDEN = 1;

	private global::TaskScheduler.TaskScheduler taskService;

	public ITaskFolder current { get; set; }

	public ITaskFolder parent { get; set; }

	private bool init { get; set; }

	public event TaskSchelderTaskHandler ActOnTask;

	public event TaskSchelderFolderHandler ActOnFolder;

	public event TaskSchelderHandlerStart ActOnStart;

	public event TaskSchelderHandler ActOnProgress;

	public TaskInfo GetTask(string name, string path = "\\")
	{
		ITaskFolder folder = taskService.GetFolder("\\");
		if (path != "\\")
		{
			folder = taskService.GetFolder(path.Remove(path.LastIndexOf(name) - 1));
		}
		foreach (IRegisteredTask task in folder.GetTasks(1))
		{
			if (!(task.Name == name))
			{
				continue;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(task.Definition.XmlText);
			TaskAction[] array = new TaskAction[xmlDocument["Task"]["Actions"].ChildNodes.Count];
			Trigger[] array2 = new Trigger[xmlDocument["Task"]["Triggers"].ChildNodes.Count];
			for (int i = 0; i < xmlDocument["Task"]["Triggers"].ChildNodes.Count; i++)
			{
				array2.SetValue(new Trigger(xmlDocument["Task"]["Triggers"].ChildNodes[i].Name, Convert.ToBoolean(xmlDocument["Task"]["Triggers"].ChildNodes[i]["Enabled"].InnerText)), i);
			}
			for (int j = 0; j < xmlDocument["Task"]["Actions"].ChildNodes.Count; j++)
			{
				XmlNodeList childNodes = xmlDocument["Task"]["Actions"].ChildNodes;
				if (childNodes[j]["Arguments"] != null)
				{
					array.SetValue(new TaskAction(childNodes[j]["Command"].InnerText, childNodes[j]["Arguments"].InnerText.Split(' ')), j);
				}
				else if (childNodes[j]["Command"]?.InnerText == null)
				{
					array.SetValue(new TaskAction("", new string[0], hasarguments: false, commandaccessible: false), j);
				}
				else
				{
					array.SetValue(new TaskAction(childNodes[j]["Command"].InnerText, new string[0], hasarguments: false), j);
				}
			}
			return new TaskInfo(task.Name, task.Definition.RegistrationInfo.Description, task.Path, task.Enabled, task.LastRunTime, task.LastTaskResult, task.NextRunTime, task.State, array, array2);
		}
		return null;
	}

	private void InitTaskSharp()
	{
		if (!init)
		{
			taskService = (global::TaskScheduler.TaskScheduler)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
			taskService.Connect(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			init = true;
		}
	}

	private void EnumFolderTasks(ITaskFolder fld)
	{
		if (this.ActOnStart != null)
		{
			this.ActOnStart();
		}
		current = fld;
		if (this.ActOnFolder != null)
		{
			foreach (ITaskFolder folder in fld.GetFolders(1))
			{
				this.ActOnFolder(folder);
			}
		}
		if (this.ActOnTask != null)
		{
			foreach (IRegisteredTask task in fld.GetTasks(1))
			{
				this.ActOnTask(task);
			}
		}
		if (this.ActOnProgress != null)
		{
			this.ActOnProgress();
		}
		parent = taskService.GetFolder(fld.Path.Remove(current.Path.LastIndexOf('\\')));
	}

	public void EnumAllTasks(string path = "\\")
	{
		InitTaskSharp();
		ITaskFolder fld = (current = taskService.GetFolder(path));
		if (current.Path.Length < current.Path.LastIndexOf('\\'))
		{
			parent = taskService.GetFolder(path.Remove(current.Path.LastIndexOf('\\')));
		}
		else
		{
			parent = taskService.GetFolder("\\");
		}
		EnumFolderTasks(fld);
	}
}
