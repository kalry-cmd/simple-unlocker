using System.Collections.Generic;
using System.Diagnostics;

namespace SU.Classes.TaskManager;

internal class ProcessComparer : IEqualityComparer<Process>
{
	public bool Equals(Process x, Process y)
	{
		return x.Id == y.Id;
	}

	public int GetHashCode(Process obj)
	{
		return (obj.Id + obj.ProcessName).GetHashCode();
	}
}
