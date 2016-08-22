﻿using System;
using System.Drawing;
using System.IO;
using Gwen.Control;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Gwen.Control;
using OpenGLTest.OBJ_Parser;
using OpenTK.Input;

namespace OpenGLTest
{
    public class Game : GameWindow
    {

        public Camera CurrentCamera;

        public readonly GameObject SceneGraph = new GameObject("root", Vector3.Zero);

        // UI stuff
        private Gwen.Input.OpenTK input;
        private Gwen.Renderer.OpenTK renderer;
        private Gwen.Skin.Base skin;
        private Canvas canvas;

        private bool altDown = false;

        public Game(int resX, int resY, string title) : base(
            resX, resY, GraphicsMode.Default, title, // Game settings
            GameWindowFlags.Default, DisplayDevice.Default, // unimportant stuff
            4, 3, GraphicsContextFlags.ForwardCompatible    // opengl version
            )
        {
            // register events
            Keyboard.KeyDown += OnKeyDown;
            Keyboard.KeyUp += OnKeyUp;

            Mouse.ButtonDown += OnButtonDown;
            Mouse.ButtonUp += OnButtonUp;
            Mouse.Move += OnMove;
            Mouse.WheelChanged += OnWheel;

            Console.WriteLine("gl version: " + GL.GetString(StringName.Version));
        }

        // called when window starts running
        protected override void OnLoad(EventArgs e)
        {
            // init UI
            renderer = new Gwen.Renderer.OpenTK();
            using (Stream stream = new MemoryStream(Properties.Resources.DefaultSkin1))
                skin = new Gwen.Skin.TexturedBase(renderer, stream);
            canvas = new Canvas(skin);
            input = new Gwen.Input.OpenTK(this);
            input.Initialize(canvas);
            
            canvas.SetSize(Width, Height);
            canvas.KeyboardInputEnabled = true;

            // create scene graph
            ObjMesh airboat = ObjParser.ParseFile(Properties.Resources.airboat);
            ObjMesh akm = ObjParser.ParseFile(Properties.Resources.WPN_AKM);
            ObjMesh sphere = ObjParser.ParseFile(Properties.Resources.sphere);
            ObjMesh cube = ObjParser.ParseFile(Properties.Resources.cube);
            ObjMesh polygon = ObjParser.ParseFile(Properties.Resources.polygon);

            var go = new GameObject("test object", Vector3.One, SceneGraph);
            go.AddComponent(new MeshRenderer(MeshUtils.CreateQuad()), false);
            new GameObject("Test", Vector3.Zero, go);
            new GameObject("Test 2", new Vector3(0, 4, -10), go).AddComponent(new MeshRenderer(MeshUtils.CreateTriangle()), false);
            new GameObject("Coordinate Origin", new Vector3(-15, -15, -15), go).AddComponent(new MeshRenderer(MeshUtils.CreateCoordinateOrigin()), false);
            go = new GameObject("Test 4", new Vector3(12, 0, 0), go);
            go.AddComponent(new MeshRenderer(cube), false);
            new GameObject("Test 5", new Vector3(-5, 0, 0), go).AddComponent(new MeshRenderer(MeshUtils.CreateCube()), false);

            go = new GameObject("Model", new Vector3(15, 15, 10), SceneGraph);
            go.AddComponent(new MeshRenderer(sphere));
            //go.AddComponent(MeshRenderer.CreateTestTriangle(), false);

            go = new GameObject("Model 2", new Vector3(5, 0, 0), SceneGraph);
            go.AddComponent(new MeshRenderer(polygon), false);

            go = new GameObject("Airboat", new Vector3(0, -5, -10), SceneGraph);
            //go.AddComponent(new MeshRenderer(airboat), false);
            go.AddComponent(new MeshRenderer(akm), false);

            var cameraObj = new GameObject("Camera", new Vector3(0, 0, 5), SceneGraph);
            // TODO: move following camera initialiion things to camera settings
            float aspectRatio = ClientSize.Width / (float)(ClientSize.Height);
            float fov = MathHelper.PiOver2;
            var camera = new Camera(Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, 0.1f, 100f));
            cameraObj.AddComponent(camera, false);
            cameraObj.AddComponent(new CameraMovement(CameraMovement.Type.FirstPerson), false);

            // create ui
            TreeControl tree = new TreeControl(canvas);
            
            BuildSceneGraphUI(tree);
            tree.SetBounds(10, 10, 250, 300);

            // init logic
            CurrentCamera = camera;

            SceneGraph.Start();

            // GL settings
            // turn vsync of to measure performance by counting frames
            //this.VSync = VSyncMode.Adaptive;

            //GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);    // backface culling
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);    // wire frame
            GL.ClearColor(Color4.AliceBlue);
            GL.FrontFace(FrontFaceDirection.Ccw);  // Ccw = Counter-clockwise = default = right hand rule
        }

        private void BuildSceneGraphUI(TreeControl tree)
        {
            foreach (var child in SceneGraph)
            {
                SubBuildSceneGraphUI(child, tree.AddNode(child.Name));
            }
        }

        private void SubBuildSceneGraphUI(GameObject go, TreeNode node)
        {
            foreach (var child in go)
            {
                SubBuildSceneGraphUI(child, node.AddNode(child.Name));
            }

            foreach (var comp in go.Components)
            {
                node.AddNode(comp.GetType()+"");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);

            base.OnResize(e);
        }

        // called every frame, game logic
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            SceneGraph.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the back buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            CurrentCamera.Render(SceneGraph);

            // TODO: implement image effects: http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-14-render-to-texture/

            // render UI
            canvas.RenderCanvas();

            // swap backbuffer
            SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            // destroy whole scene graph
            SceneGraph.Destroy();
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == global::OpenTK.Input.Key.Escape)
                Exit();
            else if (e.Key == global::OpenTK.Input.Key.AltLeft)
                altDown = true;
            else if (altDown && e.Key == global::OpenTK.Input.Key.Enter)
                if (WindowState == WindowState.Fullscreen)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Fullscreen;

            input.ProcessKeyDown(e);
        }

        private void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            altDown = false;
            input.ProcessKeyUp(e);
        }

        private void OnButtonDown(object sender, MouseButtonEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        private void OnButtonUp(object sender, MouseButtonEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        private void OnMove(object sender, MouseMoveEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        private void OnWheel(object sender, MouseWheelEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        public override void Dispose()
        {
            canvas.Dispose();
            skin.Dispose();
            renderer.Dispose();
            base.Dispose();
        }

    }
}