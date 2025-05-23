namespace SU.Classes.Autorun;

public class TaskAction
{
	public string Command;

	public string[] Arguments;

	public bool HasArguments;

	public bool CommandAccessible;

	public TaskAction(string command, string[] arguments, bool hasarguments = true, bool commandaccessible = true)
	{
		Command = command;
		Arguments = arguments;
		HasArguments = hasarguments;
		CommandAccessible = commandaccessible;
	}
}
