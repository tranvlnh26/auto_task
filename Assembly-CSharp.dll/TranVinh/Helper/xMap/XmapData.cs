using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TranVinh.Xmap
{
    internal class XmapData
    {
        const int ID_MAP_HOME_BASE = 21;
        const int ID_MAP_TTVT_BASE = 24;
        const int ID_ITEM_CAPSULE_VIP = 194;
        const int ID_ITEM_CAPSULE_NORMAL = 193;
        const int ID_MAP_TPVGT = 19;
        const int ID_MAP_TO_COLD = 109;

        internal Dictionary<int, List<MapNext>> MyLinkMaps;
        internal Dictionary<string, List<int>> GroupMaps;
        internal List<int[]> LinkMaps;
        internal bool IsLoading;
        private bool IsLoadingCapsule;

        private XmapData()
        {
            MyLinkMaps = null;
            IsLoading = false;
            IsLoadingCapsule = false;

            GroupMaps = new Dictionary<string, List<int>>();
            GroupMaps.Add("Trái Đất", new List<int>()
            {
                42, 21, 0, 1, 2, 3, 4, 5, 6, 27, 28, 29, 30, 47, 46, 45, 48, 50, 111, 24, 53, 58, 59, 60, 61, 62, 55, 56, 54, 57
            });
            GroupMaps.Add("Namec", new List<int>()
            {
                43, 22, 7, 8, 9, 11, 12, 13, 10, 31, 32, 33, 34, 25
            });
            GroupMaps.Add("Xayda", new List<int>()
            {
                44, 23, 14, 15, 16, 17, 18, 20, 19, 35, 36, 37, 38, 26, 52, 84, 129
            });
            GroupMaps.Add("Nappa", new List<int>()
            {
                68, 69, 70, 71, 72, 64, 65, 63, 66, 67, 73, 74, 75, 76, 77, 81, 82, 83, 79, 80, 131, 132, 133
            }); 
            GroupMaps.Add("Tương lai", new List<int>()
            {
                102, 92, 93, 94, 96, 97, 98, 99, 100, 103
            });
            GroupMaps.Add("Cold", new List<int>()
            {
                109, 108, 107, 110, 106, 105
            });
            GroupMaps.Add("Khí Gas", new List<int>()
            {
                149, 147, 152, 151, 148
            });
            RemoveMapsHomeInGroupMaps();

            var NpcMenu = (int)TypeMapNext.NpcMenu;
            var AutoWaypoint = (int)TypeMapNext.AutoWaypoint;
            // type - start - end - info
            LinkMaps = new List<int[]>()
            {
                new int[] { NpcMenu, 24, 25, 10, 0 },
                new int[] { NpcMenu, 25, 24, 11, 0 },
                new int[] { NpcMenu, 45, 48, 19, 3 },
                new int[] { NpcMenu, 48, 25, 20, 3, 0 },
                new int[] { NpcMenu, 48, 50, 20, 3, 0, 1 },
                new int[] { NpcMenu, 50, 48, 44, 0 },
                new int[] { NpcMenu, 24, 26, 10, 1 },
                new int[] { NpcMenu, 26, 24, 12, 0 },
                new int[] { NpcMenu, 25, 26, 11, 1 },
                new int[] { NpcMenu, 26, 25, 12, 1 },
                new int[] { NpcMenu, 24, 84, 10, 2 },
                new int[] { NpcMenu, 25, 84, 11, 2 },
                new int[] { NpcMenu, 26, 84, 12, 2 },
                new int[] { NpcMenu, 19, 68, 12, 1 },
                new int[] { NpcMenu, 68, 19, 12, 0 },
                new int[] { NpcMenu, 80, 131, 60, 0 },
                new int[] { NpcMenu, 27, 102, 38, 1 },
                new int[] { NpcMenu, 28, 102, 38, 1 },
                new int[] { NpcMenu, 29, 102, 38, 1 },
                new int[] { NpcMenu, 102, 24, 38, 1 },
                new int[] { NpcMenu, 27, 53, 25, 0 },
                new int[] { NpcMenu, 52, 129, 23, 3 },
                new int[] { NpcMenu, 0, 149, 67, 3, 0 },

                new int[] { AutoWaypoint, 42, 0, 1, 2, 3, 4, 5, 6 },
                new int[] { AutoWaypoint, 3, 27, 28, 29, 30 },
                new int[] { AutoWaypoint, 2, 24 },
                new int[] { AutoWaypoint, 1, 47, 46, 45 },
                new int[] { AutoWaypoint, 5, 29 },
                new int[] { AutoWaypoint, 47, 111 },
                new int[] { AutoWaypoint, 53, 58, 59, 60, 61, 62, 55, 56, 54, 57 },
                new int[] { AutoWaypoint, 53, 27 },
                new int[] { AutoWaypoint, 43, 7, 8, 9, 11, 12, 13, 10 },
                new int[] { AutoWaypoint, 11, 31, 32, 33, 34 },
                new int[] { AutoWaypoint, 9, 25 },
                new int[] { AutoWaypoint, 13, 33 },
                new int[] { AutoWaypoint, 52, 44, 14, 15, 16, 17, 18, 20, 19 },
                new int[] { AutoWaypoint, 17, 35, 36, 37, 38 },
                new int[] { AutoWaypoint, 16, 26, },
                new int[] { AutoWaypoint, 20, 37, },
                new int[] { AutoWaypoint, 68, 69, 70, 71, 72, 64, 65, 63, 66, 67, 73, 74, 75, 76, 77, 81, 82, 83, 79, 80 },
                new int[] { AutoWaypoint, 102, 92, 93, 94, 96, 97, 98, 99, 100, 103 },
                new int[] { AutoWaypoint, 109, 108, 107, 110, 106 },
                new int[] { AutoWaypoint, 109, 105 },
                new int[] { AutoWaypoint, 109, 106 },
                new int[] { AutoWaypoint, 106, 107 },
                new int[] { AutoWaypoint, 108, 105 },
                new int[] { AutoWaypoint, 131, 132, 133 },
                new int[] { AutoWaypoint, 80, 105 },
                new int[] { AutoWaypoint, 149, 147, 152, 151, 148 },
            };

        }

        private static XmapData _Instance;

        public static XmapData Instance()
        {
            if (_Instance == null)
                _Instance = new XmapData();
            return _Instance;
        }

        public void LoadLinkMaps()
        {
            IsLoading = true;
        }

        public void Update()
        {
            if (!IsLoadingCapsule)
            {
                LoadLinkMapBase();
                if (CanUseCapsuleVip())
                {
                    XmapController.UseCapsuleVip();
                    IsLoadingCapsule = true;
                    return;
                }
                if (CanUseCapsuleNormal())
                {
                    XmapController.UseCapsuleNormal();
                    IsLoadingCapsule = true;
                    return;
                }
                IsLoading = false;
                return;
            }
            if (IsWaitInfoMapTrans())
                return;
            LoadLinkMapCapsule();
            IsLoadingCapsule = false;
            IsLoading = false;
        }

        #region Thao tác với dữ liệu xmap
        #region Lấy dữ liệu các nhóm map


        void RemoveMapsHomeInGroupMaps()
        {
            int cgender = Char.myCharz().cgender;
            switch (cgender)
            {
                case 0:
                    GroupMaps["Namec"].Remove(22);
                    GroupMaps["Xayda"].Remove(23);
                    break;
                case 1:
                    GroupMaps["Trái Đất"].Remove(21);
                    GroupMaps["Xayda"].Remove(23);
                    break;
                default:
                    GroupMaps["Trái Đất"].Remove(21);
                    GroupMaps["Namec"].Remove(22);
                    break;
            }
        }
        #endregion

        #region Lấy dữ liệu cho xmap
        private void LoadLinkMapCapsule()
        {
            AddKeyLinkMaps(TileMap.mapID);
            string[] mapNames = GameCanvas.panel.mapNames;
            int idMap;
            for (int select = 0; select < mapNames.Length; select++)
            {
                idMap = GetIdMapFromName(mapNames[select]);
                if (idMap != -1)
                {
                    int[] info = new int[] { select };
                    MyLinkMaps[TileMap.mapID].Add(new MapNext(idMap, TypeMapNext.Capsule, info));
                }
            }
        }

        private void LoadLinkMapBase()
        {
            try
            {
                MyLinkMaps = new Dictionary<int, List<MapNext>>();
                int[] info;
                int lenInfo;
                foreach (var data in LinkMaps)
                {
                    var type = (TypeMapNext)data[0];
                    switch (type)
                    {
                        case TypeMapNext.AutoWaypoint:
                            for (int i = 1; i < data.Length; i++)
                            {
                                if (i != 1)
                                    LoadLinkMap(data[i], data[i - 1], type, null);

                                if (i != data.Length - 1)
                                    LoadLinkMap(data[i], data[i + 1], type, null);
                            }
                            break;
                        default:
                            lenInfo = data.Length - 3;
                            info = new int[lenInfo];
                            Array.Copy(data, 3, info, 0, lenInfo);
                            LoadLinkMap(data[1], data[2], type, info);
                            break;
                    }
                }
                LoadLinkMapsHome();
                LoadLinkMapSieuThi();
                LoadLinkMapToCold();
            }
            catch (Exception e)
            {
                File.WriteAllText("bug.txt", e.ToString());
            }
        }

        void LoadLinkMapsHome()
        {
            const int ID_MAP_LANG_BASE = 7;
            int cgender = Char.myCharz().cgender;

            int idMapHome = ID_MAP_HOME_BASE + cgender;
            int idMapLang = ID_MAP_LANG_BASE * cgender;

            LoadLinkMap(idMapLang, idMapHome, TypeMapNext.AutoWaypoint, null);
            LoadLinkMap(idMapHome, idMapLang, TypeMapNext.AutoWaypoint, null);
        }

        void LoadLinkMapSieuThi()
        {
            const int ID_MAP_TTVT_BASE = 24;
            const int ID_MAP_SIEU_THI = 84;
            const int ID_NPC = 10;
            const int INDEX = 0;

            int offset = Char.myCharz().cgender;
            int idMapNext = ID_MAP_TTVT_BASE + offset;
            int[] info = new int[]
            {
                ID_NPC, INDEX
            };
            LoadLinkMap(ID_MAP_SIEU_THI, idMapNext, TypeMapNext.NpcMenu, info);
        }

        void LoadLinkMapToCold()
        {
            if (Char.myCharz().taskMaint.taskId <= 30)
                return;

            const int ID_NPC = 12;
            const int INDEX = 0;

            int[] info = new int[]
            {
                ID_NPC, INDEX
            };
            LoadLinkMap(ID_MAP_TPVGT, ID_MAP_TO_COLD, TypeMapNext.NpcMenu, info);
        }
        #endregion

        public List<MapNext> GetMapNexts(int idMap)
        {
            if (CanGetMapNexts(idMap))
                return MyLinkMaps[idMap];
            return null;
        }

        public bool CanGetMapNexts(int idMap)
        {
            return MyLinkMaps.ContainsKey(idMap);
        }

        void LoadLinkMap(int idMapStart, int idMapNext, TypeMapNext type, int[] info)
        {
            AddKeyLinkMaps(idMapStart);
            MapNext mapNext = new MapNext(idMapNext, type, info);
            MyLinkMaps[idMapStart].Add(mapNext);
        }

        void AddKeyLinkMaps(int idMap)
        {
            if (!MyLinkMaps.ContainsKey(idMap))
                MyLinkMaps.Add(idMap, new List<MapNext>());
        }

        bool IsWaitInfoMapTrans()
        {
            return !Pk9rXmap.IsShowPanelMapTrans;
        }

        #endregion

        #region Lấy dữ liệu từ game
        public static Waypoint FindWaypoint(int idMap)
        {
            Waypoint waypoint;
            for (int i = 0; i < TileMap.vGo.size(); i++)
            {
                waypoint = (Waypoint)TileMap.vGo.elementAt(i);
                if (waypoint.name.Equals(TileMap.mapNames[idMap]))
                {
                    return waypoint;
                }
            }
            return null;
        }

        public static int GetPosWaypointX(Waypoint waypoint)
        {
            if (waypoint.maxX < 60)
            {
                return waypoint.minX + 18;
            }
            if (waypoint.maxX > TileMap.pxw - 60)
            {
                return waypoint.maxX - 18;
            }
            return (waypoint.minX + waypoint.maxX)/2;
        }

        public static int GetPosWaypointY(Waypoint waypoint)
        {
            return waypoint.maxY;
        }

        public static bool IsMyCharDie()
        {
            return Char.myCharz().statusMe == 14 || Char.myCharz().cHP <= 0 || Char.myCharz().meDead;
        }

        public static bool CanNextMap()
        {
            return !Char.isLoadingMap && !Char.ischangingMap && !Controller.isStopReadMessage;
        }

        private static int GetIdMapFromName(string mapName)
        {
            int offset = Char.myCharz().cgender;

            if (mapName.Equals("Về nhà"))
                return ID_MAP_HOME_BASE + offset;

            if (mapName.Equals("Trạm tàu vũ trụ"))
                return ID_MAP_TTVT_BASE + offset;

            if (mapName.Contains("Về chỗ cũ: "))
            {
                mapName = mapName.Replace("Về chỗ cũ: ", "");
                if (TileMap.mapNames[Pk9rXmap.IdMapCapsuleReturn].Equals(mapName))
                    return Pk9rXmap.IdMapCapsuleReturn;
                if (mapName.Equals("Rừng đá"))
                    return -1;
            }

            for (int i = 0; i < TileMap.mapNames.Length; i++)
                if (mapName.Equals(TileMap.mapNames[i]))
                    return i;

            return -1;
        }


        private static bool CanUseCapsuleNormal()
        {
            return !IsMyCharDie() && Pk9rXmap.IsUseCapsuleNormal && HasItemCapsuleNormal();
        }

        private static bool HasItemCapsuleNormal()
        {
            Item[] items = Char.myCharz().arrItemBag;
            for (int i = 0; i < items.Length; i++)
                if (items[i] != null && items[i].template.id == ID_ITEM_CAPSULE_NORMAL)
                    return true;
            return false;
        }

        private static bool CanUseCapsuleVip()
        {
            return !IsMyCharDie() && Pk9rXmap.IsUseCapsuleVip && HasItemCapsuleVip();
        }

        private static bool HasItemCapsuleVip()
        {
            Item[] items = Char.myCharz().arrItemBag;
            for (int i = 0; i < items.Length; i++)
                if (items[i] != null && items[i].template.id == ID_ITEM_CAPSULE_VIP)
                    return true;
            return false;
        }

        #endregion
    }
}
