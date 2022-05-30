﻿using System.Collections.Generic;
using System.IO;

using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Waterbox;

namespace BizHawk.Emulation.Cores.Consoles.Nintendo.VB
{
	[PortedCore(CoreNames.VirtualBoyee, "Mednafen Team", "1.29.0", "https://mednafen.github.io/releases/")]
	public class VirtualBoyee : NymaCore
	{
		[CoreConstructor(VSystemID.Raw.VB)]
		public VirtualBoyee(CoreLoadParameters<NymaSettings, NymaSyncSettings> lp)
			: base(lp.Comm, VSystemID.Raw.VB, "VirtualBoy Controller", lp.Settings, lp.SyncSettings)
		{
			DoInit<LibNymaCore>(lp, "vb.wbx");
		}

		protected override IDictionary<string, SettingOverride> SettingOverrides { get; } = new Dictionary<string, SettingOverride>
		{
			{ "vb.cpu_emulation", new() { Default = "accurate" } },
			{ "vb.input.instant_read_hack", new() { Default = "0" } },

			{ "vb.3dmode", new() { NonSync = true/*, NoRestart = true*/ } }, // fixme: a restart shouldn't be needed, yet upstream doesn't allow that?
			{ "vb.3dreverse", new() { NonSync = true } },
			{ "vb.anaglyph.lcolor", new() { NonSync = true, NoRestart = true } },
			{ "vb.anaglyph.preset", new() { NonSync = true, NoRestart = true } },
			{ "vb.anaglyph.rcolor", new() { NonSync = true, NoRestart = true } },
			{ "vb.default_color", new() { NonSync = true, NoRestart = true } },
			{ "vb.instant_display_hack", new() { NonSync = true, NoRestart = true } },
			{ "vb.ledonscale", new() { NonSync = true, NoRestart = true } },
			{ "vb.liprescale", new() { NonSync = true } },
			{ "vb.sidebyside.separation", new() { NonSync = true } },

			{ "nyma.rtcinitialtime", new() { Hide = true } },
			{ "nyma.rtcrealtime", new() { Hide = true } },
		};

		// needed if 3d mode changes can happen mid-emulation

		/*public override int VirtualHeight => BufferHeight;

		public override int VirtualWidth => BufferWidth;*/
	}
}
