namespace SU.Classes.UR;

public class FileType
{
	public string Name { get; set; }

	public string Description { get; set; }

	public string DefaultIcon { get; set; }

	public FileCommand ShellOpenCommand { get; set; }

	public FileCommand ShellRunAsCommand { get; set; }

	public FileCommand ShellRunAsUserCommand { get; set; }
}
