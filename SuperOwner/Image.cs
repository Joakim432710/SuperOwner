using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace SuperOwner
{
    public class Image
    {
        public Shader Shader { get; private set; }
        public Texture Texture { get; private set; }
        public VertexFloatBuffer Buffer { get; private set; }

        private Matrix4 _modelViewMatrix;

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                _modelViewMatrix = Matrix4.CreateScale(_size.X, _size.Y, 1.0f) * Matrix4.CreateTranslation(_position.X, _position.Y, 0.0f);
            }
        }

        private Vector2 _size;
        public Vector2 Size
        {
            get { return _size; }
            set
            {
                if (_size == value) return;
                _size = value;
                _modelViewMatrix = Matrix4.CreateScale(_size.X, _size.Y, 1.0f) * Matrix4.CreateTranslation(_position.X, _position.Y, 0.0f);
            }
        }

        public Image(Vector2 pos, Vector2 size, string vs, string ps, string texture) : this(pos, size)
        {
            Shader = new Shader(vs, ps);
            Texture = new Texture(texture);
        }

        public Image(Vector2 pos, Vector2 size, Shader shader, Texture texture) : this(pos, size)
        {
            Shader = shader;
            Texture = texture;
        }

        protected Image(Vector2 pos, Vector2 size)
        {
            _position = pos;
            _size = size;
            _modelViewMatrix = Matrix4.CreateScale(_size.X, _size.Y, 1.0f) * Matrix4.CreateTranslation(_position.X, _position.Y, 0.0f);
            Buffer = new VertexFloatBuffer(VertexFormat.XY_UV, 4) { DrawMode = BeginMode.Quads };
            Buffer.AddVertex(0, 0, 1.0f, 1.0f);
            Buffer.AddVertex(1, 0, 0.0f, 1.0f);
            Buffer.AddVertex(1, 1, 0.0f, 0.0f);
            Buffer.AddVertex(0, 1, 1.0f, 0.0f);
            Buffer.IndexFromLength();
            Buffer.Load();
        }

        public void Render(Matrix4 vp)
        {
            GL.UseProgram(Shader.ProgramId);
            int mvp_loc = GL.GetUniformLocation(Shader.ProgramId, "mvp_matrix");
            Matrix4 MVP = _modelViewMatrix * vp;
            GL.UniformMatrix4(mvp_loc, false, ref MVP);
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureId);
            Buffer.Bind(Shader);
            GL.UseProgram(0);
        }
    }
}