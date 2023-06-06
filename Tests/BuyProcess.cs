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

            var titleOfBookElement = firstSearchResult.FindElement(By.XPath("//h2//span"));
            var titleOfBookText = titleOfBookElement.Text;
            Assert.AreEqual("Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production", titleOfBookText, "Title of book is incorrect in books section!");

            Assert.IsNotNull(firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]")),"Edition version is not paperback in books section!");
            Assert.IsNotNull(firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]/..//..//span[@class='a-price']")), "There is no price on the book in books section!");

            var paperBookPriceInBookSection = firstSearchResult.FindElement(By.XPath("//a[contains(text(), 'Paperback')]/..//..//span[@class='a-price']")).Text;
            var paperBookPriceInBookSectionFormatted = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(paperBookPriceInBookSection);

            //this is the problem with cookies in Amazon Site
            titleOfBookElement.Click();

            //UA3   
            Wait.Until(ExpectedConditions.ElementExists(By.Id("productTitle")));
            var titleOnProductDetailsPage = Driver.FindElement(By.Id("productTitle")).Text;
            Assert.AreEqual(titleOnProductDetailsPage, titleOfBookText, "Edition is not the same at product details page!");

            var paperBookPriceProductDetailsPage = Driver.FindElement(By.XPath("//div[@id='formats']//div[@id='tmmSwatches']//span[contains(text(), 'Paperback')]//..//span[@class='a-size-base a-color-price a-color-price']")).Text;
            Assert.AreEqual(paperBookPriceInBookSectionFormatted, paperBookPriceProductDetailsPage, "Price is not the same at product details page!");

            IWebElement editionOfBookProductDetailsPage = Driver.FindElement(By.XPath("//div[@id='formats']//div[@id='tmmSwatches']//span[contains(text(), 'Paperback')]"));
            var editionOfBookProductDetailsPageFormatted = editionOfBookProductDetailsPage.Text;
            Assert.IsNotNull(editionOfBookProductDetailsPage);
            Assert.AreEqual("Paperback", editionOfBookProductDetailsPageFormatted, "Edition version is not the same at product details page!");
            if (!editionOfBookProductDetailsPage.Selected)
            {
                Wait.Until(ExpectedConditions.ElementToBeClickable(editionOfBookProductDetailsPage)).Click();
            }           

            IWebElement addToBasketButton = Driver.FindElement(By.Id("add-to-cart-button"));
            addToBasketButton.Click();

            //UA4
            var paperBookBasketAdditionPrice = Driver.FindElement(By.XPath("//div[@id='sw-atc-buy-box']//span[@class='a-price sw-subtotal-amount']"));
            var paperBookBasketAdditionPriceFormatted = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(paperBookBasketAdditionPrice.Text);
            Assert.AreEqual(paperBookPriceSpecificBook, paperBookBasketAdditionPriceFormatted, "Price is not the same at addition to basket!");

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
            var titleShoppingBasket = Driver.FindElement(By.XPath("//div[@class='sc-item-content-group']//span[@class='a-truncate-cut']")).Text;
            Assert.AreEqual(titleOfBookText, titleShoppingBasket);

            var unitOfBooksInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-dropdown-prompt']"));
            var unitBooksShoppingBasket = unitOfBooksInShoppingBasket.Text;
            Assert.AreEqual("1", unitBooksShoppingBasket);

            var editionInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-size-small sc-product-binding a-text-bold']"));
            var editionThird = editionInShoppingBasket.Text;
            Assert.AreEqual(editionSecond, editionThird);

            var PaperBookPriceInShoppingBasket = Driver.FindElement(By.XPath("//span[@id='sc-subtotal-amount-buybox']//span")).Text;
            Assert.AreEqual(paperBookBasketAdditionPriceFormatted, PaperBookPriceInShoppingBasket, "Price is not the same in shopping basket!");

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
