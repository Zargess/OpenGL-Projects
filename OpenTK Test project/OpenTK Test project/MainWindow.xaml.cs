using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Reflection;
using OpenTK_Test_project.Models;

namespace OpenTK_Test_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int pgmID, vsID, fsID, attribute_vcol, attribute_vpos, uniform_mview, uniform_projection, uniform_angle, vbo_position, vbo_color, vbo_mview, vbo_projection;
        private GLControl glControl;
        private Vector4[] vertices;
        private float near = -10.0f;
        private float far = 10.0f;
        private float orthosize = 5.0f;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            InitProgram();

            var block = new Block(0.0f, 0.0f, 0.0f);
            var blockSides = block.GetBlockSides();
            var blockVertices = new List<Vector4>();

            foreach (var blockSide in blockSides)
            {
                blockVertices.AddRange(blockSide.SideVertices);
            }

            vertices = blockVertices.ToArray();


            var color = new Vector4[36];
            for (var i = 0; i < color.Length; i++)
            {
                color[i] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            }

            GL.UseProgram(pgmID);

            GL.ClearColor(.0f, .0f, 1.0f, 1.0f);
            GL.PointSize(5f);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vector4.SizeInBytes), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 4, VertexAttribPointerType.Float, false, 0, 0);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(color.Length * Vector3.SizeInBytes), color, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 4, VertexAttribPointerType.Float, true, 0, 0);

            var projectionMat = Matrix4.CreateOrthographicOffCenter(-orthosize, orthosize, -orthosize, orthosize, near, far);

            GL.UniformMatrix4(uniform_projection, false, ref projectionMat);
        }

        private float DegreesToRadians(float degree)
        {
            return (degree * (float)Math.PI) / 180.0f;
        }

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            var flags = GraphicsContextFlags.Default;

            glControl = new GLControl(new GraphicsMode(32, 24), 2, 0, flags);

            glControl.MakeCurrent();

            glControl.Paint += Render;

            glControl.Dock = DockStyle.Fill;

            (sender as WindowsFormsHost).Child = glControl;

        }

        private float angle = 90.0f;
        private void Render(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);

            // TODO : Move drawing and IO into a world class
            // Get keyboard state
            var state = Keyboard.GetState();
            
            var mviewMat = Matrix4.CreateScale(2.0f);
            mviewMat = Matrix4.Mult(Matrix4.CreateRotationY(DegreesToRadians(angle)), mviewMat);
            //angle += 0.2f;
            GL.UniformMatrix4(uniform_mview, false, ref mviewMat);


            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length);            

            GL.DisableVertexAttribArray(attribute_vpos);
            GL.DisableVertexAttribArray(attribute_vcol);
            GL.Flush();
            glControl.SwapBuffers();
            glControl.Invalidate();
        }

        private void InitProgram()
        {
            pgmID = GL.CreateProgram();
            vsID = LoadShader("Shaders/vshader.glsl", ShaderType.VertexShader, pgmID);
            fsID = LoadShader("Shaders/fshader.glsl", ShaderType.FragmentShader, pgmID);

            GL.LinkProgram(pgmID);

            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            attribute_vpos = GL.GetAttribLocation(pgmID, "vPosition");
            attribute_vcol = GL.GetAttribLocation(pgmID, "vColor");
            uniform_mview = GL.GetUniformLocation(pgmID, "modelview");
            uniform_projection = GL.GetUniformLocation(pgmID, "projection");
            uniform_angle = GL.GetUniformLocation(pgmID, "angle");

            if (attribute_vpos == -1 || attribute_vcol == -1 || uniform_mview == -1)
            {
                Console.WriteLine("Error binding attributes");
            }

            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out vbo_color);
            GL.GenBuffers(1, out vbo_mview);
            GL.GenBuffers(1, out vbo_projection);
        }

        private int LoadShader(string filename, ShaderType type, int program)
        {
            int shader = GL.CreateShader(type);

            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(shader, sr.ReadToEnd());
            }

            GL.CompileShader(shader);
            GL.AttachShader(program, shader);
            Console.WriteLine(GL.GetShaderInfoLog(shader));

            return shader;
        }
    }
}
