﻿using Newtonsoft.Json;
using Simple_Engine.Engine.Core.Interfaces;
using Simple_Engine.Engine.Fonts;
using Simple_Engine.Engine.GameSystem;
using Simple_Engine.Engine.Illumination;
using Simple_Engine.Engine.Render;
using Simple_Engine.Engine.Space.Camera;
using Simple_Engine.Engine.Space.Environment;
using Simple_Engine.Engine.Water.Render;
using System.Collections.Generic;

namespace Simple_Engine.Engine.Space.Scene
{
    public partial class SceneModel
    {
        public List<FBO> FBOs = new List<FBO>();
        public GuiFont GuiTextModel;
        public bool IsToonMode = false;
        public Stack<IDrawable> ModelsforUpload = new Stack<IDrawable>();
        public Stack<IDrawable> ModelstoRemove = new Stack<IDrawable>();
        public Shader SelectedShader;
        public List<CameraModel> CameraModels { get; set; } = new List<CameraModel>();

        [JsonIgnore]
        public Game game { get; }

        public List<IDrawable> geoModels { get; private set; } = new List<IDrawable>();
        public int Id { get; set; }
        public List<LightModel> Lights { get; set; }
        public string Name { get; set; }
        public Fog SceneFog { get; set; }
        public List<IDrawable> systemModels { get; private set; } = new List<IDrawable>();
    }
}