﻿using Simple_Engine.Engine.Core.Abstracts;
using Simple_Engine.Engine.Render;

namespace Simple_Engine.Engine.Illumination.Render
{
    public class Shadow_Shader : Shader
    {
        public Shadow_Shader() : base(ShaderMapType.LoadColor, ShaderPath.Shadow)
        {
        }

        public override void BindAttributes()
        {
            BindAttribute(PositionLayoutId, "aPosition");
            BindAttribute(PositionLayoutId, "aTextureCoor");
            BindAttribute(MatrixLayoutId, "InstanceMatrix");
        }

        public override void UploadDefaults(Base_Geo model)
        {
            Use();

            model?.UploadDefaults(this);

            Stop();
        }
    }
}