using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {

            Application.Run(new MyForm());
        }
    }

    public class MyForm : Form
    {
        Button button1 = new Button();

        Label label1 = new Label(); 
        Label label2 = new Label();
        Label label3 = new Label();

        Panel panel1 = new Panel();

        Timer timer2 = new Timer();

        Random random = new Random();

        List<SnakeCoordinates> snakePosition = new List<SnakeCoordinates>();
        List<SnakeCoordinates> snakePreviousPosition = new List<SnakeCoordinates>();

        SnakeCoordinates snack;
        Graphics graph;

        bool IfSnackEaten = true;

        float velocity = 22F;

        int score = 0;

        protected enum Direction { Right, Left, Up, Down }
        protected Direction direction;

        public MyForm()
        {
            //----------okno glowne-----------------
            Text = "Snake";
            Width = 800;
            Height = 600;
            //Paint += Draw;
            KeyDown += GetDirection;



            //---------------panel----------------
            panel1.Width = (int)(0.7 * ClientSize.Width); //panel1.width = 885 pikseli
            panel1.Height = (int)(0.9 * ClientSize.Height); //panel1.heigth = 684 pikseli
            panel1.Top = (ClientSize.Height - panel1.Height) / 2;
            panel1.Left = (panel1.Top);
            //panel1.Left = (ClientSize.Width - panel1.Width) / 2;
            panel1.BackColor = Color.White;
            //panel1.TabIndex = 1;


            //--------------labels--------------
            label2.Font = new Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            label2.AutoSize = true;
            label2.BackColor = Color.White;
            label2.Top = (int)(ClientSize.Height * 0.3);
            label2.Left = ((ClientSize.Width - (panel1.Left + panel1.Width + label1.Width)) / 2) + (panel1.Left + panel1.Width);
            label2.Text = "Twój Wynik:";

            label3.Font = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            label3.Width = ClientSize.Width / 8;
            label3.BackColor = Color.White;
            label3.Top = (int)(ClientSize.Height * 0.4);
            label3.Left = ((ClientSize.Width - (panel1.Left + panel1.Width + label1.Width)) / 2) + (panel1.Left + panel1.Width);

            label1.Font = new Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            label1.Text = "Zagraj ponownie";
            label1.AutoSize = true;
            label1.Height += 10;
            label1.Top = (int)(ClientSize.Height * 0.6);
            label1.Left = ((ClientSize.Width - (panel1.Left + panel1.Width + label1.Width)) / 2) + panel1.Width;
            //label1.TabIndex = 4;
            //label1.Focus();
            label1.Visible = false;
            label1.Click += PlayAgain;
            label1.BackColor = Color.White;




            label1.TextAlign = ContentAlignment.MiddleCenter;
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label3.TextAlign = ContentAlignment.MiddleCenter;

            //--------buttons----------
            /*
            button1.Text = "Zagraj ponownie";
            //button1.Visible = false;
            //button1.Enabled = true;
            button1.AutoSize = true;
            button1.Width += 20;
            button1.Height += 10;
            button1.Top = (int)(ClientSize.Height * 0.6);
            button1.Left = ((ClientSize.Width - (panel1.Left + panel1.Width + button1.Width)) / 2) + (panel1.Left + panel1.Width);
            button1.TabIndex = 4;
            button1.Focus();
            button1.Click += PlayAgain;
            */

            //----------dodawanie elementow-----------------

            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(label3);
            //Controls.Add(button1);
            Controls.Add(panel1);

            //AutoValidate = Enabled; 
            //Focus();
            //ResumeLayout(false);
            //PerformLayout();

            //------------inicjacja elementow gry-------------
            snakePosition.Add(new SnakeCoordinates(((Convert.ToInt16(panel1.Width / (2 * velocity))) * velocity) + ((panel1.Width % velocity) / 2),
                ((Convert.ToInt16(panel1.Height / (2 * velocity))) * velocity) + ((panel1.Height % velocity) / 2)));

            snakePreviousPosition.Add(new SnakeCoordinates());
            snack = new SnakeCoordinates(-50, -50, 20, 20);

            NumbeOfParts(3);

            //--------------------timer-----------------
            timer2.Enabled = true;
            timer2.Interval = 100;
            timer2.Tick += timerTick;
        }



        public void timerTick(object sender, EventArgs e)
        {
            MakeSnack();
            AnimateSnake();
            MoveSnakeConstantly();
            CheckForCrash();
            label3.Text = (score * 10).ToString();

        }

        private void PlayAgain(object sender, EventArgs e)
        {
            label1.Visible = false;
            score = 0;

            for (int i = snakePosition.Count - 1; i >= 0; i--)
            {
                snakePosition.Remove(snakePosition[i]);
                snakePreviousPosition.Remove(snakePreviousPosition[i]);
            }


            snakePosition.Add(new SnakeCoordinates(((Convert.ToInt16(panel1.Width / (2 * velocity))) * velocity) + ((panel1.Width % velocity) / 2),
                ((Convert.ToInt16(panel1.Height / (2 * velocity))) * velocity) + ((panel1.Height % velocity) / 2)));

            snakePreviousPosition.Add(new SnakeCoordinates());
            MakeSnack();
            NumbeOfParts(3);
            //timer2.Enabled = true;
            timer2.Start();


        }

        private void CheckForCrash()
        {
            if (snakePosition[0].x >= (panel1.Width - snakePosition[0].width) || snakePosition[0].x <= 0 || snakePosition[0].y >= (panel1.Height - snakePosition[0].heigth) || snakePosition[0].y <= 0)
            {
                label1.Visible = true;
                //timer2.Enabled = false;
                timer2.Stop();

            }
            if (
                snakePosition[0].x + snakePosition[0].width >= snack.x && snakePosition[0].x <= snack.x + snack.width &&
                snakePosition[0].y + snakePosition[0].heigth >= snack.y && snakePosition[0].y <= snack.y + snack.heigth
               )
            {
                IfSnackEaten = true;
                score++;
                
            }

            for (int i = 1; i < snakePosition.Count; i++)
            {
                if
                (
                snakePosition[0].x + snakePosition[0].width >= snakePosition[i].x && snakePosition[0].x <= snakePosition[i].x + snakePosition[i].width &&
                snakePosition[0].y + snakePosition[0].heigth >= snakePosition[i].y && snakePosition[0].y <= snakePosition[i].y + snakePosition[i].heigth
                )
                {
                    label1.Visible = true;
                    timer2.Stop();
                }
            }

        }
        private void MakeSnack()
        {
            while (IfSnackEaten)
            {
                // ---- spawnowanie przekasek w "siatce", po ktorej sie porusza waz ------
                snack = new SnakeCoordinates((velocity * random.Next(0, (panel1.Width / (int)velocity) - 1)) + (panel1.Width % (int)velocity) / 2,
                    (velocity * random.Next(0, (panel1.Height / (int)velocity) - 1)) + (panel1.Height % (int)velocity) / 2);

                IfSnackEaten = false;
                NumbeOfParts(1);

                for (int i = 0; i < snakePosition.Count; i++)
                {
                    //sprawdzanie czy przekaska jest wewnatrz węża
                    if (
                        snack.x + snack.width >= snakePosition[i].x && snack.x <= snakePosition[i].x + snakePosition[i].width &&
                        snack.y + snack.heigth >= snakePosition[i].y && snack.y <= snakePosition[i].y + snakePosition[i].heigth
                        )
                    {
                        IfSnackEaten = true;
                    }
                }
            }
        }



        private void NumbeOfParts(int value)
        {
            for (int i = 0; i < value; i++)
            {
                snakePosition.Add(new SnakeCoordinates(-50, -50));
                snakePreviousPosition.Add(new SnakeCoordinates());
            }
        }

        private void MoveSnakeConstantly()
        {
            //snakePreviousPosition[0] = snakePosition[0];
            for (int i = 0; i < snakePosition.Count - 1; i++)
            {
                snakePreviousPosition[i].x = snakePosition[i].x;
                snakePreviousPosition[i].y = snakePosition[i].y;
                //snakePreviousPosition[i] = snakePosition[i];
            }


            if (direction == Direction.Down)
            {
                snakePosition[0].y += velocity;
            }
            if (direction == Direction.Up)
            {
                snakePosition[0].y -= velocity;
            }
            if (direction == Direction.Right)
            {
                snakePosition[0].x += velocity;
            }
            if (direction == Direction.Left)
            {
                snakePosition[0].x -= velocity;

            }

            for (int i = 0; i < snakePosition.Count - 1; i++)
            {
                snakePosition[i + 1].x = snakePreviousPosition[i].x;
                snakePosition[i + 1].y = snakePreviousPosition[i].y;
                //snakePosition[i + 1] = snakePreviousPosition[i];
            }


            /*
            foreach (var partSnake in snakePosition)
            {
                if (direction == Direction.Down)
                {
                    partSnake.y += velocity;
                }
                if (direction == Direction.Up)
                {
                    partSnake.y -= velocity;
                }
                if (direction == Direction.Right)
                {
                    partSnake.x += velocity;
                }
                if (direction == Direction.Left)
                {
                    partSnake.x -= velocity;
                }
            }
            */
        }

        private void GetDirection(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.S && direction != Direction.Up)
            { direction = Direction.Down; }
            if (e.KeyCode == Keys.W && direction != Direction.Down)
            { direction = Direction.Up; }
            if (e.KeyCode == Keys.D && direction != Direction.Left)
            { direction = Direction.Right; }
            if (e.KeyCode == Keys.A && direction != Direction.Right)
            { direction = Direction.Left; }

            //if (e.KeyChar == 's')
            //{ partSnake.y += velocity; }
            //if (e.KeyChar == 'w')
            //{ partSnake.y -= velocity; }
            //if (e.KeyChar == 'd')
            //{ partSnake.x += velocity; }
            //if (e.KeyChar == 'a')
            //{ partSnake.x -= velocity; }

        }

        public void AnimateSnake()
        {
            graph = panel1.CreateGraphics();
            panel1.Refresh();
            foreach (var partOfSnake in snakePosition)
            {
                graph.FillRectangle(new SolidBrush(Color.Black), partOfSnake.x, partOfSnake.y, partOfSnake.width, partOfSnake.heigth);
            }
            graph.FillRectangle(new SolidBrush(Color.Blue), snack.x, snack.y, snack.width, snack.heigth);
            graph.FillRectangle(new SolidBrush(Color.Gray), snakePosition[0].x, snakePosition[0].y, snakePosition[0].width, snakePosition[0].heigth);

        }
    }

    public class SnakeCoordinates
    {
        public float x;
        public float y;
        public float width = 20;
        public float heigth = 20;
        //public SizeF size;


        public SnakeCoordinates()
        {
            this.x = 200;
            this.y = 200;
            //this.size = new SizeF(40, 40);

        }
        public SnakeCoordinates(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public SnakeCoordinates(float x, float y, float width, float heigth)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.heigth = heigth;
        }
    }

}
