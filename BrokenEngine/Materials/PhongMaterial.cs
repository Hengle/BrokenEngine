﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BrokenEngine.Materials
{
    public class PhongMaterial : Material
    {

        public Color4 AlbedoColor { get; set; }

        public Vector3 LightDirection { get; set; }
        
        public Color4 AmbientColor { get; set; }

        public PhongMaterial(Color4 albedoColor, Vector3 lightDirection, Color4 ambientColor) : base("phong")
        {
            AlbedoColor = albedoColor;
            LightDirection = lightDirection;
            AmbientColor = ambientColor;
        }

        public override void Apply()
        {
            base.Apply();

            shader.Program.SetValueUniform("u_albedo", AlbedoColor, GL.Uniform4);
            shader.Program.SetValueUniform("u_lightDirection", LightDirection, GL.Uniform3);
            shader.Program.SetValueUniform("u_ambientColor", AmbientColor, GL.Uniform4);
        }

    }
}
