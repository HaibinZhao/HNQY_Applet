//
using DotNetSpeech;

namespace CMCS.CarTransport.Queue.Core
{
    /// <summary>
    /// 语音播报对象
    /// </summary>
    public class VoiceSpeaker
    {
        private string _Version = "1.0.0.0";

        public string Version
        {
            get { return _Version; }
        }

        SpVoice voice = new SpVoice();

        private string lastValue = string.Empty;

        /// <summary>
        /// 上一次播报内容
        /// </summary>
        public string LastValue
        {
            get { return lastValue; }
        }

        /// <summary>
        /// 重置播报内容
        /// </summary>
        public void Reset()
        {
            lastValue = string.Empty;
        }

        /// <summary>
        /// 文本播报
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="count">次数</param>
        /// <param name="reset">播报前重置</param>
        public void Speak(string value, int count, bool reset = true)
        {
            if (reset) Reset();

            if (lastValue == value) return;

            lastValue = value;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    voice.Speak(value, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                }
                catch { }
            }

        }

        /// <summary>
        /// 文本播报（只读一次）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reset"></param>
        public void Speak(string value, bool reset = true)
        {
            Speak(value, 1, reset);
        }
    }
}