using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ImmutableChecker.Test
{
    [TestFixture]
    public class ImmutableCheckEngineTest
    {
        [TestCase()]
        public void すべてのフィールドがReadonlyな型はチェックOK()
        {
            var engine = new ImmutableCheckEngine("ImmutableChecker.Test.ImmubtaleAttribute");
            Assert.IsTrue(engine.CheckImmutable(typeof (ImmutableType)).Result);
        }

        [TestCase()]
        public void PrivateSetterはダメ()
        {
            var engine = new ImmutableCheckEngine("ImmutableChecker.Test.ImmubtaleAttribute");
            var result = engine.CheckImmutable(typeof (PrivateSetterType));
            Assert.IsFalse(result.Result);
            Assert.That(result.ErrorLog.First().ErrorCode, Is.EqualTo(ErrorCode.FieldIsNotReadonly));
        }

        [TestCase]
        public void Immutableでないフィールド型はダメ()
        {
            var engine = new ImmutableCheckEngine("ImmutableChecker.Test.ImmubtaleAttribute");
            var result = engine.CheckImmutable(typeof(NotImmutableFieldType));
            Assert.IsFalse(result.Result);
            Assert.That(result.ErrorLog.First().ErrorCode, Is.EqualTo(ErrorCode.FieldTypeIsNotImmutable));
        }
    }

    public class ImmubtaleAttribute : Attribute
    {
        
    }
    public class ReadonlyType
    {
        private readonly int _no;

        public ReadonlyType(int no)
        {
            _no = no;
        }

        public int No
        {
            get { return _no; }
        }
    }

    [Immubtale]
    public class ImmutableType
    {
        private readonly int _number;
        private readonly string _name;
        private readonly IEnumerable<string> _children;
        private readonly DayOfWeek _dayOfWeek;
        private readonly ImmutableType _immutableTypeMember;
        private readonly IEnumerable<ImmutableType> _immutableTypeArray;

        public ImmutableType(int number, string name, IEnumerable<string> children, DayOfWeek dayOfWeek, ImmutableType immutableTypeMember, IEnumerable<ImmutableType> immutableTypeArray)
        {
            _number = number;
            _name = name;
            _children = children;
            _dayOfWeek = dayOfWeek;
            _immutableTypeMember = immutableTypeMember;
            _immutableTypeArray = immutableTypeArray;
        }

        public int Number
        {
            get { return _number; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<string> Children
        {
            get { return _children; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return _dayOfWeek; }
        }

        public ImmutableType ImmutableTypeMember
        {
            get { return _immutableTypeMember; }
        }

        public IEnumerable<ImmutableType> ImmutableTypeArray
        {
            get { return _immutableTypeArray; }
        }
    }


    [Immubtale]
    public class PrivateSetterType
    {
        public PrivateSetterType(int no)
        {
            No = no;
        }
        public int No { get; private set; }
    }

    [Immubtale]
    public class NotImmutableFieldType
    {
        private readonly string[] _stringArray;

        public NotImmutableFieldType(string[] stringArray)
        {
            _stringArray = stringArray;
        }

        public string[] StringArray
        {
            get { return _stringArray; }
        }
    }
}
