using System;
//using System.Collections.Generic;
///using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
//using Timer = System.Timers.Timer;


namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
     
        int maxSize = 200;

        
        bool[,] block;
       // bool[,] quantumBlock;
        Random r = new Random();
        public long x = 0;

        int w = 4;
        int c = 0;
        int refe = 10;


        bool[,] rule = new bool[2, 9];
        bool[,] buffer;
        bool paused;
        bool rulesAreRandomized = false;
        bool staticMode;
        bool staticBuffer;

        //CellTick values, identifying states of "neighbors"
        int[] randCells;



        bool q;
        int t;

        int mouseX;
        int mouseY;

        //Checks if left or right click, calls respective MakeTrue or MakeFalse functions if either condition is true
        bool leftIsTrue;
        bool rightIsTrue;


        //Convert using(var g = graphics.Bitmap(myBitmap)) g.Drawline to BITMAP, then make that line turn values to true.
       // int old;
      //  int current;

        
       



        //Original true and false colors
        int alphaTO = 255, redTO = 0, greenTO = 255, blueTO = 255;

        int alphaFO = 255, redFO = 35, greenFO = 39, blueFO = 42;

        //True and false colors to be randomized
        int alphaT, redT, greenT, blueT;

        int alphaF, redF, greenF, blueF;

        Brush trueBrush;
        Brush falseBrush;

        

        


        Bitmap myBitmap;

        public Form1()
        {                     

            randCells = new int[17];

            InitializeComponent();
            initBlocks();

            
            loopTimer.Interval = 1;
            loopTimer.Enabled = false;
                      

            timer1.Interval = 1;
            timer1.Enabled = false;
           
                      
            timer2.Interval = 10;
            timer2.Enabled = false;


            timer1.Interval = refe;
            this.Paint += new PaintEventHandler(picPaint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyPress2);

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);


            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            //Timer 2 goes off when the game is paused and forces one more frame to pass while paused; it avoids a bug where clicking when paused advances one frame.
            timer1.Start();
            timer2.Enabled = false;

            //this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);

            //label2.Text = "Mouse Position ";

            pictureBox1.Size = new Size(maxSize * w + 20, maxSize * w + 20);
            pictureBox1.Location = new Point(0, 0);


            this.Size = new Size(830, 850);
            this.Cursor = new Cursor(Cursor.Current.Handle);

            SetSize();

            //Brushes for true and false colors
            trueBrush = new SolidBrush(Color.FromArgb(alphaTO, redTO, greenTO, blueTO));

            falseBrush = new SolidBrush(Color.FromArgb(alphaFO, redFO, greenFO, blueFO));
        }

       
        void SetSize()
        {
            this.Size = new Size(850, 870);
        }





        //Make an array of booleans; size is maxSize^2
        void initBlocks()
        {
            SetRules();
            block = new bool[maxSize, maxSize];
            buffer = new bool[maxSize, maxSize];


            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    block[i, j] = (r.Next(2) == 0);
                }
            }

            for (int i = 0; i < maxSize; i++)            
            {
                block[0, i] = false;
                block[maxSize - 1, i] = false;
                block[i, 0] = false;
                block[i, maxSize - 1] = false;
            }



        }

       
       

        void mainLoop()
        {
            //this.BeginInvoke((MethodInvoker)delegate { Refresh(); });

            Refresh();
        }

        private void SetRules()
        {
            for (int i = 0; i < 9; i++)
            {               
                rule[0, i] = false;
                rule[1, i] = false;
            }
            rule[1, 2] = true;
            rule[1, 3] = true;
            rule[0, 3] = true;

        }



        private void RandomizeRules()
        {
             
            for (int i = 0; i < 9; i++)
            {
                rule[0, i] = r.Next(2) == 0;
                rule[1, i] = r.Next(2) == 0;
            }

            q = r.Next(2) == 0;
            t = Math.Abs(r.Next(9));
            for (int p = 0; p < 5; p++)
            {
                rule[1, t] = q;
                rule[0, t] = q;
            }
        }

       
        private void Form1_Load(object sender, EventArgs e)
        {
            mainLoop();
        }

        private void CellTick()
        {
            

            if (staticMode == false)
            {
                for (int i = 1; i < maxSize - 1; i++)
                {
                    for (int j = 1; j < maxSize - 1; j++)
                    {

                        int tSum = 0;
                        if (block[i - 1, j - 1]) tSum++;
                        if (block[i, j - 1]) tSum++;
                        if (block[i + 1, j - 1]) tSum++;
                        if (block[i + 1, j]) tSum++;
                        if (block[i + 1, j + 1]) tSum++;
                        if (block[i, j + 1]) tSum++;
                        if (block[i - 1, j + 1]) tSum++;
                        if (block[i - 1, j]) tSum++;
                        //Checks # of alive neighbors.

                        if (block[i, j]) buffer[i, j] = rule[1, tSum];
                        else buffer[i, j] = rule[0, tSum];

                    }
                }
            }

            else
            {
                for (int i = 1; i < maxSize - 1; i++)
                {
                    for (int j = 1; j < maxSize - 1; j++)
                    {
                        if (staticBuffer == true)
                        {
                            if (staticMode) for (int b = 0; b < 17; b++)
                            {
                                    // fRand.FastRandom();
                                    randCells[b] = (r.Next(maxSize - 2) + 1);
                                    // randCells[b] = fRand.FastNext(2);

                                    if (randCells[b] == 0) randCells[b] = 2;
                                    if (randCells[b] > maxSize - 1) randCells[b] = maxSize - 2;

                                    //if (b % 10 == 0) staticLabel.Text = (fRand.FastNext(maxSize - 1)).ToString();

                                    

                            }
                            else staticBuffer = false;
                        }

                        

                        

                        int tSum = 0;
                        if (block[randCells[1] - 1, randCells[9] - 1]) tSum++;
                        if (block[randCells[2], randCells[1] - 1]) tSum++;
                        if (block[randCells[3] + 1, randCells[11] - 1]) tSum++;
                        if (block[randCells[4] + 1, randCells[12]]) tSum++;
                        if (block[randCells[5] + 1, randCells[13] + 1]) tSum++;
                        if (block[randCells[6], randCells[14] + 1]) tSum++;
                        if (block[randCells[7] - 1, randCells[15] + 1]) tSum++;
                        if (block[randCells[8] - 1, randCells[16]]) tSum++;
                        //Checks # of alive neighbors.

                        if (block[i, j]) buffer[i, j] = rule[1, tSum];
                        else buffer[i, j] = rule[0, tSum];

                    }
                }
            }
            


            for (int i = 1; i < maxSize - 1; i++)
            {
                for (int j = 1; j < maxSize - 1; j++)
                {
                    block[i, j] = buffer[i, j];
                }
            }
            
           
        }


        //Refreshes the screen by taking true/false data and giving corresponding truebrush/falsebrush colors

        //Invalidate(); calls this function
        private void picPaint(object sender, PaintEventArgs e)
        {

           // Refresh();
           // Invalidate();
           //^Both call this function.
            



            Graphics g = Graphics.FromImage(myBitmap);

            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (block[i, j]) g.FillRectangle(trueBrush, i * w, j * w, w, w); //if(boolean) = if boolean is true. 
                    else g.FillRectangle(falseBrush, i * w, j * w, w, w);

                }

            }
            

            //Erases the old screen so that we aren't stacking a bunch of graphics on top of each other
            g.Dispose();

            //The bitmap is true/false array values; makes the picturebox display the bitmap.
            pictureBox1.Image = myBitmap;

            //If not paused, 
            if (paused == false) picHelp();

        }

        void picHelp()
        {
            x++;

            this.Text = "Generation " + x.ToString();


            CellTick();

            timer1.Start();

        }



        //Conditional functions ahead


        //Pressing space bar stops or continues timer1, pausing or resuming the game.
        public void Form_KeyPress2(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Space)
            {
                c++;
                if (c % 2 == 1)
                {
                    timer1.Stop();
                    this.Text = "Generation " + x.ToString() + "            --PAUSED";
                    paused = true;
                    timer2.Start();

                }

                else
                {
                    timer1.Start();
                    paused = false;
                }

            }
        }


        
        private void PauseBuffer1(object sender, EventArgs e)
        {
            if (paused == true) PauseBuffer2();
        }

        private void PauseBuffer2()
        {
            Refresh();
            timer2.Stop();
        }


        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            /* if(e.KeyCode == Keys.Up)
             {
                 if (timer1.Interval <= 10000) 
                 {
                     timer1.Interval += 10;
                     label3.Text = "Refresh Rate = " + timer1.Interval.ToString() + "ms per tick";
                 }
             }

             if (e.KeyCode == Keys.Down)
             {
                 if (timer1.Interval > 10)
                 {
                     timer1.Interval -= 10;
                     label3.Text = "Refresh Rate = " + timer1.Interval.ToString() + "ms per tick";
                 }
             } */

            if (e.KeyCode == Keys.Add)
            {
                staticMode = true;
                staticBuffer = true;
                //staticLabel.Text = "Static Mode: ON";
            }

            if (e.KeyCode == Keys.Subtract)
            {
                staticMode = false;
                //staticLabel.Text = "Static Mode: OFF";
            }

            //Changes colors to original values
            if (e.KeyCode == Keys.E)
            {
                trueBrush = new SolidBrush(Color.FromArgb(alphaTO, redTO, greenTO, blueTO));
                falseBrush = new SolidBrush(Color.FromArgb(alphaFO, redFO, greenFO, blueFO));
            }

            //Randomizes colors
            if (e.KeyCode == Keys.Q)
            {


                alphaT = r.Next(256);
                redT = r.Next(256);
                greenT = r.Next(256);
                blueT = r.Next(256);
                alphaF = r.Next(256);
                redF = r.Next(256);
                greenF = r.Next(256);
                blueF = r.Next(256);

                trueBrush = new SolidBrush(Color.FromArgb(alphaT, redT, greenT, blueT));
                falseBrush = new SolidBrush(Color.FromArgb(alphaF, redF, greenF, blueF));
            }




            //Commit mass genocide
            if (e.KeyCode == Keys.Delete)
            {
                
                for (int i = 0; i < maxSize; i++)
                {
                    for (int j = 0; j < maxSize; j++)
                    {
                        block[i, j] = false;
                    }
                }

                Invalidate();
            }

            //Mass produce
            if (e.KeyCode == Keys.Insert)
            {
                for (int i = 0; i < maxSize; i++)
                {
                    for (int j = 0; j < maxSize; j++)
                    {
                        block[i, j] = true;
                    }
                }

                Invalidate();

            }


            //Randomize rules
            if (e.KeyCode == Keys.Back)
            {
                RandomizeRules();
                rulesAreRandomized = true;
            }

            //Sets rules back to conway's game of life rules
            if (e.KeyCode == Keys.Enter)
            {
                SetRules();
                rulesAreRandomized = false;
            }

            //Creates new randomized bitmap and sets rules to conway's game of life
            if (e.KeyCode == Keys.R)
            {
                initBlocks();
                rulesAreRandomized = false;
            }

            //Sets generation # to 0
            if (e.KeyCode == Keys.R ^ e.KeyCode == Keys.Enter ^ e.KeyCode == Keys.Back ^ e.KeyCode == Keys.Delete ^ e.KeyCode == Keys.Insert)
            {
                x = 0;
            }


        }

        private void MakeFalse()
        {

            if (mouseX <= 199 && mouseY <= 199)
            {

                block[mouseX, mouseY] = false;
                Invalidate();
            }
        }

        private void TrueChecker(object sender, EventArgs e)
        {

            if (leftIsTrue) MakeTrue();

            else if (rightIsTrue) MakeFalse();
            

        }

        private void MakeTrue()
        {

            // mouseX = e.X / w;
            // mouseY = e.Y / w;




            if (mouseX <= 199 && mouseY <= 199)
            {

                block[mouseX, mouseY] = true;
                Invalidate();
            }




        }



        /*protected override */
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            // base.OnMouseDown(e);
            
            loopTimer.Start();
            
            if(e.Button == MouseButtons.Left)
            {
                leftIsTrue = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                rightIsTrue = true;
            }







        }




     /* protected override */ void OnMouseUp(object sender, MouseEventArgs e)
        {
            //base.OnMouseUp(e);

            loopTimer.Stop();

            leftIsTrue = false;
            rightIsTrue = false;
                                 
        }


        //Labels mouse position on screen
        /*protected override */void OnMouseMove(object sender, MouseEventArgs e)
        {
           // base.OnMouseMove(e);

            mouseX = e.X / w;
            mouseY = e.Y / w;
            //label2.Text = "Mouse Position: " + "{X: " + mouseX.ToString() + ", " + "Y: " + mouseY.ToString() + "}";

            


        }
        //This code below let's use do Form1 controls on pictureBox1.
        /*
        class Form1ControlCollection : ControlCollection
        {
            Form1 owner;
            internal Form1ControlCollection(Form1 owner) : base(owner)
            {
                this.owner = owner;
            }

            public override void Add(Control value)
            {
                base.Add(value);
            //    value.MouseUp += Value_OnMouseUp;
                value.MouseDown += Value_OnMouseDown;
                value.MouseMove += Value_MouseMove;
            }

         //   private void Value_OnMouseUp(object sender, MouseEventArgs e)
       //     {
          //      owner.OnMouseUp(e);
        //    }

            private void Value_OnMouseDown(object sender, MouseEventArgs e)
            {
                owner.OnMouseDown(e);
            }

            private void Value_MouseMove(object sender, MouseEventArgs e)
            {
                owner.OnMouseMove(e);

            }

            protected override Control.ControlCollection CreateControlsInstance()
            {
                return new Form1ControlCollection(this);                
            }


        }
        */

    }
}

