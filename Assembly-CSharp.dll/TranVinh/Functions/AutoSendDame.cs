using System;

namespace TranVinh.Functions
{
    // Token: 0x020000B6 RID: 182
    internal class AutoSendDame
    {
        // Token: 0x060008E7 RID: 2279 RVA: 0x00073770 File Offset: 0x00071970
        public static void Ak()
        {
            if (global::Char.myCharz().meDead || global::Char.myCharz().statusMe == 14 || global::Char.myCharz().statusMe == 5 || global::Char.myCharz().myskill.template.type == 3 || global::Char.myCharz().myskill.template.id == 10 || global::Char.myCharz().myskill.template.id == 11 || global::Char.myCharz().myskill.paintCanNotUseSkill)
            {
                return;
            }
            int skill = AutoSendDame.getSkill();
            if (mSystem.currentTimeMillis() - AutoSendDame.currTimeAK[skill] > AutoSendDame.getTimeSkill(global::Char.myCharz().myskill))
            {
                if (GameScr.gI().isMeCanAttackMob(global::Char.myCharz().mobFocus))
                {
                    global::Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
                    Service.gI().sendPlayerAttack(Char.myCharz().mobFocus);
                    AutoSendDame.currTimeAK[skill] = mSystem.currentTimeMillis();
                    return;
                }
                if (global::Char.myCharz().charFocus != null && AutoSendDame.isAttackChar(global::Char.myCharz().charFocus))
                {
                    global::Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
                    Service.gI().sendPlayerAttack(Char.myCharz().charFocus);
                    AutoSendDame.currTimeAK[skill] = mSystem.currentTimeMillis();
                }
            }
        }

        // Token: 0x060008E8 RID: 2280 RVA: 0x000738A0 File Offset: 0x00071AA0
        public static bool isAttackChar(global::Char c)
        {
            if (TileMap.mapID == 113)
            {
                return c != null && global::Char.myCharz().myskill != null && (c.cTypePk == 5 || c.cTypePk == 3);
            }
            if (c != null && global::Char.myCharz().myskill != null && c.statusMe != 14 && c.statusMe != 5 && global::Char.myCharz().myskill.template.type != 2)
            {
                if ((global::Char.myCharz().cFlag != 8 || c.cFlag == 0) && (global::Char.myCharz().cFlag == 0 || c.cFlag != 8) && (global::Char.myCharz().cFlag == c.cFlag || global::Char.myCharz().cFlag == 0 || c.cFlag == 0) && (c.cTypePk != 3 || global::Char.myCharz().cTypePk != 3) && global::Char.myCharz().cTypePk != 5 && c.cTypePk != 5 && (global::Char.myCharz().cTypePk != 1 || c.cTypePk != 1))
                {
                    if (global::Char.myCharz().cTypePk != 4)
                    {
                        return global::Char.myCharz().myskill.template.type == 2 && c.cTypePk != 5;
                    }
                    if (c.cTypePk != 4)
                    {
                        return global::Char.myCharz().myskill.template.type == 2 && c.cTypePk != 5;
                    }
                }
                return true;
            }
            return false;
        }


        // Token: 0x060008EB RID: 2283 RVA: 0x00073AC4 File Offset: 0x00071CC4
        public static int getSkill()
        {
            for (int i = 0; i < GameScr.keySkill.Length; i++)
            {
                if (GameScr.keySkill[i] == global::Char.myCharz().myskill)
                {
                    return i;
                }
            }
            return 0;
        }

        // Token: 0x060008EC RID: 2284 RVA: 0x00073AFC File Offset: 0x00071CFC
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

        // Token: 0x0400101D RID: 4125
        public static long[] currTimeAK = new long[8];
    }
}
