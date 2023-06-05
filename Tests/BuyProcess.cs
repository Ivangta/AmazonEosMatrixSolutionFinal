using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SeleniumExtras.WaitHelpers;
using AmazonEosMatrixSolution.Helpers;

namespace AmazonHarryPotter.Tests
{
    [TestFixture]
    public class BuyProcess : BaseTest
    {
        private Helpers _helper;
        [SetUp]
        public void Setup()
        {
            Initialize();
            Driver.Navigate().GoToUrl("https://www.amazon.co.uk/");
            Driver.FindElement(By.Id("sp-cc-accept")).Click();
            _helper = new Helpers();
        }

        [Test]
        public void AmazonBuyProcess()
        {
            //UA1
            var pageTitle = Driver.Title;
            var pageUrl = Driver.Url;
            Assert.AreEqual("Amazon.co.uk: Low Prices in Electronics, Books, Sports Equipment & more", pageTitle, "Page Title is incorrect!");
            Assert.AreEqual("https://www.amazon.co.uk/", pageUrl, "Page Url is incorrect!");

            //UA2
            // Locate the dropdown element
            IWebElement seachDropDown = Driver.FindElement(By.Id("searchDropdownBox"));
            seachDropDown.Click();

            // Create a new Select element
            SelectElement select = new SelectElement(seachDropDown);

            // Select an option by visible text
            select.SelectByText("Books");

            //no need
            //IWebElement booksElementFromDropDown = Driver.FindElement(By.XPath("//option[@value='search-alias=stripbooks']"));
            //booksElementFromDropDown.Click();

            IWebElement searchBooksField = Driver.FindElement(By.Id("twotabsearchtextbox"));
            searchBooksField.SendKeys("Harry Potter and the Cursed Child 1 & 2");

            IWebElement searchButton = Driver.FindElement(By.Id("nav-search-submit-button"));
            searchButton.Click();


            IList<IWebElement> elements = Driver.FindElements(By.XPath("//*[starts-with(@cel_widget_id, 'MAIN-SEARCH_RESULTS')]"));
            var firstSearchResult = elements.First();

            var titleOfBook = firstSearchResult.FindElement(By.XPath("//h2//span"));
            Assert.AreEqual("Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production", titleOfBook.Text);



            //IWebElement titleOfBook = Driver.FindElement(By.XPath("//span[normalize-space(text())='Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production']"));
            //Assert.AreEqual("Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production", titleOfBook.Text);

            //Need to fix locator
            //childElement = parentElement.FindElement(By.XPath(".//a[contains(text(), 'Paperback')]"));
            //change Paperback to other text
            Assert.IsNotNull(firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]")));

            //IWebElement editionOfBookFirst = Driver.FindElement(By.XPath("//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]/div[2]/div/div/div/div/div/div[2]/div/div/div[3]/div[1]/div/div[1]/div[1]/a"));
            //var editionFirst = editionOfBookFirst.Text;
            //Assert.AreEqual("Paperback", editionOfBookFirst.Text);

            Assert.IsNotNull(firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]/..//..//span[@class='a-price']//span[@class='a-offscreen']")));
            var paperBookPrice = firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]/..//..//span[@class='a-price']//span[@class='a-offscreen']")).Text;

            //Need to fix locator
            //IWebElement price = Driver.FindElement(By.XPath("//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]/div[2]/div/div/div/div/div/div[2]/div/div/div[3]/div[1]/div/div[1]/div[2]/a/span"));
            //var priceFirstFormatted = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(price.Text);
            //Assert.IsNotEmpty(priceFirstFormatted);
            //priceFirstFormatted.Contains("£");

            //this is the problem with cookies in Amazon Site
            titleOfBook.Click();

            //UA3   
            IWebElement titleOnProductDetailsPage = Driver.FindElement(By.Id("productTitle"));
            Wait.Until(ExpectedConditions.StalenessOf(titleOnProductDetailsPage));
            Assert.AreEqual(titleOnProductDetailsPage.Text, titleOfBook.Text);

            IWebElement newPrice = Driver.FindElement(By.XPath("//div[@id='formats']//div[@id='tmmSwatches']//span[contains(text(), 'Paperback')]//..//span[@class='a-size-base a-color-price a-color-price']"));
            var priceSecond = newPrice.Text;
            Assert.AreEqual(paperBookPrice, priceSecond);

            IWebElement editionOfBook = Driver.FindElement(By.XPath("//div[@id='formats']//div[@id='tmmSwatches']//span[contains(text(), 'Paperback')]"));
            var editionSecond = editionOfBook.Text;
            Assert.IsNotNull(editionOfBook);
            Assert.AreEqual("Paperback", editionSecond);
            editionOfBook.Click();

            IWebElement giftOption = Driver.FindElement(By.Id("gift-wrap"));
            if (!giftOption.Selected)
            {
                Wait.Until(ExpectedConditions.ElementToBeClickable(giftOption)).Click();
            }

            IWebElement addToBasketButton = Driver.FindElement(By.Id("add-to-cart-button"));
            addToBasketButton.Click();

            //UA4
            var basketPrice = Driver.FindElement(By.XPath("//div[@id='sw-atc-buy-box']//span[@class='a-offscreen']"));
            var basketPriceFormatted = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(basketPrice.Text);
            Assert.AreEqual(paperBookPrice, basketPriceFormatted);

            //this part only appears at times
            var giftCheck = Driver.FindElement(By.XPath("//div[@id='sw-gift-option']//input[@type='checkbox']"));
            Assert.IsTrue(giftCheck.Selected);

            var goToBasketButton = Driver.FindElement(By.XPath("//a[@data-csa-c-content-id='sw-gtc_CONTENT']"));
            goToBasketButton.Click();

            //UA5
            var titleShoppingBasket = Driver.FindElement(By.XPath("//div[@class='sc-item-content-group']//span[@class='a-truncate-cut']")).Text;
            Assert.AreEqual(titleOfBook.Text, titleShoppingBasket);

            var unitOfBooksInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-dropdown-prompt']"));
            var unitBooksShoppingBasket = unitOfBooksInShoppingBasket.Text;
            Assert.AreEqual("1", unitBooksShoppingBasket);

            var editionInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-size-small sc-product-binding a-text-bold']"));
            var editionThird = editionInShoppingBasket.Text;
            Assert.AreEqual(editionSecond, editionThird);

            var priceInShoppingBasket = Driver.FindElement(By.XPath("//span[@id='sc-subtotal-amount-buybox']//span"));
            var priceShoppingBasket = priceInShoppingBasket.Text;
            Assert.AreEqual(basketPriceFormatted, priceShoppingBasket);

            //Two asserts, if needed
            var subtotalCheckoutBasket = Driver.FindElement(By.Id("sc-subtotal-label-buybox"));
            var subtotalCheckoutBasketFormatted = _helper.RemoveNonNumericValuesUsingRegex(subtotalCheckoutBasket.Text);
            var subtotalShoppingBasket = Driver.FindElement(By.Id("sc-subtotal-label-activecart"));
            var subtotalShoppingBasketFormatted = _helper.RemoveNonNumericValuesUsingRegex(subtotalShoppingBasket.Text);
            Assert.AreEqual("1", subtotalCheckoutBasketFormatted);
            Assert.AreEqual("1", subtotalShoppingBasketFormatted);
        }

        [TearDown]
        public void TearDown()
        {
            Driver.Quit();
        }
    }
}
