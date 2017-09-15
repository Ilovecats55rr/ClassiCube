﻿// Copyright 2014-2017 ClassicalSharp | Licensed under BSD-3
using System;
using System.IO;
using ClassicalSharp.Entities;
using ClassicalSharp.Gui.Widgets;
using ClassicalSharp.Textures;

namespace ClassicalSharp.Gui.Screens {
	public class GraphicsOptionsScreen : MenuOptionsScreen {
		
		public GraphicsOptionsScreen(Game game) : base(game) {
		}
		
		public override void Init() {
			base.Init();
			ContextRecreated();
			MakeValidators();
			MakeDescriptions();
		}
		
		protected override void ContextRecreated() {
			ClickHandler onClick = OnWidgetClick;
			widgets = new Widget[] {
				MakeOpt(-1, -50, "FPS mode",                                     onClick, GetFPS,      SetFPS),
				MakeOpt(-1, 0, "View distance",                                  onClick, GetViewDist, SetViewDist),
				MakeBool(-1, 50, "Advanced lighting", OptionsKey.SmoothLighting, onClick, GetSmooth,   SetSmooth),
				
				MakeOpt(1, -50, "Names",                                         onClick, GetNames,    SetNames),
				MakeOpt(1, 0, "Shadows",                                         onClick, GetShadows,  SetShadows),
				MakeBool(1, 50, "Mipmaps", OptionsKey.Mipmaps,                   onClick, GetMipmaps,  SetMipmaps),
				
				MakeBack(false, titleFont, SwitchOptions),
				null, null,
			};
			
			// NOTE: we need to override the default setter here, because changing FPS limit method
			// recreates the graphics context on some backends (such as Direct3D9)
			ButtonWidget btn = (ButtonWidget)widgets[0];
			btn.SetValue = SetFPSLimitMethod;
		}

		
		static string GetFPS(Game g) { return g.FpsLimit.ToString(); }
		static void SetFPS(Game g, string v) { }
		
		static string GetViewDist(Game g) { return g.ViewDistance.ToString(); }
		static void SetViewDist(Game g, string v) { g.SetViewDistance(Int32.Parse(v), true); }
		
		static string GetSmooth(Game g) { return GetBool(g.SmoothLighting); }
		static void SetSmooth(Game g, bool v) {
			g.SmoothLighting = v;
			ChunkMeshBuilder builder = g.MapRenderer.DefaultMeshBuilder();
			g.MapRenderer.SetMeshBuilder(builder);
			g.MapRenderer.Refresh();
		}
		
		static string GetNames(Game g) { return g.Entities.NamesMode.ToString(); }
		static void SetNames(Game g, string v) {
			object rawNames = Enum.Parse(typeof(NameMode), v);
			g.Entities.NamesMode = (NameMode)rawNames;
			Options.Set(OptionsKey.NamesMode, v);
		}
		
		static string GetShadows(Game g) { return g.Entities.ShadowMode.ToString(); }
		static void SetShadows(Game g, string v) {
			object rawShadows = Enum.Parse(typeof(EntityShadow), v);
			g.Entities.ShadowMode = (EntityShadow)rawShadows;
			Options.Set(OptionsKey.EntityShadow, v);
		}
		
		static string GetMipmaps(Game g) { return GetBool(g.Graphics.Mipmaps); }
		static void SetMipmaps(Game g, bool v) {
			g.Graphics.Mipmaps = v;
			
			string url = g.World.TextureUrl;
			if (url == null) {
				TexturePack.ExtractDefault(g); return;
			}
			
			using (Stream data = TextureCache.GetStream(url)) {
				if (data == null) {
					TexturePack.ExtractDefault(g); return;
				}
				
				if (url.Contains(".zip")) {
					TexturePack extractor = new TexturePack();
					extractor.Extract(data, g);
				} else {
					TexturePack.ExtractTerrainPng(g, data, url);
				}
			}
		}
		
		void MakeValidators() {
			validators = new MenuInputValidator[] {
				new EnumValidator(typeof(FpsLimitMethod)),
				new IntegerValidator(16, 4096),
				new BooleanValidator(),
				
				new EnumValidator(typeof(NameMode)),
				new EnumValidator(typeof(EntityShadow)),
				new BooleanValidator(),
			};
		}
		
		void MakeDescriptions() {
			descriptions = new string[widgets.Length][];
			descriptions[0] = new string[] {
				"&eVSync: &fNumber of frames rendered is at most the monitor's refresh rate.",
				"&e30/60/120 FPS: &f30/60/120 frames rendered at most each second.",
				"&eNoLimit: &fRenders as many frames as possible each second.",
				"&cUsing NoLimit mode is discouraged.",
			};
			descriptions[2] = new string[] {
				"&cNote: &eSmooth lighting is still experimental and can heavily reduce performance.",
			};
			descriptions[3] = new string[] {
				"&eHovered: &fName of the targeted player is drawn see-through.",
				"&eAll: &fNames of all other players are drawn normally.",
				"&eAllHovered: &fAll names of players are drawn see-through.",
				"&eAllUnscaled: &fAll names of players are drawn see-through without scaling.",
			};
			descriptions[4] = new string[] {
				"&eNone: &fNo entity shadows are drawn.",
				"&eSnapToBlock: &fA square shadow is shown on block you are directly above.",
				"&eCircle: &fA circular shadow is shown across the blocks you are above.",
				"&eCircleAll: &fA circular shadow is shown underneath all entities.",
			};
		}
	}
}