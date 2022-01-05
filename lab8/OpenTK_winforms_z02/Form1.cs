using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OpenTK_winforms_z02
{
    public partial class Form1 : Form
    {
        //Stări de control cameră.
        private int eyePosX, eyePosY, eyePosZ;

        private Point mousePos;

        private float camDepth;

        //Stări de control mouse.
        private bool statusControlMouse2D, statusControlMouse3D, statusMouseDown;

        //Stări de control axe de coordonate.
        private bool statusControlAxe;

        //Stări de control iluminare.
        private bool lightON;
        private bool lightON_0;
        private bool lightON_1;

        //Stări de control obiecte 3D.
        private string statusCube;

        //Structuri de stocare a vertexurilor și a listelor de vertexuri.
        private int[,] arrVertex = new int[50, 3];         //Stocam matricea de vertexuri; 3 coloane vor reține informația pentru X, Y, Z. Nr. de linii definește nr. de vertexuri.
        
        private int nVertex;

        private int[] arrQuadsList = new int[100];        //Lista de vertexuri pentru construirea cubului prin intermediul quadurilor. Ne bazăm pe lista de vertexuri.
        
        private int nQuadsList;

        private int[] arrTrianglesList = new int[100];    //Lista de vertexuri pentru construirea cubului prin intermediul triunghiurilor. Ne bazăm pe lista de vertexuri.
       
        private int nTrianglesList;

        //Fișiere de in/out pentru manipularea vertexurilor.
        private string fileVertex = "vertexList.txt";

        private string fileQList = "quadsVertexList.txt";

        private string fileTList = "trianglesVertexList.txt";

        private bool statusFiles;

        //Dim valuesAmbientTemplate0() As Single = {255, 0, 0, 1.0}      //Valori alternative ambientale(lumină colorată)
        //# SET 1
        private float[] valuesAmbientTemplate0 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesDiffuseTemplate0 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] valuesSpecularTemplate0 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesPositionTemplate0 = new float[] { 0.0f, 0.0f, 5.0f, 1.0f };

        private float[] valuesAmbientTemplate1 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesDiffuseTemplate1 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] valuesSpecularTemplate1 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesPositionTemplate1 = new float[] { 10.0f, 10.0f, 7.0f, 1.0f };
        //# SET 2
        //private float[] valuesAmbientTemplate1 = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
        //private float[] valuesDiffuseTemplate1 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        //private float[] valuesSpecularTemplate1 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        //private float[] valuesPositionTemplate1 = new float[] { 1.0f, 1.0f, 1.0f, 0.0f };

        private float[] valuesAmbient0 = new float[4];
        private float[] valuesDiffuse0 = new float[4];
        private float[] valuesSpecular0 = new float[4];
        private float[] valuesPosition0 = new float[4];


        private float[] valuesAmbient1 = new float[4];
        private float[] valuesDiffuse1 = new float[4];
        private float[] valuesSpecular1 = new float[4];
        private float[] valuesPosition1 = new float[4];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            SetupValues();
            SetupWindowGUI();
        }

        private void SetupValues()
        {
            eyePosX = 100;
            eyePosY = 100;
            eyePosZ = 50;

            camDepth = 1.04f;

            setLight0Values();
            setLight1Values();

            numericXeye.Value = eyePosX;
            numericYeye.Value = eyePosY;
            numericZeye.Value = eyePosZ;
        }
        private void SetupWindowGUI()
        {

            setControlMouse2D(false);
            setControlMouse3D(false);

            numericCameraDepth.Value = (int)camDepth;

            setControlAxe(true);

            setCubeStatus("OFF");
            setIlluminationStatus(false);
            setSource0Status(false);
            setSource1Status(false);

            setTrackLigh0Default();
            setColorAmbientLigh0Default();
            setColorDifuseLigh0Default();
            setColorSpecularLigh0Default();
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   MANIPULARE VERTEXURI ȘI LISTE DE COORDONATE.
        //Încărcarea coordonatelor vertexurilor și lista de compunere a obiectelor 3D.
        private void loadVertex()
        {
            try
            {
                StreamReader fileReader = new StreamReader((fileVertex));
                nVertex = Convert.ToInt32(fileReader.ReadLine().Trim());
                Console.WriteLine("Vertexuri citite: " + nVertex.ToString());

                string tmpStr = "";
                string[] str = new string[3];
                for (int i = 0; i < nVertex; i++)
                {
                    tmpStr = fileReader.ReadLine();
                    str = tmpStr.Trim().Split(' ');
                    arrVertex[i, 0] = Convert.ToInt32(str[0].Trim());
                    arrVertex[i, 1] = Convert.ToInt32(str[1].Trim());
                    arrVertex[i, 2] = Convert.ToInt32(str[2].Trim());
                }
                fileReader.Close();

            }
            catch (Exception)
            {
                statusFiles = false;
                Console.WriteLine("Fisierul cu informații vertex <" + fileVertex + "> nu exista!");
                MessageBox.Show("Fisierul cu informații vertex <" + fileVertex + "> nu exista!");
            }
        }

        private void loadQList()
        {
            try
            {
                StreamReader fileReader = new StreamReader(fileQList);

                int tmp;
                string line;
                nQuadsList = 0;

                while ((line = fileReader.ReadLine()) != null)
                {
                    tmp = Convert.ToInt32(line.Trim());
                    arrQuadsList[nQuadsList] = tmp;
                    nQuadsList++;
                }

                fileReader.Close();
            }
            catch (Exception)
            {
                statusFiles = false;
                MessageBox.Show("Fisierul cu informații vertex <" + fileQList + "> nu exista!");
            }
        }
        private void loadTList()
        {
            try
            {
                StreamReader fileReader = new StreamReader(fileTList);

                int tmp;
                string line;
                nTrianglesList = 0;

                while ((line = fileReader.ReadLine()) != null)
                {
                    tmp = Convert.ToInt32(line.Trim());
                    arrTrianglesList[nTrianglesList] = tmp;
                    nTrianglesList++;
                }

                fileReader.Close();
            }
            catch (Exception)
            {
                statusFiles = false;
                MessageBox.Show("Fisierul cu informații vertex <" + fileTList + "> nu exista!");
            }

        }

        //   CONTROL CAMERĂ
        //Controlul camerei după axa X cu spinner numeric (un cadran).
        private void numericXeye_ValueChanged(object sender, EventArgs e)
        {
            eyePosX = (int)numericXeye.Value;
            GlControl1.Invalidate(); //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
        }

        //Controlul camerei după axa Y cu spinner numeric (un cadran).
        private void numericYeye_ValueChanged(object sender, EventArgs e)
        {
            eyePosY = (int)numericYeye.Value;
            GlControl1.Invalidate(); //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
        }

        //Controlul camerei după axa Z cu spinner numeric (un cadran).
        private void numericZeye_ValueChanged(object sender, EventArgs e)
        {
            eyePosZ = (int)numericZeye.Value;
            GlControl1.Invalidate(); //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
        }

        //Controlul adâncimii camerei față de (0,0,0).
        private void numericCameraDepth_ValueChanged(object sender, EventArgs e)
        {
            camDepth = 1 + ((float)numericCameraDepth.Value) * 0.1f;
            GlControl1.Invalidate();
        }

        //   CONTROL MOUSE
        //Setăm variabila de stare pentru rotația în 2D a mouseului.
        private void setControlMouse2D(bool status)
        {
            if (status == false)
            {
                statusControlMouse2D = false;
                btnMouseControl2D.Text = "2D mouse control OFF";
            }
            else
            {
                statusControlMouse2D = true;
                btnMouseControl2D.Text = "2D mouse control ON";
            }
        }

        //Setăm variabila de stare pentru rotația în 3D a mouseului.
        private void setControlMouse3D(bool status)
        {
            if (status == false)
            {
                statusControlMouse3D = false;
                btnMouseControl3D.Text = "3D mouse control OFF";
            }
            else
            {
                statusControlMouse3D = true;
                btnMouseControl3D.Text = "3D mouse control ON";
            }
        }

        //Controlul mișcării setului de coordonate cu ajutorul mouseului (în plan 2D/3D)
        private void btnMouseControl2D_Click(object sender, EventArgs e)
        {
            if (statusControlMouse2D == true)
            {
                setControlMouse2D(false);
            }
            else
            {
                setControlMouse3D(false);
                setControlMouse2D(true);
            }
        }
        private void btnMouseControl3D_Click(object sender, EventArgs e)
        {
            if (statusControlMouse3D == true)
            {
                setControlMouse3D(false);
            }
            else
            {
                setControlMouse2D(false);
                setControlMouse3D(true);
            }
        }

        //Mișcarea lumii 3D cu ajutorul mouselui (click'n'drag de mouse).
        private void GlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (statusMouseDown == true)
            {
                mousePos = new Point(e.X, e.Y);
                GlControl1.Invalidate();     //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
            }
            if (lightON == true)
            {
                valuesPosition1[1] = e.X;
                valuesPosition1[2] = e.Y;
            }
        }
        private void GlControl1_MouseDown(object sender, MouseEventArgs e)
        {
            statusMouseDown = true;
        }
        private void GlControl1_MouseUp(object sender, MouseEventArgs e)
        {
            statusMouseDown = false;
        }

        //   CONTROL ILUMINARE
        //Setăm variabila de stare pentru iluminare.
        private void setIlluminationStatus(bool status)
        {
            if (status == false)
            {
                lightON = false;
                btnLights.Text = "Iluminare OFF";
            }
            else
            {
                lightON = true;
                btnLights.Text = "Iluminare ON";
            }
        }

        //Activăm/dezactivăm iluminarea.
        private void btnLights_Click(object sender, EventArgs e)
        {
            if (lightON == false)
            {
                setIlluminationStatus(true);
            }
            else
            {
                setIlluminationStatus(false);
            }
            GlControl1.Invalidate();
        }

        //Identifică numărul maxim de lumini pentru implementarea curentă a OpenGL.
        private void btnLightsNo_Click(object sender, EventArgs e)
        {
            int nr = GL.GetInteger(GetPName.MaxLights);
            MessageBox.Show("Nr. maxim de luminii pentru aceasta implementare este <" + nr.ToString() + ">.");
        }

        //Setăm variabila de stare pentru sursa de lumină 0.
        private void setSource0Status(bool status)
        {
            if (status == false)
            {
                lightON_0 = false;
                btnLight0.Text = "Sursa 0 OFF";
            }
            else
            {
                lightON_0 = true;
                btnLight0.Text = "Sursa 0 ON";
            }
        }
        private void setSource1Status(bool status)
        {
            if (status == false)
            {
                lightON_1 = false;
                btnLight1.Text = "Sursa 1 OFF";
            }
            else
            {
                lightON_1 = true;
                btnLight1.Text = "Sursa 1 ON";
            }
        }

        //Activăm/dezactivăm sursa 0 de iluminare (doar dacă iluminarea este activă).
        private void btnLight0_Click(object sender, EventArgs e)
        {
            if (lightON == true)
            {
                if (lightON_0 == false)
                {
                    setSource0Status(true);
                }
                else
                {
                    setSource0Status(false);
                }
                GlControl1.Invalidate();
            }
        }

        //Schimbăm poziția sursei 0 de iluminare după axele XYZ.
        private void setTrackLigh0Default()
        {
            trackLight0PositionX.Value = (int)valuesPosition0[0];
            trackLight0PositionY.Value = (int)valuesPosition0[1];
            trackLight0PositionZ.Value = (int)valuesPosition0[2];
        }

        private void trackLight0PositionX_Scroll(object sender, EventArgs e)
        {
            valuesPosition0[0] = trackLight0PositionX.Value;
            GlControl1.Invalidate();
        }
        private void trackLight0PositionY_Scroll(object sender, EventArgs e)
        {
            //MessageBox.Show(Convert.ToString(trackLight0PositionY.Value));
            valuesPosition0[1] = trackLight0PositionY.Value;
            GlControl1.Invalidate();
        }
        private void trackLight0PositionZ_Scroll(object sender, EventArgs e)
        {
            valuesPosition0[2] = trackLight0PositionZ.Value;
            GlControl1.Invalidate();
        }

        private void GlControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (lightON == true)
            {
                if (e.KeyCode == Keys.W)
                {
                    valuesPosition1[2] += 1;
                }
                if (e.KeyCode == Keys.S)
                {
                    valuesPosition1[2] -= 1;
                }
                if (e.KeyCode == Keys.A)
                {
                    valuesPosition1[0] += 1;
                }
                if (e.KeyCode == Keys.D)
                {
                    valuesPosition1[0] -= 1;
                }
                if (e.KeyCode == Keys.Q)
                {
                    valuesPosition1[1] += 1;
                }
                if (e.KeyCode == Keys.R)
                {
                    valuesPosition1[1] -= 1;
                }
                GlControl1.Invalidate();
            }
        }

        //Schimbăm culoarea sursei de lumină 0 (ambiental) în domeniul RGB.
        private void setColorAmbientLigh0Default()
        {
            numericLight0Ambient_Red.Value = (decimal)valuesAmbient0[0];
            numericLight0Ambient_Green.Value = (decimal)valuesAmbient0[1];
            numericLight0Ambient_Blue.Value = (decimal)valuesAmbient0[2];
        }

        private void numericLight0Ambient_Red_ValueChanged(object sender, EventArgs e)
        {
            valuesAmbient0[0] = (float)numericLight0Ambient_Red.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Ambient_Green_ValueChanged(object sender, EventArgs e)
        {
            valuesAmbient0[1] = (float)numericLight0Ambient_Green.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Ambient_Blue_ValueChanged(object sender, EventArgs e)
        {
            valuesAmbient0[2] = (float)numericLight0Ambient_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //Schimbăm culoarea sursei de lumină 0 (difuză) în domeniul RGB.
        private void setColorDifuseLigh0Default()
        {
            numericLight0Difuse_Red.Value = (decimal)valuesDiffuse0[0];
            numericLight0Difuse_Green.Value = (decimal)valuesDiffuse0[1];
            numericLight0Difuse_Blue.Value = (decimal)valuesDiffuse0[2];
        }
        private void numericLight0Difuse_Red_ValueChanged(object sender, EventArgs e)
        {
            valuesDiffuse0[0] = (float)numericLight0Difuse_Red.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Difuse_Green_ValueChanged(object sender, EventArgs e)
        {
            valuesDiffuse0[1] = (float)numericLight0Difuse_Green.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Difuse_Blue_ValueChanged(object sender, EventArgs e)
        {
            valuesDiffuse0[2] = (float)numericLight0Difuse_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //Schimbăm culoarea sursei de lumină 0 (specular) în domeniul RGB.
        private void setColorSpecularLigh0Default()
        {
            numericLight0Specular_Red.Value = (decimal)valuesSpecular0[0];
            numericLight0Specular_Green.Value = (decimal)valuesSpecular0[1];
            numericLight0Specular_Blue.Value = (decimal)valuesSpecular0[2];
        }
        private void numericLight0Specular_Red_ValueChanged(object sender, EventArgs e)
        {
            valuesSpecular0[0] = (float)numericLight0Specular_Red.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Specular_Green_ValueChanged(object sender, EventArgs e)
        {
            valuesSpecular0[1] = (float)numericLight0Specular_Green.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Specular_Blue_ValueChanged(object sender, EventArgs e)
        {
            valuesSpecular0[2] = (float)numericLight0Specular_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //Resetare stare sursă de lumină nr. 0.
        private void setLight0Values()
        {
            for (int i = 0; i < valuesAmbientTemplate0.Length; i++)
            {
                valuesAmbient0[i] = valuesAmbientTemplate0[i];
            }
            for (int i = 0; i < valuesDiffuseTemplate0.Length; i++)
            {
                valuesDiffuse0[i] = valuesDiffuseTemplate0[i];
            }
            for (int i = 0; i < valuesPositionTemplate0.Length; i++)
            {
                valuesPosition0[i] = valuesPositionTemplate0[i];
            }
        }
        private void btnLight0Reset_Click(object sender, EventArgs e)
        {
            setLight0Values();
            setLight1Values();
            setTrackLigh0Default();
            setColorAmbientLigh0Default();
            setColorDifuseLigh0Default();
            setColorSpecularLigh0Default();
            GlControl1.Invalidate();
        }

        private void setLight1Values()
        {
            for (int i = 0; i < valuesAmbientTemplate1.Length; i++)
            {
                valuesAmbient1[i] = valuesAmbientTemplate1[i];
            }
            for (int i = 0; i < valuesDiffuseTemplate1.Length; i++)
            {
                valuesDiffuse1[i] = valuesDiffuseTemplate1[i];
            }
            for (int i = 0; i < valuesPositionTemplate1.Length; i++)
            {
                valuesPosition1[i] = valuesPositionTemplate1[i];
            }
        }

        //   CONTROL OBIECTE 3D
        //Setăm variabila de stare pentru afișarea/scunderea sistemului de coordonate.
        private void setControlAxe(bool status)
        {
            if (status == false)
            {
                statusControlAxe = false;
                btnShowAxes.Text = "Axe Oxyz OFF";
            }
            else
            {
                statusControlAxe = true;
                btnShowAxes.Text = "Axe Oxyz ON";
            }
        }

        //Controlul axelor de coordonate (ON/OFF).
        private void btnShowAxes_Click(object sender, EventArgs e)
        {
            if (statusControlAxe == true)
            {
                setControlAxe(false);
            }
            else
            {
                setControlAxe(true);
            }
            GlControl1.Invalidate();
        }

        //Setăm variabila de stare pentru desenarea cubului. Valorile acceptabile sunt:
        //TRIANGLES = cubul este desenat, prin triunghiuri.
        //QUADS = cubul este desenat, prin quaduri.
        //OFF (sau orice altceva) = cubul nu este desenat.
        private void setCubeStatus(string status)
        {
            if (status.Trim().ToUpper().Equals("TRIANGLES"))
            {
                statusCube = "TRIANGLES";
            }
            else if (status.Trim().ToUpper().Equals("QUADS"))
            {
                statusCube = "QUADS";
            }
            else
            {
                statusCube = "OFF";
            }
        }
        private void btnCubeQ_Click(object sender, EventArgs e)
        {
            statusFiles = true;
            loadVertex();
            loadQList();
            setCubeStatus("QUADS");
            GlControl1.Invalidate();
        }
        private void btnCubeT_Click(object sender, EventArgs e)
        {
            statusFiles = true;
            loadVertex();
            loadTList();
            setCubeStatus("TRIANGLES");
            GlControl1.Invalidate();
        }
        private void btnResetObjects_Click(object sender, EventArgs e)
        {
            setCubeStatus("OFF");
            GlControl1.Invalidate();
        }

        private void btnLight1_Click(object sender, EventArgs e)
        {
            if (lightON == true)
            {
                if (lightON_1 == false)
                {
                    setSource1Status(true);
                }
                else
                {
                    setSource1Status(false);
                }
                GlControl1.Invalidate();
            }
        }

        private void GlControl1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {

        }

        //   ADMINISTRARE MOD 3D (METODA PRINCIPALĂ)
        private void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            //Resetează buffer-ele la valori default.
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            //Culoarea default a mediului.
            GL.ClearColor(Color.Black);

            //Setări preliminară pentru mediul 3D.
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(camDepth, 4 / 3, 1, 10000);    //Declararea perspectivei spatiale.
            Matrix4 lookat = Matrix4.LookAt(eyePosX, eyePosY, eyePosZ, 0, 0, 0, 0, 1, 0);             //Declararea camerei (stare inițială).
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);                                                             //Încărcarea modelului camerei.
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height);                                      //Mărimea suprafeței randate (scena 3D este proiectată pe aceasta).
            GL.Enable(EnableCap.DepthTest);                                                                //Corecții de adâncime.
            GL.DepthFunc(DepthFunction.Less);                                                                //Corecții de adâncime.

            //Pornim iluminarea (daca avem permisiunea să o facem).
            if (lightON == true)
            {
                //Permite utilizarea iluminării. Fara aceasta corecție iluminarea nu functioneaza.
                GL.Enable(EnableCap.Lighting);
            }
            else
            {
                //Dezactivează utilizarea iluminării.
                GL.Disable(EnableCap.Lighting);
            }

            //Se creeaza sursa de iluminare. In acest caz folosim o singura sursa.
            //Numarul de surse de lumini depinde de implementarea OpenGL, dar de obicei cel putin 8 surse sunt posibile simultan.
            GL.Light(LightName.Light0, LightParameter.Ambient, valuesAmbient0);
            GL.Light(LightName.Light0, LightParameter.Diffuse, valuesDiffuse0);
            GL.Light(LightName.Light0, LightParameter.Specular, valuesSpecular0);
            GL.Light(LightName.Light0, LightParameter.Position, valuesPosition0);

            GL.Light(LightName.Light1, LightParameter.Ambient, valuesAmbient1);
            GL.Light(LightName.Light1, LightParameter.Diffuse, valuesDiffuse1);
            GL.Light(LightName.Light1, LightParameter.Specular, valuesSpecular1);
            GL.Light(LightName.Light1, LightParameter.Position, valuesPosition1);

            if ((lightON == true) && (lightON_0 == true))
            {
                GL.Enable(EnableCap.Light0);
            }
            else
            {
                GL.Disable(EnableCap.Light0);
            }

            if ((lightON == true) && (lightON_1 == true))
            {
                GL.Enable(EnableCap.Light1);
            }
            else
            {
                GL.Disable(EnableCap.Light1);
            }

            if (statusControlMouse2D == true)
            {
                GL.Rotate(mousePos.X, 0, 1, 0);
            }

            if (statusControlMouse3D == true)
            {
                GL.Rotate(mousePos.X, 0, 1, 1);
            }

            if (statusControlAxe == true)
            {
                DeseneazaAxe();
            }

            if (statusCube.ToUpper().Equals("QUADS"))
            {
                DeseneazaCubQ();
            }
            else if (statusCube.ToUpper().Equals("TRIANGLES"))
            {
                DeseneazaCubT();
            }

            //Limitează viteza de randare pentru a nu supraîncarca unitatea GPU (în caz contrar randarea se face cât de rapid este posibil, pe 100% din resurse). 
            //Dezavantajul este că o viteză prea mică poate afecta negativ cursivitatea animației!
            //GraphicsContext.CurrentContext.SwapInterval = 1;                                         //Testati cu valori din 10 in 10!!!
            //GraphicsContext.CurrentContext.VSync = True

            GlControl1.SwapBuffers();
        }

        private void DeseneazaAxe()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(75, 0, 0);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 75, 0);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 75);
            GL.End();
        }

        private void DeseneazaCubQ()
        {
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < nQuadsList; i++)
            {
                switch (i % 4)
                {
                    case 0:
                        GL.Color3(Color.Blue);
                        break;
                    case 1:
                        GL.Color3(Color.Red);
                        break;
                    case 2:
                        GL.Color3(Color.Green);
                        break;
                    case 3:
                        GL.Color3(Color.Yellow);
                        break;
                }
                int x = arrQuadsList[i];
                GL.Vertex3(arrVertex[x, 0], arrVertex[x, 1], arrVertex[x, 2]);
            }
            GL.End();
        }

        private void DeseneazaCubT()
        {
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < nTrianglesList; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        GL.Color3(Color.Blue);
                        break;
                    case 1:
                        GL.Color3(Color.Red);
                        break;
                    case 2:
                        GL.Color3(Color.Green);
                        break;
                }
                int x = arrTrianglesList[i];
                GL.Vertex3(arrVertex[x, 0], arrVertex[x, 1], arrVertex[x, 2]);
            }
            GL.End();
        }

    }

}
