using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoCaro
{
    class SocketData
    {
        private int command;
        private Point? point;
        private string message;
        public SocketData(int command,string message, Point? point)
        {
            this.command = command;
            this.point = point;
            this.message = message;
        }

        public int Command { get => command; set => command = value; }
        public Point? Point { get => point; set => point = value; }
        public string Message { get => message; set => message = value; }
    }
    public enum SocketCommand
    {
        SEND_POINT,
        NOTIFY,
        NEW_GAME,
        QUIT
    }
}
