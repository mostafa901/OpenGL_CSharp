﻿using OpenTK.Graphics.OpenGL;
using Simple_Engine.Engine.Core.Interfaces;
using Simple_Engine.Engine.Render;
using Simple_Engine.Extentions;

namespace Simple_Engine.Engine.Geometry.Terrain.Render
{
    internal class TerrainRenderer : EngineRenderer
    {
        public TerrainRenderer(IDrawable3D _model) : base(_model)
        {
        }

        public override void Dispose()
        {
        }

        public override void PreDraw()
        {
            base.PreDraw();

            GL.BindVertexArray(VAO);//access to memory location
            GL.EnableVertexAttribArray(PositionLocation);//position
            GL.EnableVertexAttribArray(TextureLocation);//texture
            GL.EnableVertexAttribArray(NormalLocation);//normal

            if (geometryModel.Dynamic.HasFlag(IDrawable.DynamicFlag.Positions))
            {
                StoreDataInAttributeList(Get_VBO_Position(), PositionLocation, ((IDrawable3D)geometryModel).Positions.GetArray(), 3);
            }

            // DisableCulling();
            EnableCulling();
        }

        public override void EndDraw()
        {
            GL.DisableVertexAttribArray(PositionLocation);
            GL.DisableVertexAttribArray(TextureLocation);
            GL.DisableVertexAttribArray(NormalLocation);
            GL.BindVertexArray(0);
            DisableCulling();
        }

        public override void RenderModel()
        {
            PositionLocation = geometryModel.GetShaderModel().PositionLayoutId;
            TextureLocation = geometryModel.GetShaderModel().TextureLayoutId;
            NormalLocation = geometryModel.GetShaderModel().NormalLayoutId;

            VAO = CreateVAO();
            BindIndicesBuffer(((IDrawable3D)geometryModel).Indeces.ToArray());

            StoreDataInAttributeList(Get_VBO_Position(), PositionLocation, ((IDrawable3D)geometryModel).Positions.GetArray(), 3);
            StoreDataInAttributeList(Get_VBO_Texture(), TextureLocation, geometryModel.TextureCoordinates.GetArray(), 2);
            StoreDataInAttributeList(Get_VBO_Normals(), NormalLocation, geometryModel.Normals.GetArray(), 3);

            GL.BindVertexArray(0);
        }

        public override void DrawModel()
        {
            GL.DrawElements(geometryModel.DrawType, ((IDrawable3D)geometryModel).Indeces.Count, DrawElementsType.UnsignedInt, 0);
        }
    }
}