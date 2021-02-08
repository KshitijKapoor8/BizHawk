﻿using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using BizHawk.Common.StringExtensions;
using BizHawk.Emulation.Common;

namespace BizHawk.Tests.Emulation.Common
{
	[TestClass]
	public class FirmwareDatabaseTests
	{
		[TestMethod]
		public void CheckFormatOfHashes()
		{
			static void CustomAssert(string hash)
				=> Assert.IsTrue(hash.Length == 40 && hash == hash.ToUpperInvariant() && hash.IsHex(), $"incorrectly formatted: {hash}");
			foreach (var hash in FirmwareDatabase.FirmwareFilesByHash.Keys) CustomAssert(hash);
			foreach (var fo in FirmwareDatabase.FirmwareOptions) CustomAssert(fo.Hash);
		}
	}
}
