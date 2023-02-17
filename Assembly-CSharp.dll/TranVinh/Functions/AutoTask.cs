using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TranVinh.Helper;
using TranVinh.Xmap;
using UnityEngine;

namespace TranVinh.Functions
{
    internal class AutoTask
    {
         #region code :>

        #region biến check nv
        private static bool pean;
        private static bool chuong;
        private static bool fly;
        private static bool mobFly;
        private static bool mocNhan;
        private static bool sell200Item;
        private static bool skill3;
        #endregion

        // Check có đang đi check nv ko
        public static bool isCheckTask;

        // Tính số item đã bán
        public static int soItem = 0;

        private static int dir = 1;

        // tính bước bán item
        private static int stepSell = 1;

        private static int tick = 10;

        public static Thread threadCheck;

        public static bool[] chonNv;
        public static void update()
        {
            try
            {
                if (GameCanvas.gameTick % 33 == 0)
                {
                    tick--;
                }
                if (tick == 0)
                {
                    startCheck();
                    Wait(10000);
                }
                Char.myCharz().cspeed = 8;
                Char @char = global::Char.myCharz();
                if ((@char.cHP <= @char.cHPFull * 20 / 100 || @char.cMP <= @char.cMPFull * 10 / 100))
                {
                    GameScr.gI().doUseHP();
                }

                if (@char.statusMe == 14 || @char.cHP <= 0)
                {
                    Service.gI().returnTownFromDead();
                    Wait(5000);
                    return;
                }
                if (IsWaiting())
                    return;


                if (isCheckTask)
                {
                    if (!mobFly && chonNv[0])
                    {
                        DragonClient.send_status($"[{tick}] Đang làm nv quái bay!");
                        MobFly();
                    }
                    else if (!sell200Item && chonNv[1])
                    {
                        DragonClient.send_status($"[{tick}] Đang làm nv bán đồ: {soItem}");
                        BanDo();
                    }
                    else if (!chuong && chonNv[2])
                    {
                        DragonClient.send_status($"[{tick}] Đang làm nv chưởng!");
                        Chuong();
                    }
                    else if (!fly && chonNv[3])
                    {
                        DragonClient.send_status($"[{tick}] Đang làm nv bay!");
                        Fly();
                    }
                    else if (!mocNhan && chonNv[4])
                    {
                        DragonClient.send_status($"[{tick}] Đang làm nv mộc nhân!");
                        MocNhan();
                        return;
                    }
                    else if (!skill3 && chonNv[5])
                    {
                        DragonClient.send_status($"[{tick}] Đang làm nv Skill đặc biệt!");
                        Skill3();
                        return;
                    }
                    else if (!pean && chonNv[6])
                    {
                        DragonClient.send_status($"[{tick}] Đang làm nv nâng đậu!");
                        NangDau();
                        return;
                    }
                    else
                    {
                        DragonClient.send_status("Đã làm xong nv!");
                        //Application.Quit();
                        Process.GetCurrentProcess().Kill();
                        Wait(10000);
                    }
                }
            }
            catch (Exception e)
            {
                TextWriter text = new StreamWriter("bug.txt");
                text.Write(e.ToString());
                text.Close();
            }
        }
        #region task
        private static void NangDau()
        {
            #region di chuyển
            if (TileMap.mapID != 21 + Char.myCharz().cgender)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(21 + Char.myCharz().cgender);
                Wait(5000);
                return;
            }
            #endregion
            MagicTree m = GameScr.gI().magicTree;
            if (m.level < 5)
            {
                if (m.isUpdate)
                {
                    // đang update
                    // m.seconds; // time
                    DateTime time = DateTime.Now.AddSeconds(m.seconds);
                    DragonClient.send_status("Login lại vào: " + time.ToString("dd/MM/yyyy HH:mm:ss"));
                    Application.Quit();
                }
                else
                {
                    Service.gI().openMenu(4);
                    if (!((Command)GameCanvas.menu.menuItems.elementAt(1)).caption.StartsWith("Nâng cấp"))
                    {
                        CloseMenu();
                        Wait(2000);
                        return;
                    }
                    Service.gI().confirmMenu(4, 1);
                    Service.gI().confirmMenu(4, 0);
                }
            }
            else
            {
                startCheck();
            }
            Wait(5000);
        }

        private static void Chuong()
        {
            #region di chuyển
            if (TileMap.mapID != 7 * Char.myCharz().cgender)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(7 * Char.myCharz().cgender);
                Wait(5000);
                return;
            }
            if (Char.myCharz().cx != 900 && !Pk9rXmap.IsXmapRunning)
                XmapController.MoveMyChar(900, Utils.getYGround(900));
            #endregion

            Char c = global::Char.myCharz();
            if (c.isCharge)
            {
                Wait(500);
                return;
            }
            #region
            IdSkillsTanSat.Clear();
            TypeMobsTanSat.Clear();
            IdMobsTanSat.Clear();

            IdSkillsTanSat.Add(1);
            IdSkillsTanSat.Add(3);
            IdSkillsTanSat.Add(5);

            IdMobsTanSat.Add(0);
            IdMobsTanSat.Add(1);
            IdMobsTanSat.Add(2);
            #endregion

            if (c.mobFocus != null && !IsMobTanSat(c.mobFocus))
                c.mobFocus = null;
            if (c.mobFocus == null)
            {
                Mob mob = GetMobTanSat();
                c.focusManualTo(mob);
            }
            if (c.mobFocus != null && c.skillInfoPaint() == null)
            {
                Skill skill = GetSkillAttack();
                if (skill == null)
                {
                    var it = findItemInBag(94 + 7 * c.cgender);
                    if (it != null)
                    {
                        Service.gI().useItem(0, 1, (sbyte)it.indexUI, it.template.id);
                        Wait(2500);
                        return;
                    }
                }
                if (skill != null && !skill.paintCanNotUseSkill)
                {
                    Mob mobFocus = c.mobFocus;
                    if (c.myskill != skill)
                    {
                        Service.gI().selectSkill(skill.template.id);
                        c.myskill = skill;
                    }
                    if (c.cgender == 2 && c.cMP < c.cMPFull / 100 * 30)
                    {
                        Skill skill2 = Char.myCharz().getSkill(8);
                        if (!skill2.paintCanNotUseSkill)
                        {
                            GameScr.gI().doSelectSkill(skill2, true);
                            Wait(1000);
                            return;
                        }
                    }
                    if (skill.template.type == 3 || skill.template.id == 10 || skill.template.id == 11)
                    {
                        GameScr.gI().doSelectSkill(skill, true);
                    }
                    else if (mSystem.currentTimeMillis() - XmapController.delayTele > 0L)
                    {
                        AutoSendDame.Ak();
                    }
                    Wait(500);
                }

            }
        }

        private static void Fly()
        {
            if (TileMap.mapID != 7 * Char.myCharz().cgender)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(7 * Char.myCharz().cgender);
                Wait(5000);
                return;
            }
            if (Char.myCharz().cMP < 1)
            {
                XmapController.StartRunToMapId(21 + Char.myCharz().cgender);
                Wait(2000);
                return;
            }
            if (dir == 1)
            {
                Char.myCharz().cy = 100;
                Service.gI().charMove();
                GameScr.gI().checkClickMoveTo(60, 100);
                dir = 2;
            }
            else
            {
                Char.myCharz().cy = 100;
                Service.gI().charMove();
                GameScr.gI().checkClickMoveTo(TileMap.pxw - 60, 100);
                dir = 1;
            }
            Wait(9500);
        }
        private static void MobFly()
        {
            #region di chuyển
            if (TileMap.mapID != 3)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(3);
                Wait(5000);
                return;
            }
            #endregion

            Char @char = global::Char.myCharz();
            if (@char.isCharge)
            {
                Wait(500);
                return;
            }
            addSkill();
            IdMobsTanSat.Clear();
            TypeMobsTanSat.Clear();
            TypeMobsTanSat.Add(7);
            @char.clearFocus(0);

            if (@char.mobFocus != null && !IsMobTanSat(@char.mobFocus))
                @char.mobFocus = null;
            if (@char.mobFocus == null)
            {
                Mob mob = GetMobTanSat();
                @char.focusManualTo(mob);
            }
            if (@char.mobFocus != null && @char.skillInfoPaint() == null)
            {
                Skill skill = GetSkillAttack();
                if (skill != null && !skill.paintCanNotUseSkill)
                {
                    Mob mobFocus = @char.mobFocus;
                    mobFocus.x = mobFocus.xFirst;
                    mobFocus.y = mobFocus.yFirst;
                    if (Res.distance(mobFocus.xFirst, mobFocus.yFirst, @char.cx, @char.cy) <= 48)
                    {
                        if (skill.template.type == 3 || skill.template.id == 10 || skill.template.id == 11 || Char.myCharz().myskill != skill)
                        {
                            GameScr.gI().doSelectSkill(skill, true);
                        }
                        else if (mSystem.currentTimeMillis() - XmapController.delayTele > 0L)
                        {
                            AutoSendDame.Ak();
                        }
                    }
                    else
                    {
                        XmapController.MoveMyChar2(mobFocus.x, mobFocus.y);
                    }
                    Wait(600);
                }

            }
        }

        private static void MocNhan()
        {
            #region di chuyển
            if (TileMap.mapID != 7 * Char.myCharz().cgender)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(7 * Char.myCharz().cgender);
                Wait(5000);
                return;
            }
            if (Char.myCharz().cx != 900)
                GameScr.gI().checkClickMoveTo(900, 432);
            #endregion

            Char @char = global::Char.myCharz();
            if (@char.isCharge)
            {
                Wait(500);
                return;
            }

            addSkill();

            TypeMobsTanSat.Clear();
            IdMobsTanSat.Clear();
            IdSkillsTanSat.Clear();

            IdMobsTanSat.Add(0);
            IdMobsTanSat.Add(1);
            IdMobsTanSat.Add(2);


            IdSkillsTanSat.Add(0);
            IdSkillsTanSat.Add(2);
            IdSkillsTanSat.Add(4);
            if (@char.mobFocus != null && !IsMobTanSat(@char.mobFocus))
                @char.mobFocus = null;
            if (@char.mobFocus == null)
            {
                Mob mob = GetMobTanSat();
                @char.focusManualTo(mob);
            }
            if (@char.mobFocus != null && @char.skillInfoPaint() == null)
            {
                Skill skill = GetSkillAttack();
                if (skill != null && !skill.paintCanNotUseSkill)
                {
                    Mob mobFocus = @char.mobFocus;
                    if (@char.myskill != skill)
                    {
                        Service.gI().selectSkill(skill.template.id);
                        @char.myskill = skill;
                    }
                    if (@char.cgender == 2 && @char.cMP < @char.cMPFull / 100 * 30)
                    {
                        Skill skill2 = Char.myCharz().getSkill(8);
                        if (!skill2.paintCanNotUseSkill)
                        {
                            GameScr.gI().doSelectSkill(skill2, true);
                            Wait(1000);
                            return;
                        }
                    }
                    if (skill.template.type == 3 || skill.template.id == 10 || skill.template.id == 11)
                    {
                        GameScr.gI().doSelectSkill(skill, true);
                    }
                    else if (mSystem.currentTimeMillis() - XmapController.delayTele > 0L)
                    {
                        AutoSendDame.Ak();
                    }
                    Wait(500);
                }

            }
        }

        private static void BanDo()
        {
            #region di chuyển
            if (TileMap.mapID != 7 * Char.myCharz().cgender)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(7 * Char.myCharz().cgender);
                Wait(5000);
                return;
            }
            Npc npc = GameScr.findNPCInMap((short)(7 + Char.myCharz().cgender));
            if (Char.myCharz().cx != npc.cx)
                XmapController.MoveMyChar(npc.cx, npc.cy - 3);
            #endregion

            if (!GameCanvas.panel.isShow)
            {
                new Thread(() =>
                {

                    Service.gI().openMenu(7 + global::Char.myCharz().cgender);
                    Service.gI().openMenu(7 + global::Char.myCharz().cgender);
                    Service.gI().openMenu(7 + global::Char.myCharz().cgender);
                    Thread.Sleep(1000);
                    Service.gI().confirmMenu((short)(7 + global::Char.myCharz().cgender), 0);
                    Service.gI().confirmMenu((short)(7 + global::Char.myCharz().cgender), 0);
                    Thread.Sleep(1000);
                    CloseMenu();
                    Thread.Sleep(500);
                    stepSell = 1;
                }).Start();
                Wait(3000);
                return;
            }
            if (soItem > 199)
            {
                startCheck();
                Wait(10000);
                return;
            }
            int id = 27 + global::Char.myCharz().cgender;
            if (stepSell == 1)
            {
                stepSell = 2;
                Service.gI().buyItem(0, id, 0);
                Wait(1500);
            }
            else if (stepSell == 2)
            {
                short i = (short)getItem(id);
                Service.gI().saleItem(0, 1, i);
                stepSell = 3;
                Wait(500);
            }
            else if (stepSell == 3)
            {
                short i = (short)getItem(id);
                Service.gI().saleItem(1, 1, i);
                stepSell = 1;
                Wait(700);
                return;
            }
        }
        internal static Item findItemInBag(int id)
        {
            foreach (Item item in global::Char.myCharz().arrItemBag)
            {
                bool flag = item != null && (int)item.template.id == id;
                if (flag)
                {
                    return item;
                }
            }
            return null;
        }

        private static void Skill3()
        {
            #region di chuyển
            if (TileMap.mapID != 7 * Char.myCharz().cgender)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(7 * Char.myCharz().cgender);
                Wait(5000);
                return;
            }
            #endregion
            Char c = Char.myCharz();
            Skill s = Char.myCharz().getSkill(6 + c.cgender);
            if (s == null)
            {
                var it = findItemInBag(115 + 7 * c.cgender);
                if (it != null)
                    Service.gI().useItem(0, 1, (sbyte)it.indexUI, it.template.id);
                Wait(2500);
                return;
            }
            if (c.isCharge)
            {
                Wait(500);
                return;
            }
            if (!s.paintCanNotUseSkill && c.cMP > GetManaUseSkill(s))
            {
                if (c.cgender == 2) // XD
                {
                    #region
                    IdMobsTanSat.Clear();
                    IdSkillsTanSat.Clear();

                    IdMobsTanSat.Add(0);
                    IdMobsTanSat.Add(1);
                    IdMobsTanSat.Add(2);

                    IdSkillsTanSat.Add(1);
                    IdSkillsTanSat.Add(3);
                    IdSkillsTanSat.Add(5);
                    #endregion
                    Mob mob = GetMobTanSat();
                    if (mob != null)
                    {
                        if (Char.myCharz().cx != 900 && !Pk9rXmap.IsXmapRunning)
                        {
                            GameScr.gI().checkClickMoveTo(900, Utils.getYGround(900));
                            Wait(1000);
                            return;
                        }
                        if (c.cHP == c.cHPFull && c.cMP == c.cMPFull)
                        {
                            c.myskill = GetSkillAttack();
                            Service.gI().selectSkill(c.myskill.template.id);
                            c.focusManualTo(mob);
                            Service.gI().sendPlayerAttack(mob);
                            Wait(1500);
                            return;
                        }
                        if (!s.paintCanNotUseSkill)
                            GameScr.gI().doSelectSkill(s, true);
                    }
                }
                else if (c.cgender == 1) // NM
                {
                    if (!s.paintCanNotUseSkill)
                    {
                        Service.gI().selectSkill(s.template.id);
                        c.myskill = s;
                        MyVector myVector = new MyVector();
                        myVector.addElement(c);
                        Service.gI().sendPlayerAttack(new MyVector(), myVector, 2);
                        c.myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
                    }
                }
                else if (c.cgender == 0) // TĐ
                {
                    GameScr.gI().doSelectSkill(s, true);
                }
            }

            Wait(2000);
        }

        #endregion


        public static void startCheck()
        {
            if (threadCheck != null && threadCheck.IsAlive)
            {
                threadCheck.Abort();
                GC.Collect();
            }
            DragonClient.send_status("Đang đi check nv");
            tick = 600;
            threadCheck = new Thread(checkTT) { IsBackground = true };
            threadCheck.Start();
        }

        private static void checkTT()
        {

            isCheckTask = false;
            Thread.Sleep(1000);
            while (TileMap.mapID != 47)
            {
                if (!Pk9rXmap.IsXmapRunning)
                    XmapController.StartRunToMapId(47);
                Thread.Sleep(5000);
            }


            while (!GameCanvas.panel.isShow)
            {
                Service.gI().openMenu(17);
                Thread.Sleep(500);
                Service.gI().confirmMenu(17, 1);
                Thread.Sleep(1000);
                CloseMenu();
                Thread.Sleep(500);
            }

            Thread.Sleep(1000);

            Archivement aPean = Char.myCharz().arrArchive[2];
            Thread.Sleep(750);
            DragonClient.send_status(aPean.info2);
            Archivement aChuong = Char.myCharz().arrArchive[4];
            Thread.Sleep(750);
            DragonClient.send_status(aChuong.info2);
            Archivement aFly = Char.myCharz().arrArchive[5];
            Thread.Sleep(750);
            DragonClient.send_status(aFly.info2);
            Archivement aMobFly = Char.myCharz().arrArchive[6];
            Thread.Sleep(750);
            DragonClient.send_status(aMobFly.info2);
            Archivement aMocNhan = Char.myCharz().arrArchive[7];
            Thread.Sleep(750);
            DragonClient.send_status(aMocNhan.info2);
            Archivement aSell200Item = Char.myCharz().arrArchive[10];
            Thread.Sleep(750);
            DragonClient.send_status(aSell200Item.info2);
            Archivement aSkilDb = Char.myCharz().arrArchive[14];
            Thread.Sleep(750);
            DragonClient.send_status(aSkilDb.info2);

            pean = aPean.isFinish;
            chuong = aChuong.isFinish;
            fly = aFly.isFinish;
            mobFly = aMobFly.isFinish;
            mocNhan = aMocNhan.isFinish;
            sell200Item = aSell200Item.isFinish;
            skill3 = aSkilDb.isFinish;
            for (int i = 0; i < Char.myCharz().arrArchive.Length; i++)
            {
                Archivement aa = Char.myCharz().arrArchive[i];
                if (aa.isFinish && !aa.isRecieve)
                {
                    Service.gI().getArchivemnt(i);
                    Thread.Sleep(500);
                }
            }
            DragonClient.send_status("check xong nv!");
            isCheckTask = true;
        }
        private static void CloseMenu()
        {
            Effect2.vEffect2Outside.removeAllElements();
            Effect2.vEffect2.removeAllElements();
            GameCanvas.menu.doCloseMenu();
        }


        #region tàn sát

        private static void addSkill()
        {
            IdSkillsTanSat.Clear();
            if (chuong)
            {
                IdSkillsTanSat.Add(1);
                IdSkillsTanSat.Add(3);
                IdSkillsTanSat.Add(5);
            }
            IdSkillsTanSat.Add(0);
            IdSkillsTanSat.Add(2);
            IdSkillsTanSat.Add(4);
        }
        private static int getItem(int id)
        {
            for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
            {
                Item item = Char.myCharz().arrItemBag[i];
                if (item != null && item.template.id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public static List<int> IdMobsTanSat = new List<int>();
        public static List<int> TypeMobsTanSat = new List<int>();
        private static readonly sbyte[] IdSkillsBase = new sbyte[]
        {
            0,
            2,
            17,
            4
        };
        public static List<sbyte> IdSkillsTanSat = new List<sbyte>(IdSkillsBase);

        private static Mob GetMobTanSat()
        {
            Mob result = null;
            int num = int.MaxValue;
            global::Char @char = global::Char.myCharz();
            for (int i = 0; i < GameScr.vMob.size(); i++)
            {
                Mob mob = (Mob)GameScr.vMob.elementAt(i);
                int num2 = (mob.xFirst - @char.cx) * (mob.xFirst - @char.cx) + (mob.yFirst - @char.cy) * (mob.yFirst - @char.cy);
                if (IsMobTanSat(mob) && num2 < num)
                {
                    result = mob;
                    num = num2;
                }
            }
            return result;
        }
        private static int GetManaUseSkill(Skill skill)
        {
            if (skill.template.manaUseType == 2)
            {
                return 1;
            }
            if (skill.template.manaUseType == 1)
            {
                return skill.manaUse * global::Char.myCharz().cMPFull / 100;
            }
            return skill.manaUse;
        }
        private static bool CanUseSkill(Skill skill)
        {
            if (mSystem.currentTimeMillis() - skill.lastTimeUseThisSkill > (long)skill.coolDown)
            {
                skill.paintCanNotUseSkill = false;
            }
            return (!skill.paintCanNotUseSkill && global::Char.myCharz().cMP >= GetManaUseSkill(skill));
        }
        private static bool IsSkillBetter(Skill SkillBetter, Skill skill)
        {
            if (SkillBetter == null)
            {
                return false;
            }
            if (!CanUseSkill(SkillBetter))
            {
                return false;
            }
            bool flag = (SkillBetter.template.id == 17 && skill.template.id == 2) || (SkillBetter.template.id == 9 && skill.template.id == 0);
            return skill == null || skill.coolDown < SkillBetter.coolDown || flag;
        }
        private static Skill GetSkillAttack()
        {
            Skill skill = null;
            SkillTemplate skillTemplate = new SkillTemplate();
            foreach (sbyte id in IdSkillsTanSat)
            {
                skillTemplate.id = id;
                Skill skill2 = global::Char.myCharz().getSkill(skillTemplate);
                if (IsSkillBetter(skill2, skill))
                {
                    skill = skill2;
                }
            }
            return skill;
        }
        public static long getTimeSkill(Skill s)
        {
            if (s.template.id == 20 || s.template.id == 22 || s.template.id == 7 || s.template.id == 18 || s.template.id == 23)
            {
                return (long)s.coolDown + 500L;
            }
            long num = (long)((double)s.coolDown * 1.2);
            if (num < 406L)
            {
                return 406L;
            }
            return num;
        }
        private static bool FilterMobTanSat(Mob mob)
        {
            return (IdMobsTanSat.Count == 0 || IdMobsTanSat.Contains(mob.mobId)) && (TypeMobsTanSat.Count == 0 || TypeMobsTanSat.Contains(mob.templateId));
        }
        private static bool IsMobTanSat(Mob mob)
        {
            if (mob.status == 0 || mob.status == 1 || mob.hp <= 0 || mob.isMobMe)
            {
                return false;
            }

            return FilterMobTanSat(mob);
        }
        #endregion
        #region wait
        private static bool IsWait;

        private static long TimeStartWait;

        private static long TimeWait;
        private static void Wait(int time)
        {
            IsWait = true;
            TimeStartWait = mSystem.currentTimeMillis();
            TimeWait = (long)time;
        }

        private static bool IsWaiting()
        {
            if (IsWait && mSystem.currentTimeMillis() - TimeStartWait >= TimeWait)
            {
                IsWait = false;
            }
            return IsWait;
        }
        #endregion
        #endregion
    }
}
