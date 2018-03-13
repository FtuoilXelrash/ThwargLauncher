﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using WindowPlacementUtil;

namespace ThwargLauncher.AccountManagement
{
    /// <summary>
    /// Interaction logic for AddServer.xaml
    /// </summary>
    public partial class AddServer : Window
    {
        public AddServer()
        {
            InitializeComponent();
            UpdateUi();
            AppSettings.WpfWindowPlacementSetting.Persist(this);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) { return; }

            try
            {
                AddServerFromUiInfo();
                this.DialogResult = true;
                Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Exception: {0}", exc));
            }
        }
        private void AddServerFromUiInfo()
        {
            var sdata = GetServerDataFromUi();
            ServerManager.AddNewServer(sdata);
        }
        private ServerModel.ServerEmuEnum GetServerEmuluation()
        {
            if (IsTrue(rdPhatACServer.IsChecked)) { return ServerModel.ServerEmuEnum.Phat; }
            if (IsTrue(rdACEServer.IsChecked)) { return ServerModel.ServerEmuEnum.Ace; }
            if (IsTrue(rdDFServer.IsChecked)) { return ServerModel.ServerEmuEnum.DF; }
            return ServerModel.ServerEmuEnum.Ace; // shouldn't happen but in case
        }
        private GameManagement.ServerPersister.ServerData GetServerDataFromUi()
        {
            var emu = GetServerEmuluation();
            var rodat = (cmbDefaultRodat.SelectedValue.ToString() == "false" ? ServerModel.RodatEnum.Off : ServerModel.RodatEnum.On);
            var secure = (cmbSecureLogin.SelectedValue.ToString() == "false" ? ServerModel.SecureEnum.Off : ServerModel.SecureEnum.On);

            var sdata = new GameManagement.ServerPersister.ServerData()
            {
                ServerId = Guid.NewGuid(),
                ServerName = txtServerName.Text,
                ServerDesc = txtServeDesc.Text,
                ConnectionString = txtServerIP.Text + ":" + txtServerPort.Text,
                RodatSetting = rodat,
                SecureSetting = secure,
                EMU = emu,
                GameApiUrl = txtGameApiUrl.Text,
                LoginServerUrl = txtLoginServerUrl.Text,
                LoginEnabled = true, // ??
                ServerSource = ServerModel.ServerSourceEnum.User
            };
            return sdata;
        }
        private static bool IsTrue(bool? bval, bool defval=false)
        {
            return (bval.HasValue ? bval.Value : defval);
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtServerName.Text))
            {
                MessageBox.Show("Server Name required");
                txtServerName.Focus();
                return false;
            }
            if (ServerManager.ServerList.Any(s => s.ServerName == txtServerName.Text))
            {
                MessageBox.Show("Server Name already exists");
                txtServerName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtServerIP.Text))
            {
                MessageBox.Show("Server Address required");
                txtServerIP.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtServerPort.Text))
            {
                MessageBox.Show("Server Port required");
                txtServerPort.Focus();
                return false;
            }
            string newAddress = string.Format("{0}:{1}", txtServerIP.Text, txtServerPort.Text);
            if (ServerManager.ServerList.Any(s => s.ServerIpAndPort == newAddress))
            {
                MessageBox.Show("Server Address already exists");
                txtServerName.Focus();
                return false;
            }
            int portnum = 0;
            if (!int.TryParse(txtServerPort.Text, out portnum))
            {
                MessageBox.Show("Server Port must be numeric");
                txtServerPort.Focus();
                return false;
            }
            if (cmbDefaultRodat.SelectedValue == null)
            {
                MessageBox.Show("Rodat selection required");
                cmbDefaultRodat.Focus();
                return false;
            }
            return true;
        }
        private void rdACEServer_Click(object sender, RoutedEventArgs e)
        {
            UpdateUi();
        }
        private void rdDFServer_Click(object sender, RoutedEventArgs e)
        {
            UpdateUi();
        }
        private void rdPhatACServer_Click(object sender, RoutedEventArgs e)
        {
            UpdateUi();
        }
        private void UpdateUi()
        {
            bool showDfControls = IsTrue(rdDFServer.IsChecked);
            Visibility dfControlVisibility = (showDfControls ? Visibility.Visible : Visibility.Hidden);
            this.lblLoginServerUrl.Visibility = dfControlVisibility;
            this.txtLoginServerUrl.Visibility = dfControlVisibility;
            this.lblSecureLogin.Visibility = dfControlVisibility;
            this.cmbSecureLogin.Visibility = dfControlVisibility;
            this.lblLoginServerUrl.Visibility = dfControlVisibility;
            this.txtLoginServerUrl.Visibility = dfControlVisibility;
            this.lblGameApiUrl.Visibility = dfControlVisibility;
            this.txtGameApiUrl.Visibility = dfControlVisibility;
        }
    }
}
