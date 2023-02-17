using System;
using System.IO;
using System.Threading;
using TranVinh.Helper;
using UnityEngine;

namespace TranVinh.Xmap
{
    internal class Pk9rXmap
    {
        internal static bool IsXmapRunning = false;
        internal static bool IsMapTransAsXmap = false;
        internal static bool IsShowPanelMapTrans = true;
        internal static bool IsUseCapsuleNormal = false;
        internal static bool IsUseCapsuleVip = true;
        internal static int IdMapCapsuleReturn = -1;


        internal static bool Chat(string text)
        {
            if (text == "xmp")
            {
                if (IsXmapRunning)
                {
                    XmapController.FinishXmap();
                    GameScr.info1.addInfo("Đã huỷ Xmap", 0);
                }
                else
                {
                    XmapController.ShowXmapMenu();
                }
            }
            else if (Utils.IsGetInfoChat<int>(text, "xmp"))
            {
                if (IsXmapRunning)
                {
                    XmapController.FinishXmap();
                    GameScr.info1.addInfo("Đã huỷ Xmap", 0);
                }
                else
                {
                    int idMap = Utils.GetInfoChat<int>(text, "xmp");
                    XmapController.StartRunToMapId(idMap);
                }
            }
            else if (text == "csb")
            {
                IsUseCapsuleNormal = !IsUseCapsuleNormal;
                GameScr.info1.addInfo("Sử dụng capsule thường Xmap: " + (IsUseCapsuleNormal ? "Bật" : "Tắt"), 0);
            }
            else if (text == "csdb")
            {
                IsUseCapsuleVip = !IsUseCapsuleVip;
                GameScr.info1.addInfo("Sử dụng capsule đặc biệt Xmap: " + (IsUseCapsuleVip ? "Bật" : "Tắt"), 0);
            }
            else
            {
                return false;
            }
            return true;
        }

        internal static bool HotKeys()
        {
            try
            {
                switch (GameCanvas.keyAsciiPress)
                {
                    case 'x':
                        Chat("xmp");
                        break;
                    default:
                        return false;
                }
            }
            catch(Exception e)
            {
            }
            return true;
        }

        internal static void Update()
        {
            if (IsXmapRunning)
            {
                if (XmapData.Instance().IsLoading)
                    XmapData.Instance().Update();
                XmapController.Update();
            }
        }

        internal static void Info(string text)
        {
            if (text.Equals("Bạn chưa thể đến khu vực này"))
                XmapController.FinishXmap();
        }

        internal static bool XoaTauBay(Teleport teleport)
        {
            if (teleport.isMe)
            {
                Char.myCharz().isTeleport = false;
                if (teleport.type == 0)
                {
                    Controller.isStopReadMessage = false;
                    Char.ischangingMap = true;
                }
                Teleport.vTeleport.removeElement(teleport);
                return true;
            }
            return false;
        }

        internal static void SelectMapTrans(int selected)
        {
            try
            {
                if (IsMapTransAsXmap)
                {
                    XmapController.HideInfoDlg();
                    int idMap = int.Parse(GameCanvas.panel.planetNames[selected]);
                    XmapController.StartRunToMapId(idMap);
                    return;
                }
                XmapController.SaveIdMapCapsuleReturn();
                Service.gI().requestMapSelect(selected);
            }
            catch(Exception e)
            {
                File.WriteAllText("bug.txt", e.ToString());
            }
        }

        internal static void ShowPanelMapTrans()
        {
            IsMapTransAsXmap = false;
            if (IsShowPanelMapTrans)
            {
                GameCanvas.panel.setTypeMapTrans();
                GameCanvas.panel.show();
                return;
            }
            IsShowPanelMapTrans = true;
        }

        internal static void FixBlackScreen()
        {
            Controller.gI().loadCurrMap(0);
            Service.gI().finishLoadMap();
            Char.isLoadingMap = false;
        }

    }
}
