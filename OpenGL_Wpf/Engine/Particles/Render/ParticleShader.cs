﻿using Simple_Engine.Engine.Render;

namespace Simple_Engine.Engine.Particles.Render
{
    public class ParticleShader : Shader
    {
        public ParticleShader(ShaderMapType mapType, ShaderPath shaderType) : base(mapType, shaderType)
        {
        }

        public int TextureOffsetLayoutLocation = 6;
        public int BlendLayoutLocation = 7;

        public override void BindAttributes()
        {
            BindAttribute(PositionLayoutId, "aPosition2");
            BindAttribute(TextureLayoutId, "aTextureCoor");
            MatrixLayoutId = 2;
            BindAttribute(MatrixLayoutId, "InstanceMatrix"); //2,3,4,5
            BindAttribute(TextureOffsetLayoutLocation, "textureoffset");
            BindAttribute(BlendLayoutLocation, "Blend");
        }
    }
}