﻿using DocumentFormat.OpenXml.Office.Word;
using InSitU.Views.ThreeD.Engine.Core.Abstracts;
using InSitU.Views.ThreeD.Engine.Geometry.Core;
using InSitU.Views.ThreeD.Engine.Core.Events;
using InSitU.Views.ThreeD.Engine.Geometry.InputControls;
using InSitU.Views.ThreeD.Engine.Particles;
using InSitU.Views.ThreeD.Engine.Render;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InSitU.Views.ThreeD.Engine.Illumination;

namespace InSitU.Views.ThreeD.Engine.Core.Interfaces
{
    public interface IDrawable : IRenderable
    {
        event EventHandler<MoveingEvent> MoveEvent;

        public bool AllowReflect { get; set; }
        public bool CanBeSaved { get; set; }
        public bool CastShadow { get; set; }
        CullFaceMode CullMode { get; set; }
        Vector4 DefaultColor { get; set; }
        PrimitiveType DrawType { get; set; }
        public int Id { get; set; }
        List<int> Indeces { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlended { get; set; }
        bool IsSystemModel { get; set; }
        Matrix4 LocalTransform { get; set; }
        public KeyControl model_KeyControl { get; set; }
        public string Name { get; set; }
        List<Vector3> Normals { get; set; }
        List<Vector3> NormalTangent { get; set; }
        public List<ParticleModel> Particles { get; set; }
        public Vector3 PivotPoint { get; set; }
        public bool RecieveShadow { get; set; }
        EngineRenderer Renderer { get; set; }
        Shader ShaderModel { get; set; }
        List<Vector2> TextureCoordinates { get; set; }
        Base_Texture TextureModel { get; set; }
        void ActivateShadowMap(Light lightSource);

        void MoveLocal(Vector3 displacement);

        void MoveTo(Vector3 position);

        void MoveTo(float x, float y, float z);

        void MoveWorld(Vector3 displacement);
        void Rotate(float angle, Vector3 axis);

        void Scale(float x);

        void Scale(float x, float y, float z);

        void Scale(Vector3 scalarVector);

        void SetHeight(float height);

        void Setup_Indeces();

        void Setup_Normals();

        void Setup_Position();

        void Setup_TextureCoordinates(float xScale = 1, float yScale = 1);

        void SetWidth(float width);
    }
}