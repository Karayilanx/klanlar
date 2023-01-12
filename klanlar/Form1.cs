using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using static System.Windows.Forms.Design.AxImporter;
using OpenQA.Selenium.Support.UI;
using Timer = System.Threading.Timer;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace klanlar
{
	public partial class Form1 : Form
	{
		IWebDriver driver = new ChromeDriver();
		Timer? kislaTimer;
		Timer? temizlikTimer;
		const string KISLA_FILE_PATH = "./klanlar.xml";
		static void RT(Action action, int seconds, CancellationToken token)
		{
			if (action == null)
				return;
			Task.Run(async () =>
			{
				while (!token.IsCancellationRequested)
				{
					action();
					await Task.Delay(TimeSpan.FromSeconds(seconds), token);
				}
			}, token);
		}

		public Form1()
		{
			InitializeComponent();
			getTexts();

			CheckForIllegalCrossThreadCalls = false;
			setStatusTexts();
			driver.Manage().Window.Maximize();
			driver.Navigate().GoToUrl("https://www.klanlar.org/");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				saveKislaTexts();
				if (kislaTimer != null)
				{
					kislaTimer.Dispose();
					kislaTimer = null;
				}
				else
				{
					var startTimeSpan = TimeSpan.Zero;
					var periodTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(kislaDakikaBox.Text));

					kislaTimer = new System.Threading.Timer((e) =>
					{
						askerBas();
					}, null, startTimeSpan, periodTimeSpan);
				}


				setStatusTexts();
			}
			catch (Exception ex)
			{
				appendText("Hata(Kýþla): " + ex.ToString(), true);
			}

		}

		public void askerBas()
		{
			try
			{
				driver.Navigate().GoToUrl(kislaLinkBox.Text.ToString());

				var spear = driver.FindElement(By.Name("spear"));
				if (mizrakBox.Text == "")
				{
					var total = driver.FindElement(By.Id("spear_0_a"));
					total.Click();
				}
				else if (mizrakBox.Text == "0")
				{

				}
				else
				{
					spear.SendKeys(mizrakBox.Text);
				}


				var sendBtn = driver.FindElements(By.ClassName("btn-recruit"))[0];
				appendText("Asker Basýldý", true);
				sendBtn.Click();
			}
			catch (Exception ex)
			{
				appendText("Hata(Kýþla): " + ex.ToString(), true);
			}
		}

		private void temizlemeButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (temizlikTimer != null)
				{
					temizlikTimer.Dispose();
					temizlikTimer = null;
				}
				else
				{
					var startTimeSpan = TimeSpan.Zero;
					var periodTimeSpan = TimeSpan.FromMinutes(5);

					temizlikTimer = new System.Threading.Timer((e) =>
					{
						temizlik();
					}, null, startTimeSpan, periodTimeSpan);
				}
				setStatusTexts();
			}
			catch (Exception ex)
			{
				appendText("Hata(Temizlik): " + ex.ToString(), true);
			}
		}

		void temizlik()
		{
			var ac = new Actions(driver);

			try
			{
				saveTemizlikTexts();
				driver.Navigate().GoToUrl(temizlemeLinkBox.Text.ToString());
				Thread.Sleep(1000);
				IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
				js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
				Thread.Sleep(500);
				var spear = driver.FindElement(By.Name("spear"));
				var options = driver.FindElements(By.ClassName("status-specific"));

				for (int i = 0; i < options.Count; i++)
				{
					var item = options[i];
					var res = item.FindElements(By.ClassName("free_send_button"));

					if (res.Count() > 0)
					{
						switch (i)
						{
							case 0:
								spear.SendKeys(birinciMizrakBox.Text);
								break;
							case 1:
								spear.SendKeys(ikinciMizrakBox.Text);
								break;
							case 2:
								spear.SendKeys(ucuncuMizrakBox.Text);
								break;
							default:
								break;
						}
						Thread.Sleep(500);
						res[0].Click();
						appendText((i + 1).ToString() + ". temizlik yapýldý", true);
						Thread.Sleep(5000);
					}
				}
			}
			catch (Exception ex)
			{
				appendText("Hata(Temizlik): " + ex.ToString(), true);
			}
		}

		void appendText(string text, bool showTime)
		{
			if (showTime)
			{
				var dateText = "\n[" + DateTime.Now.ToString() + "] ";
				text = dateText + text;
			}
			else
			{
				text = "\n" + text;
			}
			consoleBox.Text += text;
			consoleBox.SelectionStart = consoleBox.Text.Length;
			consoleBox.ScrollToCaret();
		}

		void setStatusTexts()
		{
			if (kislaTimer == null)
			{
				kislaStatus.Text = "Durdu";
			}
			else
			{
				kislaStatus.Text = "Çalýþýyor";
			}

			if (temizlikTimer == null)
			{
				temizleStatus.Text = "Durdu";
			}
			else
			{
				temizleStatus.Text = "Çalýþýyor";
			}
		}

		private void getTexts()
		{
			if (!File.Exists(KISLA_FILE_PATH))
			{
				XDocument doc = new XDocument(
					new XElement("root",
						new XElement("kisla_link", ""),
						new XElement("kisla_mizrak", ""),
						new XElement("kisla_dakika", ""),
						new XElement("temizlik_link", ""),
						new XElement("temizlik_bir", ""),
						new XElement("temizlik_iki", ""),
						new XElement("temizlik_uc", "")
					)
				);

				doc.Save(KISLA_FILE_PATH);
			}

			XmlDocument currentDoc = new XmlDocument();

			currentDoc.Load(KISLA_FILE_PATH);
			XmlNode kislaLinkNode = currentDoc.DocumentElement.SelectSingleNode("/root/kisla_link");
			kislaLinkBox.Text = kislaLinkNode.InnerText;
			XmlNode kislaMizrakNode = currentDoc.DocumentElement.SelectSingleNode("/root/kisla_mizrak");
			mizrakBox.Text = kislaMizrakNode.InnerText;
			XmlNode kislaDakikaNode = currentDoc.DocumentElement.SelectSingleNode("/root/kisla_dakika");
			kislaDakikaBox.Text = kislaDakikaNode.InnerText;

			XmlNode temizlikLinkNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_link");
			temizlemeLinkBox.Text = temizlikLinkNode.InnerText;
			XmlNode temizlikBirNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_bir");
			birinciMizrakBox.Text = temizlikBirNode.InnerText;
			XmlNode temizlikIkiNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_iki");
			ikinciMizrakBox.Text = temizlikIkiNode.InnerText;
			XmlNode temizlikUcNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_uc");
			ucuncuMizrakBox.Text = temizlikUcNode.InnerText;
		}

		private void saveKislaTexts()
		{
			XmlDocument currentDoc = new XmlDocument();
			currentDoc.Load(KISLA_FILE_PATH);

			XmlNode kislaLinkNode = currentDoc.DocumentElement.SelectSingleNode("/root/kisla_link");
			kislaLinkNode.InnerText = kislaLinkBox.Text;
			XmlNode kislaMizrakNode = currentDoc.DocumentElement.SelectSingleNode("/root/kisla_mizrak");
			kislaMizrakNode.InnerText = mizrakBox.Text;
			XmlNode kislaDakikaNode = currentDoc.DocumentElement.SelectSingleNode("/root/kisla_dakika");
			kislaDakikaNode.InnerText = kislaDakikaBox.Text;

			currentDoc.Save(KISLA_FILE_PATH);
		}

		private void saveTemizlikTexts()
		{
			XmlDocument currentDoc = new XmlDocument();
			currentDoc.Load(KISLA_FILE_PATH);

			XmlNode temizlikLinkNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_link");
			temizlikLinkNode.InnerText = temizlemeLinkBox.Text;
			XmlNode temizlikBirNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_bir");
			temizlikBirNode.InnerText = birinciMizrakBox.Text;
			XmlNode temizlikIkiNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_iki");
			temizlikIkiNode.InnerText = ikinciMizrakBox.Text;
			XmlNode temizlikUcNode = currentDoc.DocumentElement.SelectSingleNode("/root/temizlik_uc");
			temizlikUcNode.InnerText = ucuncuMizrakBox.Text;

			currentDoc.Save(KISLA_FILE_PATH);
		}
	}
}