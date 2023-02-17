public class SplashScr : mScreen
{
	public static int splashScrStat;

	private bool isCheckConnect;

	private bool isSwitchToLogin;

	public static int nData = -1;

	public static int maxData = -1;

	public static SplashScr instance;

	public static Image imgLogo;

	public SplashScr()
	{
		instance = this;
	}

	public static void loadSplashScr()
	{
		splashScrStat = 0;
	}

	public override void update()
	{
		if (splashScrStat == 30 && !isCheckConnect)
		{
			isCheckConnect = true;
			SoundMn.gI().getStrOption();
			//ServerListScreen.loadIP();
			GameCanvas.connect();
		}
		splashScrStat++;
		//ServerListScreen.updateDeleteData();
	}

	

	public override void paint(mGraphics g)
	{
		if (imgLogo != null && splashScrStat < 30)
		{
			g.setColor(16777215);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			g.drawImage(imgLogo, GameCanvas.w / 2, GameCanvas.h / 2, 3);
		}
		if (nData != -1)
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			g.drawImage(LoginScr.imgTitle, GameCanvas.w / 2, GameCanvas.h / 2 - 24, StaticObj.BOTTOM_HCENTER);
			GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.h / 2 + 24, g);
			mFont.tahoma_7b_white.drawString(g, mResources.downloading_data + nData * 100 / maxData + "%", GameCanvas.w / 2, GameCanvas.h / 2, 2);
		}
		else if (splashScrStat >= 30)
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.hh, g);
			if (ServerListScreen.cmdDeleteRMS != null)
			{
				mFont.tahoma_7_white.drawString(g, mResources.xoadulieu, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
			}
		}
	}

	public static void loadImg()
	{
		imgLogo = GameCanvas.loadImage("/gamelogo.png");
	}
}
