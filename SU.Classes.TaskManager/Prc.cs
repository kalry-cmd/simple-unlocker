namespace SU.Classes.TaskManager;

internal class Prc
{
	public string ProcessName { get; set; }

	public int ProcessID { get; set; }

	public string ProcessState { get; set; }

	public string ProcessOwner { get; set; }

	public string ProcessDescription { get; set; }

	public bool? ProcessCritical { get; set; }
}
