using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GTA;
using NAudio.Wave;

namespace LogicSpawn.GTARPG.Core
{
    public class AudioHandler : Script
    {
        private IWavePlayer KillStreakDevice;
        private List<IWavePlayer> AudioDevices;
        private readonly string _basePath;
        private string MusicPath
        {   
            get { return Path.Combine(_basePath, "Audio\\Music\\"); }
        }
        private string SFXPath
        {   
            get { return Path.Combine(_basePath, "Audio\\Sfx\\"); }
        }
        public float Volume = 0.35f;

        public AudioHandler()
        {
            RPG.Audio = this;

            AudioDevices = new List<IWavePlayer>();

            _basePath = Path.Combine(Application.StartupPath, "scripts\\GTARPG");

            Tick += OnTick;
        }

        public void DisposeAll()
        {
            if(KillStreakDevice != null)
            {
                KillStreakDevice.Dispose();
            }

            foreach(var a in AudioDevices)
            {
                if(a != null) a.Dispose();
            }

        }

        private void OnTick(object sender, EventArgs e)
        {

        }

        public IWavePlayer PlayMusic(string musicName)
        {
            IWavePlayer waveOutDevice = new WaveOutEvent();
            var path = MusicPath + musicName + ".mp3";
            if(!File.Exists(path))
            {
                RPGLog.Log("Did not find music to play");
                return waveOutDevice;
            }
            AudioFileReader audioFileReader = new AudioFileReader(path);
            audioFileReader.Volume = Volume;
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();

            AudioDevices.Add(waveOutDevice);
            return waveOutDevice;
        }
        public IWavePlayer PlaySFX(string sfxName)
        {
            IWavePlayer waveOutDevice = new WaveOutEvent();
            var path = SFXPath + sfxName + ".mp3";
            if (!File.Exists(path))
            {
                RPGLog.Log("Did not find SFX to play");
                return waveOutDevice;
            }
            AudioFileReader audioFileReader = new AudioFileReader(path);
            audioFileReader.Volume = Volume;
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();

            AudioDevices.Add(waveOutDevice);
            return waveOutDevice;
        }

        public IWavePlayer PlayKillStreak(string sfxName, string ext = "wav")
        {
            KillStreakDevice = KillStreakDevice ?? new WaveOutEvent();

            var path = SFXPath + sfxName + "." + ext;
            Wait(250);
            if (!File.Exists(path))
            {
                RPGLog.Log("Did not find killstreak SFX to play");
                return KillStreakDevice;
            }
            AudioFileReader audioFileReader = new AudioFileReader(path);
            audioFileReader.Volume = 0.6f * Volume;
            
            KillStreakDevice.Stop();
            KillStreakDevice.Init(audioFileReader);
            KillStreakDevice.Play();
            return KillStreakDevice;
        }
    }
}