﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MacUI
{
    public partial class MacUIForm : Form
    {
        private int beforeX;
        private int beforeY;
        private string windowStatus = "Normal";
        private int currentWidth = 800;
        private int currentHeight = 600;
        private int currentX;
        private int currentY;
        private int cornerRadius = 20;

        private Color redBackground = Color.FromArgb(255, 96, 92);
        private Color redBorder = Color.FromArgb(233, 76, 70);
        private Color redHoverContent = Color.FromArgb(92, 5, 5);
        private Color yellowBackground = Color.FromArgb(255, 189, 68);
        private Color yellowBorder = Color.FromArgb(232, 169, 37);
        private Color yellowHoverContent = Color.FromArgb(153, 87, 0);
        private Color greenBackground = Color.FromArgb(0, 202, 78);
        private Color greenBorder = Color.FromArgb(36, 182, 54);
        private Color greenHoverContent = Color.FromArgb(0, 100, 0);

        private Color almostWhite = Color.FromArgb(250, 250, 250);
        private Color lightGray = Color.FromArgb(190, 190, 190);


        // Rounded Corners
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
                int nLeftRect,
                int nTopRect,
                int nRightRect,
                int nBottomRect,
                int nWidthEllipse,
                int nHeightEllipse
            );


        // Shadow
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000;
                cp.ExStyle = cp.ExStyle | 0x20 | 0x8000000;

                return cp;
            }
        }


        // Border
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);


        //}

        public override string Text {
            get
            {
                return base.Text;
            } 
            set
            {
                lblTitle.Text = value;
                base.Text = value;
                Invalidate();
            }  
        }

        public MacUIForm()
        {
            InitializeComponent();
        }

        private void MacUIForm_Load(object sender, EventArgs e)
        {
            DrawButton(btnClose, redBackground, redBorder);
            DrawButton(btnMinimize, yellowBackground, yellowBorder);
            DrawButton(btnFullscreen, greenBackground, greenBorder);

            SetRoundedCorners();

            this.MaximizedBounds = Screen.GetWorkingArea(this);
        }

        private void SetRoundedCorners()
        {
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, cornerRadius, cornerRadius));
        }

        private void DrawButton(PictureBox picBox, Color color, Color colorBorder)
        {
            int size = 14;
            Bitmap btn = new Bitmap(size + 4, size + 4);

            Graphics canvas = Graphics.FromImage(btn);
            canvas.SmoothingMode = SmoothingMode.HighQuality;
            canvas.FillEllipse(new SolidBrush(color), 0, 0, size, size);
            canvas.DrawEllipse(new Pen(colorBorder), 0, 0, size, size);

            picBox.Image = btn;

            canvas.Dispose();
        }

        private void DrawButtonHoverState(PictureBox picBox, Color color, Color colorBorder, Color colorHoverContent, string buttonType)
        {
            int size = 14;
            Bitmap btn = new Bitmap(size + 4, size + 4);

            Graphics canvas = Graphics.FromImage(btn);
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            canvas.FillEllipse(new SolidBrush(color), 0, 0, size, size);
            canvas.DrawEllipse(new Pen(colorBorder), 0, 0, size, size);

            if (buttonType == "Close")
            {
                canvas.DrawLine(new Pen(colorHoverContent), 4, 4, size - 4, size - 4);
                canvas.DrawLine(new Pen(colorHoverContent), size - 4, 4, 4, size - 4);
            }
            else if (buttonType == "Minimize")
            {
                canvas.DrawLine(new Pen(colorHoverContent), 4, size / 2, size - 4, size / 2);
            }
            else
            {
                canvas.FillRectangle(new SolidBrush(colorHoverContent), 4, 4, size - 8, size - 8);
                canvas.DrawLine(new Pen(color), 5, 4, size - 4, size - 5);
                canvas.DrawLine(new Pen(color), 4, 4, size - 4, size - 4);
                canvas.DrawLine(new Pen(color), 3, 4, size - 4, size - 3);
            }

            picBox.Image = btn;

            canvas.Dispose();
        }

        private void lblTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + (e.X - beforeX), this.Location.Y + (e.Y - beforeY));
            }
        }

        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beforeX = e.X;
                beforeY = e.Y;
            }
        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beforeX = e.X;
                beforeY = e.Y;
            }
        }

        private void panelTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + (e.X - beforeX), this.Location.Y + (e.Y - beforeY));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            if (windowStatus != "Maximize")
            {
                currentWidth = this.Size.Width;
                currentHeight = this.Size.Height;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = new Size(currentWidth, currentHeight);
                this.Location = new Point(currentX, currentY);
            }

            timerMinimize.Start();
        }

        private void btnFullscreen_Click(object sender, EventArgs e)
        {
            if (windowStatus == "Maximize")
            {
                this.WindowState = FormWindowState.Normal;
                timerNormal.Start();
            }
            else
            {
                currentWidth = this.Size.Width;
                currentHeight = this.Size.Height;
                currentX = this.Location.X;
                currentY = this.Location.Y;

                timerMaximize.Start();
            }
        }

        private void MacUIForm_Resize(object sender, EventArgs e)
        {
            if (windowStatus == "Minimize")
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    // Restore after minimize (click app icon on taskbar)
                    windowStatus = "Normal";
                    timerNormal.Start();
                }
            }
            else if (windowStatus == "MinimizeFromMaximize")
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    // Restore after minimize (click app icon on taskbar)
                    timerMaximize.Start();
                }
            }
        }

        private void timerMinimize_Tick(object sender, EventArgs e)
        {
            if (this.Size.Height >= 50 && this.Size.Width >= 100)
            {
                int reduction = 20;

                this.Size = new Size(this.Size.Width - reduction, this.Size.Height - reduction);
                this.Location = new Point(this.Location.X + (reduction / 2), this.Location.Y + reduction);

                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.1;
                }
            }
            else
            {
                timerMinimize.Stop();


                if (windowStatus == "Maximize")
                {
                    windowStatus = "MinimizeFromMaximize";
                }
                else
                {
                    windowStatus = "Minimize";
                }

                this.WindowState = FormWindowState.Minimized;
            }

            SetRoundedCorners();
        }

        private void timerNormal_Tick(object sender, EventArgs e)
        {
            if (windowStatus == "Maximize")
            {
                if (this.Size.Height > currentHeight && this.Size.Width > currentWidth)
                {
                    int addition = 20;

                    this.Size = new Size(this.Size.Width - addition, this.Size.Height - addition);
                    this.Location = new Point(this.Location.X + (addition / 2), this.Location.Y + (addition / 2));
                }
                else
                {
                    timerNormal.Stop();
                    windowStatus = "Normal";
                }
            }
            else
            {
                if (this.Size.Height < currentHeight && this.Size.Width < currentWidth)
                {
                    int addition = 20;

                    this.Size = new Size(this.Size.Width + addition, this.Size.Height + addition);
                    this.Location = new Point(this.Location.X - (addition / 2), this.Location.Y - addition);

                    if (this.Opacity < 1)
                    {
                        this.Opacity += 0.1;
                    }
                }
                else
                {
                    timerNormal.Stop();
                    windowStatus = "Normal";
                }
            }

            SetRoundedCorners();
        }

        private void timerMaximize_Tick(object sender, EventArgs e)
        {
            if (this.Size.Height < this.MaximizedBounds.Height && this.Size.Width < this.MaximizedBounds.Width)
            {
                int addition = 20;

                this.Size = new Size(this.Size.Width + addition, this.Size.Height + addition);

                if (windowStatus == "MinimizeFromMaximize")
                {
                    this.Location = new Point(this.Location.X - (addition / 2), this.Location.Y - addition);
                }
                else
                {
                    this.Location = new Point(this.Location.X - (addition / 2), this.Location.Y - (addition / 2));
                }

                if (this.Opacity < 1)
                {
                    this.Opacity += 0.1;
                }
            }
            else
            {
                timerMaximize.Stop();
                windowStatus = "Maximize";
                this.WindowState = FormWindowState.Maximized;
            }

            SetRoundedCorners();
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            DrawButton(btnClose, redBackground, redBorder);
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            DrawButtonHoverState(btnClose, redBackground, redBorder, redHoverContent, "Close");
        }

        private void btnMinimize_MouseEnter(object sender, EventArgs e)
        {
            DrawButtonHoverState(btnMinimize, yellowBackground, yellowBorder, yellowHoverContent, "Minimize");
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            DrawButton(btnMinimize, yellowBackground, yellowBorder);
        }

        private void btnFullscreen_MouseEnter(object sender, EventArgs e)
        {
            DrawButtonHoverState(btnFullscreen, greenBackground, greenBorder, greenHoverContent, "Maximize");
        }

        private void btnFullscreen_MouseLeave(object sender, EventArgs e)
        {
            DrawButton(btnFullscreen, greenBackground, greenBorder);
        }



        private int resizeBeforeX = 0;
        private string resizeDirection = "";
        private bool isOnResizeMode = false;

        private void MacUIForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isOnResizeMode)
            {
                if (resizeDirection == "left")
                {
                    this.Location = new Point(this.Location.X + (e.X - resizeBeforeX), this.Location.Y);
                    this.Width = this.Width + (resizeBeforeX - e.X);
                }
                else if(resizeDirection == "right")
                {
                    this.Width = this.Width + (e.X - resizeBeforeX);

                    if(resizeBeforeX != e.X)
                    {
                        resizeBeforeX = e.X;
                    }
                }

                SetRoundedCorners();
            }

            if ((e.X >= 0 && e.X <= 4) || (e.X >= this.Width - 4 && e.X <= this.Width))
            {
                this.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void MacUIForm_MouseDown(object sender, MouseEventArgs e)
        {
            if(this.Cursor == Cursors.SizeWE)
            {
                if (e.X >= 0 && e.X <= 4)
                {
                    resizeDirection = "left";
                }
                else if(e.X >= this.Width - 4 && e.X <= this.Width)
                {
                    resizeDirection = "right";
                }

                isOnResizeMode = true;
                resizeBeforeX = e.X;
            }
        }

        private void MacUIForm_MouseUp(object sender, MouseEventArgs e)
        {
            if(isOnResizeMode)
            {
                resizeDirection = "";
                isOnResizeMode = false;
                resizeBeforeX = 0;
            }
        }
    }
}
