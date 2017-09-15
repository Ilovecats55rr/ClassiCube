﻿// Copyright 2014-2017 ClassicalSharp | Licensed under BSD-3
using System;
using System.Drawing;
using ClassicalSharp.Gui.Widgets;
using ClassicalSharp.Singleplayer;

namespace ClassicalSharp.Gui.Screens {
	public class MiscOptionsScreen : MenuOptionsScreen {
		
		public MiscOptionsScreen(Game game) : base(game) {
		}
		
		public override void Init() {
			base.Init();
			ContextRecreated();
			MakeValidators();
			MakeDescriptions();
		}
		
		protected override void ContextRecreated() {
			bool multi = !game.Server.IsSinglePlayer;
			ClickHandler onClick = OnWidgetClick;
			widgets = new Widget[] {
				multi ? null : MakeOpt(-1, -100, "Click distance",                    onClick, GetReach,       SetReach),
				MakeOpt(-1, -50, "Music volume",                                      onClick, GetMusic,       SetMusic),
				MakeOpt(-1, 0, "Sounds volume",                                       onClick, GetSounds,      SetSounds),
				MakeBool(-1, 50, "View bobbing", OptionsKey.ViewBobbing,              onClick, GetViewBob,     SetViewBob),

				multi ? null : MakeBool(1, -100, "Block physics", OptionsKey.Physics, onClick, GetPhysics,     SetPhysics),
				MakeBool(1, -50, "Auto close launcher", OptionsKey.AutoCloseLauncher, onClick, GetAutoClose,   SetAutoClose),
				MakeBool(1, 0, "Invert mouse", OptionsKey.InvertMouse,                onClick, GetInvert,      SetInvert),
				MakeOpt(1, 50, "Mouse sensitivity",                                   onClick, GetSensitivity, SetSensitivity),

				MakeBack(false, titleFont, SwitchOptions),
				null, null,
			};
		}
		
		static string GetReach(Game g) { return g.LocalPlayer.ReachDistance.ToString(); }
		static void SetReach(Game g, string v) { g.LocalPlayer.ReachDistance = Utils.ParseDecimal(v); }
		
		static string GetMusic(Game g) { return g.MusicVolume.ToString(); }
		static void SetMusic(Game g, string v) {
			g.MusicVolume = Int32.Parse(v);
			Options.Set(OptionsKey.MusicVolume, v);
			g.AudioPlayer.SetMusic(g.MusicVolume);
		}
		
		static string GetSounds(Game g) { return g.SoundsVolume.ToString(); }
		static void SetSounds(Game g, string v) {
			g.SoundsVolume = Int32.Parse(v);
			Options.Set(OptionsKey.SoundsVolume, v);
			g.AudioPlayer.SetSounds(g.SoundsVolume);
		}
		
		static string GetViewBob(Game g) { return GetBool(g.ViewBobbing); }
		static void SetViewBob(Game g, bool v) { g.ViewBobbing = v; }
		
		static string GetPhysics(Game g) { return GetBool(((SinglePlayerServer)g.Server).physics.Enabled); }
		static void SetPhysics(Game g, bool v) { ((SinglePlayerServer)g.Server).physics.Enabled = v; }
		
		static string GetAutoClose(Game g) { return GetBool(Options.GetBool(OptionsKey.AutoCloseLauncher, false)); }
		static void SetAutoClose(Game g, bool v) { Options.Set(OptionsKey.AutoCloseLauncher, v); }
		
		static string GetInvert(Game g) { return GetBool(g.InvertMouse); }
		static void SetInvert(Game g, bool v) { g.InvertMouse = v; }
		
		static string GetSensitivity(Game g) { return g.MouseSensitivity.ToString(); }
		static void SetSensitivity(Game g, string v) {
			g.MouseSensitivity = Int32.Parse(v);
			Options.Set(OptionsKey.Sensitivity, v);
		}
		
		void MakeValidators() {
			IServerConnection network = game.Server;
			validators = new MenuInputValidator[] {
				network.IsSinglePlayer ? new RealValidator(1, 1024) : null,
				new IntegerValidator(0, 100),
				new IntegerValidator(0, 100),
				new BooleanValidator(),
				
				network.IsSinglePlayer ? new BooleanValidator() : null,
				new BooleanValidator(),
				new BooleanValidator(),
				new IntegerValidator(1, 200),
			};
		}
		
		void MakeDescriptions() {
			descriptions = new string[widgets.Length][];
			descriptions[0] = new string[] {
				"&eSets how far away you can place/delete blocks",
				"&fThe default click distance is 5 blocks.",
			};
		}
	}
}