using OpenTK.Graphics.OpenGL4;
using System;

namespace SuperOwner
{
    public class Shader : IDisposable
    {
        public string VertexSource { get; private set; }
        public string PixelSource { get; private set; }

        public int VertexId { get; private set; }
        public int PixelId { get; private set; }

        public int ProgramId { get; private set; }

        public int PositionLocation { get; set; }
        public int NormalLocation { get; set; }
        public int TexCoordLocation { get; set; }
        public int ColorLocation { get; set; }

        public Shader (ref string vs, ref string ps)
        {
            VertexSource = vs;
            PixelSource = ps;
            Build();
        }

        private void Build()
        {
            int status;
            string info;

            VertexId = GL.CreateShader(ShaderType.VertexShader);
            PixelId = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(VertexId, VertexSource);
            GL.CompileShader(VertexId);
            GL.GetShaderInfoLog(VertexId, out info);
            GL.GetShader(VertexId, ShaderParameter.CompileStatus, out status);
            if (status != 1)
                throw new ApplicationException(info); //TODO: Shader compile error exception

            GL.ShaderSource(PixelId, PixelSource);
            GL.CompileShader(PixelId);
            GL.GetShaderInfoLog(PixelId, out info);
            GL.GetShader(PixelId, ShaderParameter.CompileStatus, out status);
            if (status != 1)
                throw new ApplicationException(info); //TODO: Shader compile error exception

            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, VertexId);
            GL.AttachShader(ProgramId, PixelId);
            GL.LinkProgram(ProgramId);
            GL.UseProgram(ProgramId);

            PositionLocation = GL.GetAttribLocation(ProgramId, "vertex_position");
            NormalLocation = GL.GetAttribLocation(ProgramId, "vertex_normal");
            TexCoordLocation = GL.GetAttribLocation(ProgramId, "vertex_texcoord");
            ColorLocation = GL.GetAttribLocation(ProgramId, "vertex_color");

            if (PositionLocation >= 0)
                GL.BindAttribLocation(ProgramId, PositionLocation, "vertex_position");
            if (NormalLocation >= 0)
                GL.BindAttribLocation(ProgramId, NormalLocation, "vertex_normal");
            if (TexCoordLocation >= 0)
                GL.BindAttribLocation(ProgramId, TexCoordLocation, "vertex_texcoord");
            if (ColorLocation >= 0)
                GL.BindAttribLocation(ProgramId, ColorLocation, "vertex_color");

            GL.UseProgram(0);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (ProgramId != 0)
                        GL.DeleteProgram(ProgramId);
                    if (PixelId != 0)
                        GL.DeleteShader(PixelId);
                    if (VertexId != 0)
                        GL.DeleteShader(VertexId);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Shader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
