using OpenTK.Graphics.OpenGL4;
using System;

namespace SuperOwner
{
    public class VertexFloatBuffer : IDisposable
    {
        public VertexFormat Format { get; private set; }
        public int Stride { get; private set; }
        public int AttributeCount { get; private set; }
        public int TriangleCount => _indexData.Length / 3;
        public int VertexCount => _vertexData.Length / AttributeCount; 
        public bool IsLoaded { get; private set; }
        public BufferUsageHint UsageHint { get; set; }
        public BeginMode DrawMode { get; set; }

        public int VBO => _VBOId;
        public int EBO => _EBOId;

        private int _VBOId;
        private int _EBOId;
        private int _vertexPosition;
        private int _indexPosition;
        protected float[] _vertexData;
        protected uint[] _indexData;

        public VertexFloatBuffer(VertexFormat format, int limit = 1024)
        {
            Format = format;
            SetStride();
            UsageHint = BufferUsageHint.StreamDraw;
            DrawMode = BeginMode.Triangles;

            _vertexData = new float[limit * AttributeCount];
            _indexData = new uint[limit];
        }
        public void Clear()
        {
            _vertexPosition = 0;
            _indexPosition = 0;
        }

        public void SetFormat(VertexFormat format)
        {
            Format = format;
            SetStride();
            Clear();
        }

        private void SetStride()
        {
            switch(Format)
            {
                case VertexFormat.XY:
                    Stride = 8;
                    break;
                case VertexFormat.XY_COLOR:
                    Stride = 24;
                    break;
                case VertexFormat.XY_UV:
                    Stride = 16;
                    break;
                case VertexFormat.XY_UV_COLOR:
                    Stride = 32;
                    break;
                case VertexFormat.XYZ:
                    Stride = 12;
                    break;
                case VertexFormat.XYZ_COLOR:
                    Stride = 28;
                    break;
                case VertexFormat.XYZ_UV:
                    Stride = 20;
                    break;
                case VertexFormat.XYZ_UV_COLOR:
                    Stride = 36;
                    break;
                case VertexFormat.XYZ_NORMAL_UV:
                    Stride = 32;
                    break;
                case VertexFormat.XYZ_NORMAL_UV_COLOR:
                    Stride = 48;
                    break;
            }
            AttributeCount = Stride / sizeof(float);
        }

        public void Set(float[] vertices, uint[] indices)
        {
            Clear();
            _vertexData = vertices;
            _indexData = indices;
        }

        public void Load()
        {
            if (IsLoaded) return;

            //VBO
            GL.GenBuffers(1, out _VBOId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBOId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertexPosition * sizeof(float)), _vertexData, UsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            //EBO
            GL.GenBuffers(1, out _EBOId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _EBOId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_indexPosition * sizeof(uint)), _indexData, UsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            IsLoaded = true;
        }

        public void Reload()
        {
            if (!IsLoaded) return;
            //VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBOId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertexPosition * sizeof(float)), _vertexData, UsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            //EBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _EBOId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_indexPosition * sizeof(uint)), _indexData, UsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Unload()
        {
            if (!IsLoaded) return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBOId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertexPosition * sizeof(float)), IntPtr.Zero, UsageHint);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _EBOId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_indexPosition * sizeof(uint)), IntPtr.Zero, UsageHint);
            
            GL.DeleteBuffers(1, ref _VBOId);
            GL.DeleteBuffers(1, ref _EBOId);

            IsLoaded = false;
        }

        public void Bind(Shader shader)
        {
            if (!IsLoaded) return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBOId);

            switch (Format)
            {
                case VertexFormat.XY:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                    break;
                case VertexFormat.XY_COLOR:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.ColorLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 8);
                    break;
                case VertexFormat.XY_UV:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.TexCoordLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 8);
                    break;
                case VertexFormat.XY_UV_COLOR:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.TexCoordLocation);
                    GL.EnableVertexAttribArray(shader.ColorLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 8);
                    GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 16);
                    break;
                case VertexFormat.XYZ:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                    break;
                case VertexFormat.XYZ_COLOR:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.ColorLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 12);
                    break;
                case VertexFormat.XYZ_UV:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.TexCoordLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 12);
                    break;
                case VertexFormat.XYZ_UV_COLOR:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.TexCoordLocation);
                    GL.EnableVertexAttribArray(shader.ColorLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 12);
                    GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 20);
                    break;
                case VertexFormat.XYZ_NORMAL_UV:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.NormalLocation);
                    GL.EnableVertexAttribArray(shader.ColorLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.NormalLocation, 3, VertexAttribPointerType.Float, false, Stride, 12);
                    GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 24);
                    break;
                case VertexFormat.XYZ_NORMAL_UV_COLOR:
                    GL.EnableVertexAttribArray(shader.PositionLocation);
                    GL.EnableVertexAttribArray(shader.NormalLocation);
                    GL.EnableVertexAttribArray(shader.TexCoordLocation);
                    GL.EnableVertexAttribArray(shader.ColorLocation);
                    GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                    GL.VertexAttribPointer(shader.NormalLocation, 3, VertexAttribPointerType.Float, false, Stride, 12);
                    GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 24);
                    GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 32);
                    break;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBOId);
            GL.DrawElements(DrawMode, _indexPosition, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Unload();
                    Clear();
                    _vertexData = null;
                    _indexData = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~VertexFloatBuffer() {
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

        public void IndexFromLength()
        {
            int count = _vertexPosition / AttributeCount;
            _indexPosition = 0;
            for (uint i = 0; i < count; i++)
            {
                _indexData[_indexPosition++] = i;
            }
        }
        public void AddIndex(uint indexA, uint indexB, uint indexC)
        {
            _indexData[_indexPosition++] = indexA;
            _indexData[_indexPosition++] = indexB;
            _indexData[_indexPosition++] = indexC;
        }
        public void AddIndices(uint[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                _indexData[_indexPosition++] = indices[i];
            }
        }

        public void AddVertex(float x, float y)
        {
            if (Format != VertexFormat.XY)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
        }

        public void AddVertex(float x, float y, float r, float g, float b, float a)
        {
            if (Format != VertexFormat.XY_COLOR)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
            _vertexData[_vertexPosition++] = r;
            _vertexData[_vertexPosition++] = g;
            _vertexData[_vertexPosition++] = b;
            _vertexData[_vertexPosition++] = a;
        }

        public void AddVertex(float x, float y, float z)
        {
            if (Format != VertexFormat.XYZ)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
            _vertexData[_vertexPosition++] = z;
        }

        public void AddVertex(float x, float y, float z, float r, float g, float b, float a)
        {
            if (Format != VertexFormat.XYZ_COLOR)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
            _vertexData[_vertexPosition++] = z;
            _vertexData[_vertexPosition++] = r;
            _vertexData[_vertexPosition++] = g;
            _vertexData[_vertexPosition++] = b;
            _vertexData[_vertexPosition++] = a;
        }

        public void AddVertex(float x, float y, float z, float u, float v)
        {
            if (Format != VertexFormat.XYZ_UV)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
            _vertexData[_vertexPosition++] = z;
            _vertexData[_vertexPosition++] = u;
            _vertexData[_vertexPosition++] = v;
        }

        public void AddVertex(float x, float y, float z, float u, float v, float r, float g, float b, float a)
        {
            if (Format != VertexFormat.XYZ_UV_COLOR)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
            _vertexData[_vertexPosition++] = z;
            _vertexData[_vertexPosition++] = u;
            _vertexData[_vertexPosition++] = v;
            _vertexData[_vertexPosition++] = r;
            _vertexData[_vertexPosition++] = g;
            _vertexData[_vertexPosition++] = b;
            _vertexData[_vertexPosition++] = a;
        }

        public void AddVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v)
        {
            if (Format != VertexFormat.XYZ_NORMAL_UV)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
            _vertexData[_vertexPosition++] = z;
            _vertexData[_vertexPosition++] = nx;
            _vertexData[_vertexPosition++] = ny;
            _vertexData[_vertexPosition++] = nz;
            _vertexData[_vertexPosition++] = u;
            _vertexData[_vertexPosition++] = v;
        }

        public void AddVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v, float r, float g, float b, float a)
        {
            if (Format != VertexFormat.XYZ_NORMAL_UV_COLOR)
                throw new FormatException("vertex must be of the same format type as buffer");

            _vertexData[_vertexPosition++] = x;
            _vertexData[_vertexPosition++] = y;
            _vertexData[_vertexPosition++] = z;
            _vertexData[_vertexPosition++] = nx;
            _vertexData[_vertexPosition++] = ny;
            _vertexData[_vertexPosition++] = nz;
            _vertexData[_vertexPosition++] = u;
            _vertexData[_vertexPosition++] = v;
            _vertexData[_vertexPosition++] = r;
            _vertexData[_vertexPosition++] = g;
            _vertexData[_vertexPosition++] = b;
            _vertexData[_vertexPosition++] = a;
        }
    }

}
