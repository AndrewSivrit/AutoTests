using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System.Linq;
using OpenQA.Selenium.IE;
using NUnit.Framework.Interfaces;
using System.Drawing.Imaging;

namespace Selenium.Test
{
  
    [TestFixture]
    public class BrowserTests
    {
        private static IWebDriver driver;
        private static HelperTest helperTest;
        string mainURL;
        string homeUrl;
        string authUrl;
        private string password;
        private string login;

        [SetUp]
        public void SetUp()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathDrivers = directory + "/../../../drivers";
            mainURL = "https://v2dev.cascade-usa.com/";
            homeUrl = "https://v2dev.cascade-usa.com/home";
            authUrl = "https://v2dev.cascade-usa.com/auth/login";
            password = "apolo3000";
            login = "nikitin_andrew@bk.ru";

            helperTest = new HelperTest();

            driver = new ChromeDriver(pathDrivers);
            //driver = new InternetExplorerDriver(pathDrivers);

            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Window.Maximize();

            //driver.Url = "https://v2dev.cascade-usa.com/auth/login";
            //driver.Manage().Window.Maximize();
            //_edge = new EdgeDriver(pathDrivers);
            //_firefox = new FirefoxDriver(pathDrivers);
        }

        [Test]
        public void Login()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            Thread.Sleep(4000);
            Assert.AreEqual(homeUrl, driver.Url);
            IWebElement ClickUser = driver.FindElement(By.Id("username_button"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(ClickUser).Build().Perform();

            var LogOut = driver.FindElement(By.Id("logout_button"));

            LogOut.Click();

            Assert.AreEqual(authUrl, driver.Url);
        }

        [Test]
        public void LoginWrongCreds()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, "sdddf");

            Thread.Sleep(4000);

            Assert.AreEqual(authUrl, driver.Url);

        }

        [Test]
        public void SearchPopProduct()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            Thread.Sleep(4000);

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("Knee");

            Thread.Sleep(4000);

            IWebElement Product = driver.FindElement(By.Id("product_name"));
            Product.Click();
            Thread.Sleep(2000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=19784", driver.Url);
            Assert.AreEqual("UI2", driver.Title);

        }

        public void InputAndCheckAdd(string productId, int num, string nameCheck) {

            string numInput = "input-" + num.ToString();
            IWebElement InpBox = driver.FindElement(By.Id(numInput));
            InpBox.SendKeys(productId);

            driver.FindElement(By.Id(numInput)).SendKeys(Keys.Enter);

            Thread.Sleep(7000);

            String bodyText = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyText.Contains(nameCheck));

        }

        [Test]
        public void QuickOrderAndDeleteFromCart()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            Thread.Sleep(4000);

            IWebElement QuickOrderBtn = driver.FindElement(By.Id("toggleQuickOrder"));
            QuickOrderBtn.Click();
            Thread.Sleep(1000);

            IWebElement ele2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-quick-order-pad/div[1]/div[3]/app-tag-button/span/span"));
            IJavaScriptExecutor executor2 = (IJavaScriptExecutor)driver;
            executor2.ExecuteScript("arguments[0].click();", ele2);
            executor2.ExecuteScript("arguments[0].click();", ele2);

            InputAndCheckAdd("1210", 0, "Grace Plate");
            InputAndCheckAdd("3245", 1, "Profile Orthosis");
            InputAndCheckAdd("1211", 2, "Splint SM Left");
            InputAndCheckAdd("1212", 3, "Splint MD Left");
            InputAndCheckAdd("3243", 4, "Profile Orthosis XL");
      
            InputAndCheckAdd("1213", 5, "Splint LG Left");
            InputAndCheckAdd("1213", 6, "Splint LG Left");
            InputAndCheckAdd("3241", 7, "Orthosis MD");
            InputAndCheckAdd("3239", 8, "Orthosis XS");
            InputAndCheckAdd("3231", 9, "Orthosis 10");

            Thread.Sleep(1000);

            IWebElement ele = driver.FindElement(By.XPath("/html/body/app-root/div/app-quick-order-pad/div[1]/div[3]/div/app-button/div/button"));
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", ele);

            Thread.Sleep(1000);

            IWebElement CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/cart/index", driver.Url);

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

            String bodyTextCart2 = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextCart2.Contains("Grace Plate"));
            Assert.IsTrue(bodyTextCart2.Contains("Profile Orthosis"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint SM Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint MD Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Profile Orthosis XL"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint LG Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint LG Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Orthosis MD"));
            Assert.IsTrue(bodyTextCart2.Contains("Orthosis XS"));
            Assert.IsTrue(bodyTextCart2.Contains("Orthosis 10"));
        }


        [Test]
        public void SubmitOrder()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);
            
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=10627";

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=10627", driver.Url);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");

            String bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Evolution Valve Seal"));

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(5000);

            driver.Url = "https://v2dev.cascade-usa.com/product?productID=1766";

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=1766", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/div[2]/span");

            helperTest.JsClickElementId(driver, "accessory_add_to_cart_5");
            
            Thread.Sleep(20000);

            driver.Url = "https://v2dev.cascade-usa.com/product?productID=1483";

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=1483", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 5);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[5]/select", 4);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[6]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[7]/select", 3);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/div[2]/links/ul/li[2]/a/i");

            helperTest.JsClickElementId(driver, "header_cart_icon");
            

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");

            Assert.AreEqual("https://v2dev.cascade-usa.com/cart/index", driver.Url);

            helperTest.FindTextInBody(driver, "ALL-5568-E");
            helperTest.FindTextInBody(driver, "CD103WH");
            helperTest.FindTextInBody(driver, "CD103");
            helperTest.FindTextInBody(driver, "VV03-LLS SM");

            IWebElement InpBox = driver.FindElement(By.Id("poNumber"));
            InpBox.Clear();
            InpBox.SendKeys("Test");

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Order " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cartundefined");

            Assert.AreEqual("https://v2dev.cascade-usa.com/cart/review", driver.Url);

            Thread.Sleep(3000);
            helperTest.FindTextInBody(driver, "ALL-5568-E");
            helperTest.FindTextInBody(driver, "CD103WH");
            helperTest.FindTextInBody(driver, "CD103");
            helperTest.FindTextInBody(driver, "VV03-LLS SM");

            Thread.Sleep(2000);

            helperTest.JsClickElementId(driver, "submit_order");

            Thread.Sleep(60000);

            helperTest.FindTextInBody(driver, "Your order number is");

            helperTest.JsClickElement(driver, "//*[text()='" + " OK " + "']");

            Thread.Sleep(11000);

        }

        [Test]
        public void submitRMAs()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = "https://v2dev.cascade-usa.com/shopping/order-history?tab=orders&page=1";

            Thread.Sleep(1000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/shopping/order-history?tab=orders&page=1", driver.Url);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/section/section/app-history-order-item[1]/article/article/section/div[2]/section[1]/div[2]/div[2]/app-button[2]/div/button");


            helperTest.InputStringId(driver, "123456", "rma_patientID");
            helperTest.InputStringId(driver, "1234567890", "rma_serialNumbers");

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[4]/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[5]/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[6]/div[2]/select", 5);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[11]/div[2]/select", 3);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[7]/div[2]/input");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/mat-calendar-header/div/div/button[1]/span");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-multi-year-view/table/tbody/tr[2]/td[1]/div");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-year-view/table/tbody/tr[2]/td[1]/div");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-month-view/table/tbody/tr[1]/td[2]/div");

            helperTest.InputStringXpath(driver, "Broken", "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[8]/div[2]/textarea");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[9]/div[2]/input");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/mat-calendar-header/div/div/button[1]/span");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-multi-year-view/table/tbody/tr[2]/td[1]/div");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-year-view/table/tbody/tr[2]/td[1]/div");
            helperTest.JsClickElement(driver, "/html/body/div[4]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-month-view/table/tbody/tr[1]/td[2]/div");

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[11]/div[2]/select", 3);

            helperTest.InputStringXpath(driver, "This is a test", "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[12]/div[2]/textarea");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[2]/div[2]/button");

            Thread.Sleep(5000);

            helperTest.FindTextInBody(driver, "Success");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Submit for Return " + "']");

            Thread.Sleep(30000);

            helperTest.FindTextInBody(driver, "Thank you for your submission");
            
        }

        [Test]
        public void Step02()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.SendKeys("tlso 464");

            helperTest.JsClickElementId(driver, "product-card-img");
            Thread.Sleep(2000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=8707", driver.Url);

            helperTest.InputStringId(driver, "2", "qty_product_page");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("993640"));
            
            helperTest.InputStringId(driver, "patient 1", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for 993640", "notes_in_cart0");
        }

        [Test]
        public void Step03_4()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;


            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("alpha classic");

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[3]");

            IWebElement ClickUser = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[3]"));

            actions = new Actions(driver);
            actions.MoveToElement(ClickUser).Build().Perform();

            helperTest.JsClickElementId(driver, "product-card-img");
            

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=1483", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 4);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[5]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[6]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[7]/select", 3);

            helperTest.InputStringId(driver, "2", "qty_product_page");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("ALL-5066-E"));

            helperTest.InputStringId(driver, "patient 2", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item ALL-5066-E", "notes_in_cart0");

            helperTest.JsClickElementId(driver, "product-name-in-cart0");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=1483", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 4);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[5]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[6]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[7]/select", 3);

            helperTest.InputStringId(driver, "2", "qty_product_page");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("ALC-5066-E"));

            helperTest.InputStringId(driver, "patient 3", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item ALC-5066-E", "notes_in_cart0");

        }

        [Test]
        public void Step05()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;


            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            // 5 step

            SearchBox.Clear();
            SearchBox.SendKeys("Alpha Hybrid® Liners");

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[2]");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[2]"));

            actions = new Actions(driver);
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElementId(driver, "product-card-img");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=7158", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", 4);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("H352-6396"));

            helperTest.InputStringId(driver, "patient 4", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item H352-6396", "notes_in_cart0");

        }

        [Test]
        public void Step06()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;


            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            // 6 step
            SearchBox.Clear();
            SearchBox.SendKeys("aspen summit");

            Thread.Sleep(3000);
            
            helperTest.waitElementXpath(driver, 60, "//*[text()='" + " Aspen® Summit™ 637 " + "']");
            NavigateCusror = driver.FindElement(By.XPath("//*[text()='" + " Aspen® Summit™ 637 " + "']"));
            
            actions = new Actions(driver);
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElementId(driver, "product-card-img");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=7199", driver.Url);

            helperTest.JsClickElementId(driver, "add_to_cart_product_page4");

            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("992310"));

            helperTest.InputStringId(driver, "patient 5", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 992310", "notes_in_cart0");

        }

        [Test]
        public void Step07()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            // 7 step
            Thread.Sleep(3000);
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=7158";

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", 6);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", 4);

            //add to cart
            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(3000);
            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("H352-6394"));

            helperTest.InputStringId(driver, "patient 6", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item H352-6394", "notes_in_cart0");

        }

       // [Test]
        public void Step08()
        {


            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            // 8 step
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("lyn valve");

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/div/p[2]/span[4]");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/div/p[2]/span[4]"));

            actions = new Actions(driver);
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElementId(driver, "product-card-img");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=19094", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);

            //add to cart
            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            IWebElement CartBtn7 = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn7.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            String bodyTextProduct7 = driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/section/article/div[2]/app-cart-product-order/section/article[1]/app-product-card/article/div[2]/p[2]")).Text;
            Assert.IsTrue(bodyTextProduct7.Contains("PA0002"));

            helperTest.InputStringId(driver, "patient 7", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item PA0002", "notes_in_cart0");

        }

        [Test]
        public void Step09()
        {


            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            // 8 step
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            // 9 step
            SearchBox.Clear();
            SearchBox.SendKeys("limblogic");

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/div/p[2]/span[1]");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/div/p[2]/span[1]"));

            actions = new Actions(driver);
            actions.MoveToElement(NavigateCusror).Build().Perform();

            // click to image of product
            helperTest.JsClickElementId(driver, "product-card-img");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=3723", driver.Url);

            //add to cart
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article[1]/div[6]/app-button/div/button");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("LLV-2000-L"));

            helperTest.InputStringId(driver, "patient 8", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item LLV-2000-L", "notes_in_cart0");

        }

        [Test]
        public void Step10()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            // 8 step
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));


            // 10 step
            Thread.Sleep(3000);
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=14278";

            //add to cart
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article[1]/div[6]/app-button/div/button");


            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("PK2000-320-05"));

            helperTest.InputStringId(driver, "patient 9", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item PK2000-320-05", "notes_in_cart0");

        }

        [Test]
        public void Step11()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;


            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("tamarack");

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[2]");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[2]"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/p[1]");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=502", driver.Url);

            helperTest.InputStringXpath(driver, "740-L", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            Thread.Sleep(2000);
            helperTest.InputStringId(driver, "10", "qty_in_product_page0");

            helperTest.JsClickElementId(driver, "add_to_cart_product_page0");


            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("740-L"));

            helperTest.InputStringId(driver, "patient 10", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 740-L", "notes_in_cart0");

        }

        [Test]
        public void Step12()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);
            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));


            SearchBox.SendKeys("knee");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[1]/app-filter-panel/div[1]/div/mdb-card/div/mdb-card-body/div/mdb-card-text/p/div[1]/div/div[6]/p/span[1]");

            helperTest.JsClickElement(driver, "//*[text()='" + " Ossur (54) " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Balance™ Knee" + "']");

            Thread.Sleep(5000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=8911", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);

            Thread.Sleep(2000);
            //add to cart
            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");
            
            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("BKN12511"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 11", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12511", "notes_in_cart0");



        }

        [Test]
        public void Step13()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;


            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("60SL");
            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/p[1]");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=6646", driver.Url);

            helperTest.InputStringId(driver, "5", "qty_in_product_page1");
            helperTest.JsClickElementId(driver, "add_to_cart_product_page1");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("60SL"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 12", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 60SL", "notes_in_cart0");


        }

        [Test]
        public void Step14()
        {

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/a/span/i");

            helperTest.JsClickElement(driver, "//*[text()='" + " Orthotics " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + " Spinal (721) " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Prefabricated Orthoses" + "']");

            helperTest.JsClickElement(driver, "//*[text()='" + " Aspen Medical Products (28) " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Summit™ 456" + "']");

            Thread.Sleep(5000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=8080", driver.Url);

            helperTest.InputStringId(driver, "2", "qty_in_product_page6");
            helperTest.JsClickElementId(driver, "add_to_cart_product_page6");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("992710"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 13", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 992710", "notes_in_cart0");

        }

        [Test]
        public void Step15()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("acrylic resin");
            Thread.Sleep(3000);

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[1]/div/p[2]/span[2]");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[1]/div/p[2]/span[2]"));

            actions = new Actions(driver);
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElementId(driver, "product-card-img");

            Thread.Sleep(5000);

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=8580", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("EAR1"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 13", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item EAR1", "notes_in_cart0");

        }

        [Test]
        public void Step16()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);
            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            //search text
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("Ossur");

            Thread.Sleep(3000);
            helperTest.JsClickElement(driver, "//*[text()='" + " Ossur " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Prosthetics" + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Lower Extremity" + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Knee Prosthetics" + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Balance™ Knee OFM2" + "']");

            Thread.Sleep(3000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=15676", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("1721120"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 15", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 1721120", "notes_in_cart0");
            helperTest.InputStringId(driver, "2", "qty-in-cart0");

            Thread.Sleep(5000);
        }

        [Test]
        public void Step17()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            //search text
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("993740");

            Thread.Sleep(3000);

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.InputStringId(driver, "2", "qty_in_dropdown");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(3000);
            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("993740"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 16", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 993740", "notes_in_cart0");

            Thread.Sleep(5000);
        }

        [Test]
        public void Step19()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            //search text
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("tamarack");

            Thread.Sleep(3000);

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " Preview " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "View Product Details" + "']");

            Thread.Sleep(3000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=502", driver.Url);

            helperTest.InputStringXpath(driver, "740-M", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            Thread.Sleep(2000);
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article[2]/div[6]/app-button/div/button");

            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("740-M"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 17", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 740-M", "notes_in_cart0");

            Thread.Sleep(5000);
        }

        [Test]
        public void Step20()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            Thread.Sleep(3000);
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=9975";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article[1]/div[6]/app-button/div/button");
            
            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("08814"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 19", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 08814", "notes_in_cart0");

            Thread.Sleep(5000);

        }

        [Test]
        public void Step21()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            Thread.Sleep(3000);
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=14609";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[1]/div/mdb-breadcrumb/ol/mdb-breadcrumb-item[2]/li");
            
            Thread.Sleep(3000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/category/catalogsearch/configurable?searchBy=category&queryStr=14&categoryName=Cervical&viewMode=configurable&categories=%7B%22ORTHOTICS%22:0,%22CERVICAL%22:0%7D", driver.Url);


            helperTest.JsClickElement(driver, "//*[text()='" + " Aspen Medical Products (9) " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Vista® CTO4" + "']");

            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=14609", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("984550"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 20", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 984550", "notes_in_cart0");

            Thread.Sleep(5000);

        }

        [Test]
        public void Step22()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/a/span/i");
            helperTest.JsClickElement(driver, "//*[text()='" + " Prosthetics " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + " Liners / Suspension (764) " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + " L5679 (81) " + "']");

            helperTest.JsClickElement(driver, "//*[text()='" + "Alpha Hybrid® Liners" + "']");

            Thread.Sleep(3000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=7158", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", 6);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", 3);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");
            
            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("H351-5364"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 21", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item H351-5364", "notes_in_cart0");
            
            Thread.Sleep(5000);
        }

        [Test]
        public void Step23()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.SendKeys("vista collar w/pads set");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Vista® Collar" + "']");

            Thread.Sleep(3000);
            Assert.AreEqual("https://v2dev.cascade-usa.com/product?productID=3337", driver.Url);

            helperTest.InputStringId(driver, "5", "qty_in_product_page1");
            helperTest.JsClickElementId(driver, "add_to_cart_product_page1");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");

            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("984000"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 22", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 984000", "notes_in_cart0");

            Thread.Sleep(5000);
        }

        [Test]
        public void Step24()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            Thread.Sleep(3000);
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=8706";

            helperTest.InputStringId(driver, "5", "qty_product_page");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");

            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("993540"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 23", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 993540", "notes_in_cart0");

            Thread.Sleep(5000);

        }

        [Test]
        public void Step25()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.SendKeys("c1l");

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[1]");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[1]"));
            actions = new Actions(driver);
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/div[2]/div[2]/span/app-button/div/button");

            Thread.Sleep(3000);

            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("C1L"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 24", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item C1L", "notes_in_cart0");

            Thread.Sleep(5000);
        }

        [Test]
        public void Step26()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(3000);

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            Thread.Sleep(3000);
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=1274";

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");
            
            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("700-250"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 25", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 700-250", "notes_in_cart0");

            Thread.Sleep(5000);

        }

        [Test]
        public void Step27()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.SendKeys("sprystep");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.JsClickElement(driver, "//*[text()='" + "Townsend SpryStep®" + "']");

            Thread.Sleep(3000);
            driver.Url = "https://v2dev.cascade-usa.com/product?productID=15363";

            helperTest.InputStringXpath(driver, "1001", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article/div[6]/app-button/div/button");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            Thread.Sleep(3000);
            //go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("17H1001"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 26", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 17H1001", "notes_in_cart0");

            Thread.Sleep(5000);
        }

        [Test]
        public void Step28()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            IWebElement QuickOrderBtn = driver.FindElement(By.Id("toggleQuickOrder"));
            QuickOrderBtn.Click();
            Thread.Sleep(1000);

            InputAndCheckAdd("289221012", 0, "");
            InputAndCheckAdd("17H2001"  , 1, "");
            InputAndCheckAdd("H350-6396", 2, "");
            InputAndCheckAdd("453A3=2-7", 3, "");
            InputAndCheckAdd("993720"   , 4, "");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-quick-order-pad/div[1]/div[3]/app-tag-button/span/span");
            
            InputAndCheckAdd("984002"   , 5, "");
            InputAndCheckAdd("SFX26-3G" , 6, "");
            InputAndCheckAdd("993730"   , 7, "");
            InputAndCheckAdd("289222012", 8, "");
            InputAndCheckAdd("CD103"    , 9, "");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-quick-order-pad/div[1]/div[3]/app-tag-button/span/span");

            InputAndCheckAdd("3662"     , 10, "");
            InputAndCheckAdd("AKL-2638-X",11, "");
            InputAndCheckAdd("984202"   , 12, "");
            InputAndCheckAdd("1210"     , 13, "");
            InputAndCheckAdd("2100"     , 14, "");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-quick-order-pad/div[1]/div[3]/app-tag-button/span/span");

            InputAndCheckAdd("CMR-G"    , 15, "");
            InputAndCheckAdd("1SPSRGSH" , 16, "");
        }

        [Test]
        public void AllStepsExtendedShopCart()
        {
            Step02();
            Step03_4();
            Step05();
            Step06();
            Step07();
            //Step08();
            Step09();
            Step10();
            Step11();
            Step12();
            Step13();
            Step14();
            Step15();
            Step16();
            Step17();
            //Step18();
            Step19();
            Step20();
            Step21();
            Step22();
            Step23();
            Step24();
            Step25();
            Step26();
            Step27();
            Step28();


        }

        [TearDown]
        public void Cleanup()
        {
           driver?.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome != ResultState.Success)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(@"Screenshot.jpg", ScreenshotImageFormat.Jpeg);
            }
        }
    }
}
