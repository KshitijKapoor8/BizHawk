﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class HotkeyConfig : Form
	{
		public HotkeyConfig()
		{
			InitializeComponent();

			Closing += (o, e) =>
			{
				IDB_SAVE.Focus(); // A very dirty hack to avoid https://code.google.com/p/bizhawk/issues/detail?id=161
			};

			tabPage1.Focus();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			Input.Instance.ControlInputFocus(this, Input.InputFocus.Mouse, true);
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			Input.Instance.ControlInputFocus(this, Input.InputFocus.Mouse, false);
		}

		private void NewHotkeyWindow_Load(object sender, EventArgs e)
		{
			var source = new AutoCompleteStringCollection();
			source.AddRange(Global.Config.HotkeyBindings.Select(x => x.DisplayName).ToArray());

			SearchBox.AutoCompleteCustomSource = source;
			SearchBox.AutoCompleteSource = AutoCompleteSource.CustomSource;

			AutoTabCheckBox.Checked = Global.Config.HotkeyConfigAutoTab;
			DoTabs();
			DoFocus();
		}

		private void IDB_CANCEL_Click(object sender, EventArgs e)
		{
			GlobalWin.OSD.AddMessage("Hotkey config aborted");
			Close();
		}

		private void IDB_SAVE_Click(object sender, EventArgs e)
		{
			Save();
			GlobalWin.OSD.AddMessage("Hotkey settings saved");
			DialogResult = DialogResult.OK;
			Close();
		}

		private void AutoTabCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			SetAutoTab();
		}

		private void Save()
		{
			Global.Config.HotkeyConfigAutoTab = AutoTabCheckBox.Checked;

			foreach (var w in InputWidgets)
			{
				var b = Global.Config.HotkeyBindings.First(x => x.DisplayName == w.WidgetName);
				b.Bindings = w.Bindings;
			}
		}

		private IEnumerable<InputCompositeWidget> InputWidgets =>
			HotkeyTabControl.TabPages.Cast<TabPage>().SelectMany(tp => tp.Controls.OfType<InputCompositeWidget>());

		private void DoTabs()
		{
			HotkeyTabControl.SuspendLayout();
			HotkeyTabControl.TabPages.Clear();

			// Buckets
			var tabs = Global.Config.HotkeyBindings.Select(x => x.TabGroup).Distinct();

			foreach (var tab in tabs)
			{
				var tb = new TabPage { Name = tab, Text = tab };
				var bindings = Global.Config.HotkeyBindings.Where(n => n.TabGroup == tab).OrderBy(n => n.Ordinal).ThenBy(n => n.DisplayName);
				int x = UIHelper.ScaleX(6);
				int y = UIHelper.ScaleY(14);
				int iwOffsetX = UIHelper.ScaleX(110);
				int iwOffsetY = UIHelper.ScaleY(-4);
				int iwWidth = UIHelper.ScaleX(120);

				tb.SuspendLayout();

				foreach (var b in bindings)
				{
					var l = new Label
					{
						Text = b.DisplayName,
						Location = new Point(x, y),
						Size = new Size(iwOffsetX - UIHelper.ScaleX(2), UIHelper.ScaleY(15))
					};

					var w = new InputCompositeWidget
					{
						Location = new Point(x + iwOffsetX, y + iwOffsetY),
						AutoTab = AutoTabCheckBox.Checked,
						Width = iwWidth,
						WidgetName = b.DisplayName
					};

					w.SetupTooltip(toolTip1, b.ToolTip);
					toolTip1.SetToolTip(l, b.ToolTip);

					w.Bindings = b.Bindings;

					tb.Controls.Add(l);
					tb.Controls.Add(w);

					y += UIHelper.ScaleY(24);
					if (y > HotkeyTabControl.Height - UIHelper.ScaleY(35))
					{
						x += iwOffsetX + iwWidth + UIHelper.ScaleX(10);
						y = UIHelper.ScaleY(14);
					}
				}

				if (tab == "TAStudio")
				{
					tb.Controls.Add(new Label
					{
						Text = "Save States hotkeys operate with branches when TAStudio is engaged.",
						Location = new Point(x, y),
						Size = new Size(iwWidth + iwOffsetX, HotkeyTabControl.Height - y)
					});
				}

				HotkeyTabControl.TabPages.Add(tb);
				tb.ResumeLayout();
			}

			HotkeyTabControl.ResumeLayout();
		}

		private void Defaults()
		{
			foreach (var w in InputWidgets)
			{
				var b = Global.Config.HotkeyBindings.FirstOrDefault(x => x.DisplayName == w.WidgetName);
				if (b != null)
				{
					w.Bindings = b.DefaultBinding;
				}
			}
		}

		private void ClearAll(bool currentTabOnly)
		{
			if (currentTabOnly)
			{
				foreach (var w in InputWidgets)
				{
					w.Clear();
				}
			}
			else
			{
				foreach (var w in HotkeyTabControl.SelectedTab.Controls.OfType<InputCompositeWidget>())
				{
					w.Clear();
				}
			}
		}

		private void SetAutoTab()
		{
			foreach (var w in InputWidgets)
			{
				w.AutoTab = AutoTabCheckBox.Checked;
			}
		}

		private void HotkeyTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			DoFocus();
		}

		private void DoFocus()
		{
			if (HotkeyTabControl.SelectedTab != null)
			{
				foreach (var c in HotkeyTabControl.SelectedTab.Controls.OfType<InputWidget>())
				{
					c.Focus();
					return;
				}
			}
		}

		private void SearchBox_KeyDown(object sender, KeyEventArgs e)
		{
			// Tab or Enter
			if (!e.Control && !e.Alt && !e.Shift &&
				(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab))
			{
				var b = Global.Config.HotkeyBindings.FirstOrDefault(x => string.Compare(x.DisplayName, SearchBox.Text, true) == 0);

				// Found
				if (b != null)
				{
					var w = InputWidgets.FirstOrDefault(x => x.WidgetName == b.DisplayName);
					if (w != null)
					{
						HotkeyTabControl.SelectTab((TabPage)w.Parent);
						Input.Instance.BindUnpress(e.KeyCode);
						w.Focus();
					}
				}

				e.Handled = true;
			}
		}

		private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearAll(true);
		}

		private void ClearCurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearAll(false);
		}

		private void RestoreDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Defaults();
		}
	}
}
