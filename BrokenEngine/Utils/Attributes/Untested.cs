﻿using System;

namespace BrokenEngine.Utils.Attributes
{
    /// <summary>
    /// Marks a method as untested
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited =  false)]
    public class Untested : System.Attribute
    {
         
    }
}