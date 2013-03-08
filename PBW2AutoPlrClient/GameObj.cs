using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBW2AutoPlrClient
{
    class GameObject
    {
        ///<summery>
        ///holds info for each game in the games list
        ///</summery>
        ///
        public string GameName { get; set; }
        public string GameType { get; set; }
        public string GameMod { get; set; }
        public string GameModVer { get; set; }
        public string GameTurnMode { get; set; }
        public int GameTurnNum { get; set; }
        public DateTime GameStartDate { get; set; }
        public DateTime GameNextTurn { get; set; }
        public string GamePlrStatus { get; set; }
        public int GamePlrNumber { get; set; }
        public string GamePlrEmpire { get; set; }
        public string GamePlrEmpPassword { get; set; }
        public string GamePlrShipset { get; set; }
        public string GamePlrShipsetCode { get; set; }

        public GameObject(List<string> gamedata)
        {
            /// <summary>
            /// fills the object with data
            /// </summary>
            /// <param name="gamedata">a list of data which include xml tags</param>
            /// <returns>
            /// </returns>
            /// <remarks>
            /// Todo: 
            /// handle errors and exceptions,
            /// make date work.
            /// </remarks>

            foreach (string line in gamedata)
            {
                string breadtop = null;
                string breadbottom = null;
                string filling = null;
                char[] delimiters = new char[] { '<', '>' };
                //List<string> sandwitch = new List<string>();
                string[] sandwitch = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                breadtop = sandwitch[0].Trim();
                if (sandwitch.Length == 2)
                {
                    breadbottom = sandwitch[1].Trim();
                }
                else if (sandwitch.Length == 3)
                {
                    filling = sandwitch[1].Trim();
                    breadbottom = sandwitch[2].Trim();
                }
               
                if (breadtop == "game_code")
                {
                    GameName = filling;
                }
                if (breadtop == "game_type")
                {
                    GameType = filling;
                    
                }
                if (breadtop == "mod_code")
                {
                    GameMod = filling;
                    
                }
                if (breadtop == "mod_version")
                {
                    GameModVer = filling;
                    
                }
                if (breadtop == "turn_mode")
                {
                    GameTurnMode = filling;
                    
                }
                if (breadtop == "turn")
                {

                    GameTurnNum = Int32.Parse(filling);
                }
                if (breadtop == "turn_start_date")
                {

                    GameStartDate = UnixTimeStampToDateTime(Convert.ToDouble(filling));
                }
                if (breadtop == "next_turn_date")
                {

                    GameNextTurn = UnixTimeStampToDateTime(Convert.ToDouble(filling));
                }
                if (breadtop == "plr_status")
                {

                    GamePlrStatus = filling;
                }
                if (breadtop == "number")
                {

                    GamePlrNumber = Int32.Parse(filling);
                }
                if (breadtop == "empire_name")
                {

                    GamePlrEmpire = filling;
                }
                if (breadtop == "empire_password")
                {

                    GamePlrEmpPassword = filling;
                }
                if (breadtop == "shipset")
                {

                    GamePlrShipset = filling;
                }
                if (breadtop == "shipset_code")
                {
                    GamePlrShipsetCode = filling;
                }
            }  
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
