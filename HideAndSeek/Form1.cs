using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideAndSeek
{
    public partial class Form1 : Form
    {
        public const int TILE_SIZE = 32*3*2;
        int horizontalOffset = 20;

        const int tileTypes = 0;
        const int STARTING_DISTACE = 1;
        const float SPEED = 0.125f;

        //customizable 

        int mapWidth = 6;
        int mapHeight = 6;

        //

        Label debugLog2 = new Label();
        Label debugLog1 = new Label();
        Panel leftPanel = new Panel();
        Panel rightPanel = new Panel();
        Panel leftBlind = new Panel();
        Panel rightBlind = new Panel();
        Timer gameTimer = new Timer();

        private readonly Random _random = new Random();

        bool leftKeyboard;
    
        bool wait = false;
        Tile[,] gameMap;
        Player[] players = new Player[2];

        public Form1()
        {
            InitializeComponent();

            
        }

        private void Form1_Load(object sender, EventArgs e) {

           

            InitGame(mapWidth, mapHeight, true);
        }

        private void LoadGraphics()
        {
            debugLog1.Text = "";
            debugLog1.Location = new Point(0, 50);
            debugLog1.Height = 2000;
            debugLog1.Width = 450;
            debugLog1.BorderStyle = BorderStyle.Fixed3D;
            debugLog1.Font = new Font("Calibri", 8);
            debugLog1.Padding = new Padding(6);
            debugLog1.Visible = false;
            this.Controls.Add(debugLog1);

            debugLog2.Text = "";
            debugLog2.Location = new Point(650, 500);
            debugLog2.Height = 500;
            debugLog2.Width = 450;
            debugLog2.BorderStyle = BorderStyle.Fixed3D;
            debugLog2.Font = new Font("Calibri", 10);
            debugLog2.Padding = new Padding(6);
            debugLog2.Visible = false;
            this.Controls.Add(debugLog2);

            leftBlind.Location = new Point(50, 50);
            leftBlind.Width = TILE_SIZE * 3;
            leftBlind.Height = TILE_SIZE * 3;
            leftBlind.BringToFront();
            leftBlind.BackColor = Color.Black;
            leftBlind.Visible = false;
            leftBlind.Anchor = AnchorStyles.Left;
            this.Controls.Add(leftBlind);

            rightBlind.Location = new Point(this.Size.Width / 2 + 50, 50);
            rightBlind.Width = TILE_SIZE * 3;
            rightBlind.Height = TILE_SIZE * 3;
            rightBlind.BringToFront();
            rightBlind.BackColor = Color.Black;
            rightBlind.Visible = false;
            rightBlind.Anchor = AnchorStyles.Right;
            this.Controls.Add(rightBlind);


            leftPanel.Location = new Point(50, 50);
            leftPanel.Width = TILE_SIZE * 3;
            leftPanel.Height = TILE_SIZE * 3;
            leftPanel.Anchor = AnchorStyles.Left;
            this.Controls.Add(leftPanel);

            rightPanel.Location = new Point(this.Size.Width / 2 + 50, 50);
            rightPanel.Width = TILE_SIZE * 3 + 50;
            rightPanel.Height = TILE_SIZE * 3 + 50;
            rightPanel.Anchor = AnchorStyles.Right;
            this.Controls.Add(rightPanel);

           




        }
        private void InitGame(int width, int height, bool leftPlayerHider)
        {
           
            //GENERATE BIOMS
            gameMap = new Tile[width, height];
            
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    Image biom = null;
                    int spriteId = _random.Next(0, tileTypes + 1 + 1);
                    switch (spriteId)
                    {
                        case 0:
                            {
                                biom = Image.FromFile(@"..\..\Images\grassTile8.png");                         
                                break;
                            }
                        case 1:
                            {
                                biom = Image.FromFile(@"..\..\Images\grassTile6.png");                       
                                break;
                            }
                    }
                    
                    gameMap[y, x] = new Tile(new Biom(biom));
             
                }
            }

            //INIT PLAYERS
            if (players[0] == null) // if playing for the first time
            {
         
                players[0] = new Player(leftPlayerHider, true, 0, 0);    
                int x = _random.Next(STARTING_DISTACE, width - STARTING_DISTACE); //TODO expception
                int y = _random.Next(STARTING_DISTACE, height - STARTING_DISTACE);
                players[1] = new Player(!leftPlayerHider, false, 2, 2);


                gameTimer.Interval = 40;
                gameTimer.Tick += new EventHandler(GameLoop);

            }
            gameTimer.Start();

            this.Controls.Clear();
            LoadGraphics();

            // DRAW COINS


            // DRAW PLAYER 

            PictureBox p1 = createPictureBox(players[0], 1, 1, true);
            p1.Tag = "player1";
            p1.BringToFront();
            leftPanel.Controls.Add(p1);
            PictureBox p2 = createPictureBox(players[1], 1, 1, false);
            p2.Tag = "player2";
            p2.BringToFront();
            rightPanel.Controls.Add(p2);

            // DRAW TILES AROUND THE PLAYER   
            foreach (Player player in players)
            {
                Point mapPoint = new Point();
                for (int y = 0; y < 3; y++) // draw 3x3 tiles around the player
                {
                    for (int x = 0; x < 3; x++)
                    {
                        mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1); // -1 because player is in [1;1]
                        drawTile(player.getOnLeft(), x, y, mapPoint.X, mapPoint.Y); 
                    }
                    
                }
                
                endAnimation(player.getOnLeft());
            }
        
       

        }

        private Image resize(Image image, int targetWidth, int targetHeight)
        {
            var resizedImage = new Bitmap(targetWidth, targetHeight);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                var attributes = new ImageAttributes();
                attributes.SetWrapMode(WrapMode.TileFlipXY);

                var destination = new Rectangle(0, 0, targetWidth, targetHeight);
                graphics.DrawImage(image, destination, 0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel, attributes);
            }

            return resizedImage;
        }
   
        private void drawTile(bool left, int x, int y, int mapX, int mapY)
        {
            Entity biom = gameMap[mapY, mapX].getTexture();
            Panel child;
           
            if (!left)
            {
                child = rightPanel;
               
            }
            else
            {
                child = leftPanel;
            }

            PictureBox biomPB = createPictureBox(biom, x ,y, left);
         

            foreach (Entity en in gameMap[mapY, mapX].entities)
            {
                biomPB.Controls.Add(createPictureBox(en, x, y, left));
            }
            child.Controls.Add(biomPB);
        }
        private void endAnimation(bool left)
        {

            Panel child;

            if (!left)
            {
                child = rightPanel;
     
            }
            else
            {
                child = leftPanel;
       
            }
            debugLog2.Text = "";
            debugLog1.Text = "";
            int i = 0;
            int j = 0;
            int x;
            int y;
            List<Control> toBeRemoved = new List<Control>();
            foreach (Control item in child.Controls.OfType<PictureBox>())
            {
                if ((string)item.Tag != "player1" && (string)item.Tag != "player2")
                {
                    x = ((PictureBox)item).Left;
                    y = ((PictureBox)item).Top;
                    if (false)
                    {
                        if (x >= -TILE_SIZE / 2 && x <= TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Left = 0;
                        }
                        else if (x >= TILE_SIZE / 2 && x <= TILE_SIZE + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Left = TILE_SIZE;
                        }
                        else if (x >= TILE_SIZE + TILE_SIZE / 2 && x <= TILE_SIZE * 2 + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Left = TILE_SIZE * 2;
                        }

                        if (y >= -TILE_SIZE / 2 && y <= TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Top = 0;
                        }
                        else if (y >= TILE_SIZE / 2 && y <= TILE_SIZE + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Top = TILE_SIZE;
                        }
                        else if (y >= TILE_SIZE + TILE_SIZE / 2 && y <= TILE_SIZE * 2 + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Top = TILE_SIZE * 2;
                        }


                        x = ((PictureBox)item).Left;
                        y = ((PictureBox)item).Top;
                    }

                    debugLog1.Text += x + " " + y + "       ";

                    j++;
                    if (x < 0 || x >= child.Width - TILE_SIZE +1 || y < 0 || y >= child.Height - TILE_SIZE+1)
                    {
                        i++;
                        debugLog2.Text += item.Left + " " + item.Top + "   " + ((child.Width - TILE_SIZE + 1).ToString()) + " " + ((child.Height - TILE_SIZE + 1).ToString()) + "\n";
                        toBeRemoved.Add(item);
                    }
                }

            }
            debugLog2.Text += i;
            debugLog1.Text += j;
            foreach (Control item in toBeRemoved)
            {
                
                child.Controls.Remove(item);
                ((PictureBox)item).Dispose();

            }
            if (!left)
            {
                drawBiomsAround(players[1]);

            }
            else
            {
                drawBiomsAround(players[0]);

            }
         

        }

        private PictureBox createPictureBox(Entity entity, int x, int y, bool left)
        {
            PictureBox pb = new PictureBox();       
            pb.Image = resize(entity.sprite, entity.width, entity.width);
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Height = entity.height;
            pb.Width = entity.width;
            pb.Top = y * TILE_SIZE + TILE_SIZE / 2 - TILE_SIZE / (2 * TILE_SIZE / entity.height);
            pb.Left = x * TILE_SIZE + TILE_SIZE / 2 - TILE_SIZE / (2 * TILE_SIZE / entity.width);
          
            if (left)
            {
                pb.Tag = "left";
            }
            else
            {
                pb.Tag = "right";
            }


            return pb;

        }

        public Point OutOfBounds(int x, int y)
        {

            if (x< 0)
            {
                x = mapWidth + x;
            }
            else if (x> mapWidth - 1)
            {
                x = x- mapWidth;
            }

            if (y < 0)
            {
                y = mapHeight + y;
            }
            else if (y > mapHeight - 1)
            {
                y = y - mapHeight;
            }
            return new Point(x, y);
        }

        void drawBiomsAround(Player player)
        {

            int playerX = player.posX;
            int playerY = player.posY;
           
            Point mapPoint = new Point();
       
            {
                int y = -1;
                for (int x = -1; x < 3 + 1; x++)
                {
                    
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.getOnLeft(), x, y, mapPoint.X, mapPoint.Y); 
                }
            }
      
            for (int y = 0; y < 3; y++)
            {
                for (int x = -1; x < 3 + 1; x+= 3 + 1)
                {
                
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.getOnLeft(), x, y, mapPoint.X, mapPoint.Y); 
                }
            }
        
            {
                int y = 3;
                for (int x = -1; x < 3 + 1; x++)
                {
              
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.getOnLeft(), x, y, mapPoint.X, mapPoint.Y);
                }
            }


         

        }
        private void spawnItem(bool left, int x, int y)
        {

        }
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
       
            leftKeyboard = false;
            if(e.KeyCode == Keys.Left)
            {
                move(-1,0 , leftKeyboard);
          
            }
            else if(e.KeyCode == Keys.Right)
            {
                move(1, 0, leftKeyboard);
           
            }
            else if (e.KeyCode == Keys.Up)
            {
                move(0, -1, leftKeyboard);
         
            }
            else if (e.KeyCode == Keys.Down)
            {
                move(0, 1, leftKeyboard);
                
            }
            else if (e.KeyCode == Keys.P)
            {
    
                PrimarySkill(leftKeyboard);
            }
            else if (e.KeyCode == Keys.O)
            {
                SecondarySkill(leftKeyboard);
            }
            else
            {
                leftKeyboard = true;

                if (e.KeyCode == Keys.A)
                {
                    move(-1, 0, leftKeyboard);


                }
                else if (e.KeyCode == Keys.D)
                {
                    move(1, 0, leftKeyboard);

                }
                else if (e.KeyCode == Keys.W)
                {
                    move(0, -1, leftKeyboard);

                }
                else if (e.KeyCode == Keys.S)
                {
                    move(0, 1, leftKeyboard);

                }
                else if (e.KeyCode == Keys.F)
                {
                    PrimarySkill(leftKeyboard);
                }
                else if (e.KeyCode == Keys.E)
                {
                    SecondarySkill(leftKeyboard);
                }


            }

           
        }

        private void move(int deltaX, int deltaY, bool leftPlayer)
        {
        

            int id = 1;       
            if (leftPlayer)
            {
                id = 0;
            }
   

            if (players[id].movingX != 0 || players[id].movingY != 0)
            {
                return;
            }

            

            int playerX = players[id].getX();
            int playerY = players[id].getY();

           
            int mapX = playerX + deltaX * 2;
            int mapY = playerY + deltaY * 2;
            if (mapX < 0)
            {
                mapX = mapWidth + mapX;
            }
            else if (mapX > mapWidth - 1)
            {
                mapX = mapX - mapWidth;
            }

            if (mapY < 0)
            {
                mapY = mapHeight + mapY;
            }
            else if (mapY > mapHeight - 1)
            {
                mapY = mapY - mapHeight;
            }


            string tag;
            if (leftPlayer)
            {
                tag = "leftTile";
            }
            else
            {
                tag = "rightTile";

            }

            
            //drawTile(leftPlayer, deltaX * 2 + 1, deltaY * 2 + 1, mapX, mapY); // infront
            //drawTile(gameMap[mapY, mapX].getTexture(), leftPlayer, deltaX * 2 + 1, deltaY * 2 + 1, tag); // infront left
            //drawTile(gameMap[mapY, mapX].getTexture(), leftPlayer, deltaX * 2 + 1, deltaY * 2 + 1, tag); // infront right

            deltaX *= -1;
            deltaY *= -1;

            players[id].movingX = 0;
            players[id].movingY = 0;
            players[id].movingX = deltaX;
            players[id].movingY = deltaY;

        }

        private void PrimarySkill(bool leftPlayer)
        {
 
            int id;
            if (leftPlayer)
            {
                id = 0;
            }
            else
            {
                id = 1;
            }

            if (players[id].getHider())
            {

            }
            else
            {
            
                if (players[(id + 1) % 2].posX == players[id].posX && players[(id + 1) % 2].posY == players[id].posY)
                {
            
                    EndGame(leftPlayer, "Seeker found the hider");
                    
                }
            }
        }

        private void SecondarySkill(bool leftPlayer)
        {
            int id;
            if (leftPlayer)
            {
                id = 0;
            }
            else
            {
                id = 1;
            }

            if (players[id].getHider())
            {

            }
            else
            {
              
            }

        }

        private void EndGame(bool leftWon, string reason = "")
        {
            gameTimer.Stop();
         
            this.Controls.Clear();

            Label winner = new Label();       
            winner.Location = new Point(50, 200);
            winner.Height = 500;
            winner.Width = 450;
            winner.BorderStyle = BorderStyle.Fixed3D;
            winner.Font = new Font("Calibri", 30);
            winner.Padding = new Padding(6);
          
          
            if (leftWon)
            {           
                winner.Text = "Left player won\n" + reason;
                players[0].gamesWon += 1;
            }
            else
            {
                winner.Text = "Right player won\n" + reason;
                players[1].gamesWon += 1;
            }

            Button restartBtn = new Button();
            restartBtn.Location = new Point(this.Width/2, this.Height/2);
            restartBtn.Text = "Restart";
            restartBtn.AutoSize = true;
            restartBtn.BackColor = Color.LightBlue;
            restartBtn.Padding = new Padding(6);
            restartBtn.Font = new Font("French Script MT", 18);
            restartBtn.Click += new EventHandler(RestartGame);

            Button exitBtn = new Button();
            exitBtn.Location = new Point(this.Width / 2, (int)(this.Height / 1.7f));
            exitBtn.Text = "Exit";
            exitBtn.AutoSize = true;
            exitBtn.BackColor = Color.LightBlue;
            exitBtn.Padding = new Padding(6);
            exitBtn.Font = new Font("French Script MT", 18);

            this.Controls.Add(winner);
            this.Controls.Add(exitBtn);
            this.Controls.Add(restartBtn);
        }

        private void RestartGame(object sender, EventArgs e)
        {
            InitGame(mapWidth, mapHeight, !players[0].getHider());
        }
        private void GameLoop(object sender, EventArgs e)
        {
            foreach (Player player in players)
            {
                if(player.movingX != 0 || player.movingY != 0)
                {
                    Panel panel = rightPanel;
                    if (player.getOnLeft())
                    {
                        panel = leftPanel;
                    }

                    foreach (Control item in panel.Controls.OfType<PictureBox>())
                    {
                        if ((string)item.Tag == "left" || (string)item.Tag == "right")
                        {
                            ((PictureBox)item).Left += player.movingX * (int)(SPEED * TILE_SIZE);
                            ((PictureBox)item).Top += player.movingY * (int)(SPEED * TILE_SIZE);
                        }

                        
                    }

                    player.distanceTraveled++;
                    if (player.distanceTraveled == 1/SPEED)
                    {
                        player.posX += player.movingX * -1;
                        player.posY += player.movingY * -1;
                        if (player.posX < 0) 
                        {
                            player.posX = mapWidth - 1;
                        }
                        else if (player.posX > mapWidth)
                        {
                            player.posX = 0;
                        }

                        if (player.posY < 0)
                        {
                            player.posY = mapHeight - 1;
                        }
                        else if (player.posY > mapHeight)
                        {
                            player.posY = 0;
                        }
                    }
                    if (player.distanceTraveled == 1 / SPEED) // end anim
                    {
                        player.distanceTraveled = 0;
                        player.movingX = 0;
                        player.movingY = 0;


                        endAnimation(player.getOnLeft());

                      



                    }



                }

             
                
            }
        }

     
    }
}
