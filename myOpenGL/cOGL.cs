using System;
using System.Collections.Generic;
using System.Windows.Forms;

//2
using System.Drawing;
using System.Diagnostics;

namespace OpenGL
{
    class cOGL
    {
        Control p;
        int Width;
        int Height;

        public int intOptionA=1;

        GLUquadric obj;

        public cOGL(Control pb)
        {
            p=pb;
            Width = p.Width;
            Height = p.Height;
            isAtmosphere = true;
            isReflection = false;
            isLight = false;
            isAnimate = false;
            isInside = false;
            isBounds = false;
            viewAngle = 45;
            Xangl = 0;
            Yangl = 0;
            Zangl = 0;

            intOptionA = 1;

            InitializeGL();
            obj = GLU.gluNewQuadric();
            PrepareLists();

        }

        ~cOGL()
        {
            GLU.gluDeleteQuadric(obj); 
            WGL.wglDeleteContext(m_uint_RC);
        }

		uint m_uint_HWND = 0;

        public uint HWND
		{
			get{ return m_uint_HWND; }
		}
		
        uint m_uint_DC   = 0;

        public uint DC
		{
			get{ return m_uint_DC;}
		}
		uint m_uint_RC   = 0;

        public uint RC
		{
			get{ return m_uint_RC; }
		}
        



        public bool isAtmosphere;
        public bool isReflection;
        public bool isLight; // Enables light functions.
        public int viewAngle;
		public bool isAnimate;
		public bool isInside;
        public bool isBounds;
        public float Xangl;
        public float Yangl;
        public float Zangl;
        public double forward = -10, left = 0, up = 0;


        void DrawBounds()
        {
            GL.glColor3f(1.0f, 1.0f, 1.0f);

            

            drawEarth();
            GL.glDepthMask((byte)GL.GL_FALSE);
            GL.glDepthMask((byte)GL.GL_TRUE);
            GL.glDisable(GL.GL_STENCIL_TEST);

            //for dubbuging
            //drawWall();

        }

        private void drawEarth()
        {
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_TEXTURE_2D); // enables texture for the earth sphere \
            GL.glRotatef(-90, 1, 1, 1); // AFTER code update, this rotates both rocket and earth
            GLU.gluQuadricDrawStyle(obj, GLU.GLU_FILL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[6]); // applies texture to earth sphere

            GLU.gluQuadricTexture(obj, 1);
            GLU.gluQuadricNormals(obj, GLU.GLU_SMOOTH);
            GLU.gluSphere(obj, 3, 30, 30); //earth
    
            GL.glDisable(GL.GL_TEXTURE_2D); // stop using the earth textures
            GL.glColor3f(1, 0, 0);

            if (isAtmosphere)
            {
                GL.glEnable(GL.GL_BLEND);
                GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
                GL.glColor4d(0.2, 0.2, 0.4, 0.3); // must be before sphere, last colour is transperancy
                GLU.gluSphere(obj, 3.2, 31, 31);
                GL.glDisable(GL.GL_BLEND);
            }


            GL.glDisable(GL.GL_TEXTURE_2D);
            GL.glDisable(GL.GL_LIGHTING);
            GL.glRotatef(0, 1, 1, 1); // rotate earth straight for camera here before translate, cuz it's in point of origin. UPDATE: Rorates rocket now because of mat changes. Doesn't change earth textures now because above we disabled it.
                
        }


        float[,] shadowWall = new float[,]
        {
            { -3.1f, 1, 0 },
            {-3.1f, 3, 3f },
            { -3.1f, -3, 3 }
        };

        float[] lightPosition = new float[] { 0, -3, 0, 1 };

        void drawWall()//for debug
        {
            GL.glBegin(GL.GL_LINES);
            //x  RED
            GL.glColor3f(1.0f, 0.0f, 0.0f);
            GL.glVertex3f(-30.0f, 0.0f, 0.0f);
            GL.glVertex3f(30.0f, 0.0f, 0.0f);
            //y  GREEN 
            GL.glColor3f(0.0f, 1.0f, 0.0f);
            GL.glVertex3f(0.0f, -30.0f, 0.0f);
            GL.glVertex3f(0.0f, 30.0f, 0.0f);
            //z  BLUE
            GL.glColor3f(0.0f, 0.0f, 1.0f);
            GL.glVertex3f(0.0f, 0.0f, -30.0f);
            GL.glVertex3f(0.0f, 0.0f, 30.0f);
            GL.glEnd();



        }
        double[] AccumulatedRotationsTraslations = new double[16];


        void DrawReflection()//draw reflection floor
        {

            //drawWall();

            GL.glTranslatef(0, 0, 3);

            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

            GL.glEnable(GL.GL_STENCIL_TEST);
            GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
            GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
            GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
            GL.glDisable(GL.GL_DEPTH_TEST);



            GL.glEnable(GL.GL_LIGHTING);

            GL.glBegin(GL.GL_QUADS);
            GL.glColor4d(1, 1, 1, 0.3);
            GL.glVertex3d(-10, -10, 3);
            GL.glVertex3d(-10, 10, 3);
            GL.glVertex3d(10, 10, 3);
            GL.glVertex3d(10, -10, 3);
            GL.glEnd();



            // restore regular settings
                        GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
            GL.glEnable(GL.GL_DEPTH_TEST);

            // reflection is drawn only where STENCIL buffer value equal to 1
            GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
            GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

            GL.glEnable(GL.GL_STENCIL_TEST);

            // draw reflected scene
            GL.glPushMatrix();

            GL.glScalef(1, 1, -1); 
            GL.glEnable(GL.GL_CULL_FACE);
            GL.glCullFace(GL.GL_BACK);
            GL.glCallList(ROCKET_LIST);
            GL.glTranslatef(0, 0, -8);

            GL.glRotatef(90, 4, 2,1);
            GL.glScalef(1, -1, 1); 
            drawEarth();

            GL.glCullFace(GL.GL_FRONT);
            //GL.glCallList(ROCKET_LIST);
            //drawEarth();
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glPopMatrix();



            GL.glDepthMask((byte)GL.GL_FALSE);

            GL.glDepthMask((byte)GL.GL_TRUE);
            GL.glDisable(GL.GL_STENCIL_TEST);
            GL.glColor3f(0.5f, 0.5f, 0.5f);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[7]);
        


            GL.glDisable(GL.GL_LIGHTING);
            GL.glDisable(GL.GL_TEXTURE_2D);

        }

        void ReduceToUnit(float[] vector)
        {
            float length;

            // Calculate the length of the vector		
            length = (float)Math.Sqrt((vector[0] * vector[0]) +
                                (vector[1] * vector[1]) +
                                (vector[2] * vector[2]));

            // Keep the program from blowing up by providing an exceptable
            // value for vectors that may calculated too close to zero.
            if (length == 0.0f)
                length = 1.0f;

            // Dividing each element by the length will result in a
            // unit normal vector.
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }

        const int x = 0;
        const int y = 1;
        const int z = 2;

        // Points p1, p2, & p3 specified in counter clock-wise order
        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            // Calculate two vectors from the three points
            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            // Take the cross product of the two vectors to get
            // the normal vector which will be stored in out
            outp[x] = v1[y] * v2[z] - v1[z] * v2[y];
            outp[y] = v1[z] * v2[x] - v1[x] * v2[z];
            outp[z] = v1[x] * v2[y] - v1[y] * v2[x];

            // Normalize the vector (shorten length to one)
            ReduceToUnit(outp);
        }

        float[] cubeXform = new float[16];

        void MakeShadowMatrix(float[,] points)
        {
            float[] planeCoeff = new float[4];
            float dot;

            // Find the plane equation coefficients
            // Find the first three coefficients the same way we
            // find a normal.
            calcNormal(points, planeCoeff);

            // Find the last coefficient by back substitutions
            planeCoeff[3] = -(
                (planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) +
                (planeCoeff[2] * points[2, 2]));


            // Dot product of plane and light position
            dot = planeCoeff[0] * lightPosition[0] +
                    planeCoeff[1] * lightPosition[1] +
                    planeCoeff[2] * lightPosition[2] +
                    planeCoeff[3];

            // Now do the projection
            // First column
            cubeXform[0] = dot - lightPosition[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - lightPosition[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - lightPosition[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - lightPosition[0] * planeCoeff[3];

            // Second column
            cubeXform[1] = 0.0f - lightPosition[1] * planeCoeff[0];
            cubeXform[5] = dot - lightPosition[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - lightPosition[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - lightPosition[1] * planeCoeff[3];

            // Third Column
            cubeXform[2] = 0.0f - lightPosition[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - lightPosition[2] * planeCoeff[1];
            cubeXform[10] = dot - lightPosition[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - lightPosition[2] * planeCoeff[3];

            // Fourth Column
            cubeXform[3] = 0.0f - lightPosition[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - lightPosition[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - lightPosition[3] * planeCoeff[2];
            cubeXform[15] = dot - lightPosition[3] * planeCoeff[3];
        }





        float oldViewAngle = 0.0f;

        public void Draw()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;


            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);

            if (oldViewAngle != viewAngle)
            {
                GL.glMatrixMode(GL.GL_PROJECTION);
                GL.glLoadIdentity();
                oldViewAngle = viewAngle;

                GLU.gluPerspective(viewAngle, (float)Width / (float)Height, 0.45f, 30.0f);

                GL.glMatrixMode(GL.GL_MODELVIEW);
                GL.glLoadIdentity();
            }
            GL.glLoadIdentity();


            GL.glRotatef(Xangl, 1.0f, 0.0f, 0.0f);
            GL.glRotatef(Yangl, 0.0f, 1.0f, 0.0f);
            GL.glRotatef(Zangl, 0.0f, 0.0f, 1.0f);

            GLU.gluLookAt
                (forward, 0, left,
                0, 0, 0,
                0, 1, 0);
            createLight();//forward = -10 , left = 0

            GL.glDisable(GL.GL_LIGHTING);
            GL.glDisable(GL.GL_BLEND);
            GL.glDisable(GL.GL_TEXTURE_2D);
            DrawBounds();
            //drawEarth();

            //!!!!
            GL.glColor4f(1.0f, 1.0f, 1.0f, 0.5f);
            //!!!!

            GL.glEnable(GL.GL_TEXTURE_2D);


            GL.glDisable(GL.GL_BLEND);
            DrawTexturedCube();




            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_TEXTURE_2D);
            CreateRocketList();

            GL.glRotatef(ROCKET_angle, 1, 1, 0);

            GL.glCallList(ROCKET_LIST);


            GL.glDisable(GL.GL_LIGHTING);
            GL.glDisable(GL.GL_TEXTURE_2D);
            GL.glPushMatrix();
            //!!!!!!!!!!!!    		
            MakeShadowMatrix(shadowWall);
            GL.glMultMatrixf(cubeXform);
            GL.glColor3f(0.0f, 0.0f, 0.0f);
            GL.glCallList(ROCKET_LIST);


            GL.glPopMatrix();
            GL.glEnable(GL.GL_LIGHTING);


            if (isReflection)
            {
                DrawReflection();
            }


            GL.glFlush();

            WGL.wglSwapBuffers(m_uint_DC);

        }

        private void createLight()
        {
            // Light work
            if (isLight)
            {
                GL.glEnable(GL.GL_LIGHTING);


                float[] light_ambient = new float[] { 0, 0, 0, 1 }; // ambient color of material
                float[] light_diffuse = new float[] { 2, 2, 2, 0 }; // diffuse color of material
                float[] light_specular = new float[] { 5, 5, 5, 5 }; // specular color of material
                float[] light_position = new float[] { -1, 10, 2, 0 }; // (x, y, z, w), w = 0 means directional light.





                GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, light_ambient);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, light_diffuse);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, light_specular);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light_position);
                GL.glDisable(GL.GL_LIGHTING);
            }
            else
            {
                GL.glEnable(GL.GL_LIGHTING);

                float[] light_ambient = new float[] { 1, 1, 1, 1 };
                float[] light_diffuse = new float[] { 1, 1, 1, 1 };
                float[] light_specular = new float[] { 1, 1, 1, 1 };
                float[] light_position = new float[] { 1, 1, 1, 1 };

                GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, light_ambient);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, light_diffuse);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, light_specular);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light_position);
                GL.glDisable(GL.GL_LIGHTING);
            }
            // End of Light work
        }

        protected virtual void InitializeGL()
		{
			m_uint_HWND = (uint)p.Handle.ToInt32();
			m_uint_DC   = WGL.GetDC(m_uint_HWND);

            // Not doing the following WGL.wglSwapBuffers() on the DC will
			// result in a failure to subsequently create the RC.
			WGL.wglSwapBuffers(m_uint_DC);

			WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
			WGL.ZeroPixelDescriptor(ref pfd);
			pfd.nVersion        = 1; 
			pfd.dwFlags         = (WGL.PFD_DRAW_TO_WINDOW |  WGL.PFD_SUPPORT_OPENGL |  WGL.PFD_DOUBLEBUFFER); 
			pfd.iPixelType      = (byte)(WGL.PFD_TYPE_RGBA);
			pfd.cColorBits      = 32;
			pfd.cDepthBits      = 32;
			pfd.iLayerType      = (byte)(WGL.PFD_MAIN_PLANE);


            pfd.cStencilBits = 32;



            int pixelFormatIndex = 0;
			pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
			if(pixelFormatIndex == 0)
			{
				MessageBox.Show("Unable to retrieve pixel format");
				return;
			}

			if(WGL.SetPixelFormat(m_uint_DC,pixelFormatIndex,ref pfd) == 0)
			{
				MessageBox.Show("Unable to set pixel format");
				return;
			}
			//Create rendering context
			m_uint_RC = WGL.wglCreateContext(m_uint_DC);
			if(m_uint_RC == 0)
			{
				MessageBox.Show("Unable to get rendering context");
				return;
			}
			if(WGL.wglMakeCurrent(m_uint_DC,m_uint_RC) == 0)
			{
				MessageBox.Show("Unable to make rendering context current");
				return;
			}


            initRenderingGL();
        }

        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            Draw();
        }

        protected virtual void initRenderingGL()
		{
			if(m_uint_DC == 0 || m_uint_RC == 0)
				return;
			if(this.Width == 0 || this.Height == 0)
				return;
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);

            GL.glViewport(0, 0, this.Width, this.Height);
			GL.glMatrixMode ( GL.GL_PROJECTION );
			GL.glLoadIdentity();

            //! TEXTURE 1a 
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            float[] emis ={ 0.3f, 0.3f, 0.3f, 1 };
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_EMISSION, emis);
            //! TEXTURE 1a 

            
            
	        GL.glShadeModel(GL.GL_SMOOTH);
            GLU.gluPerspective(viewAngle, (float)Width / (float)Height, 0.45f, 30.0f);

            GL.glMatrixMode ( GL.GL_MODELVIEW );
			GL.glLoadIdentity();

            //! TEXTURE 1a 
            GenerateTextures();
            //! TEXTURE 1b 
        }


        //! TEXTURE b
        public uint[] Textures = new uint[8];
        
        void GenerateTextures()
        {
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA); 
            GL.glGenTextures(8, Textures); // 8 due 8 textures
            string[] imagesName ={ "front.png","back.png",
		                            "left.png","right.png","top.png","bottom.png","earth.png","metal.bmp"};
            for (int i = 0; i < imagesName.Length; i++)
            {
                Bitmap image = new Bitmap(imagesName[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[i]);
                //2D for XYZ
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }
        //! TEXTURE CUBE b
        //Draws our textured cube, VERY simple.  Notice that the faces are constructed
        //in a counter-clockwise order.  If they were done in a clockwise order you would
        //have to use the glFrontFace() function.  
        void DrawTexturedCube()
        {
            float cubeSize = 14; // to easily modify the size of skybox. At 20 edges don't render.
            GL.glRotatef(180, 1, 0, 0);
            // front
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f *cubeSize, -1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f *cubeSize, -1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f *cubeSize, 1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f *cubeSize, 1.0f * cubeSize, 1.0f * cubeSize);
            GL.glEnd();
            // back
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f * cubeSize, -1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f * cubeSize, -1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f * cubeSize, 1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f * cubeSize, 1.0f * cubeSize, -1.0f * cubeSize);
            GL.glEnd();
            // left
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[2]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f * cubeSize, -1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f * cubeSize, -1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f * cubeSize, 1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f * cubeSize, 1.0f * cubeSize, -1.0f * cubeSize);
            GL.glEnd();
            // right
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[3]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f * cubeSize, -1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f * cubeSize, -1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f * cubeSize, 1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f * cubeSize, 1.0f * cubeSize, 1.0f * cubeSize);
            GL.glEnd();
            // top
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[4]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f * cubeSize, 1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f * cubeSize, 1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f * cubeSize, 1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f * cubeSize, 1.0f * cubeSize, -1.0f * cubeSize);
            GL.glEnd();
            // bottom
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[5]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f * cubeSize, -1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f * cubeSize, -1.0f * cubeSize, -1.0f * cubeSize);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f * cubeSize, -1.0f * cubeSize, 1.0f * cubeSize);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f * cubeSize, -1.0f * cubeSize, 1.0f * cubeSize);
            GL.glEnd();
            GL.glRotatef(180, 1, 0, 0);


        }
        //! TEXTURE CUBE e




        public float rocketZangel;
        float rocketPrevZangel; // Can probably be removed now
        float burstFallAngle = 0.0f;
        float rocketAngle = 0.0f;
        float rocketHeight = 0.0f;
        float rocketHorizontal = 0.0f;
        float burstHeight = 0.0f;
        float animationFrames = 0.0f;
        float angleWings = 0.0f;
        float angleBurst = 0.0f;
        bool leaveBurst = false;
        float rocketDisFromEarth = 0f;
        float burstScaling = 1;
        float burstVertical = 0f;
        float burstHorizontal = 0f;


        public float ARM_angle;
        public float SHOULDER_angle;
        public float ROCKET_angle;
        public float alpha;




        uint WINGS_LIST, ROCKET_LIST, BODY_LIST;
        uint BURST_LIST, PARENT_LIST;
        float r;

        void PrepareLists(bool isShadow = false)
        {
           


            GL.glTranslated(-5, -2, -2);



            ROCKET_angle = 180;
            r = 0.3f;

            ROCKET_LIST = GL.glGenLists(3);
            PARENT_LIST = ROCKET_LIST + 5;
            BODY_LIST = ROCKET_LIST + 4;
            WINGS_LIST = ROCKET_LIST + 3;
            BURST_LIST = ROCKET_LIST + 2;



            GL.glColor3f(1, 1, 1);
            //need for texture

            GLU.gluQuadricDrawStyle(obj, GLU.GLU_FILL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, 0);

            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[7]);
            GLU.gluQuadricTexture(obj, 1);



            GL.glPushMatrix();
            GL.glNewList(BURST_LIST, GL.GL_COMPILE);
            drawBurst();


            GL.glEndList();
            GL.glPopMatrix();




            GL.glPushMatrix();

            GL.glNewList(WINGS_LIST, GL.GL_COMPILE);
          
            GLU.gluQuadricDrawStyle(obj, GLU.GLU_FILL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, 0);

            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[7]);
            GLU.gluQuadricTexture(obj, 1);
         



            drawWings();
            GL.glEndList();
            GL.glPopMatrix();


            GL.glPushMatrix();
            GL.glNewList(BODY_LIST, GL.GL_COMPILE);

            drawBody();
            GL.glEndList();
            GL.glPopMatrix();


            //GL.glPushMatrix();
        
            GL.glPushMatrix();
            GL.glNewList(PARENT_LIST, GL.GL_COMPILE);

            drawParent();

           // GL.glEnd();
            GL.glEndList();
            GL.glPopMatrix();
            CreateRocketList();
        }

        private static void enableTexture()
        {
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_TEXTURE_2D);
        }

        private static void drawParent()
        {
            GL.glTranslated(-5, -2, -2);
            GL.glRotatef(250, 1, 0, 0);
            GL.glRotatef(0, 0, 1, 0);
            GL.glRotatef(0, 0, 0, 1);
        }

        private void drawBody()
        {
            GL.glRotatef(0, 0.0f, 1.0f, 0.0f);
            GL.glTranslated(0, 0, -3);

            GLU.gluCylinder(obj, 1, 1, 3, 30, 40);
            GL.glTranslated(0, 0, 3);
            GLU.gluCylinder(obj, 1, 0.1, 2, 30, 40);
            GL.glTranslated(0, 0, -3);
        }

        private static void drawWings()
        {
            GL.glRotatef(90, 1.0f, 0.0f, 0.0f);
            GL.glBegin(GL.GL_TRIANGLES);
            GL.glTexCoord2f(1.0f, 0.5f);
            GL.glVertex2f(1f, 0.5f);      // V0
            GL.glTexCoord2f(3.0f, 0.0f);
            GL.glVertex2f(3.0f, 0.0f);    // V1
            GL.glTexCoord2f(1.0f, 3.0f);
            GL.glVertex2f(1.0f, 3.0f);    // V2


            GL.glTexCoord2f(-1.0f, 0.5f);
            GL.glVertex2f(-1f, 0.5f);      // V0
            GL.glTexCoord2f(-3.0f, 0.0f);
            GL.glVertex2f(-3.0f, 0.0f);    // V1
            GL.glTexCoord2f(-1.0f, 3.0f);
            GL.glVertex2f(-1.0f, 3.0f);    // V2


            GL.glTexCoord3f(0.0f, 3.0f, 1.0f);
            GL.glVertex3f(0f, 3f, 1f);      // V0
            GL.glTexCoord3f(0.0f, 0.5f, 1.0f);
            GL.glVertex3f(0f, 0.5f, 1f);    // V1
            GL.glTexCoord3f(0.0f, 0.0f, 3.0f);
            GL.glVertex3f(0f, 0.0f, 3f);    // V2


            GL.glTexCoord3f(0.0f, 3.0f, -1.0f);
            GL.glVertex3f(0f, 3f, -1f);      // V0
            GL.glTexCoord3f(0.0f, 0.5f, -1.0f);
            GL.glVertex3f(0f, 0.5f, -1f);    // V1
            GL.glTexCoord3f(0.0f, 0.0f, -3.0f);
            GL.glVertex3f(0f, 0.0f, -3f);    // V2



            GL.glEnd();
        }

        private void drawBurst()
        {
            GL.glRotatef(270, 1.0f, 0.0f, 0.0f);

            GL.glTranslated(0.4, 0.4, -1);
            GLU.gluCylinder(obj, 0.7, 0.4, 1, 30, 40);
            GL.glTranslated(-0.4, -0.4, 4);

            GL.glTranslated(-0.4, 0.4, -4);
            GLU.gluCylinder(obj, 0.7, 0.4, 1, 30, 40);
            GL.glTranslated(+0.4, -0.4, 4);

            GL.glTranslated(0.4, -0.4, -4);
            GLU.gluCylinder(obj, 0.7, 0.4, 1, 30, 40);
            GL.glTranslated(-0.4, +0.4, 4);

            GL.glTranslated(-0.4, -0.4, -4);
            GLU.gluCylinder(obj, 0.7, 0.4, 1, 30, 40);
            GL.glTranslated(+0.4, +0.4, 4);
        }

        public void CreateRocketList()
        {

            GL.glNewList(ROCKET_LIST, GL.GL_COMPILE);
            GL.glPushMatrix();

            GLU.gluQuadricDrawStyle(obj, GLU.GLU_FILL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, 0);

            GLU.gluQuadricTexture(obj, 1);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[7]);
 
            animationFrames++;

            if (animationFrames < 200)
            {

                rocketAngle = rocketAngle + 0.25f;
            }
            else
            {
                if (!leaveBurst)
                {

                    burstHeight = rocketHeight;

                }
                leaveBurst = true;
            }

            

            if (!leaveBurst)
            {

     
                GL.glCallList(PARENT_LIST);
                GL.glScalef(0.1f, 0.1f, 0.1f);
                double horizontalVec = (Math.Cos(rocketZangel * Math.PI / 180)) * 0.05;
                double verticalVec = (Math.Sin(rocketZangel * Math.PI / 180)) * 0.05;
                rocketHeight = rocketHeight + (float)verticalVec;
                rocketHorizontal = rocketHorizontal + (float)horizontalVec;
                GL.glTranslated(0, rocketHeight, rocketHorizontal);


                GL.glRotatef(rocketZangel, -1.0f, 0.0f, 0.0f);

                //GL.glRotatef(angleBurst, 1f, 0f, 0f);
                GL.glRotatef(angleWings++, 0f, 0f, 1f);

                GL.glCallList(BODY_LIST);
                GL.glCallList(WINGS_LIST);
                GL.glRotatef(angleWings, 0f, 1f, 0f);

                GL.glCallList(BURST_LIST);


            }
            else
            {


      
                if (animationFrames == 200)
                {


                    burstHorizontal = rocketHorizontal;
                    burstVertical = rocketHeight;


                }
       


                GL.glCallList(PARENT_LIST);
                GL.glScalef(0.1f,0.1f, 0.1f);

                
                double horizontalVec = (Math.Cos(rocketZangel * Math.PI / 180) ) * 0.05;
                double verticalVec = (Math.Sin(rocketZangel * Math.PI / 180))  * 0.05; 
                rocketHeight = rocketHeight + (float)verticalVec;
                rocketHorizontal = rocketHorizontal + (float)horizontalVec;
                GL.glTranslated(0, rocketHeight, rocketHorizontal);

               
                if(burstFallAngle < 90)
                {
                    burstFallAngle = burstFallAngle +0.1f;
                }
                //if(rocketDisFromEarth < 40)
                //{
                //    rocketDisFromEarth = rocketDisFromEarth + 0.01f;
                //}
          
                GL.glRotatef(rocketZangel, -1.0f, 0.0f, 0.0f);
                GL.glRotatef(angleWings++, 0f, 0f, 1f);



                GL.glCallList(BODY_LIST);

                GL.glCallList(WINGS_LIST);
                GL.glRotatef(angleWings, 0, -1.0f, 0.0f);
               

                if (burstScaling > 0)
                {
                    burstScaling = burstScaling - 0.005f;
                }
                GL.glTranslated(-rocketHeight + burstVertical, -rocketHorizontal+burstHorizontal , 0);
                float radius = (float)Math.Sqrt((Math.Pow((-rocketHorizontal + burstHorizontal) -rocketHorizontal, 2) + Math.Pow((-rocketHeight + burstVertical) - rocketHeight, 2)));
                double curX = (Math.Cos((rocketZangel+rocketPrevZangel) * Math.PI / 180)) * radius * 0.01;
                double curY = (Math.Sin((rocketZangel + rocketPrevZangel) * Math.PI / 180)) * radius * 0.01;
                GL.glTranslated(0, -curX, curY);         

                GL.glScalef(burstScaling, burstScaling, burstScaling);

                GL.glCallList(BURST_LIST);
              

            }


            GL.glPopMatrix();

            GL.glEndList();

        }


        public uint[] texture;

        void InitTexture(string imageBMPfile)
        {
            GL.glEnable(GL.GL_TEXTURE_2D);

            texture = new uint[1];      // storage for texture

            Bitmap image = new Bitmap(imageBMPfile);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
            System.Drawing.Imaging.BitmapData bitmapdata;
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            GL.glGenTextures(1, texture);
            GL.glBindTexture(GL.GL_TEXTURE_2D, texture[0]);
            //  VN-in order to use System.Drawing.Imaging.BitmapData Scan0 I've added overloaded version to
            //  OpenGL.cs
            //  [DllImport(GL_DLL, EntryPoint = "glTexImage2D")]
            //  public static extern void glTexImage2D(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);
            GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);

            GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);      // Linear Filtering
            GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);      // Linear Filtering

            image.UnlockBits(bitmapdata);
            image.Dispose();
        }

    }

}


