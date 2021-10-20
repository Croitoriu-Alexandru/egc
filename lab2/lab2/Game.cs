using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Tema
{
    class OpenGLApplication : GameWindow
    {
        private bool combo = false, comboHit;

        public OpenGLApplication(int width, int height, string title)
            : base(width, height)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color.Blue);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (comboHit)
            {
                Draw();
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();
            MouseState mouseInput = Mouse.GetCursorState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.C))
            {
                combo = true;
            }

            if (input.IsKeyDown(Key.S) && combo)
            {
                comboHit = true;
                combo = false;
            }
        }

        private void Draw()
        {
            GL.Begin(PrimitiveType.Lines);

            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(20, 0, 0);

            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 20, 0);

            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 20);

            GL.End();
        }
    }
}