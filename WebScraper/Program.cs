using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://login.yahoo.com/?.intl=us&.lang=en-US&.src=finance&authMechanism=primary&done=https%3A%2F%2Ffinance.yahoo.com%2Fportfolio%2Fp_0%2Fview%2Fv1");

                var userNameField = driver.FindElementById("login-username");
                var userButton = driver.FindElementById("login-signin");

                userNameField.SendKeys("webscrape.test@yahoo.com");
                userButton.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until((d) =>
                {
                    IWebElement element = d.FindElement(By.Id("login-passwd"));
                    return element.Displayed &&
                        element.Enabled
                        ? element
                        : null;
                });

                var userPasswordField = driver.FindElementById("login-passwd");
                var loginButton = driver.FindElementByXPath("//button[@id='login-signin']");

                userPasswordField.SendKeys("scrapepass1");
                loginButton.Click();

                wait.Until((d) =>
                {
                    IWebElement element = d.FindElement(By.Id("main"));
                    return element.Displayed &&
                        element.Enabled
                        ? element
                        : null;
                });

                // First fetch the table from which we want to scrape/parse indiv row data
                IWebElement table = driver.FindElement(By.XPath("//tbody"));

                // Second, parse indiv rows from table -- these will represent each stock row in our portfolio
                List<IWebElement> rows = new List<IWebElement>(table.FindElements(By.TagName("tr")));
                String strRowData = "";

                // Traverse each row...
                foreach (var row in rows)
                {
                    // ... to fetch the columns
                    List<IWebElement> tds = new List<IWebElement>(row.FindElements(By.TagName("td")));

                    // Then traverse each column
                    foreach (var td in tds)
                    {
                        // "\t\t" for tabs between tds
                        strRowData = strRowData + td.Text + "\t\t";
                    }
                    
                    Console.WriteLine(strRowData);
                    strRowData = String.Empty;
                }


                //Console.WriteLine(rows[3].Text);

                //foreach (IWebElement item in row)
                //{
                //    Console.WriteLine(item.Text);
                //}


            }



                // TODO: Create Stock object from res data for each scraped ticker
                // TODO: Parse res data perhaps with Regex to split by spaces? 
                // TODO: Once object is created, can use with MySQL to create database





            }
        }
    }

