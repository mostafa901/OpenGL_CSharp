﻿using com.sun.org.apache.bcel.@internal.classfile;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using InSitU.Views.ThreeD.Engine.Core;
using InSitU.Views.ThreeD.Engine.Core.Abstracts;
using InSitU.Views.ThreeD.Engine.Core.Interfaces;
using InSitU.Views.ThreeD.Engine.Fonts.Core;
using InSitU.Views.ThreeD.Engine.Geometry.Core;
using InSitU.Views.ThreeD.Engine.ImGui_Set.Controls;
using InSitU.Views.ThreeD.Engine.Render;
using InSitU.Views.ThreeD.Engine.Render.Texture;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Shared_Lib.Extention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSitU.Views.ThreeD.Engine.Fonts
{
    public class GuiFont :IRenderable
    {
        private List<CharacterModel> characterModels;

        public string Text { get; }
        public float LineWidth { get; }
        public Shader ShaderModel { get; set; }
        public Base_Texture TextureModel { get; set; }
        public Vector2 TextPosition { get; set; }
        public string Name { get ; set ; }
        public int Id { get ; set ; }
        public IRenderable.BoundingBox BBX { get ; set ; }
        public ImgUI_Controls Ui_Controls { get ; set ; }
        public AnimationComponent Animate { get; set; }

        public GuiFont(string text, float lineWidth)
        {
            Text = text;
            LineWidth = lineWidth;
        }

        public void BuildModel()
        {
            characterModels = new List<CharacterModel>();
            ShaderModel = new FontShader(ShaderMapType.LoadColor, ShaderPath.Font);
            TextureModel = new TextureSample2D(FontFactory.imgFontPath, TextureUnit.Texture0);
            ShaderModel.UploadDefaults(null);

            CharacterModel prev = null;
            float offsetx = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                var c = Text[i];
                var tmodel = FontFactory.GetCharacterModel(c);

                tmodel.ShaderModel = ShaderModel;
                tmodel.TextureModel = TextureModel;

                tmodel.BuildModel();
                tmodel.RenderModel();
                if (offsetx > LineWidth)
                {
                    offsetx = 0;
                    TextPosition += new Vector2(0, -tmodel.scaledHeight - .035f);
                    prev = null;
                }

                tmodel.MoveTo(new Vector3(TextPosition));

                if (prev != null)
                {
                    offsetx += tmodel.GetWidth() + tmodel.XOffset + prev.Advance;
                    tmodel.MoveWorld(new Vector3(offsetx, 0, 0) * tmodel.scaleValue);
                }
                tmodel.MoveWorld(new Vector3(0, -tmodel.YOffset, 0) * tmodel.scaleValue);

                tmodel.Scale(tmodel.scaleValue);

                prev = tmodel;
                characterModels.Add(tmodel);
            }
        }

        

        public void RenderModel()
        {
            PrepareForRender(ShaderModel);

            for (int i = 0; i < characterModels.Count; i++)
            {
                var rawModel = characterModels[i];
                rawModel.Live_Update(ShaderModel);

                rawModel.Renderer.Draw();
            }

            PostRender(ShaderModel);
        }

        

        public void CleanUp()
        {
        }

        public void Dispose()
        {
            foreach (var raw in characterModels)
            {
                raw.Renderer.Dispose();
            }
        }

        public void Live_Update(Shader ShaderModel)
        {
            throw new NotImplementedException();
        }

        public void PostRender(Shader ShaderModel)
        {
            ShaderModel.Stop();
            GL.Enable(EnableCap.DepthTest);
        }

        public void UploadDefaults(Shader ShaderModel)
        {
            throw new NotImplementedException();
        }

        public void PrepareForRender(Shader shaderModel)
        {

            GL.Disable(EnableCap.DepthTest);
            ShaderModel.Use();
            TextureModel.Live_Update(ShaderModel);
        }

        public string Save()
        {
            throw new NotImplementedException();
        }

        public IRenderable Load(string path)
        {
            throw new NotImplementedException();
        }

        public void Render_UIControls()
        {
            throw new NotImplementedException();
        }

        public void UpdateBoundingBox()
        {
            throw new NotImplementedException();
        }

        public void Create_UIControls()
        {
            throw new NotImplementedException();
        }
    }
}