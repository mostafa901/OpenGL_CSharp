﻿using com.sun.tools.@internal.ws.wsdl.framework;
using ImGuiNET;
using InSitU.Views.ThreeD.Engine.Core;
using InSitU.Views.ThreeD.Engine.Core.Abstracts;
using InSitU.Views.ThreeD.Engine.Core.Interfaces;
using InSitU.Views.ThreeD.Engine.Core.Serialize;
using InSitU.Views.ThreeD.Engine.Fonts;
using InSitU.Views.ThreeD.Engine.Fonts.Core;
using InSitU.Views.ThreeD.Engine.Geometry;
using InSitU.Views.ThreeD.Engine.Geometry.Core;
using InSitU.Views.ThreeD.Engine.Geometry.Cube;
using InSitU.Views.ThreeD.Engine.Geometry.ThreeDModels.Clips;
using InSitU.Views.ThreeD.Engine.Illumination;
using InSitU.Views.ThreeD.Engine.Illumination.Render;
using InSitU.Views.ThreeD.Engine.ImGui_Set;
using InSitU.Views.ThreeD.Engine.ImGui_Set.Controls;
using InSitU.Views.ThreeD.Engine.Particles;
using InSitU.Views.ThreeD.Engine.Particles.Render;
using InSitU.Views.ThreeD.Engine.Render;
using InSitU.Views.ThreeD.Engine.Space.Environment;
using InSitU.Views.ThreeD.Engine.Water.Render;
using InSitU.Views.ThreeD.Extentions;
using InSitU.Views.ThreeD.ToolBox;
using javax.swing;
using net.sf.mpxj.mpp;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Shared_Lib.Extention;
using Shared_Lib.Extention.Serialize_Ex;
using Shared_Lib.IO;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InSitU.Views.ThreeD.Engine.Water.Render.FBO;

namespace InSitU.Views.ThreeD.Engine.Space
{
    public class Scene : IRenderable
    {
        public List<FBO> FBOs = new List<FBO>();
        public GuiFont GuiTextModel;
        public bool IsToonMode = false;
        public Stack<IDrawable> ModelsforUpload = new Stack<IDrawable>();
        public Stack<IDrawable> ModelstoRemove = new Stack<IDrawable>();
        public Shader SelectedShader;
        public SkyBox SkyBoxModel;

        public Scene(Game mainGame)
        {
            game = mainGame;
            Lights = new List<Light>();
            BBX = new IRenderable.BoundingBox();

            Setup_RenderSetings();
            Setup_Events();
        }

        public IRenderable.BoundingBox BBX { get; set; }
        public List<CameraModel> CameraModels { get; set; } = new List<CameraModel>();

        [JsonIgnore]
        public Game game { get; }

        public List<IDrawable> geoModels { get; private set; } = new List<IDrawable>();
        public int Id { get; set; }
        public List<Light> Lights { get; set; }
        public string Name { get; set; }
        public Fog SceneFog { get; set; }
        public List<IDrawable> systemModels { get; private set; } = new List<IDrawable>();
        public ImgUI_Controls Ui_Controls { get; set; }
        public AnimationComponent Animate { get; set; }

        public void BuildModel()
        {
            SceneFog = new Fog();
            SceneFog.SetFogColor(new Vector3(.5f, .5f, .5f));
            SceneFog.Active = false;

            Setup_Camera();
            Setup_SceneLight();

            SelectedShader = new Shader(ShaderMapType.Blend, ShaderPath.SingleColor);
        }

        public void Create_UIControls()
        {
            Ui_Controls = new Imgui_Window("Scene");
            var models = geoModels.Where(o => o is Base_Geo).Where(o => !(o is ClipPlan)).Cast<Base_Geo>().ToArray();
            var i = new Imgui_ListBox(Ui_Controls, "Elements", models, (x) => { x.Set_Selected(false); }, (x) => { x.Set_Selected(true); });
        }

        void IRenderable.Dispose()
        {
            throw new NotImplementedException();
        }

        public void Live_Update(Shader ShaderModel)
        {
            ShaderModel.SetBool(ShaderModel.IsToonRenderLocation, IsToonMode);
            CameraModel.ActiveCamera.Live_Update(ShaderModel);
            SceneFog.Live_Update(ShaderModel);
            Lights.First().Live_Update(ShaderModel);
        }

        public IRenderable Load(string path)
        {
            return null;
        }

        public void PostRender(Shader ShaderModel)
        {
        }

        public void PrepareForRender(Shader shaderModel)
        {
            if (ModelstoRemove.Any())
            {
                while (ModelstoRemove.Any())
                {
                    var model = ModelstoRemove.Pop();
                    geoModels.Remove(model);
                    model.Dispose();
                }
            }

            if (!ModelsforUpload.Any()) return;

            var id = 0;
            if (geoModels.Any()) id = geoModels.Max(o => o.Id);
            int max = 10;
            if (ModelsforUpload.Count < 10)
            {
                max = ModelsforUpload.Count;
            }

            
            for (int i = 0; i < max; i++)
            {
                var model = ModelsforUpload.Pop();

                model.Id = ++id;

                model.RenderModel();

                geoModels.Add(model);
                model.UpdateBoundingBox();
                UpdateBoundingBox();
            }
           
            this.Ui_Controls.SubControls.Remove(this.Ui_Controls.SubControls.First(o => o.Name == "Elements"));
            Create_UIControls();
        }

        public void Render_UIControls()
        {
            Ui_Controls.BuildModel();
            foreach (var light in Lights)
            {
                light.Render_UIControls();
            }

            CameraModel.ActiveCamera.Render_UIControls();
        }

        public void RenderModel()
        {
            Render_UIControls();
           CameraModel.ActiveCamera.Animate.Update();
        }

        public string Save()
        {
            return this.JSerialize(JsonTools.GetSettings());
        }

        public void UpdateBoundingBox()
        {
            var model = geoModels.LastOrDefault();
            if (model != null)
            {
                var trans = model.LocalTransform.ExtractTranslation();
                BBX = new IRenderable.BoundingBox
                {
                    Max = new Vector3(model.BBX.Max.Max_X(BBX.Max), model.BBX.Max.Max_Y(BBX.Max), model.BBX.Max.Max_Z(BBX.Max)),
                    Min = new Vector3(model.BBX.Min.Min_X(BBX.Min), model.BBX.Min.Min_Y(BBX.Min), model.BBX.Min.Min_Z(BBX.Min)),
                };
            }
        }

        public void UploadDefaults(Shader ShaderModel)
        {
            CameraModel.ActiveCamera.UploadDefaults(ShaderModel);
            SceneFog.UploadDefaults(ShaderModel);
            UploadLightsDefaults(ShaderModel);
        }

        public virtual void UploadLightsDefaults(Shader ShaderModel)
        {
            foreach (var light in Lights)
            {
                light.UploadDefaults(ShaderModel);
            }
        }

        internal void ActivateModels()
        {
            foreach (var model in geoModels)
            {
                model.IsActive = true;
            }
        }

        internal void Dispose()
        {
            foreach (IDrawable geo in geoModels)
            {
                RemoveModels(geo);
            }
            foreach (var fbo in FBOs)
            {
                fbo.CleanUp();
            }
        }

        internal void DisposeModels(bool disposeSystemModels = false)
        {
            foreach (var model in geoModels)
            {
                if (model.IsSystemModel && !disposeSystemModels) continue;

                ModelstoRemove.Push(model);
            }
        }

        internal void IsolateModel(Base_Geo xmodel)
        {
            foreach (var model in geoModels)
            {
                model.IsActive = false;
            }
            xmodel.IsActive = true;
        }

        internal void RemoveModels(IDrawable model)
        {
            ModelstoRemove.Push(model);
        }

        internal void Render()
        {
            RenderModel();
            for (int i = 0; i < geoModels.Count; i++)
            {
                var model = geoModels.ElementAt(i);

                if (!model.IsActive) continue;

                model.PrepareForRender(model.ShaderModel);

                model.Renderer.Draw();

                model.ShaderModel.Stop();

                model.Render_UIControls();

                if (model.Particles != null)
                {
                    foreach (var particle in model.Particles)
                    {
                        ParticleSystem.Draw(this, model);
                    }
                }
            }
        }

        internal void UpLoadModels(IDrawable model)
        {
            model.Id = geoModels.Count;
            ModelsforUpload.Push(model);
        }

        private void Game_Load(object sender, EventArgs e)
        {
            Create_UIControls();
        }

        private void Game_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Imgui_Helper.IsAnyCaptured()) return;

            if (e.IsPressed && e.Button == MouseButton.Left)
            {
                var model = CameraModel.ActiveCamera.PickObject(e.Position, this, game.gameFbos.mTargets_FBO);
                return;
            }
        }

        private void Setup_Camera()
        {
            var ActiveCamera = new CameraModel(Game.Context.ActiveScene, true);
            ActiveCamera.Position = new Vector3(-2, 0, -2);
            ActiveCamera.Target = new Vector3(0, 0, 0);
            ActiveCamera.Setup_Events();
            ActiveCamera.UpdateCamera();

            CameraModel.ActiveCamera = ActiveCamera;
        }

        private void Setup_Events()
        {
            game.Load += Game_Load;
            game.MouseDown += Game_MouseDown;
        }

        private void Setup_RenderSetings()
        {
            GL.Enable(EnableCap.DepthTest); //requires ClearBufferMask.DepthBufferBit @ render frame
            GL.Enable(EnableCap.CullFace); //avoid rendering Faces that are.
            GL.CullFace(CullFaceMode.Back); //back from Camera
            GL.ShadeModel(ShadingModel.Flat);
            GL.LineWidth(1f); //greater than 1 will cause error in future opengl versions

            //GL.Enable(EnableCap.ScissorTest);
            //GL.Scissor(200, 200, 600, 600);
        }

        private void Setup_SceneLight()
        {
            var sunlight = new Light();
            sunlight.LightColor = new Vector4(1.3f);
            sunlight.LightPosition = new Vector3(-10000, 10000, -10000);
        }
    }
}