using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public class ChessBoardManager
    {
        #region Properties
        private Panel chessBoard;
        public Panel ChessBoard { get => chessBoard; set => chessBoard = value; }
        internal List<Player> Players { get => players; set => players = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public TextBox PlayerName { get => playerName; set => playerName = value; }
        public PictureBox PlayerMark { get => playerMark; set => playerMark = value; }
        public Button[,] Buttons { get => buttons; set => buttons = value; }
        private List<Player> players;
        private int currentPlayer;
        TextBox playerName;
        PictureBox playerMark;
        private Button[,] buttons;
        private Timer timer;
        private event EventHandler playerMarked;
        public event EventHandler PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }
        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }
        #endregion
        #region Initialize
        public ChessBoardManager(Panel chessBoard,TextBox playerName,PictureBox mark,Timer timer)
        {
            this.ChessBoard = chessBoard;
            this.players = new List<Player>() { 
            new Player("Người chơi 1",Image.FromFile(Application.StartupPath+"\\Resources\\anhX.png")),
            new Player("Người chơi 2",Image.FromFile(Application.StartupPath+"\\Resources\\anhO.png"))
            };
            this.timer = timer;
            this.playerName = playerName;
            this.PlayerMark = mark;
            buttons = new Button[20, 20];
        }

        #endregion
        #region Methods
        #region Art
        public void DrawChessBoard()
        {
            chessBoard.Enabled = true;
            chessBoard.Controls.Clear();
            currentPlayer = 0;
            ChangePlayer();
            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + Cons.CHESS_WIDTH, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch
                    };
                    btn.Click += btn_Click;
                    chessBoard.Controls.Add(btn);
                    buttons[i,j] = btn;
                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }
        private void btn_Click(object sender, EventArgs e)
        {
            if (!timer.Enabled)
                timer.Start();
            Button btn = sender as Button;
            int x = (btn.Location.X / Cons.CHESS_WIDTH) - 1;
            int y = btn.Location.Y / Cons.CHESS_HEIGHT;
            if (btn.BackgroundImage != null)
                return;
            if (playerMarked != null)
                playerMarked(this, new EventArgs());
            ChangeMark(btn);
            ChangePlayer();
            check(x, y, btn);
        }
        private void ChangeMark(Button btn)
        {
            btn.BackgroundImage = Players[currentPlayer].Mark;
            currentPlayer = currentPlayer == 1 ? 0 : 1;
        }
        private void ChangePlayer()
        {
            PlayerName.Text = Players[currentPlayer].Name;
            PlayerMark.Image = Players[currentPlayer].Mark;
        }
        #endregion
        #region GamePlay
        public void endGame()
        {
            if (endedGame != null)
                endedGame(this,new EventArgs());
        }
        private void check(int x,int y,Button btn)
        {
            checkDoc(y, x, btn);
            checkNgang(y, x, btn);
            checkCheoChinh(y, x, btn);
            checkCheoPhu(y, x, btn);
        }
        private bool demDoc(int i,int j, Button btn)
        {
            if (i + 3 >= Cons.CHESS_BOARD_HEIGHT-1)
            {
                return false;
            }
            int dem = 4;
            if (buttons[i + 4, j].BackgroundImage != null && btn.BackgroundImage != buttons[i + 4, j].BackgroundImage)
                return false;
            if (i > 0)
            {
                if (buttons[i - 1, j].BackgroundImage != null && btn.BackgroundImage != buttons[i - 1, j].BackgroundImage)
                    dem = 5;
            }
            for (int k = i; k < i+dem; k++)
            {
                if (buttons[k, j].BackgroundImage!=btn.BackgroundImage)
                {
                    return false;
                }
            }
            return true;
        }
        private void checkDoc(int x,int y,Button btn)
        {
            int hang = 0;
            if (x > 3)
                hang = x - 3;
            for (int i = hang; i < hang + 4; i++)
            {
                if (buttons[i, y].BackgroundImage == btn.BackgroundImage)
                {
                    if (demDoc(i, y, btn))
                        endGame();
                }
            }
            
        }
        private bool demNgang(int i, int j, Button btn)
        {
            if (j + 3 >= Cons.CHESS_BOARD_HEIGHT-1)
            {
                return false;
            }
            int dem = 4;
            if (buttons[i , j+4].BackgroundImage != null && btn.BackgroundImage != buttons[i , j+4].BackgroundImage)
                return false;
            if (j > 0)
            {
                if (buttons[i , j-1].BackgroundImage != null && btn.BackgroundImage != buttons[i , j-1].BackgroundImage)
                    dem = 5;
            }
            for (int k = j; k < j + dem; k++)
            {
                if (buttons[i, k].BackgroundImage != btn.BackgroundImage)
                {
                    return false;
                }
            }
            return true;
        }
        private void checkNgang(int x, int y, Button btn)
        {
            int cot = 0;
            if (y > 3)
                cot = y - 3;
            for (int i = cot; i < cot + 4; i++)
            {
                if (buttons[x, i].BackgroundImage == btn.BackgroundImage)
                {
                    if (demNgang(x, i, btn))
                        endGame();
                }
            }
        }
        private bool demCheoChinh(int i, int j, Button btn)
        {
            if (j + 3 >= Cons.CHESS_BOARD_HEIGHT-1 || i+3>=Cons.CHESS_BOARD_HEIGHT-1)
            {
                return false;
            }
            int dem = 4;
            if (buttons[i+4, j + 4].BackgroundImage != null && btn.BackgroundImage != buttons[i+4, j + 4].BackgroundImage)
                return false;
            if (j > 0 && i>0)
            {
                if (buttons[i-1, j - 1].BackgroundImage != null && btn.BackgroundImage != buttons[i-1, j - 1].BackgroundImage)
                    dem = 5;
            }
            int row = i, col = j;
            while (row <i+dem && col <j+dem)
            {
                if (buttons[row, col].BackgroundImage != btn.BackgroundImage)
                {
                    return false;
                }
                row++;
                col++;
            }
            return true;
        }
        private void checkCheoChinh(int x, int y, Button btn)
        {
            int cot = 0,hang=0;
            if (y > 3)
                cot = y - 3;
            if (x > 3)
                hang = x - 3;
            int i = hang, j = cot;
            while(i<hang+4 && j < cot+4)
            {
                if (buttons[i, j].BackgroundImage == btn.BackgroundImage)
                {
                    if (demCheoChinh(i, j, btn))
                        endGame();
                }
                i++;
                j++;
            }
        }
        private bool demCheoPhu(int i, int j, Button btn)
        {
            if (j - 3 < 0 || i + 3 >= Cons.CHESS_BOARD_HEIGHT-1)
            {
                return false;
            }
            int dem = 4;
            if (j > 3)
            {
                if (buttons[i + 4, j - 4].BackgroundImage != null && btn.BackgroundImage != buttons[i + 4, j - 4].BackgroundImage)
                    return false;
            }
            if (i > 0 && j <Cons.CHESS_BOARD_HEIGHT-2)
            {
                if (buttons[i - 1, j + 1].BackgroundImage != null && btn.BackgroundImage != buttons[i - 1, j + 1].BackgroundImage)
                    dem = 5;
            }
            int row = i, col = j;
            while (row < i + dem && col >=0)
            {
                if (buttons[row, col].BackgroundImage != btn.BackgroundImage)
                {
                    return false;
                }
                row++;
                col--;
            }
            return true;
        }
        private void checkCheoPhu(int x, int y, Button btn)
        {
            int cot = y+3, hang = 0;
            if (y+3 > Cons.CHESS_BOARD_HEIGHT-1)
                cot = Cons.CHESS_BOARD_HEIGHT-1;
            if (x > 3)
                hang = x - 3;
            int i = hang, j = cot;
            while (i < hang + 4 && j >0)
            {
                if (buttons[i, j].BackgroundImage == btn.BackgroundImage)
                {
                    if (demCheoPhu(i, j, btn))
                        endGame();
                }
                i++;
                j--;
            }
        }
        #endregion
        #endregion
    }
}
