using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PBW2AutoPlrClient
{
    class XmlHandler
    {

        public static XDocument LoadFile(string filename)
        {
            XDocument xmlDoc = XDocument.Load(filename);
            return xmlDoc;
        }

        public static Dictionary<string, string> ServerSettings(string xmldata)
        {
            XDocument doc;
            doc = XDocument.Parse(xmldata);
            Dictionary<string, string> serversettings = new Dictionary<string, string>();
            XContainer root = doc.Root;
            XElement element = XElement.Parse(xmldata);
            
            if (element.FirstAttribute != null)
            {
                serversettings.Add(element.FirstAttribute.Name.ToString(), element.FirstAttribute.Value);
                serversettings.Add(element.LastAttribute.Name.ToString(), element.LastAttribute.Value);
            }
            return serversettings;
        }

        public static Dictionary<string, GameTypeSettingsObj> GetGameSettings(string filename)
        {
            XDocument doc = LoadFile(filename);
            
            Dictionary<string, GameTypeSettingsObj> dicofgamesettings = new Dictionary<string, GameTypeSettingsObj>();
            
            XContainer thisContainerRoot = doc.Root;

            foreach (var xnode in thisContainerRoot.Elements())
            {
                string gametypeName = xnode.Name.ToString();
                GameTypeSettingsObj thisgamesettingobj = new GameTypeSettingsObj(); 
                dicofgamesettings.Add(gametypeName, thisgamesettingobj);
                thisgamesettingobj.GameTypeName = gametypeName;
                foreach (var gameElement in xnode.Elements())
                {
                    string gameNodeName = gameElement.Name.ToString();

                    if (gameNodeName == "gameExe")
                    {
                        thisgamesettingobj.GameExe = gameElement.Value.Trim();
                    }
                    else if (gameNodeName == "workingDirectory")
                    {
                        thisgamesettingobj.WorkingDirectory = gameElement.Value.Trim();
                    }
                    else if (gameNodeName == "gameArgs")
                    {
                        thisgamesettingobj.GameArguments = gameElement.Value.Trim();
                    }
                    else if (gameNodeName == "preScriptPath")
                    {
                        thisgamesettingobj.GamePreScript = gameElement.Value.Trim();
                    }
                    else if (gameNodeName == "postScriptPath")
                    {
                        thisgamesettingobj.GamePostScript = gameElement.Value.Trim();
                    }
                    else if (gameNodeName == "uploadFileMask")
                    {
                        thisgamesettingobj.GameUploadFileMask = gameElement.Value.Trim();
                    }
                    else if (gameNodeName == "MODS" || gameNodeName == "mods")
                    {

                        foreach (var modElement in gameElement.Elements())
                        {
                            string modNodeName = modElement.Name.ToString();
                            ModSettingsObj thismodsettingobj = new ModSettingsObj();
                            thismodsettingobj.ModName = modNodeName;

                            foreach (var moddataelemnt in modElement.Elements())
                            {
                                string moddataNodeName = moddataelemnt.Name.ToString();
                                if (moddataNodeName == "modPath")
                                {
                                    thismodsettingobj.ModPath = moddataelemnt.Value.Trim();
                                }
                                if (moddataNodeName == "savePath")
                                {
                                    thismodsettingobj.ModSavePath = moddataelemnt.Value.Trim();
                                }
                            }

                            thisgamesettingobj.GameMods.Add(modNodeName, thismodsettingobj);

                        }

                    }

                }

            }
            return dicofgamesettings;
            
         }

        /// <summary>
        /// turns game xml data from pbw, cuts it into seperate games and creates a gamesObject.
        /// </summary>
        /// <param name="xmlstring">xmlstring from PBW</param>
        /// <param name="dictionaryofgameobjects">dictionary of exsisting gameobjects</param>
        /// <returns>
        /// </returns>
        /// a dictionary of game objects.
        /// <remarks>
        /// 
        /// </remarks>
        public static Dictionary<string, GameObject> XmlToGameObj(string xmlstring, Dictionary<string, GameObject> dictionaryofgameobjects)
        {
            List<List<string>> listofgames = new List<List<string>>();

            string[] lines = xmlstring.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            int boxindex = 0;
            bool dataIsGame = false;
            foreach (string line in lines)
            {
                //List<string> thisgame = new List<string>();
                string lineTrim = line.Trim();

                if (lineTrim == @"</game>")
                {
                    dataIsGame = false;
                    GameObject thisgameobj = new GameObject(listofgames[boxindex]);
                    if (dictionaryofgameobjects.ContainsKey(thisgameobj.GameName))
                    {
                        dictionaryofgameobjects[thisgameobj.GameName] = thisgameobj;
                    }
                    else
                    {
                        dictionaryofgameobjects.Add(thisgameobj.GameName, thisgameobj);
                    }
                    boxindex += 1;
                }
                if (dataIsGame)
                {
                    listofgames[boxindex].Add(lineTrim);
                }

                if (lineTrim == "<game>")
                {
                    dataIsGame = true;
                    listofgames.Add(new List<string>());
                }
            }
            return dictionaryofgameobjects;
        }

    }
}
