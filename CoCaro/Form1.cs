using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager chessBoard;
        SocketManager socketManager;
        #endregion
        #region Methods
        #region Solve
        void endGame()
        {
            int i = 1;
            if (chessBoard.CurrentPlayer == 0)
                i = 2;
            tmCountDown.Stop();
            pnlChessBoard.Enabled = false;
            MessageBox.Show("Người chơi " + i + " chiến thắng");
        }
        void newGame()
        {
            prcbCountDown.Value = 0;
            tmCountDown.Stop();
            chessBoard.DrawChessBoard();
        }
        void Quit()
        {
                Application.Exit();
        }
        void Undo()
        {

        }
        private void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmCountDown.Start();
            prcbCountDown.Value = 0;
        }
        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            endGame();
        }
        void listen()
        {
            SocketData data =(SocketData) socketManager.Receive();
            ProcessData(data);
        }
        void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NOTIFY:
                    break;
                case (int)SocketCommand.NEW_GAME:
                    break;
                case (int)SocketCommand.SEND_POINT:
                    break;
                case (int)SocketCommand.QUIT:
                    break;
                default:
                    break;
            }
                
        }
        #endregion
        #region UI
        public Form1()
        {
            
            InitializeComponent();
            txbPlayerName.Enabled = false;
            prcbCountDown.Step = Cons.cdStep;
            prcbCountDown.Maximum = Cons.cdTime;
            prcbCountDown.Value = 0;
            tmCountDown.Interval = Cons.cdInterval;
            socketManager = new SocketManager();
            chessBoard = new ChessBoardManager(pnlChessBoard,txbPlayerName,pctbMark,tmCountDown);
            chessBoard.EndedGame += ChessBoard_EndedGame;
            chessBoard.PlayerMarked += ChessBoard_PlayerMarked;
            newGame();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pctbAvatar_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tmCountDown_Tick(object sender, EventArgs e)
        {
            prcbCountDown.PerformStep();
            if (prcbCountDown.Value >= prcbCountDown.Maximum)
                endGame();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame();
        }
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát không? ", "Thông báo", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;
        }
        #endregion

        #endregion

        private void btnLan_Click(object sender, EventArgs e)
        {
            socketManager.IP = txbIP.Text;
            if(!socketManager.connect())
            {
                socketManager.creatServer();
                Thread listenThread = new Thread(() =>
                {
                    while(true)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            listen();
                            break;
                        }
                        catch 
                        {
                        }
                    }    
                });
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            else
            {
                Thread listenThread = new Thread(() =>
                {
                    listen();
                });
                listenThread.IsBackground = true;
                listenThread.Start();
                socketManager.Send(new SocketData((int)SocketCommand.NOTIFY,"Client đã kết nối",null));
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txbIP.Text = socketManager.getLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (string.IsNullOrEmpty(txbIP.Text))
            {
                txbIP.Text = socketManager.getLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }
    }
}

