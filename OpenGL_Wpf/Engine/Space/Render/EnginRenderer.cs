﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using Shared_Lib.IO;
using Simple_Engine.Engine.Core.Abstracts;
using Simple_Engine.Engine.Core.Interfaces;
using Simple_Engine.Engine.GameSystem;
using Simple_Engine.Engine.Geometry.Core;
using Simple_Engine.Engine.Space.Camera;
using Simple_Engine.ToolBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static Simple_Engine.Engine.GameSystem.DisplayManager;

namespace Simple_Engine.Engine.Render
{
    abstract public class EngineRenderer
    {
        public int EBO = -1;
        public IDrawable geometryModel;
        public int IndexBufferLength = 0;
        public int InstancesMatrix;
        public int InstancesSelectedLocation;
        public List<int> MatrixLocations = new List<int>();
        public int NormalLocation;
        public int PositionBufferLength = 0;
        public int PositionLocation;
        public int TangentsLocation;
        public int TextureLocation;
        public int VAO = -1;
        public int VertexColorLocation;
        public bool EnableNormalTangent = false;

        public EngineRenderer(IDrawable _model)
        {
            geometryModel = _model;
            PositionLocation = geometryModel.ShaderModel.PositionLayoutId;
            TextureLocation = geometryModel.ShaderModel.TextureLayoutId;
            NormalLocation = geometryModel.ShaderModel.NormalLayoutId;
            InstancesMatrix = geometryModel.ShaderModel.MatrixLayoutId;
            InstancesSelectedLocation = geometryModel.ShaderModel.SelectedLayoutId;
            TangentsLocation = geometryModel.ShaderModel.TangentLayoutId;
            VertexColorLocation = geometryModel.ShaderModel.VertexColorLayoutId;
        }

        public bool IsToonMode { get; set; } = false;
        public List<int> VBOs { get; set; } = new List<int>();

        public void BindIndicesBuffer<T>(byte[] indices)
        {
            if (indices.Length == 0) return;
            var ebo = GL.GenBuffer();
            EBO = ebo;
            IndexBufferLength = indices.Length / sizeof(int);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length, indices, BufferUsageHint.StaticDraw);
        }

        public void BindIndicesBuffer(int[] indices)
        {
            if (indices.Length == 0) return;
            var size = indices.Length * sizeof(int);
            var pdata = Marshal.AllocHGlobal(size);
            Marshal.Copy(indices, 0, pdata, indices.Length);

            BindIndicesBuffer<int>(indices.ToByteArray());
            Marshal.FreeHGlobal(pdata);
        }

        public int CreateVAO()
        {
            var VAO = GL.GenVertexArray(); //create a store for a quick recall
            GL.BindVertexArray(VAO); //Activate
            return VAO;
        }

        public void DisableBlending()
        {
            GL.Disable(EnableCap.Blend);
        }

        public void DisableCulling()
        {
            GL.Disable(EnableCap.CullFace); //avoid rendering Faces that are..
        }

        public virtual void Dispose()
        {
            GL.DeleteBuffer(EBO);
            GL.DeleteVertexArray(VAO);

            foreach (int vbo in VBOs)
            {
                GL.DeleteBuffer(vbo);
            }
            VBOs.Clear();
        }

        public virtual void Draw()
        {
            PreDraw();
            //if (geometryModel is ISelectable && ((ISelectable)geometryModel).Selected)
            //{
            //    RenderStencil();
            //}
            //else
            {
                DrawModel();
            }
            EndDraw();
        }

        public abstract void DrawModel();

        public void EnableBlending()
        {
            if (DisplayManager.CurrentBuffer == RenderBufferType.Selection) return;
            //more info about Blending function
            //https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/blendFunc
            GL.Enable(EnableCap.Blend);
        }

        public void EnableCulling()
        {
            GL.Enable(EnableCap.CullFace); //avoid rendering Faces that are..

            if (CameraModel.EnableClipPlans)
            {
                DisableCulling();
            }
            else if (geometryModel.CullMode != CullFaceMode.FrontAndBack)
            {
                GL.CullFace(geometryModel.CullMode); //back from Camera
            }
            else
            {
                DisableCulling();
            }
        }

        public abstract void EndDraw();

        public virtual void PreDraw()
        {
            if (geometryModel == Base_Geo.SelectedModel)
            {
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.SrcColor);
                EnableBlending();
            }
            else if (geometryModel.IsBlended)
            {
                EnableBlending();
            }
        }

        public abstract void RenderModel();

        public virtual void RenderStencil()
        {
            //1st render pass
            //draw model to store in Stencil the masked Area
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.StencilFunc(StencilFunction.Always, 1, 1);
            GL.StencilMask(1);
            //GL.ColorMask(false, false, false, false);
            //GL.DepthMask(false);
            DrawModel();

            //2nd Render Pass
            //Draw a scaled model ignoring any Area of Value 1( previously stored by Mask)
            GL.Disable(EnableCap.DepthTest);
            GL.StencilFunc(StencilFunction.Notequal, 1, 1);
            GL.StencilMask(0);
            //GL.ColorMask(true, true, true, true);
            //GL.DepthMask(true);

            geometryModel.ShaderModel.SetVector4(geometryModel.ShaderModel.Location_DefaultColor, new Vector4(1, 1, 0, 1));
            geometryModel.ShaderModel.SetInt(geometryModel.ShaderModel.Location_ShaderType, (int)ShaderMapType.LoadColor);
            geometryModel.ShaderModel.SetMatrix4(geometryModel.ShaderModel.Location_LocalTransform, eMath.Scale(geometryModel.LocalTransform, new Vector3(1.05f)));

            DrawModel();

            GL.Enable(EnableCap.DepthTest);
            GL.StencilMask(1);
            GL.StencilFunc(StencilFunction.Always, 1, 1);
            GL.Disable(EnableCap.StencilTest);
        }

        public int StoreDataInAttributeList(int attributeLocation, float[] data, int componentCount, int divisor = 0)
        {
            if (data.Length == 0) return 0;
            var size = data.Length * sizeof(float);
            var pdata = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, pdata, data.Length);

            var v = StoreDataInAttributeList(attributeLocation, size, pdata, componentCount, divisor);
            Marshal.FreeHGlobal(pdata);
            return v;
        }

        public int StoreDataInAttributeList(int attributeLocation, byte[] data, int componentCount, int divisor = 0)
        {
            return StoreDataInAttributeList(attributeLocation, data.Length, data, componentCount, divisor);
        }

        public int StoreDataInAttributeList(int attributeLocation, int size, dynamic data, int componentCount, int divisor)
        {
            var VBO = GL.GenBuffer(); //Create an Id for the Vertex Buffer Object
            VBOs.Add(VBO);
            //define the type of buffer in the GPU and Activate
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //Supply the data to the buffer
            GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.StaticDraw);

            //Define the Pattern how the data is being read
            GL.VertexAttribPointer
                                (
                                    attributeLocation, //location layout --> see shader.vert
                                    componentCount, //vertex component count
                                    VertexAttribPointerType.Float, //type of vertex
                                    false, //shall normalize
                                    componentCount * sizeof(float), //the stride length
                                    0 //Start reading from What position within the stride
                                );
            if (divisor > 0)
            {
                GL.VertexAttribDivisor(attributeLocation, divisor);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            return VBO;
        }

        public void StoreDataInAttributeList(int matrixLocation, List<int> storeLocation, int divisor, int count, int componentCount)
        {
            var VBO = GL.GenBuffer(); //Create an Id for the Vertex Buffer Object
            VBOs.Add(VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);  //define the type of buffer in the GPU

            //now stream these vertex (array type) to the located buffer in the GPU

            for (int i = 0; i < count; i++)
            {
                var attributeLocation = matrixLocation + i;

                GL.VertexAttribPointer
                                    (
                                    attributeLocation,
                                    componentCount, //maximum is 4
                                    VertexAttribPointerType.Float,
                                    false,
                                    sizeof(float) * componentCount * count, //total matrix float Size
                                    sizeof(float) * i * componentCount //start reading from
                                    );

                storeLocation.Add(attributeLocation);
                GL.EnableVertexAttribArray(attributeLocation);
                GL.VertexAttribDivisor(attributeLocation, divisor);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateSelectedMeshes(int attributeLocation, List<Mesh3D> meshes)
        {
            if (!geometryModel.ShaderModel.EnableInstancing) return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs.ElementAt(attributeLocation));  //define the type of buffer in the GPU
            var isSelected = meshes.Select(o => (float)Convert.ToInt32(o.Selected)).ToArray();

            //now stream these vertex (array type) to the located buffer in the GPU
            GL.BufferData(BufferTarget.ArrayBuffer, meshes.Count * sizeof(float), isSelected, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UploadMeshes(int attributeLocation, List<Mesh3D> meshes)
        {
            if (!geometryModel.ShaderModel.EnableInstancing) return;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs.ElementAt(attributeLocation));  //define the type of buffer in the GPU
            var transforms = meshes.Select(o => o.LocalTransform).ToArray();

            //now stream these vertex (array type) to the located buffer in the GPU
            GL.BufferData(BufferTarget.ArrayBuffer, meshes.Count * sizeof(float) * 16, transforms, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}