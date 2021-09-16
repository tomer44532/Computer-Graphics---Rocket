using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenGL;
using System.Runtime.InteropServices; 

namespace myOpenGL
{
    public partial class Form1 : Form
    {
        cOGL cGL;

        public Form1()
        {

            InitializeComponent();
            cGL = new cOGL(panel1);
        }

        private void timerRepaint_Tick(object sender, EventArgs e)
        {
             cGL.Draw();
            timerRepaint.Enabled = false;
        }


        private void timerRUN_Tick(object sender, EventArgs e)
        {
            //cGL.CreateRobotList();
            cGL.Draw(); 
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            timerRUN.Interval = hScrollBar1.Value;
            label1.Text = "timer delay = " + hScrollBar1.Value;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            //cGL.OnResize(); No idea why causes error
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            cGL.intOptionA = 1 - cGL.intOptionA;
            cGL.Draw();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
	        cGL.isInside = !cGL.isInside;
            cGL.Draw();
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            cGL.viewAngle = hScrollBar2.Value;
            label2.Text = hScrollBar2.Value+"°";
            cGL.Draw();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            cGL.isBounds = !cGL.isBounds;
            cGL.Draw();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            cGL.isAnimate = !cGL.isAnimate;
            label1.Text = "delay = " + hScrollBar1.Value;
            timerRUN.Enabled = cGL.isAnimate;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case 'a':
                case 'A':
  
                        cGL.rocketZangel += 10;
                        cGL.Draw();
                    
                    break;
                case 'd':
                case 'D':

                        cGL.rocketZangel -= 10;
                        cGL.Draw();
            
                    break;
              
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timerRUN.Enabled = !timerRUN.Enabled;
            if (timerRUN.Enabled)
                button1.Text = "Stop";
            else
                button1.Text = "Run";
            label1.Text = "delay = " + hScrollBar1.Value;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            cGL.isAtmosphere = !cGL.isAtmosphere;
            cGL.Draw();

        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            cGL.isReflection = !cGL.isReflection;
            cGL.Draw();
        }

        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {
            cGL.isLight = !cGL.isLight;
            cGL.Draw();
        }
        int oldPosX = 0;
        //private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        //{
        //    NumericUpDown nUD = (NumericUpDown)sender;
        //    int pos = (int)nUD.Value;
        //    if (pos > oldPosX)
        //    {
        //        if(cGL.lightPosX < 2)
        //        {
        //            cGL.lightPosX = cGL.lightPosX + 0.25f;
        //        }
        //    }
        //    else
        //    {
        //        if(cGL.lightPosX > -2)
        //        {
        //            cGL.lightPosX = cGL.lightPosX - 0.25f;
        //        }
        //    }
        //    oldPosX = pos;
        //}

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            //cGL.lightPosX = hScrollBar3.Value;
            //cGL.Draw();
        }

        private void hScrollBar4_Scroll(object sender, ScrollEventArgs e)
        {
            //cGL.lightPosY = hScrollBar4.Value;
            //cGL.Draw();
        }

        private void hScrollBar5_Scroll(object sender, ScrollEventArgs e)
        {
            //cGL.lightPosZ = hScrollBar5.Value;
            //cGL.Draw();
        }
    }
    
}