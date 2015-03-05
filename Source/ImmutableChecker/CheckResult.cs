using System.Collections.Generic;
using System.Linq;

namespace ImmutableChecker
{
    public class CheckResult
    {
        private readonly IReadOnlyCollection<ChackResultDetail> _errorLog;
        private readonly bool _result;

        public CheckResult(bool result, IReadOnlyCollection<ChackResultDetail> errorLog)
        {
            _result = result;
            _errorLog = errorLog;
        }

        public IReadOnlyCollection<ChackResultDetail> ErrorLog
        {
            get { return _errorLog; }
        }

        public bool Result
        {
            get { return _result; }
        }

        public static CheckResult operator +(CheckResult x, CheckResult y)
        {
            return new CheckResult(x.Result && y.Result, x.ErrorLog.Concat(y.ErrorLog).ToArray());
        }

        public static CheckResult CreateErrorResult(ErrorCode errorCode, string errorLog)
        {
            return new CheckResult(false, new[] {new ChackResultDetail(errorCode, errorLog)});
        }

        public static readonly CheckResult AllOK = new CheckResult(true, new ChackResultDetail[0]);

    }

    public class ChackResultDetail
    {
        private readonly string _errorMessage;
        private readonly ErrorCode _errorCode;

        public ChackResultDetail(ErrorCode errorCode, string errorMessage)
        {
            _errorMessage = errorMessage;
            _errorCode = errorCode;
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public ErrorCode ErrorCode
        {
            get { return _errorCode; }
        }

        public override string ToString()
        {
            return string.Format("Error {0:X2}: {1}", (int)ErrorCode, ErrorMessage);
        }
    }
}