using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SU.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("SU.Properties.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static Bitmap delete => (Bitmap)ResourceManager.GetObject("delete", resourceCulture);

	internal static Bitmap folder => (Bitmap)ResourceManager.GetObject("folder", resourceCulture);

	internal static Bitmap left => (Bitmap)ResourceManager.GetObject("left", resourceCulture);

	internal static Bitmap runIcon => (Bitmap)ResourceManager.GetObject("runIcon", resourceCulture);

	internal static Bitmap SULogo => (Bitmap)ResourceManager.GetObject("SULogo", resourceCulture);

	internal static Bitmap task => (Bitmap)ResourceManager.GetObject("task", resourceCulture);

	internal static Bitmap taskResize => (Bitmap)ResourceManager.GetObject("taskResize", resourceCulture);

	internal Resources()
	{
	}
}
