using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PacManGame
{
    public partial class Form1 : Form
    {
        public int leftGhost1, leftGhost2, leftGhost3, leftGhost4, left;
        public int topGhost1, topGhost2, topGhost3, topGhost4, top;
        public int next, direction, temp = 1, factor = 1;
        public bool pacturn, ghost1Turn, ghost2Turn, ghost3Turn, ghost4Turn, start,
            ghost1CanEat, ghost2CanEat, ghost3CanEat, ghost4CanEat,
            g1Eaten, g2Eaten, g3Eaten, g4Eaten;
        public bool dir1, dir2, dir3, dir4, c,
            superPill, superPill1, superPill2, superPill3, superPill4, v1, v2, v3, v4, tresec = true;
        public int random1, random2, random3, random4, lifeLeft = 3, tic4, tic5, tic6, score, bestScore, score1;

        private void panelMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        public int prec1, prec2, prec3, prec4, startdirection = 1,
            ghost1Velocity = 2, ghost2Velocity = 2, ghost3Velocity = 2, ghost4Velocity = 2;
        public Random rand = new Random();
        public List<object> pill = new List<object>();

        private void Form1_Load(object sender, EventArgs e)
        {
            //bestScore = Properties.Settings.Default.bestScore;
            //panelName.Visible = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.bestScore = bestScore;
            Properties.Settings.Default.Save();
            if (txtName.Text != "")
                interactWithTheFile();
        }

        private void btPlayGame_MouseEnter(object sender, EventArgs e)
        {
            btPlayGame.Image = Properties.Resources.playgame2; // Cho cái hình Play Game nó bự ra thôi á.
        }

        private void btPlayGame_MouseLeave(object sender, EventArgs e)
        {
            btPlayGame.Image = Properties.Resources.playgame; // Cho cái hình Play Game nó nhỏ lại.
        }

        private void btPlayGame_Click(object sender, EventArgs e)
        {
            start = true;
            life();
            scores();
            panelMenu.Visible = false;
            resetall();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) next = 1;
            if (e.KeyCode == Keys.Right) next = 2;
            if (e.KeyCode == Keys.Up) next = 3;
            if (e.KeyCode == Keys.Down) next = 4;
            if (e.KeyCode == Keys.Escape) Close();
            temp = next;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (picPacman.Enabled)
            {
                directionMain();
                freeDirection();
            }

            if (picGhost1.Enabled || picGhost2.Enabled || picGhost3.Enabled || picGhost4.Enabled)
                ghost();

            if (c)
                control();

            superPillMain();
            scores();
            collision();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            lbGetReady.Visible = false;
            timer1.Enabled = true;
            timer2.Enabled = false;
            c = true;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (picGhost2.Top == 180) 
                startdirection = 2; // Nếu ko có thì 2 con ma Xanh và Cam đi lên trên luôn (1)
            if (picGhost2.Top == 192) 
                startdirection = 1; // 2 con ma Xanh và Cam lên xuống tại ổ dịch (2)
            if (startdirection == 2) // Làm cho (1) chạy.
            {
                picGhost2.Top++; picGhost4.Top++;
            }
            if (startdirection == 1) // Làm cho (2) chạy.
            {
                picGhost2.Top--; picGhost4.Top--;
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            tic4++;
            if (picGhost3.Top > 150 && tic4 > 100 /* ... ra chuồng */)
            {
                prec1 = 3;
                picGhost3.Top--;
            }
            if (picGhost3.Top == 150) // Hễ cứ 150 là con ma nó nhào ra
                timer4.Enabled = false;
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            tic5++;
            if (picGhost2.Left > 153 && picGhost2.Left < 176 && tic5 > 300) 
                picGhost2.Left++;
            if (picGhost2.Top > 150 && picGhost2.Left == 176) 
                picGhost2.Top--;
            if (picGhost2.Top == 150) 
            {
                timer6.Enabled = true; timer5.Enabled = false; 
            } // Hễ cứ 150 là nhào ra
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            tic6++;
            if (picGhost4.Left > 176 && picGhost4.Left < 200 && tic6 > 100) 
                picGhost4.Left--;
            if (picGhost4.Top > 150 && picGhost4.Left == 176) 
                picGhost4.Top--;
            if (picGhost4.Top == 150) 
                timer6.Enabled = false; // Hễ cứ 150 là nhào ra
        }

        private void PowerPill_Tick(object sender, EventArgs e)
        {
            tresec = false;
            if (!g1Eaten)
            {
                v1 = false;
                ghost1CanEat = true;
                superPill1 = false;
                ghost1Velocity = 2; 
            }
            if (!g2Eaten)
            {
                v2 = false;
                ghost2CanEat = true;
                superPill2 = false;
                ghost2Velocity = 2;
            }
            if (!g3Eaten)
            {
                v3 = false;
                ghost3CanEat = true;
                superPill3 = false;
                ghost3Velocity = 2;
            }
            if (!g4Eaten)
            {
                v4 = false;
                ghost4CanEat = true;
                superPill4 = false;
                ghost4Velocity = 2;
            }
            superPill = false;
            PowerPill.Enabled = false;
            PowerPill1.Enabled = false;

            if (prec1 == 1 && !g1Eaten)
            {
                if (picGhost1.Left % 2 == 0)
                    leftGhost1 = ghost1Velocity;
                picGhost1.Image = Properties.Resources.rdx;
            }
            if (prec1 == 2 && !g1Eaten)
            {
                if (picGhost1.Left % 2 == 0)
                    leftGhost1 = -ghost1Velocity;
                picGhost1.Image = Properties.Resources.rsx;
            }
            if (prec1 == 3 && !g1Eaten)
            {
                if (picGhost1.Top % 2 == 0)
                    topGhost1 = -ghost1Velocity;
                picGhost1.Image = Properties.Resources.rup;
            }
            if (prec1 == 4 && !g1Eaten)
            {
                if (picGhost1.Top % 2 == 0)
                    topGhost1 = ghost1Velocity;
                picGhost1.Image = Properties.Resources.rdown;
            }

            if (prec2 == 1 && !g2Eaten)
            {
                if (picGhost2.Left % 2 == 0)
                    leftGhost2 = ghost2Velocity;
                picGhost2.Image = Properties.Resources.adx;
            }
            if (prec2 == 2 && !g2Eaten)
            {
                if (picGhost2.Left % 2 == 0)
                    leftGhost2 = -ghost2Velocity;
                picGhost2.Image = Properties.Resources.asx;
            }
            if (prec2 == 3 && !g2Eaten)
            {
                if (picGhost2.Top % 2 == 0)
                    topGhost2 = -ghost2Velocity;
                picGhost2.Image = Properties.Resources.aup;
            }
            if (prec2 == 4 && !g2Eaten)
            {
                if (picGhost2.Top % 2 == 0)
                    topGhost2 = ghost2Velocity;
                picGhost2.Image = Properties.Resources.adown;
            }

            if (prec3 == 1 && !g3Eaten)
            {
                if (picGhost3.Left % 2 == 0)
                    leftGhost3 = ghost3Velocity;
                picGhost3.Image = Properties.Resources.vdx;
            }
            if (prec3 == 2 && !g3Eaten)
            {
                if (picGhost3.Left % 2 == 0)
                    leftGhost3 = -ghost3Velocity;
                picGhost3.Image = Properties.Resources.vsx;
            }
            if (prec3 == 3 && !g3Eaten)
            {
                if (picGhost3.Top % 2 == 0)
                    topGhost3 = -ghost3Velocity;
                picGhost3.Image = Properties.Resources.vup;
            }
            if (prec3 == 4 && !g3Eaten)
            {
                if (picGhost3.Top % 2 == 0)
                    topGhost3 = ghost3Velocity;
                picGhost3.Image = Properties.Resources.vdown;
            }

            if (prec4 == 1 && !g4Eaten)
            {
                if (picGhost4.Left % 2 == 0)
                    leftGhost4 = ghost4Velocity;
                picGhost4.Image = Properties.Resources.gdx;
            }
            if (prec4 == 2 && !g4Eaten)
            {
                if (picGhost4.Left % 2 == 0)
                    leftGhost4 = -ghost4Velocity;
                picGhost4.Image = Properties.Resources.gsx;
            }
            if (prec4 == 3 && !g4Eaten)
            {
                if (picGhost4.Top % 2 == 0)
                    topGhost4 = -ghost4Velocity;
                picGhost4.Image = Properties.Resources.gup;
            }
            if (prec4 == 4 && !g4Eaten)
            {
                if (picGhost4.Top % 2 == 0)
                    topGhost4 = ghost4Velocity;
                picGhost1.Image = Properties.Resources.gdown;
            }
        }

        private void timer8_Tick(object sender, EventArgs e)
        {
            picPacman.SetBounds(picPacman.Left, picPacman.Top, 0, 0);
            g1Eaten = false;
            g2Eaten = false;
            g3Eaten = false;
            g4Eaten = false;
            v1 = false;
            v2 = false;
            v3 = false;
            v4 = false;
            ghost1CanEat = true;
            ghost2CanEat = true;
            ghost3CanEat = true;
            ghost4CanEat = true;
            picGhost1.Visible = false;
            picGhost2.Visible = false;
            picGhost3.Visible = false;
            picGhost4.Visible = false;
            picPacman.Visible = false;
            tic4 = 0;
            tic5 = 0;
            tic6 = 0;
            superPill = false;
            superPill1 = false;
            superPill2 = false;
            superPill3 = false;
            superPill4 = false;
            ghost1Velocity = 2;
            ghost2Velocity = 2;
            ghost3Velocity = 2;
            ghost4Velocity = 2;
            lifeLeft--;
            life();
            prec1 = 0;
            prec2 = 0;
            prec3 = 0;
            prec4 = 0;
            startdirection = 1;
            dir1 = false;
            dir2 = false;
            dir3 = false;
            dir4 = false;
            c = false;
            next = 0;
            temp = 1;
            direction = 0;

            picPacman.Left = 24; picPacman.Top = 364;
            picGhost1.Top = 150; picGhost1.Left = 176;
            picGhost2.Top = 185; picGhost2.Left = 154;
            picGhost3.Top = 185; picGhost3.Left = 176;
            picGhost4.Top = 185; picGhost4.Left = 199;

            topGhost1 = 0; leftGhost1 = 0;
            topGhost2 = 0; leftGhost2 = 0;
            topGhost3 = 0; leftGhost3 = 0;
            topGhost4 = 0; leftGhost4 = 0;
            
            top = 0; left = 0;
            timer8.Enabled = false;

            picPacman.Image = Properties.Resources._1dx;
            picGhost1.Image = Properties.Resources.rup;
            picGhost2.Image = Properties.Resources.aup;
            picGhost3.Image = Properties.Resources.vup;
            picGhost4.Image = Properties.Resources.gup;

            picGhost1.Visible = true;
            picGhost2.Visible = true;
            picGhost3.Visible = true;
            picGhost4.Visible = true;
            picPacman.SetBounds(picPacman.Left, picPacman.Top, 22, 22);
            picPacman.Visible = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            timer8.Interval = 1900;
        }

        private void timer9_Tick(object sender, EventArgs e)
        {
            pictureBox3.Visible = true;
            btPlayGame.Visible = true;
            timer9.Enabled = false;
        }

        private void resetall()
        {
            g1Eaten = false; g2Eaten = false; g3Eaten = false; g4Eaten = false;
            v1 = false; v2 = false; v3 = false; v4 = false;
            ghost1CanEat = true; ghost2CanEat = true; ghost3CanEat = true; ghost4CanEat = true;
            lbGameOver.Visible = false;
            lbGameWin.Visible = false;
            picPacman.SetBounds(picPacman.Left, picPacman.Top, 22, 22);
            tic4 = 0;
            tic5 = 0;
            tic6 = 0;
            superPill = false; superPill1 = false; superPill2 = false; superPill3 = false; superPill4 = false;
            ghost1Velocity = 2; ghost2Velocity = 2; ghost3Velocity = 2; ghost4Velocity = 2;
            life();
            prec1 = 0; prec2 = 0; prec3 = 0; prec4 = 0;
            startdirection = 1;
            dir1 = false; dir2 = false; dir3 = false; dir4 = false;
            c = false;
            next = 0;
            temp = 1;
            direction = 0;
            picPacman.Left = 24;
            picPacman.Top = 364;
            picGhost1.Left = 176;
            picGhost1.Top = 150;
            picGhost2.Left = 154;
            picGhost2.Top = 185;
            picGhost3.Left = 176;
            picGhost3.Top = 185;
            picGhost4.Left = 199;
            picGhost4.Top = 185;
            leftGhost1 = 0;
            leftGhost2 = 0;
            leftGhost3 = 0;
            leftGhost4 = 0;
            topGhost1 = 0;
            topGhost2 = 0;
            topGhost3 = 0;
            topGhost4 = 0;
            left = 0;
            top = 0;
            picPacman.Image = Properties.Resources._1dx;
            picGhost1.Image = Properties.Resources.rup;
            picGhost2.Image = Properties.Resources.aup;
            picGhost3.Image = Properties.Resources.vup;
            picGhost4.Image = Properties.Resources.gup;
            timer1.Enabled = false;
            timer4.Enabled = false;
            timer5.Enabled = false;
            timer6.Enabled = false;
            timer7.Enabled = false;
            timer8.Enabled = false;
            PowerPill.Enabled = false;
            PowerPill1.Enabled = false;
            if (start)
            {
                lbGetReady.Visible = true;
                timer2.Enabled = true;
                timer3.Enabled = true;
            }
            start = false;
            for (int i = 0; i < 332; i++) ((Label)pill[i]).Visible = true;
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            timer7.Enabled = false;
            panelMenu.Visible = true;
            Waiting.Enabled = true;
        }
        
        private void Waiting_Tick(object sender, EventArgs e) //Time Waiting
        {
            resetall();
            Waiting.Enabled = false;
        }

        // Chuyển động ban đầu của những con ma
        private void control()
        {
            if (picGhost2.Top == 185)
            {
                timer3.Enabled = false;
                timer4.Enabled = true;
                c = false;
                timer5.Enabled = true;
            }
        }

        // Pacman có 3 mạng sống.
        private void life()
        {
            if (lifeLeft == 3)
            {
                picLife1.Visible = true;
                picLife2.Visible = true;
            }
            if (lifeLeft == 2)
            {
                picLife1.Visible = true;
                picLife2.Visible = false;
            }
            if (lifeLeft == 1)
            {
                picLife1.Visible = false;
                picLife2.Visible = false;
            }
        }

        private void superPillMain()
        {
            if (picPacman.Bounds.IntersectsWith(lbSuper1.Bounds) && lbSuper1.Visible == true) 
                superPillMain2();
            if (picPacman.Bounds.IntersectsWith(lbSuper2.Bounds) && lbSuper2.Visible == true) 
                superPillMain2();
            if (picPacman.Bounds.IntersectsWith(lbSuper3.Bounds) && lbSuper3.Visible == true)
                superPillMain2();
            if (picPacman.Bounds.IntersectsWith(lbSuper4.Bounds) && lbSuper4.Visible == true)
                superPillMain2();
        }

        private void superPillMain2()
        {
            if (!g1Eaten)
            {
                v1 = false;
                ghost1Velocity = 2;
                superPill1 = true;
                picGhost1.Image = Properties.Resources.crazy;
                ghost1CanEat = true;
            }
            if (!g2Eaten)
            {
                v2 = false;
                ghost2Velocity = 2;
                superPill2 = true;
                picGhost2.Image = Properties.Resources.crazy;
                ghost2CanEat = true;
            }
            if (!g3Eaten)
            {
                v3 = false;
                ghost3Velocity = 2;
                superPill3 = true;
                picGhost3.Image = Properties.Resources.crazy;
                ghost3CanEat = true;
            }
            if (!g4Eaten)
            {
                v4 = false;
                ghost4Velocity = 2;
                superPill4 = true;
                picGhost4.Image = Properties.Resources.crazy;
                ghost4CanEat = true;
            }
            PowerPill.Enabled = false;
            PowerPill1.Enabled = false;
            PowerPill1.Enabled = true;
            tresec = false;
            PowerPill.Enabled = true;
            superPill = true;
        }

        private void PowerPill1_Tick(object sender, EventArgs e)
        {
            tresec = true;
            PowerPill1.Enabled = false;
        }

        private void collision()
        {   
            if (picPacman.Bounds.IntersectsWith(picGhost1.Bounds) || 
                picPacman.Bounds.IntersectsWith(picGhost2.Bounds) || 
                picPacman.Bounds.IntersectsWith(picGhost3.Bounds) || 
                picPacman.Bounds.IntersectsWith(picGhost4.Bounds))
            {
                if (picPacman.Bounds.IntersectsWith(picGhost1.Bounds) && !superPill1) 
                    eaten();
                if (picPacman.Bounds.IntersectsWith(picGhost2.Bounds) && !superPill2) 
                    eaten();
                if (picPacman.Bounds.IntersectsWith(picGhost3.Bounds) && !superPill3) 
                    eaten();
                if (picPacman.Bounds.IntersectsWith(picGhost4.Bounds) && !superPill4) 
                    eaten();
                if (superPill)
                {
                    if (picPacman.Bounds.IntersectsWith(picGhost1.Bounds) && !superPill1) 
                        eaten();
                    if (picPacman.Bounds.IntersectsWith(picGhost2.Bounds) && !superPill2) 
                        eaten();
                    if (picPacman.Bounds.IntersectsWith(picGhost3.Bounds) && !superPill3) 
                        eaten();
                    if (picPacman.Bounds.IntersectsWith(picGhost4.Bounds) && !superPill4)
                        eaten();
                }
                if (picPacman.Bounds.IntersectsWith(picGhost1.Bounds) && ghost1CanEat && superPill1)
                {
                    g1Eaten = true;
                    v1 = true;
                    ghost1CanEat = false;
                    timer1.Enabled = false;
                    GhostEaten.Enabled = true;
                    ghost1Velocity = 4;
                }
                if (picPacman.Bounds.IntersectsWith(picGhost2.Bounds) && ghost2CanEat && superPill2)
                {
                    g2Eaten = true;
                    v2 = true;
                    ghost2CanEat = false;
                    timer1.Enabled = false;
                    GhostEaten.Enabled = true;
                    ghost2Velocity = 4;
                }
                if (picPacman.Bounds.IntersectsWith(picGhost3.Bounds) && ghost3CanEat && superPill3)
                {
                    g3Eaten = true;
                    v3 = true;
                    ghost3CanEat = false;
                    timer1.Enabled = false;
                    GhostEaten.Enabled = true;
                    ghost3Velocity = 4;
                }
                if (picPacman.Bounds.IntersectsWith(picGhost4.Bounds) && ghost4CanEat && superPill4)
                {
                    g4Eaten = true;
                    v4 = true;
                    ghost4CanEat = false;
                    timer1.Enabled = false;
                    GhostEaten.Enabled = true;
                    ghost4Velocity = 4;
                }
            }
        }

        private void GhostEaten_Tick(object sender, EventArgs e) // Timer Con ma ăn thịt người :((
        {
            timer1.Enabled = true;
            GhostEaten.Enabled = false;
        }

        private void eaten()
        {
            picPacman.Image = Properties.Resources.pacmorto; // Lúc chết thì Pacman teo lại :((
            if (lifeLeft - 1 <= 0)
            {
                lbGameOver.Visible = true;
                gameOver();
            }
            else
            {
                top = 0; left = 0;
                topGhost1 = 0; topGhost2 = 0; topGhost3 = 0; topGhost4 = 0;
                leftGhost1 = 0; leftGhost2 = 0; leftGhost3 = 0; leftGhost4 = 0;
                timer1.Enabled = false;
                timer4.Enabled = false;
                timer5.Enabled = false;
                timer6.Enabled = false;
                pacturn = false;
                ghost1Turn = false;
                ghost2Turn = false;
                ghost3Turn = false;
                ghost4Turn = false;
                timer8.Enabled = true;
            }
        }

        // Kiểm soát điểm
        private void scores()
        {
            for (int i = 0; i < 332; i++)
            {
                if (((Label)pill[i]).Visible == true && picPacman.Bounds.IntersectsWith(((Label)pill[i]).Bounds))
                {
                    score += 1;
                    ((Label)pill[i]).Visible = false;
                    score1 = score;
                }
            }
            if (bestScore < score)
            {
                lbScore.Text = score.ToString() + " / " + score.ToString();
                bestScore = score;
            }
            else
                lbScore.Text = score.ToString() + " / " + bestScore.ToString();

            while (score >= 332 * factor)
            {
                if (score >= 332 * factor)
                {
                    gameWin();
                    lbGameWin.Visible = true;
                    factor++;
                }
            }
        }

        // Game Win
        private bool gameWin()
        {
            lifeLeft = 3;
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            timer4.Enabled = false;
            timer5.Enabled = false;
            timer6.Enabled = false;
            PowerPill1.Enabled = false;
            PowerPill.Enabled = false;
            timer7.Enabled = true;
            return true;
        }
        // Game Over
        private bool gameOver()
        {
            //bestScore = score;
            score = 0;
            factor = 1;
            lifeLeft = 3;
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            timer4.Enabled = false;
            timer5.Enabled = false;
            timer6.Enabled = false;
            PowerPill1.Enabled = false;
            PowerPill.Enabled = false;
            timer7.Enabled = true;
            return true;
        }

        // Thay đổi hướng của pacman của những con ma và pacman
        private void freeDirection()
        {
            switch (direction)
            {
                case 1:
                    if (next == 2)
                    {
                        left = 2;
                        picPacman.Image = Properties.Resources.pacdx; // Phải
                        direction = next;
                        temp = next;
                    }
                    break;
                case 2:
                    if (next == 1)
                    {
                        left = -2;
                        picPacman.Image = Properties.Resources.pacsx; // Trái
                        direction = next;
                        temp = next;
                    }
                    break;
                case 3:
                    if (next == 4)
                    {
                        top = 2;
                        picPacman.Image = Properties.Resources.pacdown; // Dưới
                        direction = next;
                        temp = next;
                    }
                    break;
                case 4:
                    if (next == 3)
                    {
                        top = -2;
                        picPacman.Image = Properties.Resources.pacup; // Trên
                        direction = next;
                        temp = next;
                    }
                    break;
            }
        }

        // Thay đổi hướng của những con ma và Pacman
        private void leftRight(int i, int y, int n, int m)
        {
            // Con ma 1
            if (ghost1Turn)
            {
                if (superPill1 && !ghost1CanEat)
                {
                    if (picGhost1.Left < 180 && picGhost1.Top == 150 || picGhost1.Left > 170 && picGhost1.Top == 150)
                    {
                        ghost1Velocity = 2;
                        superPill1 = false;
                        v1 = false;
                        ghost1CanEat = true;
                        g1Eaten = false;
                    }
                    else
                    {
                        if (picGhost1.Top < 150) { if (n == 1 && m == 1) n = 0; }
                        if (picGhost1.Top > 150) { if (n == 1 && m == 1) m = 0; }
                        if (picGhost1.Left < 176) { if (i == 1 && y == 1) i = 0; }
                        if (picGhost1.Left > 176) { if (i == 1 && y == 1) y = 0; }
                    }
                }
                topGhost1 = 0;
                leftGhost1 = 0;
                while (!dir1)
                {
                    random1 = rand.Next(1, 5);
                    if (random1 == 1 && !dir1 && random1 != prec1) // CON MAI DI CHUYỂN QUA TRÁI
                        if (i == 1)
                        {
                            leftGhost1 = -ghost1Velocity; dir1 = true;
                            if (!superPill1 && ghost1CanEat)
                                picGhost1.Image = Properties.Resources.rsx; // Con ma di chuyển qua trái
                            else
                            if (!tresec || g1Eaten)
                            {
                                if (!tresec)
                                    picGhost1.Image = Properties.Resources.crazy; // Con ma bị chuyển sang màu xanh
                                if (g1Eaten)
                                    picGhost1.Image = Properties.Resources.msx;  // Di chuyển đôi mắt con ma qua trái khi bị Pacman ăn
                            }
                            else
                                picGhost1.Image = Properties.Resources.tempo; // Con ma cbi trở lại ban đầu
                        }
                    if (random1 == 2 && !dir1 && random1 != prec1) //  CON MAI DI CHUYỂN QUA PHẢI
                        if (y == 1)
                        {
                            leftGhost1 = ghost1Velocity; dir1 = true;
                            if (!superPill1 && ghost1CanEat)
                                picGhost1.Image = Properties.Resources.rdx; // Con ma di chuyển qua trái
                            else
                            if (!tresec || g1Eaten)
                            {
                                if (!tresec)
                                    picGhost1.Image = Properties.Resources.crazy; // Con ma bị chuyển sang màu xanh
                                if (g1Eaten)
                                    picGhost1.Image = Properties.Resources.mdx;  // Di chuyển đôi mắt con ma qua phải khi bị Pacman ăn
                            }
                            else
                                picGhost1.Image = Properties.Resources.tempo;
                        }
                    if (random1 == 3 && !dir1 && random1 != prec1) if (m == 1) { topGhost1 = ghost1Velocity; dir1 = true; if (!superPill1 && ghost1CanEat) picGhost1.Image = Properties.Resources.rdown; else if (!tresec || g1Eaten) { if (!tresec) picGhost1.Image = Properties.Resources.crazy; if (g1Eaten) picGhost1.Image = Properties.Resources.mdown; } else picGhost1.Image = Properties.Resources.tempo; }
                    if (random1 == 4 && !dir1 && random1 != prec1) if (n == 1) { topGhost1 = -ghost1Velocity; dir1 = true; if (!superPill1 && ghost1CanEat) picGhost1.Image = Properties.Resources.rup; else if (!tresec || g1Eaten) { if (!tresec) picGhost1.Image = Properties.Resources.crazy; if (g1Eaten) picGhost1.Image = Properties.Resources.mup1; } else picGhost1.Image = Properties.Resources.tempo; }
                }
                if (random1 == 1) prec1 = 2; // Nếu ko có mấy dòng này thì khi con ma bị ăn, có thể sẽ mất con ma vĩnh viễn
                if (random1 == 2) prec1 = 1;
                if (random1 == 3) prec1 = 4;
                if (random1 == 4) prec1 = 3;
                dir1 = false;
            }
            // Con ma 2
            if (ghost2Turn)
            {
                if (superPill2 && !ghost2CanEat)
                {
                    if (picGhost2.Left < 180 && picGhost2.Top == 150 || picGhost2.Left > 170 && picGhost2.Top == 150) { ghost2Velocity = 2; superPill2 = false; v2 = false; ghost2CanEat = true; g2Eaten = false; }
                    else
                    {
                        if (picGhost2.Top < 150) { if (n == 1 && m == 1) n = 0; }
                        if (picGhost2.Top > 150) { if (n == 1 && m == 1) m = 0; }
                        if (picGhost2.Left < 176) { if (i == 1 && y == 1) i = 0; }
                        if (picGhost2.Left > 176) { if (i == 1 && y == 1) y = 0; }
                    }
                }
                topGhost2 = 0;
                leftGhost2 = 0;
                while (!dir2)
                {
                    random2 = rand.Next(1, 5);
                    if (random2 == 1 && !dir2 && random2 != prec2) if (i == 1) { leftGhost2 = -ghost2Velocity; dir2 = true; if (!superPill2 && ghost2CanEat) picGhost2.Image = Properties.Resources.asx; else if (!tresec || g2Eaten) { if (!tresec) picGhost2.Image = Properties.Resources.crazy; if (g2Eaten) picGhost2.Image = Properties.Resources.msx; } else picGhost2.Image = Properties.Resources.tempo; }
                    if (random2 == 2 && !dir2 && random2 != prec2) if (y == 1) { leftGhost2 = ghost2Velocity; dir2 = true; if (!superPill2 && ghost2CanEat) picGhost2.Image = Properties.Resources.adx; else if (!tresec || g2Eaten) { if (!tresec) picGhost2.Image = Properties.Resources.crazy; if (g2Eaten) picGhost2.Image = Properties.Resources.mdx; } else picGhost2.Image = Properties.Resources.tempo; }
                    if (random2 == 3 && !dir2 && random2 != prec2) if (m == 1) { topGhost2 = ghost2Velocity; dir2 = true; if (!superPill2 && ghost2CanEat) picGhost2.Image = Properties.Resources.adown; else if (!tresec || g2Eaten) { if (!tresec) picGhost2.Image = Properties.Resources.crazy; if (g2Eaten) picGhost2.Image = Properties.Resources.mdown; } else picGhost2.Image = Properties.Resources.tempo; }
                    if (random2 == 4 && !dir2 && random2 != prec2) if (n == 1) { topGhost2 = -ghost2Velocity; dir2 = true; if (!superPill2 && ghost2CanEat) picGhost2.Image = Properties.Resources.aup; else if (!tresec || g2Eaten) { if (!tresec) picGhost2.Image = Properties.Resources.crazy; if (g2Eaten) picGhost2.Image = Properties.Resources.mup1; } else picGhost2.Image = Properties.Resources.tempo; }
                }
                if (random2 == 1) prec2 = 2;
                if (random2 == 2) prec2 = 1;
                if (random2 == 3) prec2 = 4;
                if (random2 == 4) prec2 = 3;
                dir2 = false;
            }
            //  Con ma 3
            if (ghost3Turn)
            {
                if (superPill3 && !ghost3CanEat)
                {
                    if (picGhost3.Left < 180 && picGhost3.Top == 150 || picGhost3.Left > 170 && picGhost3.Top == 150) { ghost3Velocity = 2; superPill3 = false; v3 = false; ghost3CanEat = true; g3Eaten = false; }
                    else
                    {
                        if (picGhost3.Top > 150) { if (n == 1 && m == 1) m = 0; }
                        if (picGhost3.Top < 150) { if (n == 1 && m == 1) n = 0; }
                        if (picGhost3.Left < 176) { if (i == 1 && y == 1) i = 0; }
                        if (picGhost3.Left > 176) { if (i == 1 && y == 1) y = 0; }
                    }
                }
                topGhost3 = 0;
                leftGhost3 = 0;
                while (!dir3)
                {
                    random3 = rand.Next(1, 5);
                    if (random3 == 1 && !dir3 && random3 != prec3) if (i == 1) { leftGhost3 = -ghost3Velocity; dir3 = true; if (!superPill3 && ghost3CanEat) picGhost3.Image = Properties.Resources.vsx; else if (!tresec || g3Eaten) { if (!tresec) picGhost3.Image = Properties.Resources.crazy; if (g3Eaten) picGhost3.Image = Properties.Resources.msx; } else picGhost3.Image = Properties.Resources.tempo; }
                    if (random3 == 2 && !dir3 && random3 != prec3) if (y == 1) { leftGhost3 = ghost3Velocity; dir3 = true; if (!superPill3 && ghost3CanEat) picGhost3.Image = Properties.Resources.vdx; else if (!tresec || g3Eaten) { if (!tresec) picGhost3.Image = Properties.Resources.crazy; if (g3Eaten) picGhost3.Image = Properties.Resources.mdx; } else picGhost3.Image = Properties.Resources.tempo; }
                    if (random3 == 3 && !dir3 && random3 != prec3) if (m == 1) { topGhost3 = ghost3Velocity; dir3 = true; if (!superPill3 && ghost3CanEat) picGhost3.Image = Properties.Resources.vdown; else if (!tresec || g3Eaten) { if (!tresec) picGhost3.Image = Properties.Resources.crazy; if (g3Eaten) picGhost3.Image = Properties.Resources.mdown; } else picGhost3.Image = Properties.Resources.tempo; }
                    if (random3 == 4 && !dir3 && random3 != prec3) if (n == 1) { topGhost3 = -ghost3Velocity; dir3 = true; if (!superPill3 && ghost3CanEat) picGhost3.Image = Properties.Resources.vup; else if (!tresec || g3Eaten) { if (!tresec) picGhost3.Image = Properties.Resources.crazy; if (g3Eaten) picGhost3.Image = Properties.Resources.mup1; } else picGhost3.Image = Properties.Resources.tempo; }
                }
                if (random3 == 1) prec3 = 2;
                if (random3 == 2) prec3 = 1;
                if (random3 == 3) prec3 = 4;
                if (random3 == 4) prec3 = 3;
                dir3 = false;
            }
            //  Con ma 4
            if (ghost4Turn)
            {
                if (superPill4 && !ghost4CanEat)
                {
                    if (picGhost4.Left < 180 && picGhost4.Top == 150 || picGhost4.Left > 170 && picGhost4.Top == 150) { ghost4Velocity = 2; superPill4 = false; v4 = false; ghost4CanEat = true; g4Eaten = false; }
                    else
                    {
                        if (picGhost4.Top > 150) { if (n == 1 && m == 1) m = 0; }
                        if (picGhost4.Top < 150) { if (n == 1 && m == 1) n = 0; }
                        if (picGhost4.Left < 176) { if (i == 1 && y == 1) i = 0; }
                        if (picGhost4.Left > 176) { if (i == 1 && y == 1) y = 0; }
                    }
                }
                topGhost4 = 0;
                leftGhost4 = 0;
                while (!dir4)
                {
                    random4 = rand.Next(1, 5);
                    if (random4 == 1 && !dir4 && random4 != prec4) if (i == 1) { leftGhost4 = -ghost4Velocity; dir4 = true; if (!superPill4 && ghost4CanEat) picGhost4.Image = Properties.Resources.gsx; else if (!tresec || g4Eaten) { if (!tresec) picGhost4.Image = Properties.Resources.crazy; if (g4Eaten) picGhost4.Image = Properties.Resources.msx; } else picGhost4.Image = Properties.Resources.tempo; }
                    if (random4 == 2 && !dir4 && random4 != prec4) if (y == 1) { leftGhost4 = ghost4Velocity; dir4 = true; if (!superPill4 && ghost4CanEat) picGhost4.Image = Properties.Resources.gdx; else if (!tresec || g4Eaten) { if (!tresec) picGhost4.Image = Properties.Resources.crazy; if (g4Eaten) picGhost4.Image = Properties.Resources.mdx; } else picGhost4.Image = Properties.Resources.tempo; }
                    if (random4 == 3 && !dir4 && random4 != prec4) if (m == 1) { topGhost4 = ghost4Velocity; dir4 = true; if (!superPill4 && ghost4CanEat) picGhost4.Image = Properties.Resources.gdown; else if (!tresec || g4Eaten) { if (!tresec) picGhost4.Image = Properties.Resources.crazy; if (g4Eaten) picGhost4.Image = Properties.Resources.mdown; } else picGhost4.Image = Properties.Resources.tempo; }
                    if (random4 == 4 && !dir4 && random4 != prec4) if (n == 1) { topGhost4 = -ghost4Velocity; dir4 = true; if (!superPill4 && ghost4CanEat) picGhost4.Image = Properties.Resources.gup; else if (!tresec || g4Eaten) { if (!tresec) picGhost4.Image = Properties.Resources.crazy; if (g4Eaten) picGhost4.Image = Properties.Resources.mup1; } else picGhost4.Image = Properties.Resources.tempo; }
                }
                if (random4 == 1) prec4 = 2;
                if (random4 == 2) prec4 = 1;
                if (random4 == 3) prec4 = 4;
                if (random4 == 4) prec4 = 3;
                dir4 = false;
            }
            // Pacman
            if (pacturn)
            {
                top = 0;
                left = 0;
                //if (temp == 1 && i == 1 || temp == 2 && y == 1 || temp == 3 && n == 1 || temp == 4 && m == 1)
                //{
                //    next = temp;
                //}
                if (next == 1 && i == 1)
                {
                    left = -2;
                    picPacman.Image = Properties.Resources.pacsx; // Pacman di chuyển qua trái
                    direction = next;
                }
                if (next == 2 && y == 1)
                {
                    left = 2;
                    picPacman.Image = Properties.Resources.pacdx; // Pacman di chuyển qua phải
                    direction = next;
                }
                if (next == 3 && n == 1)
                {
                    top = -2;
                    picPacman.Image = Properties.Resources.pacup; // Pacman di chuyển lên
                    direction = next;
                }
                if (next == 4 && m == 1)
                {
                    top = 2;
                    picPacman.Image = Properties.Resources.pacdown; // Pacman di chuyển xuống
                    direction = next;
                }
                if (top == 0 && left == 0) // Pac man trái-phải-trên-dưới (Hình ảnh không chuyển động)
                { // top và left == 0, nghĩa là con ma đứng yên, chạm tường
                    temp = next;
                    next = direction;
                    if (next == 1) picPacman.Image = Properties.Resources._1sx;
                    if (next == 2) picPacman.Image = Properties.Resources._1dx;
                    if (next == 3) picPacman.Image = Properties.Resources._1up;
                    if (next == 4) picPacman.Image = Properties.Resources._1down;
                }
            }
        }

        // Hướng thay đổi hiệu quả (Hướng chính) của Pacman
        private void directionMain()
        {
            pacturn = true;
            a(picPacman.Left, picPacman.Top);
            picPacman.Left += left;
            picPacman.Top += top;
        }
        
        private void a(int left, int top) // Giao lộ trên bản đồ
        {
            if (pacturn)
            {
                switch (left)
                {
                    case 174:
                        if (top == 148) { leftRight(1, 1, 0, 0); break; }
                        break;
                    case 24:
                        if (top == 364) { leftRight(0, 1, 1, 0); break; }
                        if (top == 328) { leftRight(0, 1, 0, 1); break; }
                        if (top == 292) { leftRight(0, 1, 1, 0); break; }
                        if (top == 256) { leftRight(0, 1, 0, 1); break; }
                        if (top == 112) { leftRight(0, 1, 1, 0); break; }
                        if (top == 76) { leftRight(0, 1, 1, 1); break; }
                        if (top == 28) { leftRight(0, 1, 0, 1); break; }
                        break;
                    case 48:
                        if (top == 328) { leftRight(1, 1, 1, 0); break; }
                        if (top == 292) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 84:
                        if (top == 256) { leftRight(1, 1, 1, 1); break; }
                        if (top == 328) { leftRight(1, 0, 1, 0); break; }
                        if (top == 292) { leftRight(0, 1, 1, 1); break; }
                        if (top == 184) { leftRight(1, 1, 1, 1); break; }
                        if (top == 112) { leftRight(1, 0, 1, 1); break; }
                        if (top == 28) { leftRight(1, 1, 0, 1); break; }
                        if (top == 76) { leftRight(1, 1, 1, 1); break; }
                        break;
                    case 120:
                        if (top == 292) { leftRight(1, 1, 0, 1); break; }
                        if (top == 328) { leftRight(0, 1, 1, 0); break; }
                        if (top == 256) { leftRight(1, 1, 1, 0); break; }
                        if (top == 220) { leftRight(0, 1, 1, 1); break; }
                        if (top == 76) { leftRight(1, 1, 0, 1); break; }
                        if (top == 112) { leftRight(0, 1, 1, 0); break; }
                        if (top == 148) { leftRight(0, 1, 0, 1); break; }
                        if (top == 184) { leftRight(1, 0, 1, 1); break; }
                        break;
                    case 156:
                        if (top == 328) { leftRight(1, 0, 0, 1); break; }
                        if (top == 364) { leftRight(1, 1, 1, 0); break; }
                        if (top == 256) { leftRight(1, 0, 0, 1); break; }
                        if (top == 292) { leftRight(1, 1, 1, 0); break; }
                        if (top == 76) { leftRight(1, 1, 1, 0); break; }
                        if (top == 112) { leftRight(1, 0, 0, 1); break; }
                        if (top == 148) { leftRight(1, 1, 1, 0); break; }
                        if (top == 28) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 192:
                        if (top == 292) { leftRight(1, 1, 1, 0); break; }
                        if (top == 256) { leftRight(0, 1, 0, 1); break; }
                        if (top == 364) { leftRight(1, 1, 1, 0); break; }
                        if (top == 328) { leftRight(0, 1, 0, 1); break; }
                        if (top == 28) { leftRight(0, 1, 0, 1); break; }
                        if (top == 76) { leftRight(1, 1, 1, 0); break; }
                        if (top == 112) { leftRight(0, 1, 0, 1); break; }
                        if (top == 148) { leftRight(1, 1, 1, 0); break; }
                        break;
                    case 228:
                        if (top == 256) { leftRight(1, 1, 1, 0); break; }
                        if (top == 328) { leftRight(1, 0, 1, 0); break; }
                        if (top == 292) { leftRight(1, 1, 0, 1); break; }
                        if (top == 220) { leftRight(1, 0, 1, 1); break; }
                        if (top == 184) { leftRight(0, 1, 1, 1); break; }
                        if (top == 76) { leftRight(1, 1, 0, 1); break; }
                        if (top == 112) { leftRight(1, 0, 1, 0); break; }
                        if (top == 148) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 264:
                        if (top == 256) { leftRight(1, 1, 1, 1); break; }
                        if (top == 292) { leftRight(1, 0, 1, 1); break; }
                        if (top == 328) { leftRight(0, 1, 1, 0); break; }
                        if (top == 184) { leftRight(1, 1, 1, 1); break; }
                        if (top == 112) { leftRight(0, 1, 1, 1); break; }
                        if (top == 28) { leftRight(1, 1, 0, 1); break; }
                        if (top == 76) { leftRight(1, 1, 1, 1); break; }
                        break;
                    case 324:
                        if (top == 256) { leftRight(1, 0, 0, 1); break; }
                        if (top == 292) { leftRight(1, 0, 1, 0); break; }
                        if (top == 328) { leftRight(1, 0, 0, 1); break; }
                        if (top == 364) { leftRight(1, 0, 1, 0); break; }
                        if (top == 112) { leftRight(1, 0, 1, 0); break; }
                        if (top == 76) { leftRight(1, 0, 1, 1); break; }
                        if (top == 28) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 300:
                        if (top == 292) { leftRight(0, 1, 0, 1); break; }
                        if (top == 328) { leftRight(1, 1, 1, 0); break; }
                        break;
                    case 374:
                        if (top == 184) { transport(0, 1); break; }
                        break;
                    case -26:
                        if (top == 184) { transport(1, 0); break; }
                        break;
                }
            }
            else
            {
                switch (left)
                {
                    case 176:
                        if (top == 150) { leftRight(1, 1, 0, 0); break; }
                        break;
                    case 26:
                        if (top == 366) { leftRight(0, 1, 1, 0); break; }
                        if (top == 330) { leftRight(0, 1, 0, 1); break; }
                        if (top == 294) { leftRight(0, 1, 1, 0); break; }
                        if (top == 258) { leftRight(0, 1, 0, 1); break; }
                        if (top == 114) { leftRight(0, 1, 1, 0); break; }
                        if (top == 78) { leftRight(0, 1, 1, 1); break; }
                        if (top == 30) { leftRight(0, 1, 0, 1); break; }
                        break;
                    case 50:
                        if (top == 330) { leftRight(1, 1, 1, 0); break; }
                        if (top == 294) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 86:
                        if (top == 258) { leftRight(1, 1, 1, 1); break; }
                        if (top == 330) { leftRight(1, 0, 1, 0); break; }
                        if (top == 294) { leftRight(0, 1, 1, 1); break; }
                        if (top == 186) { leftRight(1, 1, 1, 1); break; }
                        if (top == 114) { leftRight(1, 0, 1, 1); break; }
                        if (top == 30) { leftRight(1, 1, 0, 1); break; }
                        if (top == 78) { leftRight(1, 1, 1, 1); break; }
                        break;
                    case 122:
                        if (top == 294) { leftRight(1, 1, 0, 1); break; }
                        if (top == 330) { leftRight(0, 1, 1, 0); break; }
                        if (top == 258) { leftRight(1, 1, 1, 0); break; }
                        if (top == 222) { leftRight(0, 1, 1, 1); break; }
                        if (top == 78) { leftRight(1, 1, 0, 1); break; }
                        if (top == 114) { leftRight(0, 1, 1, 0); break; }
                        if (top == 150) { leftRight(0, 1, 0, 1); break; }
                        if (top == 186) { leftRight(1, 0, 1, 1); break; }
                        break;
                    case 158:
                        if (top == 330) { leftRight(1, 0, 0, 1); break; }
                        if (top == 366) { leftRight(1, 1, 1, 0); break; }
                        if (top == 258) { leftRight(1, 0, 0, 1); break; }
                        if (top == 294) { leftRight(1, 1, 1, 0); break; }
                        if (top == 78) { leftRight(1, 1, 1, 0); break; }
                        if (top == 114) { leftRight(1, 0, 0, 1); break; }
                        if (top == 150) { leftRight(1, 1, 1, 0); break; }
                        if (top == 30) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 194:
                        if (top == 294) { leftRight(1, 1, 1, 0); break; }
                        if (top == 258) { leftRight(0, 1, 0, 1); break; }
                        if (top == 366) { leftRight(1, 1, 1, 0); break; }
                        if (top == 330) { leftRight(0, 1, 0, 1); break; }
                        if (top == 30) { leftRight(0, 1, 0, 1); break; }
                        if (top == 78) { leftRight(1, 1, 1, 0); break; }
                        if (top == 114) { leftRight(0, 1, 0, 1); break; }
                        if (top == 150) { leftRight(1, 1, 1, 0); break; }
                        break;
                    case 230:
                        if (top == 258) { leftRight(1, 1, 1, 0); break; }
                        if (top == 330) { leftRight(1, 0, 1, 0); break; }
                        if (top == 294) { leftRight(1, 1, 0, 1); break; }
                        if (top == 222) { leftRight(1, 0, 1, 1); break; }
                        if (top == 186) { leftRight(0, 1, 1, 1); break; }
                        if (top == 78) { leftRight(1, 1, 0, 1); break; }
                        if (top == 114) { leftRight(1, 0, 1, 0); break; }
                        if (top == 150) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 266:
                        if (top == 258) { leftRight(1, 1, 1, 1); break; }
                        if (top == 294) { leftRight(1, 0, 1, 1); break; }
                        if (top == 330) { leftRight(0, 1, 1, 0); break; }
                        if (top == 186) { leftRight(1, 1, 1, 1); break; }
                        if (top == 114) { leftRight(0, 1, 1, 1); break; }
                        if (top == 30) { leftRight(1, 1, 0, 1); break; }
                        if (top == 78) { leftRight(1, 1, 1, 1); break; }
                        break;
                    case 326:
                        if (top == 258) { leftRight(1, 0, 0, 1); break; }
                        if (top == 294) { leftRight(1, 0, 1, 0); break; }
                        if (top == 330) { leftRight(1, 0, 0, 1); break; }
                        if (top == 366) { leftRight(1, 0, 1, 0); break; }
                        if (top == 114) { leftRight(1, 0, 1, 0); break; }
                        if (top == 78) { leftRight(1, 0, 1, 1); break; }
                        if (top == 30) { leftRight(1, 0, 0, 1); break; }
                        break;
                    case 302:
                        if (top == 294) { leftRight(0, 1, 0, 1); break; }
                        if (top == 330) { leftRight(1, 1, 1, 0); break; }
                        break;
                    case 376:
                        if (top == 186) { transport(0, 1); break; }
                        break;
                    case -28:
                        if (top == 186) { transport(1, 0); break; }
                        break;
                }
            }
            pacturn = false;
            ghost1Turn = false;
            ghost2Turn = false;
            ghost3Turn = false;
            ghost4Turn = false;
        }

        // Xử lý Pacman và các con ma ra khỏi form1
        private void transport(int i, int n)
        {
            if (pacturn)
            {
                if (i == 0) picPacman.Left = -26;
                if (n == 0) picPacman.Left = 374;
            }
            if (ghost1Turn)
            {
                if (i == 0) picGhost1.Left = -26;
                if (n == 0) picGhost1.Left = 374;
            }
            if (ghost2Turn)
            {
                if (i == 0) picGhost2.Left = -26;
                if (n == 0) picGhost2.Left = 374;
            }
            if (ghost3Turn)
            {
                if (i == 0) picGhost3.Left = -26;
                if (n == 0) picGhost3.Left = 374;
            }
            if (ghost4Turn)
            {
                if (i == 0) picGhost4.Left = -26;
                if (n == 0) picGhost4.Left = 374;
            }
        }

        // Sửa đổi thực tế vị trí của các con ma
        private void ghost()
        {
            if (picGhost1.Enabled == true)
            {
                picGhost1.Left += leftGhost1;
                picGhost1.Top += topGhost1;
                ghost1Turn = true;
                a(picGhost1.Left, picGhost1.Top);
            }
            if (picGhost2.Enabled == true)
            {
                picGhost2.Left += leftGhost2;
                picGhost2.Top += topGhost2;
                ghost2Turn = true;
                a(picGhost2.Left, picGhost2.Top);
            }
            if (picGhost3.Enabled == true)
            {
                picGhost3.Left += leftGhost3;
                picGhost3.Top += topGhost3;
                ghost3Turn = true;
                a(picGhost3.Left, picGhost3.Top);
            }
            if (picGhost4.Enabled == true)
            {
                picGhost4.Left += leftGhost4;
                picGhost4.Top += topGhost4;
                ghost4Turn = true;
                a(picGhost4.Left, picGhost4.Top);
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && txtName.Text.Trim() != "")
                panelName.Visible = false;
        }

        // Ghi xuống file.txt
        private void interactWithTheFile()
        {
            DateTime aDateTime = DateTime.Now;
            StreamWriter sWrite = new StreamWriter("PHP.txt", true);
            FileInfo fInfo = new FileInfo("PHP.txt");
            if (fInfo.Length == 0)
                sWrite.WriteLine("Nickname\t\t\tDate\t\t\t\t\tScore\r\n");
            if (gameOver())
                sWrite.WriteLine(txtName.Text + "\t\t\t\t" + aDateTime.ToString() + "\t\t\t" + score1.ToString());
            else
                sWrite.WriteLine(txtName.Text + "\t\t\t\t" + aDateTime.ToString() + "\t\t\t" + score.ToString());
            sWrite.Close();
        }

        public Form1()
        {
            InitializeComponent();

            pill.Add(label4); pill.Add(label5); pill.Add(label6); pill.Add(label7); pill.Add(label8); pill.Add(label9); pill.Add(label10); pill.Add(label11); pill.Add(label12); pill.Add(label13);
            pill.Add(label14); pill.Add(label15); pill.Add(label16); pill.Add(label17); pill.Add(label18); pill.Add(label19); pill.Add(label20); pill.Add(label21); pill.Add(label22); pill.Add(label23);
            pill.Add(label24); pill.Add(label25); pill.Add(label26); pill.Add(label27); pill.Add(label28); pill.Add(label29); pill.Add(label30); pill.Add(label31); pill.Add(label32); pill.Add(label33);
            pill.Add(label34); pill.Add(label35); pill.Add(label36); pill.Add(label37); pill.Add(label38); pill.Add(label39); pill.Add(label40); pill.Add(label41); pill.Add(label42); pill.Add(label43);
            pill.Add(label44); pill.Add(label45); pill.Add(label46); pill.Add(label47); pill.Add(label48); pill.Add(label49); pill.Add(label50); pill.Add(label51); pill.Add(label52); pill.Add(label53);
            pill.Add(label54); pill.Add(label55); pill.Add(label56); pill.Add(label57); pill.Add(label58); pill.Add(label59); pill.Add(label60); pill.Add(label61); pill.Add(label62); pill.Add(label63);
            pill.Add(label64); pill.Add(label65); pill.Add(label66); pill.Add(label67); pill.Add(label68); pill.Add(label69); pill.Add(label70); pill.Add(lbSuper2); pill.Add(label72); pill.Add(label73);
            pill.Add(label74); pill.Add(label75); pill.Add(label76); pill.Add(label77); pill.Add(label78); pill.Add(label79); pill.Add(label80); pill.Add(label81); pill.Add(label82); pill.Add(label83);
            pill.Add(label84); pill.Add(label85); pill.Add(label86); pill.Add(label87); pill.Add(label88); pill.Add(label89); pill.Add(label90); pill.Add(label91); pill.Add(label92); pill.Add(label93);
            pill.Add(label94); pill.Add(label95); pill.Add(label96); pill.Add(label97); pill.Add(label98); pill.Add(label99); pill.Add(label100); pill.Add(label101); pill.Add(label102); pill.Add(label103);
            pill.Add(label104); pill.Add(label105); pill.Add(label106); pill.Add(label107); pill.Add(label108); pill.Add(label109); pill.Add(label110); pill.Add(label111); pill.Add(label112); pill.Add(label113);
            pill.Add(label114); pill.Add(label115); pill.Add(label116); pill.Add(label117); pill.Add(label118); pill.Add(label119); pill.Add(label120); pill.Add(label121); pill.Add(label122); pill.Add(label123);
            pill.Add(label125); pill.Add(label126); pill.Add(label127); pill.Add(label128); pill.Add(label129); pill.Add(label130); pill.Add(label131); pill.Add(label132); pill.Add(label133); pill.Add(label134);
            pill.Add(label135); pill.Add(label136); pill.Add(label137); pill.Add(label138); pill.Add(label139); pill.Add(label140); pill.Add(label143); pill.Add(label144); pill.Add(label145); pill.Add(label146);
            pill.Add(label147); pill.Add(label148); pill.Add(label149); pill.Add(label150); pill.Add(label151); pill.Add(label152); pill.Add(label153); pill.Add(label154); pill.Add(label155); pill.Add(label156);
            pill.Add(label157); pill.Add(label158); pill.Add(label159); pill.Add(label160); pill.Add(label161); pill.Add(label162); pill.Add(label163); pill.Add(label164); pill.Add(label165); pill.Add(label166);
            pill.Add(label167); pill.Add(label168); pill.Add(label169); pill.Add(label170); pill.Add(label171); pill.Add(label172); pill.Add(lbSuper3); pill.Add(label174); pill.Add(label175); pill.Add(label176);
            pill.Add(label177); pill.Add(label178); pill.Add(label179); pill.Add(label180); pill.Add(label181); pill.Add(label182); pill.Add(label183); pill.Add(label184); pill.Add(label185); pill.Add(label186);
            pill.Add(label187); pill.Add(label188); pill.Add(label189); pill.Add(label190); pill.Add(label191); pill.Add(label192); pill.Add(label193); pill.Add(label194); pill.Add(label195); pill.Add(label196);
            pill.Add(label197); pill.Add(label198); pill.Add(label199); pill.Add(label200); pill.Add(label201); pill.Add(label202); pill.Add(label203); pill.Add(label204); pill.Add(label205); pill.Add(label206);
            pill.Add(label207); pill.Add(label208); pill.Add(label209); pill.Add(label210); pill.Add(label211); pill.Add(label212); pill.Add(label213); pill.Add(label214); pill.Add(label215); pill.Add(label216);
            pill.Add(label217); pill.Add(label218); pill.Add(label219); pill.Add(lbSuper1); pill.Add(label221); pill.Add(label222); pill.Add(label223); pill.Add(label224); pill.Add(label225); pill.Add(label226);
            pill.Add(label227); pill.Add(label228); pill.Add(label229); pill.Add(label230); pill.Add(label231); pill.Add(label232); pill.Add(label233); pill.Add(label234); pill.Add(label235); pill.Add(label236);
            pill.Add(label237); pill.Add(label238); pill.Add(label239); pill.Add(label240); pill.Add(label241); pill.Add(label242); pill.Add(label243); pill.Add(label244); pill.Add(label245); pill.Add(label246);
            pill.Add(label247); pill.Add(label248); pill.Add(label249); pill.Add(label250); pill.Add(label251); pill.Add(label252); pill.Add(label256); pill.Add(label257); pill.Add(label258); pill.Add(label259);
            pill.Add(label260); pill.Add(label261); pill.Add(label262); pill.Add(label263); pill.Add(label264); pill.Add(label265); pill.Add(label266); pill.Add(label267); pill.Add(label268); pill.Add(label269);
            pill.Add(label270); pill.Add(label271); pill.Add(label272); pill.Add(label273); pill.Add(label274); pill.Add(label275); pill.Add(label276); pill.Add(label277); pill.Add(label278); pill.Add(label279);
            pill.Add(label280); pill.Add(label281); pill.Add(label282); pill.Add(label283); pill.Add(label284); pill.Add(label285); pill.Add(label286); pill.Add(label287); pill.Add(label288); pill.Add(label289);
            pill.Add(label290); pill.Add(label291); pill.Add(label292); pill.Add(label293); pill.Add(label294); pill.Add(label295); pill.Add(label296); pill.Add(label297); pill.Add(label298); pill.Add(label299);
            pill.Add(label300); pill.Add(label301); pill.Add(label302); pill.Add(label303); pill.Add(label304); pill.Add(label305); pill.Add(label306); pill.Add(lbSuper4); pill.Add(label308); pill.Add(label309);
            pill.Add(label310); pill.Add(label311); pill.Add(label312); pill.Add(label313); pill.Add(label314); pill.Add(label315); pill.Add(label316); pill.Add(label317); pill.Add(label318); pill.Add(label319);
            pill.Add(label320); pill.Add(label321); pill.Add(label322); pill.Add(label323); pill.Add(label324); pill.Add(label325); pill.Add(label326); pill.Add(label327); pill.Add(label328); pill.Add(label329);
            pill.Add(label330); pill.Add(label331); pill.Add(label332); pill.Add(label333); pill.Add(label334); pill.Add(label335); pill.Add(label336); pill.Add(label337); pill.Add(label338); pill.Add(label339); pill.Add(label340); pill.Add(label341);
        }
    }
}