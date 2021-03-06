﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BrokenEngine.Materials
{
    public class GenericPhongMaterial : Material
    {

        public Color4 AlbedoColor { get; set; }

        public Vector3 LightDirection { get; set; }
        public Color4 LightColor { get; set; }
        public float LightIntensity { get; set; }

        public float SpecularIntensity { get; set; }
        public float SpecularShininess { get; set; }
        
        public Color4 AmbientColor { get; set; }
        public float AmbientIntensity { get; set; }


        // has to be public because of https://github.com/wojtpl2/ExtendedXmlSerializer/issues/8
        public GenericPhongMaterial()
        {    
        }

        public GenericPhongMaterial(Color4 albedoColor, Vector3 lightDirection, Color4 ambientColor, string shaderFileName) : base(shaderFileName)
        {
            AlbedoColor = albedoColor;
            LightDirection = lightDirection;
            AmbientColor = ambientColor;
        }

        public override void Apply()
        {
            base.Apply();

            SetValueUniform("u_albedo", AlbedoColor, GL.Uniform4);

            SetValueUniform("u_lightDirection", LightDirection, GL.Uniform3);
            SetValueUniform("u_lightColor", LightColor, GL.Uniform4);
            SetValueUniform("u_lightIntensity", LightIntensity, GL.Uniform1);

            SetValueUniform("u_specularIntensity", SpecularIntensity, GL.Uniform1);
            SetValueUniform("u_specularShininess", SpecularShininess, GL.Uniform1);

            SetValueUniform("u_ambientColor", AmbientColor, GL.Uniform4);
            SetValueUniform("u_ambientIntensity", AmbientIntensity, GL.Uniform1);
        }

    }
}
