using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using static System.Windows.Forms.Design.AxImporter;
using OpenQA.Selenium.Support.UI;
using Timer = System.Threading.Timer;
using OpenQA.Selenium.Interactions;

namespace klanlar
{
    public partial class Form1 : Form
    {
        IWebDriver driver = new ChromeDriver();
        Timer? kislaTimer;
        Timer? temizlikTimer;
        static void RT(Action action, int seconds, CancellationToken token)
        {
            if (action == null)
                return;
            Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    action();
                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                }
            }, token);
        }

        public Form1()
        {
            InitializeComponent();//            

            //options.AddArgument("--disable-extensions");
            //options.AddExcludedArgument("excludeSwitches");
            //options.AddAdditionalOption("useAutomationExtension",false);

            Control.CheckForIllegalCrossThreadCalls = false;
            setStatusTexts();
            driver.Navigate().GoToUrl("https://www.klanlar.org/");
            //var user = driver.FindElement(By.Id("user")); 
            //var pass = driver.FindElement(By.Id("password"));
            //var loginBtn = driver.FindElement(By.ClassName("btn-login"));
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(5000);

            //user.SendKeys("ebuzerkara");
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(5000);

            //pass.SendKeys("efegur017");
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(5000);

            //loginBtn.Click();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
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
                consoleBox.Text += "\n" + "Hata(Kýþla): " + ex.ToString();
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
                consoleBox.Text += "\n" + "Asker Basýldý";
                sendBtn.Click();
            }
            catch (Exception ex)
            {
                consoleBox.Text += "\n" + "Hata(Kýþla): " + ex.ToString();
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
                consoleBox.Text +=  "\n" + "Hata(Temizlik): " + ex.ToString();
            }
        }

        void temizlik()
        {
            var ac = new Actions(driver);

            try
            {
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
                        consoleBox.Text += "\n" + (i+1).ToString() + ". temizlik yapýldý";
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                consoleBox.Text += "\n" + "Hata(Temizlik): " + ex.ToString();
            }
        }

        void setStatusTexts()
        {
            if(kislaTimer == null)
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
    }
}