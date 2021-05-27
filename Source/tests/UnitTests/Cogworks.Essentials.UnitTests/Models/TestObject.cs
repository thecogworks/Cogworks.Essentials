using System;

namespace Cogworks.Essentials.UnitTests.Models
{
    public class TestObject : IEquatable<TestObject>
    {
        public string Key { get; }

        public TestObject(string key)
            => Key = key;

        public bool Equals(TestObject other)
            => other != null && other.Key == Key;
    }
}