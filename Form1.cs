using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDP_AnimatedMovement
{
    public partial class Form1 : Form
    {//form attributes
        Image player; //holds an image of a player sprite
        bool goLeft, goRight, goUp, goDown;//identifies which direction the player sprite will move
        int playerX =0;
        int playerY = 0;
        int playerHeight = 100;
        int playerWidth = 100;
        int playerSpeed = 8;
        int steps = 0;
        int slowDownFramerate = 0;
        List<string> playerMovements = new List<string>();

        //items variables
        List<string> item_locations = new List<string>();
        List<Item> item_list = new List<Item>();
        int spawnTimeLimit = 50;
        int timeCounter = 0;
        Random rand = new Random();
        string[] itemNames = { "red sword", "medium armour", "green shoes", "gold lamp",
         "red potion", "fast sword", "instruction manual", "giant sword", "warm jacket",
        "wizards hat", "red bow and arrow", "red spear", "green potion", "heavy armour",
        "cursed axe", "gold ring", "purple ring"};


        public Form1()
        {//constructor
            InitializeComponent();
            this.BackgroundImage = Image.FromFile("bg.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Height = 700;
            this.Width = 1400;
            player = Image.FromFile($"player/character_01.png");
            this.DoubleBuffered = true;

            playerMovements = Directory.GetFiles("player", "*.png").ToList();
            player = Image.FromFile(playerMovements[0]);
            item_locations = Directory.GetFiles("items", "*.png").ToList();
        }


        private void GameLoop_Tick(object sender, EventArgs e)
        {//Game loop that is based on the timer tick event
            CheckCollision();

            //identify the direction and move the player sprite
            if (goLeft && playerX > 0)
            {
                playerX -= playerSpeed;
                AnimatePlayer(4, 7);
                
            }
            else if (goRight && playerX + playerWidth < this.ClientSize.Width)
            {
                playerX += playerSpeed;
                AnimatePlayer(0, 11);

            }
            else if (goUp && playerY > 0)
            {
                playerY -= playerSpeed;
                AnimatePlayer(12, 15);
            }
            else if (goDown && playerY + playerHeight < this.ClientSize.Height)
            {
                playerY += playerSpeed;
                AnimatePlayer(0, 3);
            }
            else
            {
                AnimatePlayer(0, 0);
            }

            this.Invalidate();

            if (timeCounter > 1)
            {
                timeCounter--;
            }
            else
            {
                MakeItems();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {//event procedure activated when a key is released
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
            }
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {//event procedure activated when a key is pressed
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void AnimatePlayer(int start, int end)
        {
            slowDownFramerate += 1;
            if (slowDownFramerate == 4)
            {
                steps++;
                slowDownFramerate = 0;
            }
            if (steps > 1 || steps < start)
            {
                steps = start;
            }
            player = Image.FromFile(playerMovements[steps]);
        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics Canvas = e.Graphics;
            Canvas.DrawImage(player, playerX, playerY, playerWidth, playerHeight);
            
            if (item_list != null)
            {
                foreach (Item newItem in item_list)
                {
                    Canvas.DrawImage(newItem.getItemImage(), newItem.getPositionX(), newItem.getPositionY(),
                        newItem.getWidth(), newItem.getHeight());
                }
            }
        }

        private void MakeItems()
        {
            //initiate a new item object with random x and y coordinates and add to list
            int i = rand.Next(0, item_locations.Count);
            Item newItem = new Item(item_locations[i]);
            newItem.setName(itemNames[i]);
            timeCounter = spawnTimeLimit;
            item_list.Add(newItem);
        }

        private bool DetectCollision(int object1X, int object1Y, int object1Width, int object1Height, int object2X, int object2Y, int object2Width, int object2Height)
        {
            if (object1X + object1Width <= object2X || object1X >= object2X + object2Width || object1Y + object1Height <= object2Y || object1Y >= object2Y + object2Height)
            { return false; }
            else
            { return true; }
        }

        private void CheckCollision()
        {
            foreach(Item item in item_list.ToList())
            {
                item.CheckLifeTime();
                if (item.getExpired())
                {
                    item.setImage("null");
                    item_list.Remove(item);
                }
                bool collision = DetectCollision(playerX, playerY, player.Width, player.Height, item.getPositionX(), item.getPositionY(), item.getWidth(), item.getHeight());
                if (collision)
                {
                    lblCollected.Text = "Collected the: " + item.getName();
                    item.setImage("null");
                    item_list.Remove(item);
                }
            }
        }
    }

    internal class Item
    {
        private int positionX;
        private int positionY;
        private Image item_image;
        private int height;
        private int width;
        private string name;

        private Random range = new Random();
        private int lifeTime = 200;
        private bool expired = false;

       

        public Item(string itemImage)
        {
            this.item_image = Image.FromFile(itemImage);
            this.positionX = range.Next(10, 1370);
            this.positionY = range.Next(10, 670);
            this.height = 50;
            this.width = 50;
        }

        public int getPositionX() { return this.positionX; }
        public int getPositionY() { return this.positionY; }
        public int getHeight() { return this.height;}
        public int getWidth() { return this.width;}
        public Image getItemImage() { return this.item_image; }
        public string getName() { return this.name; }
        public bool getExpired() { return this.expired; }
        public void setImage(string filename)
        {
            if (filename == null) { this.item_image = Image.FromFile(filename); }
            else { this.item_image = null; }
        }
        public void setName(string newName) { this.name = newName; }

        public void CheckLifeTime()
        {
            lifeTime--;
            if (lifeTime < 1)
            {
                expired = true;
            }
        }
       

        
    }
}
