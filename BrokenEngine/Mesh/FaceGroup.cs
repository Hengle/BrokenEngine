﻿using System.Collections.Generic;

namespace OpenGLTest
{
    public class FaceGroup
    {

        public string Name;
        public string Comments;
        public string Material;

        public List<Face> Faces = new List<Face>();

        public FaceGroup()
        {
            
        }

        public FaceGroup(string name)
        {
            Name = name;
        }

    }
}