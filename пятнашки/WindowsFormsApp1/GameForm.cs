using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class GameForm : Form
    {
        private string playerName;
        private PictureBox[] tiles = new PictureBox[16];
        private int[] boardState = new int[16];
        private int emptyTileIndex = 15;
        private Image[] tileImages = new Image[16];
        private Stopwatch gameTimer = new Stopwatch();

        public GameForm(string name)
        {
            InitializeComponent();
            playerName = name;

            this.ClientSize = new Size(400, 400);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            LoadResources();
            InitializeBoard();
            ShuffleBoard();
            UpdateUI();

            gameTimer.Start();
        }

        private void LoadResources()
        {
            var resManager = WindowsFormsApp1.Properties.Resources.ResourceManager;
            for (int i = 0; i <= 15; i++)
            {
                tileImages[i] = (Image)resManager.GetObject($"_{i}");
            }
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 16; i++)
            {
                boardState[i] = i;

                PictureBox pb = new PictureBox();
                pb.Width = this.ClientSize.Width / 4;
                pb.Height = this.ClientSize.Height / 4;
                pb.Left = (i % 4) * pb.Width;
                pb.Top = (i / 4) * pb.Height;
                pb.BorderStyle = BorderStyle.FixedSingle;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Click += Tile_Click;

                tiles[i] = pb;
                this.Controls.Add(pb);
            }
        }

        private void ShuffleBoard()
        {
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int emptyRow = emptyTileIndex / 4;
                int emptyCol = emptyTileIndex % 4;

                int direction = rnd.Next(4);
                int moveTo = -1;

                if (direction == 0 && emptyRow > 0) moveTo = emptyTileIndex - 4;
                else if (direction == 1 && emptyRow < 3) moveTo = emptyTileIndex + 4;
                else if (direction == 2 && emptyCol > 0) moveTo = emptyTileIndex - 1;
                else if (direction == 3 && emptyCol < 3) moveTo = emptyTileIndex + 1;

                if (moveTo != -1)
                {
                    SwapTiles(emptyTileIndex, moveTo);
                    emptyTileIndex = moveTo;
                }
            }
        }

        private void Tile_Click(object sender, EventArgs e)
        {
            PictureBox clickedTile = (PictureBox)sender;
            int clickedIndex = (int)clickedTile.Tag;

            if (IsAdjacent(clickedIndex, emptyTileIndex))
            {
                SwapTiles(clickedIndex, emptyTileIndex);

                emptyTileIndex = clickedIndex;

                UpdateUI();
                CheckWin();
            }
        }

        private bool IsAdjacent(int index1, int index2)
        {
            int r1 = index1 / 4, c1 = index1 % 4;
            int r2 = index2 / 4, c2 = index2 % 4;
            return Math.Abs(r1 - r2) + Math.Abs(c1 - c2) == 1;
        }

        private void SwapTiles(int index1, int index2)
        {
            int temp = boardState[index1];
            boardState[index1] = boardState[index2];
            boardState[index2] = temp;
        }

        private void UpdateUI()
        {
            for (int i = 0; i < 16; i++)
            {
                int pieceIndex = boardState[i];

                tiles[i].Tag = i;

                if (pieceIndex == 15)
                {
                    tiles[i].Image = null;
                    tiles[i].BackColor = Color.White;
                }
                else
                {
                    tiles[i].Image = tileImages[pieceIndex];
                    tiles[i].BackColor = Color.Transparent;
                }
            }
        }

        private void CheckWin()
        {
            for (int i = 0; i < 16; i++)
            {
                if (boardState[i] != i) return;
            }

            GameWon();
        }

        private void GameWon()
        {
            gameTimer.Stop();
            TimeSpan elapsed = gameTimer.Elapsed;
            string timeString = $"{elapsed.Minutes:00}:{elapsed.Seconds:00}";

            string logEntry = $"победа {playerName} {timeString}\n";
            File.AppendAllText("stats.txt", logEntry);

            MessageBox.Show($"Поздравляем, {playerName}! Вы собрали пазл за {timeString}.", "Победа!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            tiles[15].Image = tileImages[15];
            tiles[15].BackColor = Color.Transparent;

            foreach (var tile in tiles)
            {
                tile.Click -= Tile_Click;
            }
        }
    }
}