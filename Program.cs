using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.Threading;
using System.Windows.Forms; // Для использования SendKeys
using System.Data.Entity;

namespace WpfAppAutomation
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var app = FlaUI.Core.Application.Launch(@"C:\Users\TizKizOy\Desktop\РСПО Декстоп\RspoManager\MyKpiyapProject\bin\Debug\net8.0-windows\MyKpiyapProject.exe");

            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                //Thread.Sleep(1);

                var usernameField = window.FindFirstDescendant(cf => cf.ByAutomationId("LoginTextBox"))?.AsTextBox();
                var passwordField = window.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"))?.AsTextBox();
                var loginButton = window.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

                if (usernameField != null && passwordField != null && loginButton != null)
                {
                    usernameField.Text = "Shabun";

                    passwordField.Focus();
                    SendKeys.SendWait("Shabun");

                    loginButton.Click();
                }
                else
                {
                    Console.WriteLine("Не удалось найти элементы интерфейса.");
                    if (usernameField == null) Console.WriteLine("Username field не найден.");
                    if (passwordField == null) Console.WriteLine("Password field не найден.");
                    if (loginButton == null) Console.WriteLine("Login button не найден.");
                }
            }
        }
    }
}