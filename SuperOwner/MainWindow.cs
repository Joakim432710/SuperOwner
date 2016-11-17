using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SuperOwner
{
    public class MainWindow : GameWindow
    {
        private Matrix4 ProjectionMatrix;
        private Matrix4 WorldMatrix;
        private Matrix4 ModelViewMatrix;

        private Vector3 CameraPosition;
        private Shader Shader;
        private VertexFloatBuffer Buffer;

        public MainWindow() 
            : base(GameConstants.StartupWidth, GameConstants.StartupHeight, 
                  GraphicsMode.Default, GameConstants.StartupTitle, 
                  GameWindowFlags.Default, DisplayDevice.Default, 
                  4, 0, GraphicsContextFlags.ForwardCompatible)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            #region OpenGL Version Printing
            int major, minor;
            GL.GetInteger(GetPName.MajorVersion, out major);
            GL.GetInteger(GetPName.MinorVersion, out minor);
            Debug.WriteLine("OpenGL {0}.{1} with GLSL {2}", major, minor, GL.GetString(StringName.ShadingLanguageVersion));
            #endregion

            //ProjectionMatrix = Matrix4.CreateOrthographic(1.0f, 1.0f, 0.0f, 1.0f);
            ProjectionMatrix = Matrix4.CreateOrthographic(1.0f, 1.0f, 0.0f, 10.0f);
            //ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.5f, 10000.0f);
            WorldMatrix = new Matrix4();
            ModelViewMatrix = new Matrix4();

            CameraPosition = new Vector3(0.5f, 0.5f, 0.0f);

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var vertexSrc = File.ReadAllText(Path.Combine(directory, "VertexShader.vs"));
            var pixelSrc = File.ReadAllText(Path.Combine(directory, "PixelShader.ps"));
            Shader = new Shader(ref vertexSrc, ref pixelSrc);

            Buffer = new VertexFloatBuffer(VertexFormat.XYZ_COLOR, 3);
            Buffer.AddVertex(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.9f, 1.0f);
            Buffer.AddVertex(0.5f, 1.0f, 0.0f, 0.0f, 0.9f, 0.0f, 1.0f);
            Buffer.AddVertex(1.0f, 0.0f, 0.0f, 0.9f, 0.0f, 0.0f, 1.0f);
            Buffer.IndexFromLength();
            Buffer.Load();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            WorldMatrix = Matrix4.CreateTranslation(-CameraPosition);
            ModelViewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -2.0f);
            Matrix4 MVP = ModelViewMatrix * WorldMatrix * ProjectionMatrix;

            GL.UseProgram(Shader.ProgramId);

            int mvp_loc = GL.GetUniformLocation(Shader.ProgramId, "mvp_matrix");
            GL.UniformMatrix4(mvp_loc, false, ref MVP);
            GL.UseProgram(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(Shader.ProgramId);
            Buffer.Bind(Shader);
            GL.UseProgram(0);

            SwapBuffers();
        }
    }
}
