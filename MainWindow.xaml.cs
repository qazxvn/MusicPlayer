using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MusicPlayer
{
    public class Track
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
    }

    public partial class MainWindow : Window
    {
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;

        public ObservableCollection<Track> Tracks { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Tracks = new ObservableCollection<Track>();
            this.DataContext = this;
        }

        private void PlayTrack(string filePath)
        {
            StopPlayback();

            try
            {
                waveOutDevice = new WaveOutEvent();
                audioFileReader = new AudioFileReader(filePath);

                waveOutDevice.PlaybackStopped += OnPlaybackStopped;

                waveOutDevice.Init(audioFileReader);
                waveOutDevice.Play();

                VolumeSlider.Value = waveOutDevice.Volume;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка воспроизведения: {ex.Message}");
            }
        }

        private void StopPlayback()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
        }

        void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            StopPlayback();
        }

        private void TracksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TracksListBox.SelectedItem is Track selectedTrack)
            {
                PlayTrack(selectedTrack.FilePath);
                TracksListBox.SelectedItem = selectedTrack.Name;
            }
        }

        private void AddTrack(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Аудиофайлы (*.mp3;*.wav)|*.mp3;*.wav";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    Tracks.Add(new Track { Name = Path.GetFileNameWithoutExtension(filename), FilePath = filename });
                }
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Volume = (float)VolumeSlider.Value;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            StopPlayback();
            base.OnClosing(e);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (TracksListBox.SelectedItem is Track selectedTrack)
            {
                PlayTrack(selectedTrack.FilePath);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopPlayback();
        }
    }
}