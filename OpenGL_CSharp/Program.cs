﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_CSharp.Geometery;
using OpenGL_CSharp.Graphic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGL_CSharp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //initialize window
            var win = new GameWindow(800, 800, OpenTK.Graphics.GraphicsMode.Default, "Test", GameWindowFlags.Default, DisplayDevice.Default, 3, 3, OpenTK.Graphics.GraphicsContextFlags.Debug);

            //setupsceansettings
            SetupScene(win);
            pipe.win = win;

            win.Load += Win_Load; //one time load on start
            win.UpdateFrame += Win_UpdateFrame; //on each frame do this     

            //Navigate setting
            //----------------
            win.Closing += Win_Closing; //on termination do this
            win.KeyDown += Win_KeyDown; //keydown event
            win.MouseWheel += Win_MouseWheel;

            //start game window
            win.Run(30);


        }
        private static void SetupScene(GameWindow win)
        {
            //intialize holder for the project main variables
            pipe = new Pipelinevars();
            //defin viewport size
            GL.Viewport(100, 100, 700, 700);
            GL.ClearColor(Color.Black);//set background color
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back); //set which face to be hidden            
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill); //set polygon draw mode
            GL.Enable(EnableCap.DepthTest);
        }


        #region Navigation
        private static void Win_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            cam.Fov(e.DeltaPrecise);
            cam.updateCamera();
        }

        private static void Win_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {

            var win = (GameWindow)sender;

            //var rh = Vector3.CalculateAngle(cam.Position, Vector3.UnitZ);
            //var rv = Vector3.CalculateAngle(cam.Position, Vector3.UnitX);


            //if (Math.Cos(rh) <= 0 && Math.Sin(rh) >= 0) Hangle = MathHelper.RadiansToDegrees(Math.PI - rh);
            //if (Math.Cos(rh) <= 0 && Math.Sin(rh) <= 0) Hangle = MathHelper.RadiansToDegrees(Math.PI + rh);
            //if (Math.Cos(rh) >= 0 && Math.Sin(rh) <= 0) Hangle = MathHelper.RadiansToDegrees(2 * Math.PI + rh);


            Debug.WriteLine("Hangle: " + Hangle);
            if (e.Key == Key.Z || e.Key == Key.X)
            {
                if (e.Key == Key.Z)
                {
                    BaseGeometry.specintens -= 5f;
                }
                else
                {
                    BaseGeometry.specintens += 5f;
                }

                pipe.geos.ForEach(o =>
                {
                    BaseGeometry.specintens = BaseGeometry.specintens < 0 ? 0.1f : BaseGeometry.specintens;

                });
            }

            if (e.Key == OpenTK.Input.Key.Right)
            {
                cam.Position -= new Vector3(.2f, 0, 0);
                cam.Target -= new Vector3(.2f, 0, 0);
            }
            if (e.Key == OpenTK.Input.Key.Left)
            {
                cam.Position += new Vector3(.2f, 0, 0);
                cam.Target += new Vector3(.2f, 0, 0);

            }

            if (e.Key == OpenTK.Input.Key.Down)
            {
                cam.Position -= new Vector3(0f, .2f, 0);
                cam.Target -= new Vector3(0f, .2f, 0);
            }
            if (e.Key == OpenTK.Input.Key.Up)
            {
                cam.Position += new Vector3(0f, 0.2f, 0);
                cam.Target += new Vector3(0, 0.2f, 0);

            }

            if (e.Key == OpenTK.Input.Key.Escape)
            {
                ((GameWindow)sender).Close();
            }


            if (e.Key == OpenTK.Input.Key.W)
            {
                Vangle += 10;

                //var m1 = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Vangle));
                //cam.View = cam.View * m1;
                //cam.Position = cam.View.ExtractTranslation();

                cam.Position = new Vector3(cam.Position.X, (float)Math.Cos(MathHelper.DegreesToRadians(Vangle)) * r, (float)Math.Sin(MathHelper.DegreesToRadians(Vangle)) * r);

            }

            if (e.Key == OpenTK.Input.Key.S)
            {
                Vangle -= 10;


                //var m1 = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Vangle));
                //cam.View = cam.View * m1;
                //cam.Position = cam.View.ExtractTranslation();
                cam.Position = new Vector3(cam.Position.X, (float)Math.Cos(MathHelper.DegreesToRadians(Vangle)) * r, (float)Math.Sin(MathHelper.DegreesToRadians(Vangle)) * r);

            }

            if (e.Key == OpenTK.Input.Key.A)
            {
                Hangle -= 10;

                cam.Position = new Vector3((float)Math.Sin(MathHelper.DegreesToRadians(Hangle)) * r, cam.Position.Y, (float)Math.Cos(MathHelper.DegreesToRadians(Hangle)) * r);

            }

            if (e.Key == OpenTK.Input.Key.D)
            {
                Hangle += 10;


                cam.Position = new Vector3((float)Math.Sin(MathHelper.DegreesToRadians(Hangle)) * r, cam.Position.Y, (float)Math.Cos(MathHelper.DegreesToRadians(Hangle)) * r);
            }

            var mouse = Mouse.GetState();

            if (e.Key == Key.ControlLeft && win.Focused)
            {
                var dx = mouse.X / 800f - oldx;
                var dy = mouse.Y / 800f - oldy;

                win.CursorVisible = true;
                cam.Target += new Vector3(dx, dy, 0);
                oldx = mouse.X / 800f;
                oldy = mouse.Y / 800f;
            }
            else
            {
                win.CursorVisible = true;
                cam.Target = Vector3.Zero;
            }

            cam.updateCamera();
        }

        static float oldx = 0;
        static float oldy = 0;

        static float r = 5f;
        static double Hangle = 0;
        static double Vangle = 0;
        #endregion

        public static Camera cam = new Camera();
        public static Pipelinevars pipe; //just global class for all required variables
        public class Pipelinevars
        {
            public int programId = -1;

            public float offsetX = 0.5f;
            public float speed = .3f;
            internal GameWindow win;

            public List<Geometery.BaseGeometry> geos = new List<BaseGeometry>();
        }

        private static void Win_Load(object sender, EventArgs e)
        {
            r = cam.Position.Length; //update the current distance from the camera to position 0

            //defin the shap to be drawn             
            pipe.geos.Add(new CreateCube());
            pipe.geos.Add(new Pyramid());
            pipe.geos[0].model = pipe.geos[0].model * Matrix4.CreateTranslation(-0.75f, 0f, 0f);
            pipe.geos[1].model = pipe.geos[1].model * Matrix4.CreateTranslation(0.75f, 0f, 0f);
        }

        private static void Win_UpdateFrame(object sender, FrameEventArgs e)
        {
            //clear the scean from any drawing before drawing
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit); //this is required to redraw all the depth changes due to camera/View/Object movement

            for (int i = 0; i < pipe.geos.Count; i++)
            {
                pipe.geos[i].Init();
                //bind vertex object
                GL.BindVertexArray(pipe.geos[i].vao);


                Shaders.VertexShaders.SetUniformMatrix(pipe.programId, nameof(cam.View), ref cam.View);
                Shaders.VertexShaders.SetUniformMatrix(pipe.programId, nameof(cam.Projection), ref cam.Projection);


                GL.DrawElements(PrimitiveType.Triangles, pipe.geos[i].Indeces.Length, DrawElementsType.UnsignedInt, 0);

                //clear the buffer
                GL.BindVertexArray(0);
            }
            //swap the buffer (bring what has been rendered in theback to the front)
            pipe.win.SwapBuffers();
        }

        private static void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cleanup();
        }

        static void cleanup()
        {
            //clear resources from here
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            for (int i = 0; i < pipe.geos.Count; i++)
            {
                GL.DeleteBuffer(pipe.geos[i].vbo);
                GL.DeleteVertexArray(pipe.geos[i].vao);
                GL.DeleteShader(pipe.geos[i].vershad);
                GL.DeleteShader(pipe.geos[i].fragshad);
                GL.DeleteShader(pipe.geos[i].texid1);
                GL.DeleteShader(pipe.geos[i].texid2);

            }

            GL.DeleteProgram(pipe.programId);
        }

    }
}
