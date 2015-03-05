using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ImmutableChecker
{
    public class ImmutableCheckEngine
    {
        private readonly string _immutableAttributeTypeFullName;

        private static readonly string[] ReadOnlyTypeList =
        {
            "System.Boolean",
            "System.Byte",
            "System.SByte",
            "System.Char",
            "System.ConsoleKeyInfo",
            "System.DateTime",
            "System.DateTimeOffset",
            "System.Decimal",
            "System.Double",
            "System.Double",
            "System.Guid",
            "System.Int16",
            "System.Int32",
            "System.Int64",
            "System.IntPtr",
            "System.RuntimeArgumentHandle",
            "System.RuntimeFieldHandle",
            "System.RuntimeMethodHandle",
            "System.RuntimeTypeHandle",
            "System.SByte",
            "System.Single",
            "System.String",
            "System.TimeSpan",
            "System.UInt16",
            "System.UInt32",
            "System.UInt64",
            "System.UIntPtr",
        };

        private static readonly Type[] ReadonlyGenericTypeDefinitions =
        {
            typeof(IEnumerable<>),
            typeof(IReadOnlyCollection<>),
            typeof(IReadOnlyDictionary<,>),
            typeof(IReadOnlyList<>),
            typeof(ReadOnlyCollection<>),
            typeof(ReadOnlyDictionary<,>),
            typeof(ReadOnlyObservableCollection<>),
        };

        public ImmutableCheckEngine(string immutableAttributeTypeFullName)
        {
            _immutableAttributeTypeFullName = immutableAttributeTypeFullName;
        }

        public CheckResult CheckImmutable(Type type)
        {
            var result = CheckResult.AllOK;
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var notReadOnlyField = fields.Where(_ => _.IsInitOnly == false).ToArray();
            if (notReadOnlyField.Any())
            {
                var notReadOnlyFieldNames = notReadOnlyField.Select(_ => _.Name).ToArray();
                result += CheckResult.CreateErrorResult(
                    ErrorCode.FieldIsNotReadonly,
                    string.Format("{0} is not readonly in {1}",
                        string.Join(", ", notReadOnlyFieldNames),
                        type.FullName));
            }

            var notImmutableTypeFields = fields.Where(_ => IsImmutableType(_.FieldType) == false).ToArray();
            if (notImmutableTypeFields.Any())
            {
                var notImmutableTypeFieldInfos = notImmutableTypeFields.ToArray();
                result += CheckResult.CreateErrorResult(
                    ErrorCode.FieldTypeIsNotImmutable,
                    string.Format("{0} is not immutable type in {1}",
                        string.Join(", ", notImmutableTypeFieldInfos.Select(_ => _.FieldType.Name + " " + _.Name)),
                        type.FullName));
            }

            return result;
        }

        public bool IsImmutableType(Type type)
        {
            if (ReadOnlyTypeList.Contains(type.FullName))
            {
                return true;
            }
            if (type.IsEnum)
            {
                return true;
            }
            if (type.IsGenericType)
            {
                if (ReadonlyGenericTypeDefinitions.Contains(type.GetGenericTypeDefinition()))
                {
                    if (type.GetGenericArguments().All(IsImmutableType))
                    {
                        return true;
                    }
                }
            }
            if (Attribute.GetCustomAttributes(type).Any(_ => _.GetType().FullName == _immutableAttributeTypeFullName))
            {
                return true;
            }
            return false;
        }

        public bool IsCheckTargetType(Type type)
        {
            return Attribute.GetCustomAttributes(type).Any(_ => _.GetType().FullName == _immutableAttributeTypeFullName);
        }
    }

    public enum ErrorCode
    {
        NoError = 0x00,
        FieldIsNotReadonly = 0x01,
        FieldTypeIsNotImmutable=0x02,
    }
}