using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLTK.Models
{
    public class Server
    {
        //Green 01:103.48.194.46:14445:0,Blue 01:103.48.194.146:14445:0,Blue 02:103.48.194.152:14445:0,Blue 03:45.119.81.28:14445:0,Blue 04:45.119.81.51:14445:0,Blue 05:103.48.194.173:14445:0,Blue 06:103.48.194.137:14445:0,0,0
        public static Server[] servers = new Server[]
        {
            new Server("Vũ Trụ 1", "dragon1.teamobi.com", 14445),
            new Server("Vũ Trụ 2", "dragon2.teamobi.com", 14445),
            new Server("Vũ Trụ 3", "dragon3.teamobi.com", 14445),
            new Server("Vũ Trụ 4", "dragon4.teamobi.com", 14445),
            new Server("Vũ Trụ 5", "dragon5.teamobi.com", 14445),
            new Server("Vũ Trụ 6", "dragon6.teamobi.com", 14445),
            new Server("Vũ Trụ 7", "dragon7.teamobi.com", 14445),
            new Server("Vũ Trụ 8", "dragon8.teamobi.com", 14445),
            new Server("Vũ Trụ 9", "dragon9.teamobi.com", 14445),
            new Server("Vũ Trụ 10", "dragon10.teamobi.com", 14445),
        };
        public string name { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public int language { get; set; }

        public Server(string name, string ip, int port)
        {
            this.name = name;
            this.ip = ip;
            this.port = port;
            this.language = 0;
        }
        public Server(string name, string ip, int port, int language)
        {
            this.name = name;
            this.ip = ip;
            this.port = port;
            this.language = language;
        }
        
        public override string ToString()
            => name;
        
    }
}
