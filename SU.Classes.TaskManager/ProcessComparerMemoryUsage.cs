using System.Collections.Generic;
using System.Diagnostics;

namespace SU.Classes.TaskManager;

internal class ProcessComparerMemoryUsage : IEqualityComparer<Process>
{
	public bool Equals(Process x, Process y)
	{
		if (x.Id == y.Id)
		{
			return x.WorkingSet64 != y.WorkingSet64;
		}
		return false;
	}

	public int GetHashCode(Process obj)
	{
		return (obj.Id + obj.ProcessName).GetHashCode();
	}
}
