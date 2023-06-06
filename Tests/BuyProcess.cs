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
            IWebElement seachDropDown = Driver.FindElement(By.Id("searchDropdownBox"));
            seachDropDown.Click();
            SelectElement select = new SelectElement(seachDropDown);
            select.SelectByText("Books");

            IWebElement searchBooksField = Driver.FindElement(By.Id("twotabsearchtextbox"));
            searchBooksField.SendKeys("Harry Potter and the Cursed Child 1 & 2");

            IWebElement searchButton = Driver.FindElement(By.Id("nav-search-submit-button"));
            searchButton.Click();


            IList<IWebElement> elements = Driver.FindElements(By.XPath("//*[starts-with(@cel_widget_id, 'MAIN-SEARCH_RESULTS')]"));
            var firstSearchResult = elements.First();

            var editionOfBookInBookSectionElement = firstSearchResult.FindElement(By.XPath("//h2//span"));
            var editionOfBookInBookSectionText = editionOfBookInBookSectionElement.Text;
            Assert.AreEqual("Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production", editionOfBookInBookSectionText, "Title of book is incorrect in books section!");

            Assert.IsNotNull(firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]")),"Edition version is not paperback in books section!");
            Assert.IsNotNull(firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]/..//..//span[@class='a-price']")), "There is no price on the book in books section!");

            var paperBookPriceInBookSectionElement = firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]/..//..//span[@class='a-price']")).Text;
            var paperBookPriceInBookSectionText = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(paperBookPriceInBookSectionElement);

            //this is the problem with cookies in Amazon Site
            editionOfBookInBookSectionElement.Click();

            //UA3   
            Wait.Until(ExpectedConditions.ElementExists(By.Id("productTitle")));
            var editionOnProductDetailsPage = Driver.FindElement(By.Id("productTitle")).Text;
            Assert.AreEqual(editionOnProductDetailsPage, editionOfBookInBookSectionText, "Edition is not the same at product details page!");

            var paperBookPriceProductDetailsPage = Driver.FindElement(By.XPath("//div[@id='formats']//div[@id='tmmSwatches']//span[contains(text(), 'Paperback')]//..//span[@class='a-size-base a-color-price a-color-price']")).Text;
            Assert.AreEqual(paperBookPriceInBookSectionText, paperBookPriceProductDetailsPage, "Price is not the same at product details page!");

            IWebElement editionVersionProductDetailsPageElement = Driver.FindElement(By.XPath("//div[@id='formats']//div[@id='tmmSwatches']//span[contains(text(), 'Paperback')]"));
            var editionVersionProductDetailsPageText = editionVersionProductDetailsPageElement.Text;
            Assert.IsNotNull(editionVersionProductDetailsPageElement);
            Assert.AreEqual("Paperback", editionVersionProductDetailsPageText, "Edition version is not the same at product details page!");
            if (!editionVersionProductDetailsPageElement.Selected)
            {
                Wait.Until(ExpectedConditions.ElementToBeClickable(editionVersionProductDetailsPageElement)).Click();
            }           

            IWebElement addToBasketButton = Driver.FindElement(By.Id("add-to-cart-button"));
            addToBasketButton.Click();

            //UA4
            var paperBookBasketAdditionPriceElement = Driver.FindElement(By.XPath("//div[@id='sw-atc-buy-box']//span[@class='a-price sw-subtotal-amount']"));
            var paperBookBasketAdditionPriceText = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(paperBookBasketAdditionPriceElement.Text);
            Assert.AreEqual(paperBookPriceProductDetailsPage, paperBookBasketAdditionPriceText, "Price is not the same at addition to basket!");

            //this part only appears at times
            var giftCheck = Driver.FindElement(By.XPath("//div[@id='sw-gift-option']//input[@type='checkbox']"));
            if (!giftCheck.Selected)
            {
                Wait.Until(ExpectedConditions.ElementToBeClickable(giftCheck)).Click();
            }
            Assert.IsTrue(giftCheck.Selected,"Gift option is not selected in basket addition page!");

            var basketCheckoutButton = Driver.FindElement(By.XPath("//a[@data-csa-c-content-id='sw-gtc_CONTENT']"));
            basketCheckoutButton.Click();

            //UA5
            var editionShoppingBasket = Driver.FindElement(By.XPath("//div[@class='sc-item-content-group']//span[@class='a-truncate-cut']")).Text;
            Assert.AreEqual(editionOnProductDetailsPage, editionShoppingBasket, "Edition is not the same at shopping basket page!");

            var unitOfBooksInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-dropdown-prompt']")).Text;
            Assert.AreEqual("1", unitOfBooksInShoppingBasket, "Unit of books is different than 1 at shopping basket page!");

            var editionVersionInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-size-small sc-product-binding a-text-bold']")).Text;
            Assert.AreEqual(editionVersionProductDetailsPageText, editionVersionInShoppingBasket, "Edition version is not the same at shopping basket page!");

            var PaperBookPriceInShoppingBasket = Driver.FindElement(By.XPath("//span[@id='sc-subtotal-amount-buybox']//span")).Text;
            Assert.AreEqual(paperBookBasketAdditionPriceText, PaperBookPriceInShoppingBasket, "Price is not the same at shopping basket page!");

            //Two asserts, if needed
            var subtotalCheckoutBasket = Driver.FindElement(By.Id("sc-subtotal-label-buybox"));
            var subtotalCheckoutBasketFormatted = _helper.RemoveNonNumericValuesUsingRegex(subtotalCheckoutBasket.Text);
            var subtotalShoppingBasket = Driver.FindElement(By.Id("sc-subtotal-label-activecart"));
            var subtotalShoppingBasketFormatted = _helper.RemoveNonNumericValuesUsingRegex(subtotalShoppingBasket.Text);
            Assert.AreEqual("1", subtotalCheckoutBasketFormatted, "Subtotal at checkout basket is different than 1!");
            Assert.AreEqual("1", subtotalShoppingBasketFormatted, "Subtotal at shopping basket is different than 1!");
        }

        [TearDown]
        public void TearDown()
        {
            Driver.Quit();
        }
    }
}
