﻿namespace DevBaseFormat.Structure
{
    public class LyricElement
    {
        private long _timeStamp;
        private string _line;

        public LyricElement(long timeStamp, string line)
        {
            this._timeStamp = timeStamp;
            this._line = line;
        }

        public long TimeStamp
        {
            get => _timeStamp;
        }

        public string Line
        {
            get => _line;
        }
    }
}
