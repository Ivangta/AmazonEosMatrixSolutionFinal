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
        public void LoginWithValidData()
        {
            IWebElement searchBooksField = Driver.FindElement(By.Id("twotabsearchtextbox"));
            searchBooksField.SendKeys("Harry Potter and the Cursed Child 1 & 2");

            IWebElement seachDropDown = Driver.FindElement(By.Id("searchDropdownBox"));
            seachDropDown.Click();

            IWebElement booksElementFromDropDown = Driver.FindElement(By.XPath("//option[@value='search-alias=stripbooks']"));
            booksElementFromDropDown.Click();

            IWebElement searchButton = Driver.FindElement(By.Id("nav-search-submit-button"));
            searchButton.Click();

            IWebElement titleOfBook = Driver.FindElement(By.XPath("//span[normalize-space(text())='Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production']"));
            Assert.AreEqual("Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production", titleOfBook.Text);

            //Need to fix locator
            IWebElement editionOfBookFirst = Driver.FindElement(By.XPath("//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]/div[2]/div/div/div/div/div/div[2]/div/div/div[3]/div[1]/div/div[1]/div[1]/a"));
            var editionFirst = editionOfBookFirst.Text;
            Assert.AreEqual("Paperback", editionOfBookFirst.Text);

            //Need to fix locator
            IWebElement price = Driver.FindElement(By.XPath("//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]/div[2]/div/div/div/div/div/div[2]/div/div/div[3]/div[1]/div/div[1]/div[2]/a/span"));
            var priceFirstFormatted = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(price.Text);
            Assert.IsNotEmpty(priceFirstFormatted);
            priceFirstFormatted.Contains("£");

            //this is the problem with cookies in Amazon Site
            titleOfBook.Click();

            IWebElement newPrice = Driver.FindElement(By.XPath("//span[@class='a-color-base']/descendant::span[.=' £4.00 ']"));
            var priceSecond = newPrice.Text;
            Assert.AreEqual(priceFirstFormatted, priceSecond);

            IWebElement editionOfBookSecond = Driver.FindElement(By.XPath("//a[(@class='a-button-text')]/descendant::*[normalize-space(text())='Paperback']"));
            var editionSecond = editionOfBookSecond.Text;
            Assert.AreEqual(editionFirst, editionSecond);

            IWebElement giftOption = Driver.FindElement(By.Id("gift-wrap"));
            if (!giftOption.Selected)
            {
                Wait.Until(ExpectedConditions.ElementToBeClickable(giftOption)).Click();
            }

            IWebElement addToBasketButton = Driver.FindElement(By.Id("add-to-cart-button"));
            addToBasketButton.Click();

            var basketPrice = Driver.FindElement(By.XPath("//span[@class='a-price sw-subtotal-amount']"));
            var basketPriceFormatted = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(basketPrice.Text);
            Assert.AreEqual(priceSecond, basketPriceFormatted);

            //this part only appears at times
            //var giftCheck = Driver.FindElement(By.XPath("//input[@type='checkbox']"));
            //Assert.IsTrue(giftCheck.Selected);

            var goToBasketButton = Driver.FindElement(By.XPath("//a[@data-csa-c-content-id='sw-gtc_CONTENT']"));
            goToBasketButton.Click();

            var unitOfBooksInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-dropdown-prompt']"));
            var unitBooksShoppingBasket = unitOfBooksInShoppingBasket.Text;
            Assert.AreEqual("1", unitBooksShoppingBasket);

            var editionInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-size-small sc-product-binding a-text-bold']"));
            var editionThird = editionInShoppingBasket.Text;
            Assert.AreEqual(editionSecond, editionThird);

            var priceInShoppingBasket = Driver.FindElement(By.XPath("//span[@class='a-size-medium a-color-base sc-price sc-white-space-nowrap sc-product-price a-text-bold']"));
            var priceShoppingBasket = priceInShoppingBasket.Text;
            Assert.AreEqual(basketPriceFormatted, priceShoppingBasket);

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
