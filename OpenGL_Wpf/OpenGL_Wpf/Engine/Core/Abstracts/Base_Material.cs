﻿using InSitU.Views.ThreeD.Engine.Core.Interfaces;
using InSitU.Views.ThreeD.Engine.ImGui_Set.Controls;
using InSitU.Views.ThreeD.Engine.Opticals;
using InSitU.Views.ThreeD.Engine.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSitU.Views.ThreeD.Engine.Core.Abstracts
{
    public class Base_Material :IMaterial
    {
        public  Gloss Glossiness { get; set; }
        public string Name { get ; set ; }
        public int Id { get ; set ; }
        public IRenderable.BoundingBox BBX { get ; set ; }
        public ImgUI_Controls Ui_Controls { get ; set ; }
        public AnimationComponent Animate { get; set; }

        public void BuildModel()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Live_Update()
        {
           
        }

        public void Live_Update(Shader ShaderModel)
        {
            throw new NotImplementedException();
        }

        public IRenderable Load(string path)
        {
            throw new NotImplementedException();
        }

        public void PostRender(Shader ShaderModel)
        {
            throw new NotImplementedException();
        }

        public void PrepareForRender(Shader shaderModel)
        {
            throw new NotImplementedException();
        }

        public void RenderModel()
        {
            throw new NotImplementedException();
        }

        public string Save()
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

        public virtual void UploadDefaults(Shader ShaderModel)
        {
            Glossiness?.UploadDefaults(ShaderModel);    
        }

         
    }
}