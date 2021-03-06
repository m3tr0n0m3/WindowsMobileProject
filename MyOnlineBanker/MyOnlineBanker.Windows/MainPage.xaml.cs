﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using MyOnlineBanker.Models;
using Parse;
using SQLite;

namespace MyOnlineBanker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public List<LoginUser> users { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            LogoutButton.Visibility = Visibility.Collapsed;
            ToAccountsButton.Visibility = Visibility.Collapsed;
            this.LoginHistoryAppBarButton.Visibility = Visibility.Collapsed;
        }

        private async Task<bool> CheckDbAsync(string dbName)
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(dbName);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        private async Task CreateDatabaseAsync()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(ParseUser.CurrentUser.Username);
            await conn.CreateTableAsync<LoginUser>();
        }

        private async Task AddUserAsync()
        {

            var user = new LoginUser(ParseUser.CurrentUser.Username);


            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(ParseUser.CurrentUser.Username);
            await conn.InsertAsync(user);
        }

        public async void LoginUser()
        {
            try
            {
                await ParseUser.LogInAsync(this.UsernameTextBox.Text, this.PasswordTextBox.Password);

                ShowNotification("Login", "Login successful!");


                bool dbExists = await CheckDbAsync(ParseUser.CurrentUser.Username);
                if (!dbExists)
                {
                    await CreateDatabaseAsync();
                    await AddUserAsync();
                }
                else
                {
                    await AddUserAsync();
                }



                LoginButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;
                Frame.Navigate(typeof(CustomerDetailsPage));
                this.UsernameTextBox.Text = string.Empty;
                this.PasswordTextBox.Password = string.Empty;
                this.UsernameBlock.Visibility = Visibility.Collapsed;
                this.PasswordBlock.Visibility = Visibility.Collapsed;
                this.UsernameTextBox.Visibility = Visibility.Collapsed;
                this.PasswordTextBox.Visibility = Visibility.Collapsed;
                this.ToAccountsButton.Visibility = Visibility.Visible;
                this.LoginHistoryAppBarButton.Visibility = Visibility.Visible;


            }
            catch (Exception e)
            {
                ShowNotification("Login Error", e.Message);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            this.LoginUser();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ParseUser.LogOut();
            ShowNotification("Logout", "You were logged out!");
            LogoutButton.Visibility = Visibility.Collapsed;
            LoginButton.Visibility = Visibility.Visible;
            this.UsernameBlock.Visibility = Visibility.Visible;
            this.PasswordBlock.Visibility = Visibility.Visible;
            this.UsernameTextBox.Visibility = Visibility.Visible;
            this.PasswordTextBox.Visibility = Visibility.Visible;
            this.ToAccountsButton.Visibility = Visibility.Collapsed;
            this.LoginHistoryAppBarButton.Visibility = Visibility.Collapsed;
        }

        private void Maps_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MapPage));
        }

        private void Contacts_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ContactsPage));
        }

        public static void ShowNotification(string title, string message)
        {
            const ToastTemplateType template =
                ToastTemplateType.ToastText02;
            var toastXml =
                ToastNotificationManager.GetTemplateContent(template);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(message));

            var toast = new ToastNotification(toastXml);

            var toastNotifier =
                ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toast);
            Task.Delay(2500).Wait();
        }

        private void ToAccountsButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CustomerDetailsPage));
        }

        private async void LoginHistoryAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(LoginHistoryPage), ParseUser.CurrentUser.Username);
        }
    }
}
