using System.Threading.Tasks;
using Microsoft.Win32;

namespace SU.Classes.UR;

internal class Restriction
{
	public RegistryHive[] RestrictionHive { get; set; }

	public string RestrictionKey { get; set; }

	public string RestrictionName { get; set; }

	public string RestrictionDescription { get; set; }

	public object DisableValue { get; set; }

	public void Unlock(bool Delete = false)
	{
		Parallel.ForEach(RestrictionHive, delegate(RegistryHive hive)
		{
			using RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
			using RegistryKey registryKey2 = registryKey.OpenSubKey(RestrictionKey, writable: true);
			if (registryKey2 != null && registryKey2.GetValue(RestrictionName) != null)
			{
				if (Delete)
				{
					registryKey2.DeleteValue(RestrictionName);
				}
				else
				{
					registryKey2.SetValue(RestrictionName, DisableValue ?? ((object)0));
				}
			}
		});
	}
}
