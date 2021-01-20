using System.IO;
using System.Linq;
using System.Collections.Generic;
using BizHawk.Common;
using SharpCompress.Archives;

namespace BizHawk.Client.Common
{
	/// <summary>A <see cref="IFileDearchivalMethod{T}">dearchival method</see> for <see cref="HawkFile"/> implemented using <c>SharpCompress</c> from NuGet.</summary>
	public class SharpCompressDearchivalMethod : IFileDearchivalMethod<SharpCompressArchiveFile>
	{
		private SharpCompressDearchivalMethod() {}

		public bool CheckSignature(string fileName, out int offset, out bool isExecutable)
		{
			offset = 0;
			isExecutable = false;

			try
			{
				using var arcTest = ArchiveFactory.Open(fileName);
			}
			catch
			{
				return false;
			}
			return true; // no exception? good enough
		}

		public SharpCompressArchiveFile Construct(string path) => new SharpCompressArchiveFile(path);

		public static readonly SharpCompressDearchivalMethod Instance = new SharpCompressDearchivalMethod();

		//don't try any .tar.* formats, they don't work
		//don't try .gz, it's illogical (gz contains no useful archive directory information. we would need to synthesize some.)
		static readonly IReadOnlyCollection<string> archiveExts = new[] { ".zip", ".7z", ".rar" };

		public IReadOnlyCollection<string> AllowedArchiveExtensions { get { return archiveExts; } }
	}
}