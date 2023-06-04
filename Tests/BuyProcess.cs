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

            IWebElement editionOfBookFirst = Driver.FindElement(By.XPath("//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]/div[2]/div/div/div/div/div/div[2]/div/div/div[3]/div[1]/div/div[1]/div[1]/a"));
            var editionFirst = editionOfBookFirst.Text;
            Assert.AreEqual("Paperback", editionOfBookFirst.Text);

            IWebElement price = Driver.FindElement(By.XPath("//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]/div[2]/div/div/div/div/div/div[2]/div/div/div[3]/div[1]/div/div[1]/div[2]/a/span"));
            var priceFirstFormatted = _helper.RemoveWhiteSpacesAddDecimalPointUsingRegex(price.Text);
            Assert.IsNotEmpty(priceFirstFormatted);
            priceFirstFormatted.Contains("£");

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

        //public static string RemoveWhiteSpacesAddDecimalPointUsingRegex(string source)
        //{
        //    var newSourceFormat = source.Insert(source.Length - 2, ".");
        //    var finalSourceFormat = Regex.Replace(newSourceFormat, @"\s", string.Empty);
        //    return finalSourceFormat;
        //}

        //public static string RemoveNonNumericValuesUsingRegex(string source)
        //{
        //    return Regex.Replace(source, "[^\\d.]", string.Empty);
        //}

        [TearDown]
        public void TearDown()
        {
            Driver.Quit();
        }
        //i[@class='a-icon a-icon-checkbox']

        //input[@type='checkbox']/ancestor::i[@class='a-icon a-icon-checkbox']
        //i[@class='a-icon a-icon-checkbox']/ancestor::input[@type='checkbox']

        //span[@class='a-price sw-subtotal-amount']
        //span[@class='a-price sw-subtotal-amount']/descendant::span[@class='a-offscreen']

        //div[@data-index='2']
        //span[@class='a-size-medium a-color-base a-text-normal']
        //span[normalize-space(text())='Harry Potter and the Cursed Child - Parts One and Two: The Official Playscript of the Original West End Production']
        //*[normalize-space(text())='Paperback']/ancestor::a[contains(@class, 'a-button-text')]
        //a[(@class, 'a-button-text')]/ancestor::*[normalize-space(text())='Paperback']
        //a[(@class='a-button-text')]/descendant::*[normalize-space(text())='Paperback']
        //span[contains(@class, 'a-color-base')]/descendat::*[normalize-space(text())=' £4.00 ']
        //span[contains(@class, 'a-color-base')]/descendat::span[.=' £4.00 ']
        //span[@class='a-color-base']/descendant::span[.=' £4.00 ']
        //span[@class='a-color-base']

        //span[@class='a-price']/descendat::span[.='£4.00']



        //div[@class='a-section a-spacing-none a-spacing-top-micro puis-price-instructions-style']

        //a[@class=a-row a-size-base a-color-base']/descendant::*[normalize-space(text())='Paperback']

        //a[@href="/Harry-Potter-Cursed-Child-Playscript/dp/0751565369/ref=sr_1_1?crid=1Z52EFRTH90TA&keywords=Harry+Potter+and+the+Cursed+Child+1+%26+2&qid=1685823506&s=books&sprefix=harry+potter+and+the+cursed+child+1+%26+2%2Cstripbooks%2C428&sr=1-1" and .='Paperback']





        //div[@class='a-row a-size-base a-color-base']/ancestor::div[@class='a-section a-spacing-none a-spacing-top-micro puis-price-instructions-style']
        //div[@class='a-section a-spacing-none a-spacing-top-micro puis-price-instructions-style']/descendant::div[@class='a-row a-size-base a-color-base']





        //a[@class='a-size-base a-link-normal s-no-hover s-underline-text s-underline-link-text s-link-style a-text-normal' and @href="/Harry-Potter-Cursed-Child-Playscript/dp/0751565369/ref=sr_1_1?crid=1Z52EFRTH90TA&keywords=Harry+Potter+and+the+Cursed+Child+1+%26+2&qid=1685823506&s=books&sprefix=harry+potter+and+the+cursed+child+1+%26+2%2Cstripbooks%2C428&sr=1-1"]
        //span[@class='a-price' and @data-a-size='xl' and @data-a-color='base']
        //a[@class='a-size-base a-link-normal s-no-hover s-underline-text s-underline-link-text s-link-style a-text-normal' and @href="/Harry-Potter-Cursed-Child-Playscript/dp/0751565369/ref=sr_1_1?crid=1Z52EFRTH90TA&keywords=Harry+Potter+and+the+Cursed+Child+1+%26+2&qid=1685823506&s=books&sprefix=harry+potter+and+the+cursed+child+1+%26+2%2Cstripbooks%2C428&sr=1-1"]/descendant::span[@class='a-price' and @data-a-size='xl' and @data-a-color='base']
        //a[@class='a-size-base a-link-normal s-no-hover s-underline-text s-underline-link-text s-link-style a-text-normal' and @href="/Harry-Potter-Cursed-Child-Playscript/dp/0751565369/ref=sr_1_1?crid=1Z52EFRTH90TA&keywords=Harry+Potter+and+the+Cursed+Child+1+%26+2&qid=1685823506&s=books&sprefix=harry+potter+and+the+cursed+child+1+%26+2%2Cstripbooks%2C428&sr=1-1"]/descendant::span[@class='a-offscreen' and .='£4.00']
    }
}
