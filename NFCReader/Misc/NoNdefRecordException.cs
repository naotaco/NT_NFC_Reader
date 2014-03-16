using System;

namespace SonyNdefUtils
{
    public class NoNdefRecordException : Exception
    {

        public NoNdefRecordException()
        {
        }

        public NoNdefRecordException(String message)
            : base(message)
        {
        }


    }
}
