﻿using InSitU.Views.ThreeD.Engine.Core.Abstracts;
using InSitU.Views.ThreeD.Engine.Core.Interfaces;
using InSitU.Views.ThreeD.Engine.Geometry;
using InSitU.Views.ThreeD.Engine.Geometry.Core;
using InSitU.Views.ThreeD.Engine.Geometry.Cube;
using InSitU.Views.ThreeD.Engine.Illumination.Render;
using InSitU.Views.ThreeD.Engine.Render;
using InSitU.Views.ThreeD.Engine.Space;
using InSitU.Views.ThreeD.ToolBox;
using OfficeOpenXml.Drawing.Chart;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSitU.Views.ThreeD.Engine.Illumination
{
    public class SkyBox : Base_Geo3D
    {
        public float BlendFactor { get; set; } = .2f;

        public SkyBox(CubeModel cube) : base(cube)
        {
            CullMode = CullFaceMode.FrontAndBack;
            IsSystemModel = true;
        }

        private int isDay = 1;

        public override void BuildModel()
        {
            ShaderModel = new Shader(ShaderMapType.LoadCubeTexture, ShaderPath.SkyBox);
            ShaderModel.BrightnessLevels = 10;
            TextureModel = new SkyBoxTexture();
        }

        public override void Setup_Position()
        {
        }

        public override void Setup_TextureCoordinates(float xScale = 1, float yScale = 1)
        {
        }

        public override void Setup_Normals()
        {
        }

        public override void Setup_Indeces()
        {
        }

        public void AnimateBlend()
        {
            Rotate(.01f, new Vector3(0, 1, 0));
            BlendFactor += .001f * isDay;
            BlendFactor = MathHelper.Clamp(BlendFactor, 0, 1);
            if (BlendFactor == 1)
            {
                isDay = -1;
            }
            if (BlendFactor == 0)
            {
                isDay = 1;
            }
        }

        private void AnimateRotation()
        {
            LocalTransform = eMath.MoveTo(LocalTransform, CameraModel.ActiveCamera.Position);
            Rotate(.01f, Vector3.UnitY);
        }

        public override void Live_Update(Shader ShaderModel)
        {
            base.Live_Update(ShaderModel);    
            AnimateBlend();
            AnimateRotation();

            ShaderModel.SetFloat(ShaderModel.BlendFactorLocation, BlendFactor);
            ShaderModel.SetMatrix4(ShaderModel. Location_LocalTransform, LocalTransform);
             
        }

        public override void UploadDefaults(Shader ShaderModel)
        {
           ShaderModel. Location_LocalTransform = ShaderModel.GetLocation(nameof(LocalTransform));
            
            base.UploadDefaults(ShaderModel);
        }
        public override void RenderModel()
        {
            Renderer = new SkyBoxRenderer(this);
            Default_RenderModel();

        }

    }
}