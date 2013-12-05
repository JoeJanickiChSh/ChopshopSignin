﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ChopshopSignin
{
    class SettingsWindowViewModel : ViewModelBase
    {
        public int TotalTimeUpdateInterval
        {
            get { return m_TotalTimeUpdateInterval; }
            set { m_TotalTimeUpdateInterval = value; FirePropertyChanged(); IsDirty = true; }
        }

        public int ScanInTimeoutWindow
        {
            get { return m_ScanInTimeoutWindow; }
            set { m_ScanInTimeoutWindow = value; FirePropertyChanged(); IsDirty = true; }
        }

        public int ScanDataResetTime
        {
            get { return m_ScanDataResetTime; }
            set { m_ScanDataResetTime = value; FirePropertyChanged(); IsDirty = true; }
        }

        public int ClearScanStatusTime
        {
            get { return m_ClearScanStatusTime; }
            set { m_ClearScanStatusTime = value; FirePropertyChanged(); IsDirty = true; }
        }

        public int MaxBackupFilesToKeep
        {
            get { return m_MaxBackupFilesToKeep; }
            set { m_MaxBackupFilesToKeep = value; FirePropertyChanged(); IsDirty = true; }
        }

        public bool ShowTimeUntilShip
        {
            get { return m_ShowTimeUntilShip; }
            set { m_ShowTimeUntilShip = value; FirePropertyChanged(); IsDirty = true; }
        }

        public DateTime Kickoff
        {
            get { return m_Kickoff; }
            set { m_Kickoff = value; FirePropertyChanged(); IsDirty = true; }
        }

        public DateTime Ship
        {
            get { return m_Ship; }
            set { m_Ship = value; FirePropertyChanged(); IsDirty = true; }
        }

        public DateTime TimeSince
        {
            get { return m_TimeSince; }
            set { m_TimeSince = value; FirePropertyChanged(); IsDirty = true; }
        }

        public bool IsDirty
        {
            get { return m_IsDirty; }
            private set { m_IsDirty = value; FirePropertyChanged(); }
        }

        public SettingsWindowViewModel(Properties.Settings currentSettings)
        {
            settings = currentSettings;

            TotalTimeUpdateInterval = settings.TotalTimeUpdateInterval;
            ScanInTimeoutWindow = settings.ScanInTimeoutWindow;
            ScanDataResetTime = settings.ScanDataResetTime;
            ClearScanStatusTime = settings.ClearScanStatusTime;
            MaxBackupFilesToKeep = settings.MaxBackupFilesToKeep;
            ShowTimeUntilShip = settings.ShowTimeUntilShip;

            if (settings.Kickoff == DateTime.MinValue)
                Kickoff = new DateTime(DateTime.Now.Year, 1, 1);
            else
                Kickoff = settings.Kickoff;

            if (settings.Ship == DateTime.MinValue)
                Ship = new DateTime(DateTime.Now.Year, 2, 1);
            else
                Ship = settings.Ship;

            if (settings.TimeSince == DateTime.MinValue)
                TimeSince = Kickoff;
            else
                TimeSince = settings.TimeSince;

            IsDirty = false;

            Properties.Settings.Default.SettingChanging += SettingChanging;
        }

        void SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            var settings = (Properties.Settings)sender;

            switch (e.SettingName)
            {
                case "TotalTimeUpdateInterval":
                case "ScanInTimeoutWindow":
                case "ScanDataResetTime":
                case "ClearScanStatusTime":
                case "MaxBackupFilesToKeep":
                    e.Cancel = (int)settings[e.SettingName] == (int)e.NewValue;
                    break;

                case "ShowTimeUntilShip":
                    e.Cancel = (bool)settings[e.SettingName] == (bool)e.NewValue;
                    break;

                case "Kickoff":
                case "Ship":
                case "TimeSince":
                    e.Cancel = (DateTime)settings[e.SettingName] == (DateTime)e.NewValue;
                    break;
            }
        }

        public void Save()
        {
            if (IsDirty)
            {
                settings.TotalTimeUpdateInterval = TotalTimeUpdateInterval;
                settings.ScanInTimeoutWindow = ScanInTimeoutWindow;
                settings.ScanDataResetTime = ScanDataResetTime;
                settings.ClearScanStatusTime = ClearScanStatusTime;
                settings.MaxBackupFilesToKeep = MaxBackupFilesToKeep;
                settings.ShowTimeUntilShip = ShowTimeUntilShip;
                settings.Kickoff = Kickoff;
                settings.Ship = Ship;
                settings.TimeSince = TimeSince;

                settings.Save();
                IsDirty = false;
            }
        }

        private int m_TotalTimeUpdateInterval;
        private int m_ScanInTimeoutWindow;
        private int m_ScanDataResetTime;
        private int m_ClearScanStatusTime;
        private int m_MaxBackupFilesToKeep;
        private bool m_ShowTimeUntilShip;
        private DateTime m_Kickoff;
        private DateTime m_Ship;
        private DateTime m_TimeSince;

        private bool m_IsDirty;

        private Properties.Settings settings;
    }
}
