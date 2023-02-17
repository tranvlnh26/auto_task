using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TranVinh.Helper;

namespace TranVinh.Xmap
{
    public class XmapController : IActionListener
    {
        const int TIME_DELAY_NEXTMAP = 300;
        const int TIME_DELAY_RENEXTMAP = 500;
        const int ID_ITEM_CAPSULE_VIP = 194;
        const int ID_ITEM_CAPSULE = 193;
        const int ID_ICON_ITEM_TDLT = 4387;

        static readonly XmapController _Instance = new XmapController();

        static int IdMapEnd;
        static List<int> WayXmap;
        static int IndexWay;
        static bool IsNextMapFailed;
        static bool IsWait;
        static long TimeStartWait;
        static long TimeWait;
        static bool IsWaitNextMap;

        public static long delayTele;
        public static void MoveMyChar2(int x, int y)
        {
            global::Math.abs((global::Char.myCharz().cx - x) % 48);
            int num = global::Math.abs((global::Char.myCharz().cx - x) / 48);
            int num2 = 0;
            if (global::Char.myCharz().cx > x)
            {
                num2 = -1;
            }
            if (global::Char.myCharz().cx < x)
            {
                num2 = 1;
            }
            for (int i = 0; i <= num; i++)
            {
                global::Char.myCharz().cy = Utils.getYGround(global::Char.myCharz().cx + num2 * 48);
                global::Char.myCharz().cx = global::Char.myCharz().cx + num2 * 48;
                Service.gI().charMove();
            }
            global::Char.myCharz().cx = x;
            global::Char.myCharz().cy = y;
            Service.gI().charMove();
            delayTele = mSystem.currentTimeMillis() + (long)(num * 100);
        }
        internal static void Update()
        {
            try
            {
                if (IsWaiting())
                    return;

                if (XmapData.Instance().IsLoading)
                    return;

                if (IsWaitNextMap)
                {
                    Wait(TIME_DELAY_NEXTMAP);
                    IsWaitNextMap = false;
                    return;
                }

                if (IsNextMapFailed)
                {
                    XmapData.Instance().MyLinkMaps = null;
                    WayXmap = null;
                    IsNextMapFailed = false;
                    return;
                }

                if (TileMap.mapID == 21 + Char.myCharz().cgender)
                {
                    var it = GameScr.vItemMap.elementAt(0) as ItemMap;
                    if (it != null)
                        Service.gI().pickItem(it.itemMapID);
                }

                if (WayXmap == null)
                {
                    if (XmapData.Instance().MyLinkMaps == null)
                    {
                        XmapData.Instance().LoadLinkMaps();
                        return;
                    }
                    WayXmap = XmapAlgorithm.FindWay(TileMap.mapID, IdMapEnd);
                    IndexWay = 0;
                    if (WayXmap == null)
                    {
                        GameScr.info1.addInfo("Không thể tìm thấy đường đi", 0);
                        FinishXmap();
                        return;
                    }
                }

                if ((TileMap.mapID == WayXmap[WayXmap.Count - 1] || TileMap.mapID == IdMapEnd) && !XmapData.IsMyCharDie())
                {
                    FinishXmap();
                    return;
                }

                if (TileMap.mapID == WayXmap[IndexWay])
                {
                    if (XmapData.IsMyCharDie())
                    {
                        Service.gI().returnTownFromDead();
                        IsWaitNextMap = IsNextMapFailed = true;
                    }
                    else if (XmapData.CanNextMap())
                    {
                        NextMap(WayXmap[IndexWay + 1]);
                        IsWaitNextMap = true;
                    }
                    Wait(TIME_DELAY_RENEXTMAP);
                    return;
                }

                if (TileMap.mapID == WayXmap[IndexWay + 1])
                {
                    IndexWay++;
                    return;
                }
            }
            catch(Exception e)
            {
                File.WriteAllText("bug.txt", e.ToString());
            }
            IsNextMapFailed = true;
        }

        public void perform(int idAction, object p)
        {
            switch (idAction)
            {
                case 1:
                    List<int> idMaps = (List<int>)p;
                    ShowPanelXmap(idMaps);
                    break;
            }
        }

        static void Wait(int time)
        {
            IsWait = true;
            TimeStartWait = mSystem.currentTimeMillis();
            TimeWait = time;
        }

        static bool IsWaiting()
        {
            if (IsWait && mSystem.currentTimeMillis() - TimeStartWait >= TimeWait)
                IsWait = false;
            return IsWait;
        }

        #region Thao tác của xmap
        internal static void ShowXmapMenu()
        {
            var myVector = new MyVector();
            foreach (var groupMap in XmapData.Instance().GroupMaps)
                myVector.addElement(new Command(groupMap.Key, _Instance, 1, groupMap.Value));
            GameCanvas.menu.startAt(myVector, 3);
        }

        internal static void ShowPanelXmap(List<int> idMaps)
        {
            Pk9rXmap.IsMapTransAsXmap = true;
            int len = idMaps.Count;
            GameCanvas.panel.mapNames = new string[len];
            GameCanvas.panel.planetNames = new string[len];
            for (int i = 0; i < len; i++)
            {
                string nameMap = TileMap.mapNames[idMaps[i]];
                GameCanvas.panel.mapNames[i] =  "[" + idMaps[i] + "] " + nameMap;
                GameCanvas.panel.planetNames[i] = idMaps[i].ToString();
            }
            GameCanvas.panel.setTypeMapTrans();
            GameCanvas.panel.show();
        }

        internal static void StartRunToMapId(int idMap)
        {
            IdMapEnd = idMap;
            Pk9rXmap.IsXmapRunning = true;
        }

        internal static void FinishXmap()
        {
            Pk9rXmap.IsXmapRunning = false;
            IsNextMapFailed = false;
            XmapData.Instance().MyLinkMaps = null;
            WayXmap = null;
        }

        internal static void SaveIdMapCapsuleReturn()
        {
            Pk9rXmap.IdMapCapsuleReturn = TileMap.mapID;
        }

        static void NextMap(int idMapNext)
        {
            List<MapNext> mapNexts = XmapData.Instance().GetMapNexts(TileMap.mapID);
            if (mapNexts != null)
            {
                foreach (MapNext mapNext in mapNexts)
                {
                    if (mapNext.MapID == idMapNext)
                    {
                        NextMap(mapNext);
                        return;
                    }
                }
            }
            GameScr.info1.addInfo("Lỗi tại dữ liệu", 0);
        }

        static void NextMap(MapNext mapNext)
        {
            switch (mapNext.Type)
            {
                case TypeMapNext.AutoWaypoint:
                    NextMapAutoWaypoint(mapNext);
                    break;
                case TypeMapNext.NpcMenu:
                    NextMapNpcMenu(mapNext);
                    break;
                case TypeMapNext.NpcPanel:
                    NextMapNpcPanel(mapNext);
                    break;
                case TypeMapNext.Position:
                    NextMapPosition(mapNext);
                    break;
                case TypeMapNext.Capsule:
                    NextMapCapsule(mapNext);
                    break;
            }
        }

        private static void NextMapAutoWaypoint(MapNext mapNext)
        {
            Waypoint waypoint = XmapData.FindWaypoint(mapNext.MapID);
            if (waypoint != null)
            {
                int x = XmapData.GetPosWaypointX(waypoint);
                int y = XmapData.GetPosWaypointY(waypoint);
                MoveMyChar(x, y);
                RequestChangeMap(waypoint);
            }
        }
        public static int step;
        public static void findNpc()
        {
            if (TileMap.mapID == 27)
            {
                NextMap(28);
                IsWaitNextMap = true;
                step = 0;
                return;
            }
            if (TileMap.mapID == 29)
            {
                NextMap(28);
                IsWaitNextMap = true;
                step = 1;
                return;
            }
            if (step == 0)
            {
                NextMap(29);
                IsWaitNextMap = true;
                return;
            }
            if (step == 1)
            {
                NextMap(27);
                IsWaitNextMap = true;
            }
        }
        private static void NextMapNpcMenu(MapNext mapNext)
        {
           
            int idNpc = mapNext.Info[0];
            if (GameScr.findNPCInMap((short)idNpc) == null)
            {
                findNpc();
                return;
            }
            Service.gI().openMenu(idNpc);
            for (int i = 1; i < mapNext.Info.Length; i++)
            {
                int select = mapNext.Info[i];
                Service.gI().confirmMenu((short)idNpc, (sbyte)select);
            }
        }

        private static void NextMapNpcPanel(MapNext mapNext)
        {
            int idNpc = mapNext.Info[0];
            int selectMenu = mapNext.Info[1];
            int selectPanel = mapNext.Info[2];
            Service.gI().openMenu(idNpc);
            Service.gI().confirmMenu((short)idNpc, (sbyte)selectMenu);
            Service.gI().requestMapSelect(selectPanel);
        }

        private static void NextMapPosition(MapNext mapNext)
        {
            int xPos = mapNext.Info[0];
            int yPos = mapNext.Info[1];
            MoveMyChar(xPos, yPos);
            Service.gI().requestChangeMap();
            Service.gI().getMapOffline();
        }

        private static void NextMapCapsule(MapNext mapNext)
        {
            SaveIdMapCapsuleReturn();
            int index = mapNext.Info[0];
            Service.gI().requestMapSelect(index);
        }
        #endregion

        #region Thao tác với game
        public static void UseCapsuleNormal()
        {
            Pk9rXmap.IsShowPanelMapTrans = false;
            Service.gI().useItem(0, 1, -1, ID_ITEM_CAPSULE);
        }

        public static void UseCapsuleVip()
        {
            Pk9rXmap.IsShowPanelMapTrans = false;
            Service.gI().useItem(0, 1, -1, ID_ITEM_CAPSULE_VIP);
        }
        
        public static void HideInfoDlg()
        {
            InfoDlg.hide();
        }

        public static void MoveMyChar(int x, int y)
        {
            Char.myCharz().cx = x;
            Char.myCharz().cy = y;
            Service.gI().charMove();

            if (ItemTime.isExistItem(ID_ICON_ITEM_TDLT))
                return;

            Char.myCharz().cx = x;
            Char.myCharz().cy = y + 1;
            Service.gI().charMove();
            Char.myCharz().cx = x;
            Char.myCharz().cy = y;
            Service.gI().charMove();
        }

        private static void RequestChangeMap(Waypoint waypoint)
        {
            waypoint.popup.doClick(1);
            //if (waypoint.isOffline)
            //{
            //    Service.gI().getMapOffline();
            //    return;
            //}
            //Service.gI().requestChangeMap();
        }
        #endregion
    }
}
